using System;
using Mirror;
using UniRx;

namespace ConnectData {
    public class VotePlayer {
        public struct Request : NetworkMessage {
            /// <summary>
            /// 投票先のプレイヤーのID
            /// </summary>
            public int VoteForwardPlayerConnectionId;
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public Exception Exception;
        }
        
        public enum Result {
            FailureUnknown,
            Succeed,
            FailureNonExistPlayer,
            FailureNoJoinRoom,
            FailureNoPlaying,
        }
        
        public struct SendRoom : NetworkMessage {
            /// <summary>
            /// 投票元のプレイヤーのID
            /// </summary>
            public int VoteOriginPlayerConnectionId;
        }
    }
}