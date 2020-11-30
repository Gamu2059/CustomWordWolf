using System;
using System.Collections.Generic;
using System.Linq;
using ConnectData;
using Mirror;
using UnityEditor.Experimental.GraphView;

namespace ManagedData {
    public enum RoomState {
        ReadyGame,
        PlayGame,
    }

    public class RoomData {
        public Guid RoomGuid;
        public DateTime DateTime;
        public uint CurrentHostNetId;
        public string RoomName;
        public string HostName;

        private RoomState state;
        private Dictionary<NetworkConnection, DateTime> memberDictionary;
        private Dictionary<uint, uint> voteDictionary;

        public RoomData() {
            state = RoomState.ReadyGame;
            memberDictionary = new Dictionary<NetworkConnection, DateTime>();
            voteDictionary = new Dictionary<uint, uint>();
        }

        public bool JoinRoom(NetworkConnection joinedPlayerConnection) {
            if (memberDictionary.ContainsKey(joinedPlayerConnection)) {
                return false;
            }

            memberDictionary.Add(joinedPlayerConnection, DateTime.Now);
            return true;
        }

        public bool LeaveRoom(NetworkConnection leftPlayerConnection) {
            if (!memberDictionary.ContainsKey(leftPlayerConnection)) {
                return false;
            }

            memberDictionary.Remove(leftPlayerConnection);
            return true;
        }

        public void StartGame() {
            state = RoomState.PlayGame;
            voteDictionary.Clear();
        }

        public void StopGame() {
            state = RoomState.ReadyGame;
            voteDictionary.Clear();
        }

        public void VotePlayer(uint voteOriginPlayerNetId, uint voteForwardPlayerNetId) {
            if (voteDictionary.ContainsKey(voteOriginPlayerNetId)) {
                voteDictionary[voteOriginPlayerNetId] = voteForwardPlayerNetId;
            } else {
                voteDictionary.Add(voteOriginPlayerNetId, voteForwardPlayerNetId);
            }
        }

        public IEnumerable<NetworkConnection> GetAllMember() {
            return memberDictionary.Keys;
        }

        public List<NetworkConnection> GetAllMemberList() {
            return GetAllMember().ToList();
        }

        public IEnumerable<NetworkConnection> GetAllMemberOrderByAccess() {
            return memberDictionary
                .OrderBy(m => m.Value)
                .Select(m => m.Key);
        }

        public List<NetworkConnection> GetAllMemberListOrderByAccess() {
            return GetAllMemberOrderByAccess().ToList();
        }

        public bool ContainMember(uint playerNetId) {
            return memberDictionary.Keys.Any(m => m.identity.netId == playerNetId);
        }

        public ConnectRoomData CreateConnectRoomData() {
            return new ConnectRoomData {
                RoomGuid = RoomGuid,
                RoomName = RoomName,
                HostName = HostName,
                MemberNum = memberDictionary.Count,
            };
        }
    }
}