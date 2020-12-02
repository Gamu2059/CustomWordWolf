using System;
using Api;
using Common;
using ConnectData;
using CustomAssets.Scripts.Game;
using Cysharp.Threading.Tasks;
using Dialog;
using Game.Ready;
using Game.Result;
using Manager;
using UnityEngine;

namespace Game {
    public class GameGroupArg : IChangeStateArg {
        public Guid RoomGuid;
    }

    public class GameGroupPresenter :
        StateMachineMonoBehavior<GameState>,
        IStateChangeable,
        IStateMachineInjectable<GroupState> {
        [SerializeField]
        private GameGroupView view;

        [SerializeField]
        private ReadyPresenter readyPresenter;

        [SerializeField]
        private PlayPresenter playPresenter;

        [SerializeField]
        private ResultPresenter resultPresenter;

        private StateMachine<GroupState> parentStateMachine;

        protected override void OnInitialize() {
            base.OnInitialize();
            view.Initialize();

            InitializeChild();
            BindState();
            InjectStateMachine();
        }

        private void InitializeChild() {
            readyPresenter.Initialize();
            playPresenter.Initialize();
            resultPresenter.Initialize();
        }

        private void BindState() {
            StateDictionary.Add(GameState.Ready, readyPresenter);
            StateDictionary.Add(GameState.Play, playPresenter);
            StateDictionary.Add(GameState.Result, resultPresenter);
        }

        private void InjectStateMachine() {
            readyPresenter.InjectStateMachine(StateMachine);
            playPresenter.InjectStateMachine(StateMachine);
            resultPresenter.InjectStateMachine(StateMachine);
        }

        public void InjectStateMachine(StateMachine<GroupState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            BindEvents();

            var gameGroupArg = arg as GameGroupArg;
            if (gameGroupArg == null) {
                Debug.LogError("ステート遷移の引数が適切ではありません");
                return;
            }

            StateMachine.RequestChangeState(GameState.Ready, new ReadyArg {RoomGuid = gameGroupArg.RoomGuid});
        }

        public async UniTask StateOutAsync() {
            StateMachine.RequestChangeState(GameState.None);
            UnbindEvents();
            await view.HideAsync();
        }

        private void BindEvents() {
            readyPresenter.OnLeaveRoomEvent += OnRoomLeave;
        }

        private void UnbindEvents() {
            readyPresenter.OnLeaveRoomEvent -= OnRoomLeave;
        }

        private async void OnRoomLeave() {
            var leaveRoomApi = new LeaveRoomApi();
            var response = await leaveRoomApi.Request(new LeaveRoom.Request());

            if (response.Result == LeaveRoom.Result.Succeed) {
                parentStateMachine.RequestChangeState(GroupState.Lobby);
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
        }
    }
}