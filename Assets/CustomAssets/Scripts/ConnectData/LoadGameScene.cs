using Mirror;

namespace ConnectData {
    public class LoadGameScene {
        public struct Request : NetworkMessage {
            
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
            public bool IsHost;
        }
        
        public enum Result {
            Succeed,
            Failure,
        }
    }
}