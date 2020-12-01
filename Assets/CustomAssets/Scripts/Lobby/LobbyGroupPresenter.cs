using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Api;
using Api.SequenceCommand;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Dialog;
using Lobby.EditPlayerName;
using Lobby.JoinRoom;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class LobbyGroupPresenter :
        StateMachineMonoBehavior<LobbyState>,
        IStateChangeable,
        IStateMachineInjectable<GroupState> {
        [SerializeField]
        private LobbyGroupView view;

        [SerializeField]
        private JoinRoomPresenter joinRoomPresenter;

        [SerializeField]
        private EditPlayerNamePresenter editPlayerNamePresenter;

        private StateMachine<GroupState> parentStateMachine;

        protected override void OnInitialize() {
            base.OnInitialize();
            view.Initialize();

            InitializeChild();
            BindState();
            InjectStateMachine();
        }

        private void InitializeChild() {
            joinRoomPresenter.Initialize();
            editPlayerNamePresenter.Initialize();
        }

        private void BindState() {
            StateDictionary.Add(LobbyState.JoinRoom, joinRoomPresenter);
            StateDictionary.Add(LobbyState.EditPlayerName, editPlayerNamePresenter);
        }

        private void InjectStateMachine() {
            joinRoomPresenter.InjectStateMachine(StateMachine);
            editPlayerNamePresenter.InjectStateMachine(StateMachine);
        }

        public void InjectStateMachine(StateMachine<GroupState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            BindEvents();
            StateMachine.RequestChangeState(LobbyState.JoinRoom);
        }

        public async UniTask StateOutAsync() {
            UnbindEvents();
            await view.HideAsync();
        }

        private void BindEvents() {
            joinRoomPresenter.OnTitleBackEvent += OnTitleBack;
            joinRoomPresenter.OnJoinRoomDecidedEvent += OnJoinRoomDecided;

            editPlayerNamePresenter.OnPlayerNameEditedEvent += OnPlayerNameEdited;
        }

        private void UnbindEvents() {
            editPlayerNamePresenter.OnPlayerNameEditedEvent -= OnPlayerNameEdited;

            joinRoomPresenter.OnJoinRoomDecidedEvent -= OnJoinRoomDecided;
            joinRoomPresenter.OnTitleBackEvent -= OnTitleBack;
        }

        private async void OnTitleBack() {
            var disconnectClient = new DisconnectClient();
            var (isSuccess, errorCode) = await disconnectClient.DisconnectClientAsync();

            if (isSuccess) {
                parentStateMachine.RequestChangeState(GroupState.Title);
            } else {
                Debug.LogError("サーバとの接続に失敗しました " + errorCode);
            }
        }

        private async void OnJoinRoomDecided(Guid roomGuid) {
            var joinRoomApi = new JoinRoomApi();
            var response = await joinRoomApi.Request(new ConnectData.JoinRoom.Request() {RoomGuid = roomGuid});

            if (response.Result == ConnectData.JoinRoom.Result.Succeed) {
                parentStateMachine.RequestChangeState(GroupState.Game);
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
                joinRoomPresenter.SetPlayerName(playerName);
                StateMachine.RequestBackState();
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