using Firebase;
using System;
using UnityEngine;

/// <summary>
/// This should be added to an initial scene. It will initialize Firebase
/// then indicate with an event when initialization is complete, storing
/// itself into a presisting singleton instance.
/// Credits to: Patrick Martin and xzippyzachx
/// </summary>
public class FirebaseController : MonoBehaviour
{
    private static FirebaseController _instance;
    public static FirebaseController Instance { get { return _instance; } }

    private FirebaseApp _app;
    public FirebaseApp App { get { return _app; } }

    #region UnityEvents

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _app = FirebaseApp.DefaultInstance;
                }
                else
                {
                    Debug.LogError(String.Format(
                      "Could not resolve all Firebase dependencies: {0}", 
                      dependencyStatus));
                }
            });
        }
        else
        {
            Debug.LogError(
                $"An intance of {nameof(FirebaseController)} already exists!");
        }
    }

    private void OnDestroy()
    {
        _app = null;
        if (_instance == this)
        {
            _instance = null;
        }
    }

    #endregion
}
