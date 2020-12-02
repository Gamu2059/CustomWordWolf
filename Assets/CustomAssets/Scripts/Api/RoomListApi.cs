using ConnectData;
using Manager;

namespace Api {
    public class RoomListApi : ApiBase<GetRoomList.Request, GetRoomList.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, GetRoomList.Request request) {
            networkManager.RequestGetRoomList();
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnGetRoomListResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnGetRoomListResponseEvent -= OnGetResponseEvent;
        }
    }
}