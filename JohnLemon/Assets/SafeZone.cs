using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SafeZoneType
{
    Big,
    Small,
}
public class SafeZone : MonoBehaviour
{
    [SerializeField] private bool exampleProperty;
    [SerializeField] private int safeZoneCooldown;
    [SerializeField] private SafeZoneType safeZoneType;
}
