using System;
using ConnectData;
using Manager;

namespace Api {
    public class ShutOutGameReceiver : ReceiverApiBase<ShutOutGame.SendRoom> {
        public ShutOutGameReceiver(Action<ShutOutGame.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnShutOutGameReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnShutOutGameReceiveEvent -= Invoke;
        }
    }
}