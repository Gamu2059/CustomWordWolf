using ConnectData;
using Mirror;

namespace Api {
    public class FetchTheme {
        public struct Response : NetworkMessage {
            public Result Result;
            public ThemeData ThemeData;
        }
        
        public enum Result {
            Failure,
            Succeed,
        }
    }
}