using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using UnityEngine;

namespace Api.SequenceCommand {
    public class StartClient {
        private bool isDoneConnect;
        private bool isSuccessConnect;
        private int errorCode;

        public async UniTask<(bool, int)> StartClientAsync() {
            var networkManager = NetworkManager.singleton as CustomNetworkManager;
            if (networkManager == null) {
                Debug.LogError("適切なネットワークマネージャではありません");
                return (false, -1);
            }

            isDoneConnect = false;

            networkManager.OnClientConnectEvent += OnConnectSucceed;
            networkManager.OnClientErrorEvent += OnConnectError;
            networkManager.StartClient();

            await UniTask.WaitUntil(() => isDoneConnect);

            networkManager.OnClientConnectEvent -= OnConnectSucceed;
            networkManager.OnClientErrorEvent -= OnConnectError;

            return (isSuccessConnect, errorCode);
        }

        private void OnConnectSucceed() {
            isDoneConnect = true;
            isSuccessConnect = true;
            errorCode = -1;
        }

        private void OnConnectError(int errorCode) {
            isDoneConnect = true;
            isSuccessConnect = false;
            this.errorCode = errorCode;
        }
    }
}