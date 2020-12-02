using ConnectData;
using Manager;

namespace Api {
    public class LeaveRoomApi : ApiBase<LeaveRoom.Request, LeaveRoom.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, LeaveRoom.Request request) {
            networkManager.RequestLeaveRoom(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnLeaveRoomResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnLeaveRoomResponseEvent -= OnGetResponseEvent;
        }
    }
}