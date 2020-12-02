using System;
using Manager;
using Mirror;
using UnityEngine;

namespace Api {
    public abstract class ReceiverApiBase<TSendData> : IDisposable where TSendData : NetworkMessage {
        private event Action<TSendData> OnReceived;

        protected ReceiverApiBase(Action<TSendData> action) {
            OnReceived += action;

            if (NetworkManager.singleton is CustomNetworkManager networkManager) {
                Bind(networkManager);
            }
        }

        public void Dispose() {
            if (NetworkManager.singleton is CustomNetworkManager networkManager) {
                Unbind(networkManager);
            }

            OnReceived = null;
        }

        protected void Invoke(TSendData data) {
            OnReceived?.Invoke(data);
        }

        protected abstract void Bind(CustomNetworkManager networkManager);

        protected abstract void Unbind(CustomNetworkManager networkManager);
    }
}