using System;
using System.Collections.Generic;
using ManagedData;
using Mirror;

namespace ConnectData {
    public class GetRoomList {
        public struct Request : NetworkMessage {
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
            public List<RoomSimpleData> RoomDataList;
        }

        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
        }
    }
}