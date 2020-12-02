using Mirror;

namespace ConnectData {
    public class UpdateMember {
        public struct SendRoom : NetworkMessage {
            public bool IsHost;
            public RoomDetailData RoomData;
        }
    }
}