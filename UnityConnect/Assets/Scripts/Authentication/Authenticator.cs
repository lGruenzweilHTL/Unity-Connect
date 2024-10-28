using System;
using TMPro;
using UnityEngine;
using Auth = Unity.Services.Authentication.AuthenticationService;

public class Authenticator : MonoBehaviour
{
    [SerializeField] private TMP_Text errorText;

    public event Action OnSignedIn;

    private string _username;
    private string _password;

    private void Start()
    {
        Unity.Services.Core.UnityServices.Initialized += () =>
        {
            Auth.Instance.SignedIn += () =>
            {
                print($"Signed in as {UserData.PlayerName}");
                gameObject.SetActive(false);

                OnSignedIn?.Invoke();
            };
            Auth.Instance.SignInFailed += error => ShowError(error);
            Auth.Instance.Expired += () => print("Access token expired");
        };
    }

    public void SetUsername(string username)
    {
        HideError();
        _username = username;
    }
    public void SetPassword(string password)
    {
        HideError();
        _password = password;
    }

    public void SignUp()
    {
        try
        {
            Auth.Instance.SignUpWithUsernamePasswordAsync(_username, _password);
        }
        catch (Exception e)
        {
            ShowError(e);
        }
    }
    public void SignIn()
    {
        try
        {
            Auth.Instance.SignInWithUsernamePasswordAsync(_username, _password);
        }
        catch (Exception e)
        {
            ShowError(e);
        }

    }
    public void SignOut()
    {
        Auth.Instance.SignOut(false);
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