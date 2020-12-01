using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using UnityEngine;

namespace Api.SequenceCommand {
    public class DisconnectClient {
        private bool isDone;
        private bool isSuccess;
        private int errorCode;

        public async UniTask<(bool, int)> DisconnectClientAsync() {
            var networkManager = NetworkManager.singleton as CustomNetworkManager;
            if (networkManager == null) {
                Debug.LogError("適切なネットワークマネージャではありません");
                return (false, -1);
            }

            isDone = false;

            networkManager.OnStopClientEvent += OnConnectSucceed;
            networkManager.OnClientErrorEvent += OnConnectError;
            networkManager.StopClient();

            await UniTask.WaitUntil(() => isDone);

            networkManager.OnStopClientEvent -= OnConnectSucceed;
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