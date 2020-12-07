using System;
using System.Collections.Generic;
using ConnectData;

namespace Game.Result {
    public class ResultModel {
        public RoomDetailData RoomData { get; private set; }

        public string PeopleTheme { get; private set; }
        public string WolfTheme { get; private set; }
        public List<int> WolfMemberList { get; private set; }

        public void SetRoomData(RoomDetailData roomData) {
            RoomData = roomData;
        }

        public void SetResultArg(ResultArg resultArg) {
            PeopleTheme = resultArg.PeopleTheme;
            WolfTheme = resultArg.WolfTheme;
            WolfMemberList = resultArg.WolfMemberList;
        }
    }
}