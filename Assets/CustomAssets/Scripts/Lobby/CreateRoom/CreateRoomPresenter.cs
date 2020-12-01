using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.CreateRoom {
    public class CreateRoomPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<LobbyState> {
        [SerializeField]
        private CreateRoomView view;

        private StateMachine<LobbyState> parentStateMachine;
        private CompositeDisposable viewDisposable;

        public event Action<string> OnCreateRoomNameDecidedEvent;

        public void Initialize() {
            view.Initialize();
        }

        public void InjectStateMachine(StateMachine<LobbyState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
        }

        public async UniTask StateOutAsync() {
            DisposeViewEvents();
            await view.HideAsync();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackObservable
                    .Subscribe(_ => parentStateMachine.RequestBackState())
                    .AddTo(gameObject),
                view.ApplyObservable
                    .Subscribe(_ => OnCreateRoomNameDecidedEvent?.Invoke(view.RoomName))
                    .AddTo(gameObject)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }
    }
}