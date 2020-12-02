using System;
using System.Linq;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Game.PlayerList;
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

        private StateMachine<GameState> parentStateMachine;
        private CompositeDisposable viewDisposable;
        private IDisposable updateMemberDisposable;
        private IDisposable startGameDisposable;

        public event Action OnLeaveRoomEvent;

        public void Initialize() {
            listPresenter.Initialize();
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
            if (response.Result == GetRoomDetailData.Result.Succeed) {
                listPresenter.SetMember(response.RoomData);
            }
        }

        public async UniTask StateOutAsync() {
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
            listPresenter.UpdateMember(data.RoomData);
        }

        private void ApplyViewOnUpdateMember(RoomDetailData data) {
            
        }

        private void OnStartGame(StartGame.SendRoom data) {
            parentStateMachine.RequestChangeState(GameState.Play);
        }
    }
}