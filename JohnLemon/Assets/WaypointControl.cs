using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointControl : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] waypoints;

    [SerializeField] private bool isChaserGhost;
    public Transform playerTransform;
    public Transform waypoint3Transform;
    public Observer observer;

    [SerializeField] int currentWaypoint = 0;

    void Start()
    {
        transform.position = waypoint3Transform.position;
    }

    void Update()
    {
        Debug.Log($"Remaining distance: {agent.remainingDistance}");
        if(isChaserGhost)
        {
            ChaserGhost();
        }
        else
        {
            Patroller();
        }
    }

    void ChaserGhost()
    {
        if(agent.pathPending || agent.remainingDistance > 1f)
        {
            return;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(playerTransform.position, out hit, 10.0f, NavMesh.AllAreas))
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
        if(agent.remainingDistance < agent.stoppingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}
