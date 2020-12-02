using System;
using Mirror;

namespace ConnectData {
    public class GetRoomDetailData {
        public struct Request : NetworkMessage {
            public Guid RoomGuid;
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
            public bool IsHost;
            public RoomDetailData RoomData;
            public ChangeGameTime.SendRoom GameTime;
            public ChangeWolfNum.SendRoom WolfNum;
        }

        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureNonExistRoom,
        }
    }
}