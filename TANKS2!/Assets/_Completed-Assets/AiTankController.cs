using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    public class AiTankController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float firingRange = 15f;
        [SerializeField] private float optimalRange = 10f;
        [SerializeField] private float updateTargetInterval = 0.5f;
        [SerializeField] private float rotationSpeed = 90f; // Degrees per second
        [SerializeField] private float aimThreshold = 5f; // Degrees of accuracy needed to fire
        [SerializeField] private float fireDelay = 0.5f; // Time needed to aim before firing

        [Header("Debug Visualization")]
        [SerializeField] private bool showDebugVisuals = true;
        [SerializeField] private Color aimLineColor = Color.red;
        [SerializeField] private Color rangeColor = Color.yellow;
        [SerializeField] private Color optimalRangeColor = Color.green;

        private NavMeshAgent agent;
        private AiTankShooting shooting;
        private Transform target;
        private float nextUpdateTime;
        private float nextFireTime;
        private bool isAimed;

        // Add new fields
        private Rigidbody m_Rigidbody;
        private Vector3 m_DesiredMovement;
        [SerializeField] private float m_Speed = 12f;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            shooting = GetComponent<AiTankShooting>();
            m_Rigidbody = GetComponent<Rigidbody>();

            // Configure Rigidbody
            m_Rigidbody.drag = 5f;
            m_Rigidbody.angularDrag = 5f;
            m_Rigidbody.isKinematic = false;

            // Configure NavMeshAgent
            if (agent != null)
            {
                agent.radius = 2f;
                agent.height = 2f;
                agent.speed = m_Speed;
                agent.acceleration = 8f;
                agent.angularSpeed = rotationSpeed;
                agent.stoppingDistance = 0.1f;
                agent.updatePosition = false;
                agent.updateRotation = false;
            }

        }

        private void DrawDebugCircle(Vector3 center, float radius, Color color, int segments = 20)
        {
            float angleStep = 360f / segments;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = (i + 1) * angleStep;

                Vector3 point1 = center + Quaternion.Euler(0, angle1, 0) * Vector3.forward * radius;
                Vector3 point2 = center + Quaternion.Euler(0, angle2, 0) * Vector3.forward * radius;

                Debug.DrawLine(point1, point2, color);
            }
        }
        void FixedUpdate()
        {
            if (agent != null && agent.hasPath)
            {
                // Calculate movement based on NavMeshAgent's desired velocity
                Vector3 movement = Vector3.ClampMagnitude(agent.desiredVelocity, m_Speed);

                // Apply movement using Rigidbody
                m_Rigidbody.velocity = movement;

                // Handle rotation separately
                if (movement.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    m_Rigidbody.MoveRotation(Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.fixedDeltaTime
                    ));
                }
            }
            else
            {
                // Stop movement when no path
                m_Rigidbody.velocity = Vector3.zero;
            }
        }

        void Update()
        {
            // Update NavMeshAgent's position to match actual position
            if (agent != null)
            {
                agent.nextPosition = transform.position;
            }

            // Rest of the existing Update code...
            if (Time.time >= nextUpdateTime)
            {
                FindTarget();
                nextUpdateTime = Time.time + updateTargetInterval;
            }

            // Draw debug visuals
            if (showDebugVisuals)
            {
                // Draw aim line
                Debug.DrawLine(transform.position, target.position, aimLineColor);

                // Draw firing range circle
                DrawDebugCircle(transform.position, firingRange, rangeColor);

                // Draw optimal range circle
                DrawDebugCircle(transform.position, optimalRange, optimalRangeColor);

                // Draw forward direction
                Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
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
            // Find all player tanks (excluding AI tanks)
            var playerTanks = GameObject.FindObjectsOfType<TankMovement>()
                .Select(t => t.gameObject)
                .Where(go => go.activeSelf && go != gameObject)
                .ToArray();

            float closestDistance = Mathf.Infinity;
            Transform nearestPlayer = null;

            foreach (GameObject player in playerTanks)
            {
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
            // Get target direction
            Vector3 targetDirection = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            // Draw aim direction
            if (showDebugVisuals)
            {
                float lineLength = 10f;
                Vector3 aimLine = transform.forward * lineLength;
                Vector3 targetLine = targetDirection * lineLength;
                Debug.DrawRay(transform.position, aimLine, Color.blue); // Current aim
                Debug.DrawRay(transform.position, targetLine, Color.red); // Target aim
            }
            // Smoothly rotate towards target
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Check if aimed at target
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDifference < aimThreshold)
            {
                if (!isAimed)
                {
                    isAimed = true;
                    nextFireTime = Time.time + fireDelay;
                }
                else if (Time.time >= nextFireTime)
                {
                    shooting.Fire();
                }
            }
            else
            {
                isAimed = false;
            }
        }

    }



    [Serializable]
    public class AiTankManager : TankManager
    {
        public void Setup()
        {

            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">NPC " + "</color>";

            // Get all of the renderers of the tank.
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... set their material color to the color specific to this tank.
                renderers[i].material.color = m_PlayerColor;
            }
        }

        public void Reset()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive(false);
            m_Instance.SetActive(true);
        }

        public void EnableControl()
        {
            AiTankController aiController = m_Instance.GetComponent<AiTankController>();
            if (aiController != null)
            {
                aiController.enabled = true;
            }
        }

        public void DisableControl()
        {
            AiTankController aiController = m_Instance.GetComponent<AiTankController>();
            if (aiController != null)
            {
                aiController.enabled = false;
            }
        }
    }
}
