using ConnectData;
using Manager;

namespace Api {
    public class ShowThemeApi : ApiBase<ShowTheme.Request, ShowTheme.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ShowTheme.Request request) {
            networkManager.RequestShowTheme(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShowThemeResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShowThemeResponseEvent -= OnGetResponseEvent;
        }
    }
}