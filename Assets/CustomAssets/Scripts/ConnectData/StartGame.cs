using System;
using System.Collections.Generic;
using Common;
using Mirror;

namespace ConnectData {
    public class StartGame {
        public struct Request : NetworkMessage {
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }

        public struct SendRoom : NetworkMessage {
            public string Theme;
            public int GameTime;
            public DateTime GameStartDateTime;
            public string PeopleTheme;
            public string WolfTheme;
            public List<int> WolfMemberList;
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