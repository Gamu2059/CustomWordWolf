using ConnectData;
using Manager;

namespace Api {
    public class ChangeGameTimeApi : ApiBase<ChangeGameTime.Request, ChangeGameTime.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ChangeGameTime.Request request) {
            networkManager.RequestChangeGameTime(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnChangeGameTimeResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnChangeGameTimeResponseEvent -= OnGetResponseEvent;
        }
    }
}