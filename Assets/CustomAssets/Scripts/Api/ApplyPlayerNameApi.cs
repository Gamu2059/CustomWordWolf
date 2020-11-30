using ConnectData;
using Manager;

namespace Api {
    public class ApplyPlayerNameApi : ApiBase<ApplyPlayerName.Request, ApplyPlayerName.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ApplyPlayerName.Request request) {
            networkManager.RequestApplyName(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnApplyPlayerNameResponse += OnResponse;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnApplyPlayerNameResponse -= OnResponse;
        }
    }
}