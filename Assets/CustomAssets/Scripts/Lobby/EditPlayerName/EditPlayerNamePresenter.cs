using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.EditPlayerName {
    public class EditPlayerNameArg : IChangeStateArg {
        public string PlayerName;
    }

    public class EditPlayerNamePresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<LobbyState> {
        [SerializeField]
        private EditPlayerNameView view;

        private StateMachine<LobbyState> parentStateMachine;
        private CompositeDisposable viewDisposable;

        public event Action<string> OnPlayerNameEditedEvent;

        public void Initialize() {
            view.Initialize();
        }

        public void InjectStateMachine(StateMachine<LobbyState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync(arg);
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
                    .Subscribe(_ => OnPlayerNameEditedEvent?.Invoke(view.PlayerName))
                    .AddTo(gameObject)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }
    }
}