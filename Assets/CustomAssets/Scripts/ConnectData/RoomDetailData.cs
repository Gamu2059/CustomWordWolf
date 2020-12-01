using System;
using System.Collections.Generic;

namespace ConnectData {
    public struct RoomDetailData {
        public Guid RoomGuid;
        public string RoomName;
        public int MaxMemberNum;
        public List<PlayerSimpleData> PlayerDataList;
    }
}