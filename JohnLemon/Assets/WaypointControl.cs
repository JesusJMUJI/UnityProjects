using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointControl : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] waypoints;
    int currentWaypoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(waypoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.remainingDistance < agent.stoppingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination (waypoints[currentWaypoint].position);

        }
    }
}
