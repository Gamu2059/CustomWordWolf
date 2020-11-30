using System;
using System.Collections.Generic;
using ManagedData;
using Mirror;

namespace ConnectData {
    public class RoomList {
        public struct Request : NetworkMessage {
            
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public List<ConnectRoomData> RoomDataList;
        }
        
        public enum Result {
            Succeed,
            Failure,
        }
    }
}