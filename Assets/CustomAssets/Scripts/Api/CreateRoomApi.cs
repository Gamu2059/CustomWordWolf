using System;
using ConnectData;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using UniRx;

namespace Api {
    public class CreateRoomApi : ApiBase<CreateRoom.Request, CreateRoom.Response> {
        protected override void OnRequest(CustomNetworkManager networkManager, CreateRoom.Request request) {
            networkManager.RequestCreateRoom(request);
        }

        protected override void BindResponse(CustomNetworkManager networkManager) {
            networkManager.OnCreateRoomResponse += OnResponse;
        }

        protected override void UnbindResponse(CustomNetworkManager networkManager) {
            networkManager.OnCreateRoomResponse -= OnResponse;
        }
    }
}