using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Friends.Models;
using Unity.Services.Friends;

namespace UnityConnect
{
    public static class UserData
    {
        public static PlayerInfo PlayerInfo => AuthenticationService.Instance.PlayerInfo;
        public static string PlayerName => PlayerInfo.Username;
        public static DateTime CreationTime => PlayerInfo.CreatedAt ?? DateTime.MinValue;
        public static string PlayerId => PlayerInfo.Id;
        public static List<Notification> AllUsers => AuthenticationService.Instance.Notifications;

        public static IReadOnlyList<Relationship> Friends => FriendsService.Instance.Friends;
        public static IReadOnlyList<Relationship> Blocked => FriendsService.Instance.Blocks;
        public static IReadOnlyList<Relationship> IncomingFriendRequests => FriendsService.Instance.IncomingFriendRequests;
        public static IReadOnlyList<Relationship> OutgoingFriendRequests => FriendsService.Instance.OutgoingFriendRequests;
    }
}