using System;
using Common;
using Cysharp.Threading.Tasks;
using Lobby.EditPlayerName;
using Lobby.JoinRoom.RoomList;
using UnityEngine;
using UniRx;
using UnityEngine.PlayerLoop;

namespace Lobby.JoinRoom {
    public class JoinRoomPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<LobbyState> {
        [SerializeField]
        private JoinRoomView view;

        [SerializeField]
        private JoinRoomListPresenter listPresenter;

        private StateMachine<LobbyState> parentStateMachine;
        private JoinRoomModel model;
        private CompositeDisposable viewDisposable;
        private CompositeDisposable modelDisposable;

        public event Action OnTitleBackEvent;
        public event Action<Guid> OnJoinRoomDecidedEvent;

        public void Initialize() {
            view.Initialize();
            model = new JoinRoomModel();

            listPresenter.Initialize();
        }

        public void InjectStateMachine(StateMachine<LobbyState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
            SetModelEvents();
            BindEvents();

            listPresenter.UpdateRoomList().Forget();
        }

        public async UniTask StateOutAsync() {
            UnbindEvents();
            DisposeModelEvents();
            DisposeViewEvents();
            await view.HideAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackTitleObservable
                    .Subscribe(_ => OnTitleBackEvent?.Invoke())
                    .AddTo(gameObject),
                view.EditPlayerNameObservable
                    .Subscribe(_ => EditPlayerName())
                    .AddTo(gameObject),
                view.UpdateRoomListObservable
                    .Subscribe(_ => UpdateRoomList())
                    .AddTo(gameObject),
                view.CreateRoomObservable
                    .Subscribe(_ => parentStateMachine.RequestChangeState(LobbyState.CreateRoom))
                    .AddTo(gameObject)
            );
        }

        private void SetModelEvents() {
            modelDisposable = new CompositeDisposable(
                model.PlayerNameObservable
                    .Subscribe(view.SetPlayerName)
                    .AddTo(gameObject)
            );
        }

        private void BindEvents() {
            listPresenter.OnJoinRoomDecidedEvent += OnJoinRoomDecided;
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        private void DisposeModelEvents() {
            modelDisposable?.Dispose();
            modelDisposable = null;
        }

        private void UnbindEvents() {
            listPresenter.OnJoinRoomDecidedEvent -= OnJoinRoomDecided;
        }

        private void OnJoinRoomDecided(Guid guid) {
            OnJoinRoomDecidedEvent?.Invoke(guid);
        }

        private void EditPlayerName() {
            var arg = new EditPlayerNameArg { PlayerName = model.PlayerName};
            parentStateMachine.RequestChangeState(LobbyState.EditPlayerName, arg);
        }

        public void UpdateRoomList() {
            listPresenter.UpdateRoomList().Forget();
        }

        public void SetPlayerName(string playerName) {
            model.SetPlayerName(playerName);
        }
    }
}