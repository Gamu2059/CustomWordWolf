using ConnectData;
using Manager;

namespace Api {
    public class ChangeWolfNumApi : ApiBase<ChangeWolfNum.Request, ChangeWolfNum.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ChangeWolfNum.Request request) {
            networkManager.RequestChangeWolfNum(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnChangeWolfNumResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnChangeWolfNumResponseEvent -= OnGetResponseEvent;
        }
    }
}