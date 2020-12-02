using System;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Game.PlayerList;
using Game.Result;
using UniRx;
using UnityEngine;

namespace Game {
    public class PlayArg : IChangeStateArg {
        public string Theme;
        public int GameTime;
        public DateTime GameStartDateTime;
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

        private CompositeDisposable modelDisposable;
        private CompositeDisposable apiDisposable;

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
            SetModelEvents();
            SetApiEvents();

            if (arg is PlayArg playArg) {
                model.StartGame(playArg);
                view.SetTheme(playArg.Theme);
                listPresenter.SetMember(playArg.RoomData);
            }
        }

        public async UniTask StateOutAsync() {
            DisposeApiEvents();
            DisposeModelEvents();
            await view.HideAsync();
        }

        private void SetModelEvents() {
            modelDisposable = new CompositeDisposable(
                model.GameTimeObservable
                    .Subscribe(t => view.SetGameTime(t))
                    .AddTo(gameObject)
            );
        }

        private void SetApiEvents() {
            apiDisposable = new CompositeDisposable(
                new TimeOverReceiver(OnTimeOverReceived)
            );
        }

        private void DisposeModelEvents() {
            modelDisposable?.Dispose();
            modelDisposable = null;
        }

        private void DisposeApiEvents() {
            apiDisposable?.Dispose();
            apiDisposable = null;
        }

        private void OnTimeOverReceived(TimeOver.SendRoom data) {
            model.TimeOverGame();
            var arg = new ResultArg();
            arg.RoomData = model.RoomData;
            arg.PeopleTheme = data.PeopleTheme;
            arg.WolfTheme = data.WolfTheme;
            arg.WolfMemberList = data.WolfMemberList;
            parentStateMachine.RequestChangeState(GameState.Result, arg);
        }
    }
}