using ConnectData;

namespace Game.Ready {
    public class ReadyModel {
        public RoomDetailData RoomData { get; private set; }

        public void SetRoomData(RoomDetailData roomData) {
            RoomData = roomData;
        }
    }
}