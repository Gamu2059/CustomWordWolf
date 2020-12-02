using System;
using Mirror;

namespace ConnectData {
    public class LeaveRoom {
        public struct Request : NetworkMessage {
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }
        
        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureNoJoinRoom,
            FailureLeaveRoom,
        }
    }
}