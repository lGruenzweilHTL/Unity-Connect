using System;
using Unity.Services.Authentication;
public static class UserData
{
    public static PlayerInfo PlayerInfo => AuthenticationService.Instance.PlayerInfo;
    public static string PlayerName => PlayerInfo.Username;
    public static DateTime CreationTime => PlayerInfo.CreatedAt ?? DateTime.MinValue;
    public static string PlayerId => PlayerInfo.Id;
}