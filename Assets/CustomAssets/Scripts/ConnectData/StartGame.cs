using System;
using Mirror;

namespace ConnectData {
    public class StartGame {
        public struct Request : NetworkMessage {
        }

        public struct Response : NetworkMessage {
            public Result Result;
        }

        public enum Result {
            Succeed,
            Failure,
        }

        public struct SendRoom : NetworkMessage {
            public string Theme;
            public int RemainTime;
            public DateTime CountStartDateTime;
        }
    }
}