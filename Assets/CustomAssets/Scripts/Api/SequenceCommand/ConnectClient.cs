using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using UnityEngine;

namespace Api.SequenceCommand {
    public class ConnectClient {
        private bool isDone;
        private bool isSuccess;
        private int errorCode;

        public async UniTask<(bool, int)> ConnectClientAsync() {
            var networkManager = NetworkManager.singleton as CustomNetworkManager;
            if (networkManager == null) {
                Debug.LogError("適切なネットワークマネージャではありません");
                return (false, -1);
            }

            isDone = false;

            networkManager.OnClientConnectEvent += OnConnectSucceed;
            networkManager.OnClientErrorEvent += OnConnectError;
            networkManager.StartClient();

            await UniTask.WaitUntil(() => isDone);

            networkManager.OnClientConnectEvent -= OnConnectSucceed;
            networkManager.OnClientErrorEvent -= OnConnectError;

            return (isSuccess, errorCode);
        }

        private void OnConnectSucceed() {
            isDone = true;
            isSuccess = true;
            errorCode = -1;
        }

        private void OnConnectError(int errorCode) {
            isDone = true;
            isSuccess = false;
            this.errorCode = errorCode;
        }
    }
}