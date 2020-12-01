using System;
using ManagedData;
using Mirror;

namespace ConnectData {
    public class CreateRoom {
        public struct Request : NetworkMessage {
            public string RoomName;
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }
        
        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureMultipleRoomHost,
        }
    }
}