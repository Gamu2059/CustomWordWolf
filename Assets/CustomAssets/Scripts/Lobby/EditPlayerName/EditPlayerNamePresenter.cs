using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.EditPlayerName {
    /// <summary>
    /// プレイヤー名変更UIに渡す引数。
    /// </summary>
    public class EditPlayerNameArg : IChangeStateArg {
        public string PlayerName;
    }

    /// <summary>
    /// プレイヤー名変更UIのPresenterコンポーネント。
    /// </summary>
    public class EditPlayerNamePresenter :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable,
        // 上位のステート変更に対して呼び出し可能
        IStateChangeable,
        // LobbyStateに関するステートマシンを受け取り可能
        IStateMachineInjectable<LobbyState> {
        /// <summary>
        /// プレイヤー名変更UIのView
        /// </summary>
        [SerializeField]
        private EditPlayerNameView view;

        /// <summary>
        /// LobbyGroupのステートマシン。
        /// </summary>
        private StateMachine<LobbyState> parentStateMachine;

        /// <summary>
        /// ViewのUniRxストリームをまとめる。
        /// </summary>
        private CompositeDisposable viewDisposable;

        /// <summary>
        /// プレイヤー名を適用するボタンを押した時のアクション。
        /// </summary>
        public event Action<string> OnClickApplyPlayerNameEvent;

        /// <summary>
        /// プレイヤー名変更UIを初期化する。
        /// </summary>
        public void Initialize() {
            // Viewも初期化する
            view.Initialize();
        }

        /// <summary>
        /// 上位のステートマシンを受け取る。
        /// </summary>
        public void InjectStateMachine(StateMachine<LobbyState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        /// <summary>
        /// プレイヤー名変更UIに画面が切り割ってほしい(EditPlayerNameに状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        /// <param name="arg">状態遷移元からの引数</param>
        /// <param name="isBack">一つ先の状態から戻ってきたかどうか</param>
        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync(arg);
            SetViewEvents();
        }

        /// <summary>
        /// プレイヤー名変更UIから画面が切り替わってほしい(EditPlayerNameから状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        public async UniTask StateOutAsync() {
            DisposeViewEvents();
            await view.HideAsync();
        }

        /// <summary>
        /// Viewのイベントを紐づける。
        /// </summary>
        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackObservable
                    .Subscribe(_ => parentStateMachine.RequestBackState())
                    .AddTo(gameObject),
                view.ApplyObservable
                    .Subscribe(_ => OnClickApplyPlayerNameEvent?.Invoke(view.PlayerName))
                    .AddTo(gameObject)
            );
        }

        /// <summary>
        /// Viewのイベントを解消する。
        /// </summary>
        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }
    }
}