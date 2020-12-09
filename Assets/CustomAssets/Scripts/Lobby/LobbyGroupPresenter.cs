using System;
using Api;
using Api.SequenceCommand;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Dialog;
using Game;
using Lobby.CreateRoom;
using Lobby.EditPlayerName;
using Lobby.JoinRoom;
using Manager;
using UnityEngine;

namespace Lobby {
    /// <summary>
    /// ロビーに属するUIを管理する概念のPresenterコンポーネント。
    /// </summary>
    public class LobbyGroupPresenter :
        // LobbyStateに関するステートマシンを持つMonoBehavior
        StateMachineMonoBehavior<LobbyState>,
        // 上位のステート変更に対して呼び出し可能
        IStateChangeable,
        // GroupStateに関するステートマシンを受け取り可能
        IStateMachineInjectable<GroupState> {
        /// <summary>
        /// ロビー全体のView
        /// </summary>
        [SerializeField]
        private LobbyGroupView view;

        /// <summary>
        /// 部屋参加UIののPresenter
        /// </summary>
        [SerializeField]
        private JoinRoomPresenter joinRoomPresenter;

        /// <summary>
        /// プレイヤー名変更UIのPresenter
        /// </summary>
        [SerializeField]
        private EditPlayerNamePresenter editPlayerNamePresenter;

        /// <summary>
        /// 部屋作成UIのPresenter
        /// </summary>
        [SerializeField]
        private CreateRoomPresenter createRoomPresenter;

        /// <summary>
        /// GameManagerのステートマシン。
        /// </summary>
        private StateMachine<GroupState> parentStateMachine;

        /// <summary>
        /// ロビーグループを初期化する。
        /// Initializeを呼び出された時に自動的に呼び出される。
        /// </summary>
        protected override void OnInitialize() {
            // 抽象クラスの処理呼び出し
            base.OnInitialize();

            // Viewも初期化する
            view.Initialize();

            InitializeChild();
            BindState();
            InjectStateMachine();
        }

        /// <summary>
        /// 子要素のPresenterを初期化する。
        /// </summary>
        private void InitializeChild() {
            joinRoomPresenter.Initialize();
            editPlayerNamePresenter.Initialize();
            createRoomPresenter.Initialize();
        }

        /// <summary>
        /// ステートと子要素のPresenterを紐づける。
        /// あるステートに状態遷移した時、結びつけられた子要素のPresenterが呼び出される。
        /// </summary>
        private void BindState() {
            StateDictionary.Add(LobbyState.JoinRoom, joinRoomPresenter);
            StateDictionary.Add(LobbyState.EditPlayerName, editPlayerNamePresenter);
            StateDictionary.Add(LobbyState.CreateRoom, createRoomPresenter);
        }

        /// <summary>
        /// 子要素で状態遷移させる処理を実現するためにステートマシンを子要素にも伝える。
        /// </summary>
        private void InjectStateMachine() {
            joinRoomPresenter.InjectStateMachine(StateMachine);
            editPlayerNamePresenter.InjectStateMachine(StateMachine);
            createRoomPresenter.InjectStateMachine(StateMachine);
        }

        /// <summary>
        /// 上位のステートマシンを受け取る。
        /// </summary>
        public void InjectStateMachine(StateMachine<GroupState> stateMachine) {
            parentStateMachine = stateMachine;
        }

        /// <summary>
        /// ロビーに画面が切り割ってほしい(Lobbyに状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        /// <param name="arg">状態遷移元からの引数</param>
        /// <param name="isBack">一つ先の状態から戻ってきたかどうか</param>
        public async UniTask StateInAsync(IChangeStateArg arg, bool isBack) {
            await view.ShowAsync();
            BindChildrenEvents();

            // ロビーに遷移した時は、必ず部屋参加UIに遷移する
            StateMachine.RequestChangeState(LobbyState.JoinRoom);
        }

        /// <summary>
        /// ロビーから画面が切り替わってほしい(Lobbyから状態遷移した)時に呼び出される。
        /// 呼び出し元はStateMachineBehavior
        /// </summary>
        public async UniTask StateOutAsync() {
            // ロビーから抜ける時は、Noneを入れることで後始末処理を正しく実行させる
            StateMachine.RequestChangeState(LobbyState.None);

            UnbindChildrenEvents();
            await view.HideAsync();
        }

        /// <summary>
        /// 子要素のイベントを紐づける。
        /// </summary>
        private void BindChildrenEvents() {
            joinRoomPresenter.OnClickBackTitleEvent += OnClickBackTitle;
            joinRoomPresenter.OnClickJoinRoomEvent += OnClickJoinRoom;
            editPlayerNamePresenter.OnClickApplyPlayerNameEvent += OnClickApplyPlayerName;
            createRoomPresenter.OnClickCreateRoomEvent += OnClickCreateRoom;
        }

        /// <summary>
        /// 子要素のイベントを解消する。
        /// </summary>
        private void UnbindChildrenEvents() {
            createRoomPresenter.OnClickCreateRoomEvent -= OnClickCreateRoom;
            editPlayerNamePresenter.OnClickApplyPlayerNameEvent -= OnClickApplyPlayerName;
            joinRoomPresenter.OnClickJoinRoomEvent -= OnClickJoinRoom;
            joinRoomPresenter.OnClickBackTitleEvent -= OnClickBackTitle;
        }

        /// <summary>
        /// タイトルに戻る。
        /// </summary>
        private void OnClickBackTitle() {
            // ActionにUniTaskを直接紐づけることは出来ないので、内部でUniTaskの処理を呼び出す
            BackTitleAsync().Forget();

            async UniTask BackTitleAsync() {
                var disconnectClient = new DisconnectClient();
                var (isSuccess, errorCode) = await disconnectClient.DisconnectClientAsync();

                if (isSuccess) {
                    parentStateMachine.RequestChangeState(GroupState.Title);
                } else {
                    Debug.LogError("サーバとの接続に失敗しました " + errorCode);
                }
            }
        }

        /// <summary>
        /// 部屋に参加する。
        /// </summary>
        /// <param name="roomGuid">参加したい部屋のGUID</param>
        private void OnClickJoinRoom(Guid roomGuid) {
            // ActionにUniTaskを直接紐づけることは出来ないので、内部でUniTaskの処理を呼び出す
            JoinRoomAsync().Forget();

            async UniTask JoinRoomAsync() {
                // JoinRoomで渡されたroomGuidを使っている(クロージャという概念)が、ややこしいので気にしないで
                var request = new ConnectData.JoinRoom.Request {RoomGuid = roomGuid};
                var response = await new JoinRoomApi().Request(request);

                // レスポンスが成功ならゲームステートに遷移する
                if (response.Result == ConnectData.JoinRoom.Result.Succeed) {
                    var arg = new GameGroupArg {RoomGuid = roomGuid};
                    parentStateMachine.RequestChangeState(GroupState.Game, arg);
                    return;
                }

                // レスポンスが失敗なら部屋一覧を更新する
                joinRoomPresenter.UpdateRoomList();
            }
        }

        /// <summary>
        /// プレイヤー名を適用する。
        /// </summary>
        /// <param name="playerName">適用したいプレイヤー名</param>
        private void OnClickApplyPlayerName(string playerName) {
            // ActionにUniTaskを直接紐づけることは出来ないので、内部でUniTaskの処理を呼び出す
            ApplyPlayerNameAsync().Forget();

            async UniTask ApplyPlayerNameAsync() {
                // ApplyPlayerNameで渡されたplayerNameを使っている(クロージャという概念)が、ややこしいので気にしないで
                var request = new ApplyPlayerName.Request {PlayerName = playerName};
                var response = await new ApplyPlayerNameApi().Request(request);

                // レスポンスが成功ならビューにも名前を適用して、ロビーステートを一つ前に戻す
                if (response.Result == ConnectData.ApplyPlayerName.Result.Succeed) {
                    joinRoomPresenter.SetPlayerName(playerName);
                    StateMachine.RequestBackState();
                }
            }
        }

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        /// <param name="roomName">作成したい部屋の名前</param>
        private void OnClickCreateRoom(string roomName) {
            // ActionにUniTaskを直接紐づけることは出来ないので、内部でUniTaskの処理を呼び出す
            CreateRoomAsync().Forget();

            async UniTask CreateRoomAsync() {
                // UnityのRemoteConfigを使ってテーマ一覧を取得する
                var fetchThemeResponse = await new FetchThemeApi().Request();
                if (fetchThemeResponse.Result != FetchTheme.Result.Succeed) {
                    Debug.LogError("テーマの取得に失敗");
                    return;
                }

                // CreateRoomで渡されたroomNameを使っている(クロージャという概念)が、ややこしいので気にしないで
                var createRoomRequest = new ConnectData.CreateRoom.Request {
                    RoomName = roomName,
                    ThemeData = fetchThemeResponse.ThemeData
                };
                var createRoomResponse = await new CreateRoomApi().Request(createRoomRequest);

                // レスポンスが成功ならゲームステートに遷移する
                if (createRoomResponse.Result == ConnectData.CreateRoom.Result.Succeed) {
                    var arg = new GameGroupArg {RoomGuid = createRoomResponse.RoomGuid};
                    parentStateMachine.RequestChangeState(GroupState.Game, arg);
                }
            }
        }
    }
}