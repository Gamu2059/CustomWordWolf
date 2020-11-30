using ConnectData;
using Manager;

namespace Api {
    public class LoadGameSceneApi : ApiBase<LoadGameScene.Request, LoadGameScene.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, LoadGameScene.Request request) {
            networkManager.RequestLoadGameScene(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnLoadGameSceneResponse += OnResponse;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnLoadGameSceneResponse -= OnResponse;
        }
    }
}