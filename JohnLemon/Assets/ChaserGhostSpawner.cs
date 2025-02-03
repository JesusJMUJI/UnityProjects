using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserGhostSpawner : MonoBehaviour
{
    public GameObject chaserGhostPrefab;
    public Transform playerTransform;
    public Transform waypoint3Transform;
    public Observer observer;

    private GameObject chaserGhost;

    void Start()
    {
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        while (true)
        {
            chaserGhost = Instantiate(chaserGhostPrefab, waypoint3Transform.position, Quaternion.identity);
            chaserGhost.GetComponent<WaypointControl>().playerTransform = playerTransform;
            chaserGhost.GetComponent<WaypointControl>().waypoint3Transform = waypoint3Transform;
            chaserGhost.GetComponent<WaypointControl>().observer = observer;
            yield return new WaitForSeconds(1f);
        }
    }
}
