using System;
using Auth = Unity.Services.Authentication.AuthenticationService;

public static class Authenticator
{
    public static event Action OnSignedIn;
    public static event Action<Exception> OnErrorOccured;
    public static event Action OnSignedOut;
    public static event Action OnAccessTokenExpired;

    private static string _username;
    private static string _password;

    public static void Initialize()
    {
        Auth.Instance.SignedIn += () => OnSignedIn?.Invoke();
        Auth.Instance.SignInFailed += error => OnErrorOccured?.Invoke(error);
        Auth.Instance.Expired += () => OnAccessTokenExpired?.Invoke();
    }

    public static void SetUsername(string username)
    {
        _username = username;
    }
    public static void SetPassword(string password)
    {
        _password = password;
    }

    public static void SignUp()
    {
        try
        {
            Auth.Instance.SignUpWithUsernamePasswordAsync(_username, _password);
        }
        catch (Exception e)
        {
            OnErrorOccured?.Invoke(e);
        }
    }
    public static void SignIn()
    {
        try
        {
            Auth.Instance.SignInWithUsernamePasswordAsync(_username, _password);
        }
        catch (Exception e)
        {
            OnErrorOccured?.Invoke(e);
        }

    }
    public static void SignOut()
    {
        OnSignedOut?.Invoke();
        Auth.Instance.SignOut(false);
    }
}