using Unity.Services.Core;
using UnityConnect;
using UnityEngine;

namespace UnityConect
{
    public class UnityServiceManager : MonoBehaviour
    {
        private async void Start()
        {
            UnityServices.Initialized += () =>
            {
                Debug.Log("Unity services initialized");
            };
            UnityServices.InitializeFailed += error =>
            {
                Debug.Log($"Unity service initialization failed: {error}");
            };

            await UnityServices.InitializeAsync();
            Authenticator.Initialize();
        }
    }
}