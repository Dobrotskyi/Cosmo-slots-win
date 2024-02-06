using System;
using UnityEngine;

public class SceneLoadTracker : MonoBehaviour
{
    public static event Action SceneLoaded;
    public static event Action SceneUnloaded;

    private void OnDestroy()
    {
        SceneUnloaded?.Invoke();
    }

    private void Awake()
    {
        SceneLoaded?.Invoke();
    }
}
