using ConnectData;
using Manager;

namespace Api {
    public class StartGameApi : ApiBase<StartGame.Request, StartGame.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, StartGame.Request request) {
            networkManager.RequestStartGame(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnStartGameResponse += OnResponse;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnStartGameResponse -= OnResponse;
        }
    }
}