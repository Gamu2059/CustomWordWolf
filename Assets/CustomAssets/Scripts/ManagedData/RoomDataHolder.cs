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

        public RoomData GetRoomDataByGuid(Guid guid) {
            CheckDictionary();
            if (roomDictionary.TryGetValue(guid, out var roomData)) {
                return roomData;
            }

            return null;
        }

        public RoomData GetRoomDataByContainPlayer(uint playerNetId) {
            CheckDictionary();
            return roomDictionary.Values.First(d => d.ContainMember(playerNetId));
        }

        public RoomData GetRoomDataByHostPlayer(uint hostPlayerNetId) {
            CheckDictionary();
            return roomDictionary.Values.First(d => d.CurrentHostNetId == hostPlayerNetId);
        }

        public bool ExistRoomByGuid(Guid guid) {
            CheckDictionary();
            return roomDictionary.ContainsKey(guid);
        }

        public bool ExistRoomByContainPlayer(uint playerNetId) {
            CheckDictionary();
            return roomDictionary.Values.Any(d => d.ContainMember(playerNetId));
        }

        public bool ExistRoomByHostPlayer(uint hostPlayerNetId) {
            CheckDictionary();
            return roomDictionary.Values.Any(d => d.CurrentHostNetId == hostPlayerNetId);
        }
    }
}