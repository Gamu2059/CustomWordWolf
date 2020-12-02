using System.Collections.Generic;
using Mirror;

namespace ConnectData {
    public class TimeOver {
        public struct SendRoom : NetworkMessage {
            public string PeopleTheme;
            public string WolfTheme;
            public List<int> WolfMemberList;
        }
    }
}