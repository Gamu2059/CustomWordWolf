using System;
using Mirror;

namespace ConnectData {
    public class StartGame {
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
            FailureNonHost,
            FailurePlaying,
        }

        public struct SendRoom : NetworkMessage {
            public string Theme;
            public int GameTime;
            public DateTime GameStartDateTime;
        }
    }
}