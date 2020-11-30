using ManagedData;
using Mirror;

namespace ConnectData {
    public class CreateRoom {
        public struct Request : NetworkMessage {
            public string RoomName;
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public ConnectRoomData CreatedRoomData;
        }
        
        public enum Result {
            Succeed,
            Failure,
        }
    }
}