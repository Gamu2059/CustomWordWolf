using System;
using ConnectData;
using Manager;

namespace Api {
    public class ChangeWolfNumReceiver : ReceiverApiBase<ChangeWolfNum.SendRoom> {
        public ChangeWolfNumReceiver(Action<ChangeWolfNum.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnChangeWolfNumReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnChangeWolfNumReceiveEvent -= Invoke;
        }
    }
}