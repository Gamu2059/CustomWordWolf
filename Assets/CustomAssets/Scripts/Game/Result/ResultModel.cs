using System;
using ConnectData;

namespace Game.Result {
    public class ResultModel {
        public RoomDetailData RoomData { get; private set; }

        public void SetRoomData(RoomDetailData roomData) {
            RoomData = roomData;
        }
    }
}