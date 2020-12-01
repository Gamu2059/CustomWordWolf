using Mirror;

namespace ConnectData {
    public class RoomUpdate {
        public struct SendRoom : NetworkMessage {
            public RoomDetailData RoomData;
            public bool IsHost;
        }
    }
}