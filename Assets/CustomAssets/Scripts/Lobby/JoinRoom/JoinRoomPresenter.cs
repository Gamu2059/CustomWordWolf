using System;
using Common;
using Cysharp.Threading.Tasks;
using Lobby.JoinRoom.RoomList;
using UnityEngine;
using UniRx;
using UnityEngine.PlayerLoop;

namespace Lobby.JoinRoom {
    public class JoinRoomPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateRequestable<LobbyState, IChangeStateArg> {
        [SerializeField]
        private JoinRoomView view;

        [SerializeField]
        private JoinRoomListPresenter listPresenter;

        private CompositeDisposable viewDisposable;

        public event Func<LobbyState, IChangeStateArg, bool> OnStateRequestEvent;
        public event Action OnTitleBackEvent;
        public event Action<Guid> OnJoinRoomDecidedEvent;

        public void Initialize() {
            view.Initialize();
            listPresenter.Initialize();
        }

        public async UniTask StateInAsync(IChangeStateArg arg) {
            await view.StateInAsync(arg);
            SetViewEvents();
            BindEvents();

            listPresenter.UpdateRoomList().Forget();
        }

        public async UniTask StateOutAsync() {
            UnbindEvents();
            DisposeViewEvents();
            await view.StateOutAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackTitleObservable
                    .Subscribe(_ => OnTitleBackEvent?.Invoke())
                    .AddTo(gameObject),
                view.EditPlayerNameObservable
                    .Subscribe(_ => OnStateRequestEvent(LobbyState.EditPlayerName, null))
                    .AddTo(gameObject),
                view.UpdateRoomListObservable
                    .Subscribe(_ => UpdateRoomList())
                    .AddTo(gameObject),
                view.CreateRoomObservable
                    .Subscribe(_ => OnStateRequestEvent(LobbyState.CreateRoom, null))
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

        private void UnbindEvents() {
            listPresenter.OnJoinRoomDecidedEvent -= OnJoinRoomDecided;
        }

        private void OnJoinRoomDecided(Guid guid) {
            OnJoinRoomDecidedEvent?.Invoke(guid);
        }

        public void UpdateRoomList() {
            listPresenter.UpdateRoomList().Forget();
        }
    }
}