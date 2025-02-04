using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointControl : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] waypoints;

    public Transform playerTransform;
    public Transform waypoint3Transform;
    public Observer observer;

    [SerializeField] int currentWaypoint = 0;

    [SerializeField] private bool isChaserGhost;
    [Header("Chase Settings")]
    [SerializeField] private float destinationReachedThreshold = 1f;
    [Tooltip("Distance in meters to consider the ghost has reached its destination")]
    [Range(0.1f, 5f)]
    [SerializeField] private float patrolReachedThreshold = 0.5f;

    void Start()
    {
        transform.position = waypoint3Transform.position;
    }

    void Update()
    {
        // Debug.Log($"Remaining distance: {agent.remainingDistance}");
        if(isChaserGhost)
            ChaserGhost();
        else
            Patroller();
    }

    void ChaserGhost()
    {
        // Skip if still moving towards destination
        if (agent.pathPending || agent.remainingDistance > destinationReachedThreshold) return;

        if (!SafeZoneManager.Instance.ReturnIsSafe())
        {
            observer.transform.gameObject.SetActive(true);
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            observer.transform.gameObject.SetActive(false);
            agent.SetDestination(waypoint3Transform.position);
        }
    }

    void Patroller()
    {
        if (agent.remainingDistance < patrolReachedThreshold)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}
