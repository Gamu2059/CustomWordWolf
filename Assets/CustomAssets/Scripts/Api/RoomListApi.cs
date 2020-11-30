using ConnectData;
using Manager;

namespace Api {
    public class RoomListApi : ApiBase<RoomList.Request, RoomList.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, RoomList.Request request) {
            networkManager.RequestRoomList();
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnRoomListResponse += OnResponse;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnRoomListResponse -= OnResponse;
        }
    }
}