using Mirror;

namespace ConnectData {
    public class ApplyPlayerName {
        public struct Request : NetworkMessage {
            public string PlayerName;
        }
        
        public struct Response : NetworkMessage {
            public Result Result;
        }
        
        public enum Result {
            Succeed,
            Failure,
        }
    }
}