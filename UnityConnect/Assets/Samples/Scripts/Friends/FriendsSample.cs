using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using UnityEngine;

namespace UnityConnect.Samples
{
    public class FriendsSample : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendsList;
        [SerializeField] private TMP_Text blockList;

        private string _friendTargetName;
        private string _blockTargetName;
        private string _unblockTargetName;
        
        public void UpdateFriendsList()
        {
            friendsList.text = "<size=120%>Friends</size>\n\n" + string.Join("\n", UserData.Friends.Select(rel => rel.Member.Id));
            blockList.text = "<size=120%>Blocked</size>\n\n" + string.Join("\n", UserData.Blocked.Select(rel => rel.Member.Id));
        }

        public void SetFriendTargetName(string name)
        {
            _friendTargetName = name;
        }
        public void SetBlockTargetName(string name)
        {
            _blockTargetName = name;
        }
        public void SetUnblockTargetName(string name)
        {
            _unblockTargetName = name;
        }

        public void SendFriendRequest()
        {
            SendFriendRequest(_friendTargetName);
        }
        public void BlockPlayer()
        {
            BlockPlayer(_blockTargetName);
        }
        public void UnblockPlayer()
        {
            UnblockPlayer(_unblockTargetName);
        }
        
        [Command]
        private static void SendFriendRequest(string memberId)
        {
            FriendsService.Instance.AddFriendAsync(memberId);
        }
        [Command]
        private static void BlockPlayer(string memberId)
        {
            FriendsService.Instance.AddBlockAsync(memberId);
        }
        [Command]
        private static void UnblockPlayer(string memberId)
        {
            FriendsService.Instance.DeleteBlockAsync(memberId);
        }

        [Command]
        private static void RecallFriendRequest(string memberId)
        {
            FriendsService.Instance.DeleteOutgoingFriendRequestAsync(memberId);
        }
        [Command]
        public static string AcceptFriendRequest(string memberId)
        {
            // Check if there is an incoming friend request from the member
            var incomingRequest = UserData.IncomingFriendRequests.FirstOrDefault(req => req.Member.Id == memberId);
            if (incomingRequest != null)
            {
                // If there is an incoming friend request, add the member as a friend
                SendFriendRequest(memberId);
                return "Successfully accepted the request from " + memberId;
            }
            else
            {
                return $"No incoming friend request from member with ID: {memberId}";
            }
        }
        [Command]
        public static void DeclineFriendRequest(string memberId)
        {
            FriendsService.Instance.DeleteIncomingFriendRequestAsync(memberId);
        }

        [Command("Gets the relationship between the specified player and the current user")]
        private static RelationshipType GetRelationshipWithPlayer(string playerId)
        {
            var relationships = FriendsService.Instance.Relationships;
            return (from relationship in relationships
                where relationship.Member.Id == playerId
                select relationship.Type).FirstOrDefault();
        }
        
        [Command("Lists all relationships")]
        public static string ListRelationships()
        {
            var relationships = FriendsService.Instance.Relationships;
            return string.Join("\n", relationships.Select(r => $"Player {r.Member.Id}: {r.Type}"));
        }

        [Command("Lists all incoming and outgoing friend requests")]
        public static string ListFriendRequests()
        {
            var incoming = UserData.IncomingFriendRequests;
            var outgoing = UserData.OutgoingFriendRequests;
            return $"Incoming: {string.Join(", ", incoming.Select(req => req.Member.Id))}\nOutgoing: {string.Join(", ", outgoing.Select(req => req.Member.Id))}";
        }

        [Command("Refreshes all relationships")]
        public static void RefreshAll()
        {
            FriendsService.Instance.ForceRelationshipsRefreshAsync();
            FriendsService.Instance.
        }
    }
}