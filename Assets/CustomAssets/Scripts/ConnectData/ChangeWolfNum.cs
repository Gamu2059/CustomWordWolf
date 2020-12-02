using System;
using Common;
using Mirror;

namespace ConnectData {
    public class ChangeWolfNum {
        public struct Request : NetworkMessage {
            /// <summary>
            /// 変更方向。0より大きければ増やし、0より小さければ減らす。
            /// </summary>
            public int ChangeForward;
        }

        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }

        public struct SendRoom : NetworkMessage, IOptionMessage {
            public bool IsLowerLimit { get; set; }
            public bool IsUpperLimit { get; set; }
            public int NewWolfNum;
        }

        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureNonHost,
            FailurePlaying,
            NoChange,
        }
    }
}