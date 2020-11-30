using System;
using ManagedData;
using Mirror;

namespace ConnectData {
    public class JoinRoom {
        public struct Request : NetworkMessage {
            public Guid RoomGuid;
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public ConnectRoomData JoinedRoomData;
        }
        
        public enum Result {
            Succeed,
            NotExist,
            Started,
        }
    }
}