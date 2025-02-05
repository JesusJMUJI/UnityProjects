using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserGhostSpawner : MonoBehaviour
{
    public GameObject chaserGhostPrefab;
    public Transform waypoint3Transform;
    public Transform playerTransform;
    public GameEnding gameEnding;

    void Start()
    {
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        while (true)
        {
            GameObject spawnedGhost = Instantiate(chaserGhostPrefab, waypoint3Transform.position, Quaternion.identity);

            // Setup WaypointControl
            WaypointControl waypointControl = spawnedGhost.GetComponent<WaypointControl>();
            waypointControl.playerTransform = playerTransform;
            waypointControl.waypoint3Transform = waypoint3Transform;

            // Setup Observer
            Observer observer = spawnedGhost.GetComponentInChildren<Observer>();
            observer.player = playerTransform;
            observer.gameEnding = gameEnding;

            yield return new WaitForSeconds(1f);
        }
    }
}
