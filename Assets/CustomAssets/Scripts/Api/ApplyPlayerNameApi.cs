using ConnectData;
using Manager;

namespace Api {
    public class ApplyPlayerNameApi : ApiBase<ApplyPlayerName.Request, ApplyPlayerName.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ApplyPlayerName.Request request) {
            networkManager.RequestApplyPlayerName(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnApplyPlayerNameResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnApplyPlayerNameResponseEvent -= OnGetResponseEvent;
        }
    }
}