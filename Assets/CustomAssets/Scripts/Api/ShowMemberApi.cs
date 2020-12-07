using ConnectData;
using Manager;

namespace Api {
    public class ShowMemberApi : ApiBase<ShowMember.Request, ShowMember.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, ShowMember.Request request) {
            networkManager.RequestShowMember(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShowMemberResponseEvent += OnGetResponseEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnShowMemberResponseEvent -= OnGetResponseEvent;
        }
    }
}