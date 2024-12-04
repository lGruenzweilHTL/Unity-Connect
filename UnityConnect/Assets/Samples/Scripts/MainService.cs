using System.Collections;
using System.Collections.Generic;
using UnityConnect;
using UnityEngine;

public class MainService : MonoBehaviour
{
    private void Awake()
    {
        Authenticator.OnSignedIn += () =>
        {
            gameObject.SetActive(true);
            Debug.Log($"Signed in as {UserData.PlayerName}");
        };
        Debug.Log("MainService Initialized");
        gameObject.SetActive(false);
    }
}
