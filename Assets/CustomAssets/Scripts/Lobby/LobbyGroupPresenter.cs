using System;
using System.Collections;
using System.Collections.Generic;
using Api;
using Api.SequenceCommand;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Dialog;
using Lobby.JoinRoom;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class LobbyGroupPresenter :
        StateMachineMonoBehavior<LobbyState>,
        IStateChangeable,
        IStateRequestable<GroupState, IChangeStateArg> {
        [SerializeField]
        private LobbyGroupView view;

        [SerializeField]
        private JoinRoomPresenter joinRoomPresenter;

        public event Func<GroupState, IChangeStateArg, bool> OnStateRequestEvent;

        protected override void OnInitialize() {
            base.OnInitialize();
            view.Initialize();
            
            StateDictionary.Add(LobbyState.JoinRoom, joinRoomPresenter);
            InitializeChild();
        }

        private void InitializeChild() {
            joinRoomPresenter.Initialize();
        }

        public async UniTask StateInAsync(IChangeStateArg arg) {
            StateMachine.StateRequestChange(LobbyState.JoinRoom);
            BindEvents();
            await view.ShowAsync();
        }

        public async UniTask StateOutAsync() {
            Debug.Log("LobbyOut");
            await view.HideAsync();
            UnbindEvents();
        }

        private void BindEvents() {
            joinRoomPresenter.OnStateRequestEvent += StateMachine.StateRequestChange;

            joinRoomPresenter.OnTitleBackEvent += OnTitleBack;
            joinRoomPresenter.OnJoinRoomDecidedEvent += OnJoinRoomDecided;
        }

        private void UnbindEvents() {
            joinRoomPresenter.OnJoinRoomDecidedEvent -= OnJoinRoomDecided;
            joinRoomPresenter.OnTitleBackEvent -= OnTitleBack;

            joinRoomPresenter.OnStateRequestEvent -= StateMachine.StateRequestChange;
        }

        private async void OnTitleBack() {
            var disconnectClient = new DisconnectClient();
            var (isSuccess, errorCode) = await disconnectClient.DisconnectClientAsync();

            if (isSuccess) {
                var isa = OnStateRequestEvent?.Invoke(GroupState.Title, null);
            } else {
                Debug.LogError("サーバとの接続に失敗しました " + errorCode);
            }
        }

        private async void OnJoinRoomDecided(Guid roomGuid) {
            var joinRoomApi = new JoinRoomApi();
            var response = await joinRoomApi.Request(new ConnectData.JoinRoom.Request() {RoomGuid = roomGuid});

            if (response.Result == ConnectData.JoinRoom.Result.Succeed) {
                OnStateRequestEvent?.Invoke(GroupState.Game, null);
                return;
            }

            using (var dialog = DialogManager.Instance.GetDialog<GeneralDialogPresenter>()) {
                if (dialog != null) {
                    var arg = new GeneralDialogArg {
                        Title = "エラー",
                        Message = response.Result.ToString()
                    };
                    await dialog.ShowAsync(arg);
                }
            }

            joinRoomPresenter.UpdateRoomList();
        }

        private async void OnPlayerNameEdited(string playerName) {
            var applyPlayerNameApi = new ApplyPlayerNameApi();
            var response = await applyPlayerNameApi.Request(new ApplyPlayerName.Request {PlayerName = playerName});
            if (response.Result == ApplyPlayerName.Result.Succeed) {
                PlayerPrefsManager.PlayerName = playerName;
                // view.SetPlayerName(playerName);
            }
        }

        private async void OnRoomNameDecided(string roomName) {
            var createRoomApi = new CreateRoomApi();
            var response = await createRoomApi.Request(new CreateRoom.Request {RoomName = roomName});
            if (response.Result == CreateRoom.Result.Succeed) {
            }
        }
    }
}