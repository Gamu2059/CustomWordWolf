using System;
using ConnectData;
using Manager;

namespace Api {
    public class ShowMemberReceiver : ReceiverApiBase<ShowMember.SendRoom> {
        public ShowMemberReceiver(Action<ShowMember.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnShowMemberReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnShowMemberReceiveEvent -= Invoke;
        }
    }
}