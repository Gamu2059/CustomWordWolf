using ConnectData;
using Manager;

namespace Api {
    public class ShutOutGameApi : ApiBase<ShutOutGame.Request, ShutOutGame.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ShutOutGame.Request request) {
            networkManager.RequestShutOutGame(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShutOutGameResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShutOutGameResponseEvent -= OnGetResponseEvent;
        }
    }
}