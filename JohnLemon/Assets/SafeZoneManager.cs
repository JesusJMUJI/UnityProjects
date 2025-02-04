using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    public static SafeZoneManager Instance { get; private set; }

    [SerializeField] private bool isInSafeZone;

    [SerializeField] private List<SafeZone> safeZones;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        isInSafeZone = false;
    }

    private void SetSafeZone(bool value)
    {
        isInSafeZone = value;
    }

    public bool ReturnIsSafe()
    {
        return isInSafeZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<SafeZone>(out _))
        {
            SetSafeZone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<SafeZone>(out _))
        {
            SetSafeZone(false);
        }
    }
}
