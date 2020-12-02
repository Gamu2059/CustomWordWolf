using ConnectData;
using Manager;

namespace Api {
    public class JoinRoomApi : ApiBase<JoinRoom.Request, JoinRoom.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, JoinRoom.Request request) {
            networkManager.RequestJoinRoom(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnJoinRoomResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnJoinRoomResponseEvent -= OnGetResponseEvent;
        }
    }
}