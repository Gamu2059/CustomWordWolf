using System;
using System.Linq;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Game.PlayerList;
using Game.Ready.Option;
using UniRx;
using UnityEngine;

namespace Game.Ready {
    public class ReadyArg : IChangeStateArg {
        public Guid RoomGuid;
    }

    public class ReadyPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<GameState> {
        [SerializeField]
        private ReadyView view;

        [SerializeField]
        private PlayerListPresenter listPresenter;

        [SerializeField]
        private OptionPresenter optionPresenter;

        private StateMachine<GameState> parentStateMachine;
        private ReadyModel model;

        private CompositeDisposable viewDisposable;
        private IDisposable updateMemberDisposable;
        private IDisposable startGameDisposable;

        public event Action OnLeaveRoomEvent;

        public void Initialize() {
            model = new ReadyModel();
            listPresenter.Initialize();
            optionPresenter.Initialize();
        }

        public void InjectStateMachine(StateMachine<GameState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();

            updateMemberDisposable = new UpdateMemberReceiver(OnUpdateMember);
            startGameDisposable = new StartGameReceiver(OnStartGame);

            var readyArg = arg as ReadyArg;
            if (readyArg == null) {
                Debug.LogError("ステート遷移の引数が適切ではありません");
                return;
            }

            var getRoomDetailApi = new GetRoomDetailApi();
            var response = await getRoomDetailApi.Request(new GetRoomDetailData.Request {RoomGuid = readyArg.RoomGuid});

            await optionPresenter.ShowAsync();
            optionPresenter.OnChangeGameTime(response.GameTime);
            optionPresenter.OnChangeWolfNum(response.WolfNum);

            if (response.Result == GetRoomDetailData.Result.Succeed) {
                model.SetRoomData(response.RoomData);
                listPresenter.SetMember(response.RoomData);
                OnUpdateHostUi(response.IsHost, response.RoomData.PlayerDataList.Count);
            }
        }

        public async UniTask StateOutAsync() {
            await optionPresenter.HideAsync();

            startGameDisposable?.Dispose();
            startGameDisposable = null;

            updateMemberDisposable?.Dispose();
            updateMemberDisposable = null;

            DisposeViewEvents();
            await view.HideAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.LeaveRoomObservable
                    .Subscribe(_ => OnLeaveRoomEvent?.Invoke())
                    .AddTo(gameObject),
                view.StartGameObservable
                    .Subscribe(_ => StartGame())
                    .AddTo(gameObject)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        private async void StartGame() {
            var startGameApi = new StartGameApi();
            await startGameApi.Request(new StartGame.Request());
        }

        private void OnUpdateMember(UpdateMember.SendRoom data) {
            model.SetRoomData(data.RoomData);
            listPresenter.UpdateMember(data.RoomData);
            OnUpdateHostUi(data.IsHost, data.RoomData.PlayerDataList.Count);
        }

        private void OnUpdateHostUi(bool isHost, int memberNum) {
            view.SetActiveStartGameButton(isHost && memberNum > 1);
            optionPresenter.SetActiveOperatorButton(isHost);
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