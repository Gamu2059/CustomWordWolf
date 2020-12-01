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
    public class TitleGroupPresenter : MonoBehaviour, Initializable, IGroupStateChangeable {
        [SerializeField]
        private TitleGroupView view;

        private GroupStateMachine stateMachine;

        private IDisposable startDisposable;

        public void Initialize() {
        }

        public void InjectStateMachine(GroupStateMachine stateMachine) {
            this.stateMachine = stateMachine;
        }

        public async UniTask GroupInAsync(IChangeGroupArg arg) {
            SetViewEvents();

            await view.ShowAsync();
        }

        public async UniTask GroupOutAsync() {
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
            var startClientCmd = new StartClient();
            var (isSuccessConnect, errorCode) = await startClientCmd.StartClientAsync();

            if (isSuccessConnect) {
                stateMachine.RequestChangeState(GroupState.Lobby);
            } else {
                Debug.LogError("サーバとの接続に失敗しました " + errorCode);
            }
        }
    }
}