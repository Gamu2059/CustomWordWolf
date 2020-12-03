using System;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Game.PlayerList;
using Game.Ready;
using Game.Result;
using UniRx;
using UnityEngine;

namespace Game {
    public class PlayArg : IChangeStateArg {
        public StartGame.SendRoom StartGameData;
        public RoomDetailData RoomData;
    }

    public class PlayPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<GameState> {
        [SerializeField]
        private PlayView view;

        [SerializeField]
        private PlayerListPresenter listPresenter;

        private StateMachine<GameState> parentStateMachine;
        private PlayModel model;

        private CompositeDisposable viewDisposable;
        private CompositeDisposable modelDisposable;

        public void Initialize() {
            model = new PlayModel();
            view.Initialize();
            listPresenter.Initialize();
        }

        public void InjectStateMachine(StateMachine<GameState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
            SetModelEvents();

            if (arg is PlayArg playArg) {
                model.StartGame(playArg);
                view.SetTheme(playArg.StartGameData.Theme);
                listPresenter.SetMember(playArg.RoomData);
            }
        }

        public async UniTask StateOutAsync() {
            DisposeModelEvents();
            DisposeViewEvents();
            await view.HideAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackReadyObservable
                    .Subscribe(_ => BackReady())
                    .AddTo(gameObject)
            );
        }

        private void SetModelEvents() {
            modelDisposable = new CompositeDisposable(
                model.GameTimeObservable
                    .Subscribe(t => view.SetGameTime(t))
                    .AddTo(gameObject),
                model.TimeOverObservable
                    .Subscribe(_ => OnTimeOver())
                    .AddTo(gameObject)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        private void DisposeModelEvents() {
            modelDisposable?.Dispose();
            modelDisposable = null;
        }

        private void OnTimeOver() {
            var arg = new ResultArg();
            arg.RoomData = model.PlayArg.RoomData;
            arg.PeopleTheme = model.PlayArg.StartGameData.PeopleTheme;
            arg.WolfTheme = model.PlayArg.StartGameData.WolfTheme;
            arg.WolfMemberList = model.PlayArg.StartGameData.WolfMemberList;
            parentStateMachine.RequestChangeState(GameState.Result, arg);
        }

        private void BackReady() {
            parentStateMachine.RequestChangeState(GameState.Ready,
                new ReadyArg {RoomGuid = model.PlayArg.RoomData.RoomGuid});
        }
    }
}