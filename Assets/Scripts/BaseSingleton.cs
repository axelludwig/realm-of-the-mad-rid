using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{

    private static T _instance;
    private static readonly object _instanceLock = new object();
    private static bool _quitting = false;

    public static T instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null && !_quitting)
                {

                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        Instantiate();
                    }
                }

                return _instance;
            }
        }
    }

    public static void Instantiate()
    {
        GameObject go = new GameObject(typeof(T).ToString());
        _instance = go.AddComponent<T>();

        DontDestroyOnLoad(_instance.gameObject);
    }

    protected virtual void Awake()
    {
        if (_instance == null) _instance = gameObject.GetComponent<T>();
        else if (_instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }
}