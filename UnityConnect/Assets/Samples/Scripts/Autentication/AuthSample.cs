using System;
using TMPro;
using UnityEngine;

public class AuthSample : MonoBehaviour
{
    [SerializeField] private TMP_Text errorText;

    private void Start()
    {
        Authenticator.OnSignedIn += () =>
        {
            HideError();
            gameObject.SetActive(false);
            Debug.Log($"Signed in as {UserData.PlayerName}");
        };
        Authenticator.OnSignedOut += () =>
        {
            Debug.Log("Signed out");
        };
        Authenticator.OnErrorOccured += error =>
        {
            ShowError(error);
        };
        Authenticator.OnAccessTokenExpired += () =>
        {
            Debug.Log("Access token expired");
        };
    }

    public void SetUsername(string username)
    {
        Authenticator.SetUsername(username);
        HideError();
    }
    public void SetPassword(string password)
    {
        Authenticator.SetPassword(password);
        HideError();
    }
    private void ShowError(Exception error)
    {
        ShowError(error.Message);
    }
    private void ShowError(string error)
    {
        errorText.text = error;
        errorText.gameObject.SetActive(true);
    }
    private void HideError()
    {
        errorText.gameObject.SetActive(false);
    }
}