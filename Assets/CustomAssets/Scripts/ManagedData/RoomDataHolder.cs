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

        public RoomData CreateRoomData(Guid guid, string roomName, string hostName, uint hostNetId) {
            CheckDictionary();
            if (ExistRoomByGuid(guid)) {
                return null;
            }

            var roomData = new RoomData {
                RoomGuid = guid,
                DateTime = DateTime.Now,
                CurrentHostNetId = hostNetId,
                RoomName = roomName,
                HostName = hostName,
            };
            roomDictionary.Add(guid, roomData);
            return roomData;
        }

        public void RemoveRoomData(Guid guid) {
            CheckDictionary();
            roomDictionary.Remove(guid);
        }

        public RoomData GetRoomData(Guid guid) {
            CheckDictionary();
            if (roomDictionary.TryGetValue(guid, out var roomData)) {
                return roomData;
            }

            return null;
        }

        public bool ExistRoomByPlayer(uint playerNetId) {
            CheckDictionary();
            return GetAllRoomData().Exists(d => d.CurrentHostNetId == playerNetId);
        }

        public bool ExistRoomByGuid(Guid guid) {
            CheckDictionary();
            return roomDictionary.ContainsKey(guid);
        }
    }
}