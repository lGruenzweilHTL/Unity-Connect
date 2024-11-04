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
            friendsList.text = "<size=120%>Friends</size>\n\n" + string.Join("\n", UserData.Friends.Select(rel => rel.Member.Profile.Name));
            blockList.text = "<size=120%>Blocked</size>\n\n" + string.Join("\n", UserData.Blocked.Select(rel => rel.Member.Profile.Name));
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
            FriendsService.Instance.AddFriendByNameAsync(_friendTargetName);
        }
        public void BlockPlayer()
        {
            FriendsService.Instance.AddBlockAsync(_blockTargetName);
        }
        public void UnblockPlayer()
        {
            FriendsService.Instance.DeleteBlockAsync(_unblockTargetName);
        }

        public void RecallFriendRequest(string memberId)
        {
            FriendsService.Instance.DeleteOutgoingFriendRequestAsync(memberId);
        }
        public void AcceptFriendRequest(string memberId)
        {
            // Sending a friend request to a player you have a request incoming from, automatically creates a Friend relationship between the players
            if (GetRelationshipWithPlayer(memberId) == RelationshipType.Friend)
            {
                FriendsService.Instance.AddFriendAsync(memberId);
            }
        }
        public void DeclineFriendRequest(string memberId)
        {
            FriendsService.Instance.DeleteIncomingFriendRequestAsync(memberId);
        }

        public RelationshipType GetRelationshipWithPlayer(string playerId)
        {
            var relationships = FriendsService.Instance.Relationships;
            foreach (var relationship in relationships)
            {
                if (relationship.Member.Id == playerId)
                {
                    return relationship.Type;
                }
            }
            return 0;
        }
    }
}