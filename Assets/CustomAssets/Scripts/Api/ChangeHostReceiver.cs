using System;
using ConnectData;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;

namespace Api {
    public class ChangeHostReceiver : ReceiverApiBase<ChangeHost.SendPlayer> {
        public ChangeHostReceiver(Action<ChangeHost.SendPlayer> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnUpdateMemberReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnUpdateMemberReceiveEvent -= Invoke;
        }
    }
}