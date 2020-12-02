using System;
using ConnectData;
using Manager;

namespace Api {
    public class ChangeGameTimeReceiver : ReceiverApiBase<ChangeGameTime.SendRoom> {
        public ChangeGameTimeReceiver(Action<ChangeGameTime.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnChangeGameTimeReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnChangeGameTimeReceiveEvent -= Invoke;
        }
    }
}