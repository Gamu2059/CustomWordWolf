using System;
using System.Collections.Generic;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Game.Ready;
using UniRx;
using UnityEngine;

namespace Game.Result {
    public class ResultArg : IChangeStateArg {
        public RoomDetailData RoomData;
        public string PeopleTheme;
        public string WolfTheme;
        public List<int> WolfMemberList;
    }

    public class ResultPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<GameState> {
        [SerializeField]
        private ResultView view;

        [SerializeField]
        private ResultListPresenter peopleListPresenter;

        [SerializeField]
        private ResultListPresenter wolfListPresenter;

        private StateMachine<GameState> parentStateMachine;
        private ResultModel model;

        private CompositeDisposable viewDisposable;
        private CompositeDisposable apiDisposable;

        public void Initialize() {
            model = new ResultModel();
            view.Initialize();
        }

        public void InjectStateMachine(StateMachine<GameState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
            SetApiEvents();

            if (arg is ResultArg resultArg) {
                model.SetRoomData(resultArg.RoomData);
                view.SetPeopleTheme(resultArg.PeopleTheme);
                view.SetWolfTheme(resultArg.WolfTheme);
                
                
            }
        }

        public async UniTask StateOutAsync() {
            DisposeApiEvents();
            DisposeViewEvents();
            await view.HideAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackObservable
                    .Subscribe(_ => BackReady())
                    .AddTo(gameObject)
            );
        }

        private void SetApiEvents() {
            // これらは、リザルトからプレイに直接遷移する可能性があるためにイベントを設定している
            apiDisposable = new CompositeDisposable(
                new UpdateMemberReceiver(OnUpdateMember),
                new StartGameReceiver(OnStartGame)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        private void DisposeApiEvents() {
            apiDisposable?.Dispose();
            apiDisposable = null;
        }

        private void BackReady() {
            parentStateMachine.RequestChangeState(GameState.Ready, new ReadyArg {RoomGuid = model.RoomData.RoomGuid});
        }

        private void OnUpdateMember(UpdateMember.SendRoom data) {
            model.SetRoomData(data.RoomData);
        }

        private void OnStartGame(StartGame.SendRoom data) {
            var arg = new PlayArg();
            arg.GameTime = data.GameTime;
            arg.Theme = data.Theme;
            arg.GameStartDateTime = data.GameStartDateTime;
            arg.RoomData = model.RoomData;
            parentStateMachine.RequestChangeState(GameState.Play, arg);
        }
    }
}