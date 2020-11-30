using Mirror;
using UniRx;

namespace ConnectData {
    public class VotePlayer {
        public struct Request : NetworkMessage {
            /// <summary>
            /// 投票先のプレイヤーのID
            /// </summary>
            public uint VoteForwardPlayerNetId;
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
        }
        
        public enum Result {
            Succeed,
            Failure,
        }
        
        public struct SendRoom : NetworkMessage {
            /// <summary>
            /// 投票元のプレイヤーのID
            /// </summary>
            public uint VoteOriginPlayerNetId;
        }
    }
}