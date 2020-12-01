using ConnectData;
using Manager;

namespace Api {
    public class LoadGameSceneApi : ApiBase<GetRoomDetailData.Request, GetRoomDetailData.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, GetRoomDetailData.Request request) {
            networkManager.RequestGetRoomDetailData(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnGetRoomDetailDataResponseEvent += OnGetResponseEventEventEventEventEventEventEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnGetRoomDetailDataResponseEvent -= OnGetResponseEventEventEventEventEventEventEvent;
        }
    }
}