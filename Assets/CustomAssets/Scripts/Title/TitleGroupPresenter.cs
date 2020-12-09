using Api;
using Api.SequenceCommand;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Title {
    /// <summary>
    /// タイトルに属するUIを管理する概念のPresenterコンポーネント。
    /// </summary>
    public class TitleGroupPresenter :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable,
        // 上位のステート変更に対して呼び出し可能
        IStateChangeable,
        // GroupStateに関するステートマシンを受け取り可能
        IStateMachineInjectable<GroupState> {
        /// <summary>
        /// タイトル全体のView
        /// </summary>
        [SerializeField]
        private TitleGroupView view;

        /// <summary>
        /// GameManagerのステートマシン。
        /// </summary>
        private StateMachine<GroupState> parentStateMachine;

        /// <summary>
        /// ViewのUniRxストリームをまとめる。
        /// </summary>
        private CompositeDisposable viewDisposable;

        /// <summary>
        /// タイトルグループを初期化する。
        /// </summary>
        public void Initialize() {
            // Viewも初期化する
            view.Initialize();
        }

        /// <summary>
        /// 上位のステートマシンを受け取る。
        /// </summary>
        public void InjectStateMachine(StateMachine<GroupState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        /// <summary>
        /// タイトルに画面が切り割ってほしい(Titleに状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        /// <param name="arg">状態遷移元からの引数</param>
        /// <param name="isBack">一つ先の状態から戻ってきたかどうか</param>
        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            SetViewEvents();
        }

        /// <summary>
        /// タイトルから画面が切り替わってほしい(Titleから状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        public async UniTask StateOutAsync() {
            DisposeViewEvents();
            await view.HideAsync();
        }

        /// <summary>
        /// ViewのUniRxのイベントを紐づける。
        /// </summary>
        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                view.StartObservable.Subscribe(_ => OnClickStartAsync().Forget())
            );
        }

        /// <summary>
        /// ViewのUniRxのイベントを解消する。
        /// </summary>
        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        /// <summary>
        /// スタートボタンクリック時の処理。
        /// </summary>
        private async UniTask OnClickStartAsync() {
            // サーバと接続を開始する
            var (isSuccess, errorCode) = await new ConnectClient().ConnectClientAsync();

            if (isSuccess) {
                // 初期の名前をサーバに伝える
                var request = new ApplyPlayerName.Request {PlayerName = PlayerPrefsManager.PlayerName};
                var response = await new ApplyPlayerNameApi().Request(request);

                // レスポンスが成功ならロビーステートに遷移する
                if (response.Result == ApplyPlayerName.Result.Succeed) {
                    parentStateMachine.RequestChangeState(GroupState.Lobby);
                } else {
                    Debug.LogError("サーバとの接続に失敗しました");
                }
            } else {
                Debug.LogError("サーバとの接続に失敗しました " + errorCode);
            }
        }
    }
}