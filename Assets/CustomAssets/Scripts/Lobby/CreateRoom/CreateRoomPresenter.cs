using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.CreateRoom {
    /// <summary>
    /// 部屋作成UIのPresenterコンポーネント。
    /// </summary>
    public class CreateRoomPresenter :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable,
        // 上位のステート変更に対して呼び出し可能
        IStateChangeable,
        // LobbyStateに関するステートマシンを受け取り可能
        IStateMachineInjectable<LobbyState> {
        /// <summary>
        /// 部屋作成UIのView
        /// </summary>
        [SerializeField]
        private CreateRoomView view;

        /// <summary>
        /// LobbyGroupのステートマシン。
        /// </summary>
        private StateMachine<LobbyState> parentStateMachine;

        /// <summary>
        /// ViewのUniRxストリームをまとめる。
        /// </summary>
        private CompositeDisposable viewDisposable;

        /// <summary>
        /// 部屋を作成するボタンを押した時のアクション。
        /// </summary>
        public event Action<string> OnClickCreateRoomEvent;

        /// <summary>
        /// 部屋作成UIを初期化する。
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
        /// 部屋作成UIに画面が切り割ってほしい(CreateRoomに状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        /// <param name="arg">状態遷移元からの引数</param>
        /// <param name="isBack">一つ先の状態から戻ってきたかどうか</param>
        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
        }

        /// <summary>
        /// 部屋作成UIから画面が切り替わってほしい(CreateRoomから状態遷移した)時に呼び出される。
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
                    .Subscribe(_ => OnClickCreateRoomEvent?.Invoke(view.RoomName))
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