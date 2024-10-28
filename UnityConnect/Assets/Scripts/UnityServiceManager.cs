using Unity.Services.Core;
using UnityEngine;

public class UnityServiceManager : MonoBehaviour
{
    private void Start()
    {
        UnityServices.Initialized += () =>
        {
            print("Unity services initialized");
        };
        UnityServices.InitializeFailed += error =>
        {
            print($"Unity service initialization failed: {error}");
        };

        UnityServices.InitializeAsync();
    }
}