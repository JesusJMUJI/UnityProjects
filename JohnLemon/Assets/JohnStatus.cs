using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnStatus : MonoBehaviour
{
    private static JohnStatus _instance;

    public static JohnStatus Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<JohnStatus>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(JohnStatus).Name);
                    _instance = singleton.AddComponent<JohnStatus>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool isJohnInSafeZone = false;
}
