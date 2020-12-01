using System;
using ConnectData;
using Manager;

namespace Api {
    public class StartGameReceiver : ReceiverApiBase<StartGame.SendRoom> {
        public StartGameReceiver(Action<StartGame.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnStartGameReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnStartGameReceiveEvent -= Invoke;
        }
    }
}