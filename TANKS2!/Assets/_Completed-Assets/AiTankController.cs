using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    public class AiTankController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float firingRange = 15f;         // Maximum range at which the tank can fire
        [SerializeField] private float optimalRange = 10f;        // Preferred distance to maintain from target
        [SerializeField] private float updateTargetInterval = 0.5f;

        private NavMeshAgent agent;
        private AiTankShooting shooting;
        private Transform target;
        private float nextUpdateTime;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            shooting = GetComponent<AiTankShooting>();
        }

        void Update()
        {
            if (Time.time >= nextUpdateTime)
            {
                FindTarget();
                nextUpdateTime = Time.time + updateTargetInterval;
            }

            if (target != null)
            {
                // Calculate distance and direction
                Vector3 directionToTarget = target.position - transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // Calculate position to move to
                if (distanceToTarget > optimalRange)
                {
                    // Get position at optimal range from target
                    Vector3 targetPosition = target.position - (directionToTarget.normalized * optimalRange);
                    agent.SetDestination(targetPosition);
                }
                else
                {
                    // Already at good range, stop moving
                    agent.SetDestination(transform.position);
                }

                // Attack if in range
                if (distanceToTarget <= firingRange)
                {
                    AimAndFire();
                }
            }
        }


        void FindTarget()
        {
            // Find all objects in the Players layer
            int playerLayer = LayerMask.NameToLayer("Players");
            GameObject[] players = GameObject.FindObjectsOfType<GameObject>()
                .Where(go => go.layer == playerLayer && go.activeSelf)
                .ToArray();

            float closestDistance = Mathf.Infinity;
            Transform nearestPlayer = null;

            foreach (GameObject player in players)
            {
                // Don't target self
                if (player == gameObject)
                    continue;

                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPlayer = player.transform;
                }
            }

            target = nearestPlayer;
        }

        void AimAndFire()
        {
            // Aim
            Vector3 targetDirection = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(targetDirection);

            // Fire
            shooting.Fire();
        }

    }
}

[Serializable]
public class AiTankManager
{
    public Color m_PlayerColor;                             // This is the color this tank will be tinted.
    public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.for.
    [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
    [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
    [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.

    public void Setup ()
    {

        // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">NPC " + "</color>";

        // Get all of the renderers of the tank.
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer> ();

        // Go through all the renderers...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            renderers[i].material.color = m_PlayerColor;
        }
    }
}
