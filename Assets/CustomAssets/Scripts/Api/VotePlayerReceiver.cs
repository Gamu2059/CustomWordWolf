using System;
using ConnectData;
using Manager;

namespace Api {
    public class VotePlayerReceiver : ReceiverApiBase<VotePlayer.SendRoom> {
        public VotePlayerReceiver(Action<VotePlayer.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnVotePlayerReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnVotePlayerReceiveEvent -= Invoke;
        }
    }
}