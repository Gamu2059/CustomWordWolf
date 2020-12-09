using System;
using Common;
using Cysharp.Threading.Tasks;
using Lobby.EditPlayerName;
using Lobby.JoinRoom.RoomList;
using UnityEngine;
using UniRx;
using UnityEngine.PlayerLoop;

namespace Lobby.JoinRoom {
    /// <summary>
    /// 部屋参加UIのPresenterコンポーネント。
    /// </summary>
    public class JoinRoomPresenter :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable,
        // 上位のステート変更に対して呼び出し可能
        IStateChangeable,
        // LobbyStateに関するステートマシンを受け取り可能
        IStateMachineInjectable<LobbyState> {
        /// <summary>
        /// 部屋参加UIのView
        /// </summary>
        [SerializeField]
        private JoinRoomView view;

        /// <summary>
        /// 部屋一覧のPresenter
        /// </summary>
        [SerializeField]
        private JoinRoomListPresenter listPresenter;

        /// <summary>
        /// LobbyGroupのステートマシン。
        /// </summary>
        private StateMachine<LobbyState> parentStateMachine;

        /// <summary>
        /// 部屋参加UIのModel
        /// </summary>
        private JoinRoomModel model;

        /// <summary>
        /// ViewのUniRxストリームをまとめる。
        /// </summary>
        private CompositeDisposable viewDisposable;

        /// <summary>
        /// ModelのUniRxストリームをまとめる。
        /// </summary>
        private CompositeDisposable modelDisposable;

        /// <summary>
        /// タイトルに戻るボタンを押した時のアクション。
        /// </summary>
        public event Action OnClickBackTitleEvent;

        /// <summary>
        /// 部屋に参加するボタンを押した時のアクション。
        /// </summary>
        public event Action<Guid> OnClickJoinRoomEvent;

        /// <summary>
        /// 部屋参加UIを初期化する。
        /// </summary>
        public void Initialize() {
            // Viewも初期化する
            view.Initialize();
            model = new JoinRoomModel();

            // 子要素の初期化
            listPresenter.Initialize();
        }

        /// <summary>
        /// 上位のステートマシンを受け取る。
        /// </summary>
        public void InjectStateMachine(StateMachine<LobbyState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        /// <summary>
        /// 部屋参加UIに画面が切り割ってほしい(JoinRoomに状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        /// <param name="arg">状態遷移元からの引数</param>
        /// <param name="isBack">一つ先の状態から戻ってきたかどうか</param>
        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
            SetModelEvents();
            BindChildrenEvents();

            // 遷移した時は、必ず部屋一覧を更新する
            listPresenter.UpdateRoomListAsync().Forget();
        }

        /// <summary>
        /// 部屋参加UIから画面が切り替わってほしい(JoinRoomから状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        public async UniTask StateOutAsync() {
            UnbindChildrenEvents();
            DisposeModelEvents();
            DisposeViewEvents();
            await view.HideAsync();
        }

        /// <summary>
        /// Viewのイベントを紐づける。
        /// </summary>
        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.BackTitleObservable
                    .Subscribe(_ => OnClickBackTitleEvent?.Invoke())
                    .AddTo(gameObject),
                view.EditPlayerNameObservable
                    .Subscribe(_ => OnClickApplyPlayerName())
                    .AddTo(gameObject),
                view.UpdateRoomListObservable
                    .Subscribe(_ => UpdateRoomList())
                    .AddTo(gameObject),
                view.CreateRoomObservable
                    .Subscribe(_ => parentStateMachine.RequestChangeState(LobbyState.CreateRoom))
                    .AddTo(gameObject)
            );
        }

        /// <summary>
        /// Modelのイベントを紐づける。
        /// </summary>
        private void SetModelEvents() {
            modelDisposable = new CompositeDisposable(
                model.PlayerNamePropertyObservable
                    .Subscribe(view.SetPlayerName)
                    .AddTo(gameObject)
            );
        }

        /// <summary>
        /// 子要素のイベントを紐づける。
        /// </summary>
        private void BindChildrenEvents() {
            listPresenter.OnClickJoinRoomEvent += OnClickClickJoinRoom;
        }

        /// <summary>
        /// Viewのイベントを解消する。
        /// </summary>
        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        /// <summary>
        /// Modelのイベントを解消する。
        /// </summary>
        private void DisposeModelEvents() {
            modelDisposable?.Dispose();
            modelDisposable = null;
        }

        /// <summary>
        /// 子要素のイベントを解消する。
        /// </summary>
        private void UnbindChildrenEvents() {
            listPresenter.OnClickJoinRoomEvent -= OnClickClickJoinRoom;
        }

        /// <summary>
        /// 部屋参加ボタンのクリックした時の処理。
        /// </summary>
        /// <param name="roomGuid">参加したい部屋のGUID</param>
        private void OnClickClickJoinRoom(Guid roomGuid) {
            OnClickJoinRoomEvent?.Invoke(roomGuid);
        }

        /// <summary>
        /// プレイヤー名変更ボタンをクリックした時の処理。
        /// </summary>
        private void OnClickApplyPlayerName() {
            // EditPlayerNameステートに遷移する
            var arg = new EditPlayerNameArg {PlayerName = model.PlayerName};
            parentStateMachine.RequestChangeState(LobbyState.EditPlayerName, arg);
        }

        /// <summary>
        /// 部屋一覧を更新する。
        /// </summary>
        public void UpdateRoomList() {
            listPresenter.UpdateRoomListAsync().Forget();
        }

        /// <summary>
        /// プレイヤー名をセットする。
        /// </summary>
        /// <param name="playerName">表示したいプレイヤー名</param>
        public void SetPlayerName(string playerName) {
            model.SetPlayerName(playerName);
        }
    }
}