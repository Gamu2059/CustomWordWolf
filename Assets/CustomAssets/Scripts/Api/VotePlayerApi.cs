using ConnectData;
using Manager;

namespace Api {
    public class VotePlayerApi : ApiBase<VotePlayer.Request, VotePlayer.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, VotePlayer.Request request) {
            networkManager.RequestVotePlayer(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnVotePlayerResponseEvent += OnGetResponseEventEventEventEventEventEventEvent;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnVotePlayerResponseEvent -= OnGetResponseEventEventEventEventEventEventEvent;
        }
    }
}