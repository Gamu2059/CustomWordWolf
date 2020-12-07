using System;
using ConnectData;
using Manager;

namespace Api {
    public class ShowThemeReceiver : ReceiverApiBase<ShowTheme.SendRoom> {
        public ShowThemeReceiver(Action<ShowTheme.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnShowThemeReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnShowThemeReceiveEvent -= Invoke;
        }
    }
}