using System;
using System.Collections;
using System.Collections.Generic;
using Api.SequenceCommand;
using Common;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using UniRx;
using UnityEngine;

namespace Title {
    public class TitleGroupPresenter :
        MonoBehaviour,
        Initializable,
        IStateChangeable,
        IStateMachineInjectable<GroupState> {
        [SerializeField]
        private TitleGroupView view;

        private StateMachine<GroupState> parentStateMachine;
        private IDisposable startDisposable;

        public void Initialize() {
            view.Initialize();
        }

        public void InjectStateMachine(StateMachine<GroupState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            SetViewEvents();
            await view.ShowAsync();
        }

        public async UniTask StateOutAsync() {
            await view.HideAsync();
            DisposeViewEvents();
        }

        private void SetViewEvents() {
            startDisposable = view.StartObservable.Subscribe(_ => OnClickStart());
        }

        private void DisposeViewEvents() {
            startDisposable?.Dispose();
            startDisposable = null;
        }

        private async void OnClickStart() {
            var connectClient = new ConnectClient();
            var (isSuccess, errorCode) = await connectClient.ConnectClientAsync();

            if (isSuccess) {
                parentStateMachine.RequestChangeState(GroupState.Lobby, null);
            } else {
                Debug.LogError("サーバとの接続に失敗しました " + errorCode);
            }
        }
    }
}