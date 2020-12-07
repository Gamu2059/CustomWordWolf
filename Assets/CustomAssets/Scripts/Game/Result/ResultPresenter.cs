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

            peopleListPresenter.Initialize();
            wolfListPresenter.Initialize();
        }

        public void InjectStateMachine(StateMachine<GameState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
            SetApiEvents();

            view.SetPeopleTheme("？？？");
            view.SetWolfTheme("？？？");

            peopleListPresenter.Refresh();
            wolfListPresenter.Refresh();

            if (arg is ResultArg resultArg) {
                model.SetResultArg(resultArg);

                var getRoomDetailApi = new GetRoomDetailApi();
                var response = await getRoomDetailApi.Request(new GetRoomDetailData.Request
                    {RoomGuid = resultArg.RoomData.RoomGuid});

                if (response.Result == GetRoomDetailData.Result.Succeed) {
                    model.SetRoomData(response.RoomData);
                    view.SetActiveHostButton(response.IsHost);
                }
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
                    .AddTo(gameObject),
                view.ShowThemeObservable
                    .Subscribe(_ => ShowTheme())
                    .AddTo(gameObject),
                view.ShowMemberObservable
                    .Subscribe(_ => ShowMember())
                    .AddTo(gameObject)
            );
        }

        private void SetApiEvents() {
            // これらは、リザルトからプレイに直接遷移する可能性があるためにイベントを設定している
            apiDisposable = new CompositeDisposable(
                new UpdateMemberReceiver(OnUpdateMember),
                new StartGameReceiver(OnStartGame),
                new ShowThemeReceiver(OnShowTheme),
                new ShowMemberReceiver(OnShowMember)
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

        private void ShowTheme() {
            new ShowThemeApi().Request(new ShowTheme.Request()).Forget();
        }

        private void OnShowTheme(ShowTheme.SendRoom data) {
            view.SetPeopleTheme(model.PeopleTheme);
            view.SetWolfTheme(model.WolfTheme);
        }

        private void ShowMember() {
            new ShowMemberApi().Request(new ShowMember.Request()).Forget();
        }

        private void OnShowMember(ShowMember.SendRoom data) {
            Debug.LogFormat("roomData {0} wolf {1}", model.RoomData, model.WolfMemberList);
            peopleListPresenter.ShowMember(model.RoomData, model.WolfMemberList, false);
            wolfListPresenter.ShowMember(model.RoomData, model.WolfMemberList, true);
        }

        private void OnUpdateMember(UpdateMember.SendRoom data) {
            model.SetRoomData(data.RoomData);
            view.SetActiveHostButton(data.IsHost);
        }

        private void OnStartGame(StartGame.SendRoom data) {
            var arg = new PlayArg();
            arg.StartGameData = data;
            arg.RoomData = model.RoomData;
            parentStateMachine.RequestChangeState(GameState.Play, arg);
        }
    }
}