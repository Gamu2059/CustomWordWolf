using System;
using System.Linq;
using System.Collections.Generic;
using Mirror;
using UniRx.Triggers;

namespace ManagedData {
    public class RoomDataHolder {
        private Dictionary<Guid, RoomData> roomDictionary;

        private void CheckDictionary() {
            if (roomDictionary == null) {
                roomDictionary = new Dictionary<Guid, RoomData>();
            }
        }

        public List<RoomData> GetAllRoomData() {
            CheckDictionary();
            return roomDictionary.Values.ToList();
        }

        public RoomData CreateRoomData(PlayerDataHolder playerDataHolder, NetworkConnection connection,
            ConstArg constArg, VariableArg variableArg) {
            CheckDictionary();

            var guid = Guid.NewGuid();
            while (ExistRoomByGuid(guid)) {
                guid = Guid.NewGuid();
            }

            constArg.RoomGuid = guid;
            var roomData = new RoomData(playerDataHolder, connection, constArg, variableArg);
            roomDictionary.Add(guid, roomData);
            return roomData;
        }

        public void RemoveRoomData(Guid guid) {
            CheckDictionary();
            roomDictionary.Remove(guid);
        }

        public RoomData GetRoomDataByGuid(Guid guid) {
            CheckDictionary();
            if (roomDictionary.TryGetValue(guid, out var roomData)) {
                return roomData;
            }

            return null;
        }

        public RoomData GetRoomDataByContainPlayer(int playerConnectionId) {
            CheckDictionary();
            return roomDictionary.Values.FirstOrDefault(d => d.ContainMember(playerConnectionId));
        }

        public RoomData GetRoomDataByHostPlayer(int hostPlayerConnectionId) {
            CheckDictionary();
            return roomDictionary.Values.FirstOrDefault(d => d.HostConnectionId == hostPlayerConnectionId);
        }

        public bool ExistRoomByGuid(Guid guid) {
            CheckDictionary();
            return roomDictionary.ContainsKey(guid);
        }

        public bool ExistRoomByContainPlayer(int playerConnectionId) {
            CheckDictionary();
            return roomDictionary.Values.Any(d => d.ContainMember(playerConnectionId));
        }

        public bool ExistRoomByHostPlayer(int hostPlayerConnectionId) {
            CheckDictionary();
            return roomDictionary.Values.Any(d => d.HostConnectionId == hostPlayerConnectionId);
        }
    }
}