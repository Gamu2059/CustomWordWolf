using System;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;

namespace Api {
    public abstract class ApiBase<TRequest, TResponse>
        where TRequest : NetworkMessage
        where TResponse : NetworkMessage {
        private bool isReserveResponse;
        private TResponse response;

        public async UniTask<TResponse> Request(TRequest request) {
            if (NetworkManager.singleton is CustomNetworkManager networkManager) {
                isReserveResponse = false;
                BindResponse(networkManager);
                OnRequest(networkManager, request);
                await UniTask.WaitUntil(() => isReserveResponse);
                UnbindResponse(networkManager);
                return response;
            }

            throw new Exception("NetworkManagerがAPIに対応していません");
        }

        protected abstract void OnRequest(CustomNetworkManager networkManager, TRequest request);

        protected abstract void BindResponse(CustomNetworkManager networkManager);

        protected abstract void UnbindResponse(CustomNetworkManager networkManager);
        
        protected void OnGetResponseEventEventEventEventEventEventEvent(TResponse responseData) {
            response = responseData;
            isReserveResponse = true;
        }
    }
}