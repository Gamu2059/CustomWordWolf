using System;
using Mirror;

namespace ConnectData {
    public class ShowTheme {
        public struct Request : NetworkMessage {
            
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }
        
        public struct SendRoom : NetworkMessage {
            
        }
        
        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureNonHost,
            FailurePlaying,
        }
    }
}