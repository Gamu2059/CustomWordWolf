using System;
using ConnectData;
using Manager;

namespace Api {
    public class TimeOverReceiver : ReceiverApiBase<TimeOver.SendRoom> {
        public TimeOverReceiver(Action<TimeOver.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnTimeOverReceived += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnTimeOverReceived -= Invoke;
        }
    }
}