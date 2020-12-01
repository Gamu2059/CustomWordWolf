using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectData;
using ManagedData;
using Mirror;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager {
    public class CustomNetworkManager : NetworkManager {
        #region Inspector

        [Range(2, 10)]
        [SerializeField]
        private int defaultMaxRoomMemberNum;

        [Range(60, 600)]
        [SerializeField]
        private int defaultGameTime;

        [Range(1, 3)]
        [SerializeField]
        private int defaultWolfNum;

        #endregion

        #region Field

        private PlayerDataHolder playerDataHolder;
        private RoomDataHolder roomDataHolder;

        private List<(string, string)> themeList = new List<(string, string)> {
            ("Java", "JavaScript"),
            ("CyberAgent", "DeNA"),
            ("ななさん", "さとこさん"),
        };

        #endregion

        #region Server System Callbacks

        public override void OnServerConnect(NetworkConnection conn) {
            base.OnServerConnect(conn);
            Debug.Log("OnServerConnect");
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            base.OnServerDisconnect(conn);
            Debug.Log("OnServerDisconnect");
        }

        public override void OnServerReady(NetworkConnection conn) {
            base.OnServerReady(conn);
            Debug.Log("OnServerReady");
        }

        public override void OnServerAddPlayer(NetworkConnection conn) {
            base.OnServerAddPlayer(conn);
            Debug.Log("OnServerAddPlayer");
            InitialPlayerData(conn);
        }

        private void InitialPlayerData(NetworkConnection connection) {
            playerDataHolder.CreatePlayerData(connection.identity, "");
        }

        public override void OnServerError(NetworkConnection conn, int errorCode) {
            base.OnServerError(conn, errorCode);
            Debug.Log("OnServerError");
        }

        public override void OnServerChangeScene(string newSceneName) {
            base.OnServerChangeScene(newSceneName);
            Debug.Log("OnServerChangeScene");
        }

        public override void OnServerSceneChanged(string sceneName) {
            base.OnServerSceneChanged(sceneName);
            Debug.Log("OnServerSceneChanged");
        }

        #endregion

        #region Client System Callback

        public event Action OnClientConnectEvent;

        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
            OnClientConnectEvent?.Invoke();
        }

        public event Action OnClientDisconnectEvent;

        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);
            Debug.Log("OnClientDisconnectEvent");
            OnClientDisconnectEvent?.Invoke();
        }

        public event Action<int> OnClientErrorEvent;

        public override void OnClientError(NetworkConnection conn, int errorCode) {
            base.OnClientError(conn, errorCode);
            OnClientErrorEvent?.Invoke(errorCode);
        }

        #endregion

        #region Start & Stop callbacks

        public event Action OnStartServerEvent;

        public override void OnStartServer() {
            InitializeServer();
            OnStartServerEvent?.Invoke();
        }

        public event Action OnStopServerEvent;

        public override void OnStopServer() {
            OnStopServerEvent?.Invoke();
        }

        public event Action OnStartClientEvent;

        public override void OnStartClient() {
            InitializeClient();
            OnStartClientEvent?.Invoke();
        }

        public event Action OnStopClientEvent;

        public override void OnStopClient() {
            Debug.Log("StopClientEvent!");
            OnStopClientEvent?.Invoke();
        }

        #endregion

        private void InitializeServer() {
            playerDataHolder = new PlayerDataHolder();
            roomDataHolder = new RoomDataHolder();

            NetworkServer.RegisterHandler<GetRoomDetailData.Request>(RequestedGetRoomDetailData);

            NetworkServer.RegisterHandler<ApplyPlayerName.Request>(RequestedApplyPlayerName);
            NetworkServer.RegisterHandler<CreateRoom.Request>(RequestedCreateRoom);
            NetworkServer.RegisterHandler<GetRoomList.Request>(RequestedGetRoomList);
            NetworkServer.RegisterHandler<JoinRoom.Request>(RequestedJoinRoom);

            NetworkServer.RegisterHandler<StartGame.Request>(RequestedStartGame);
            NetworkServer.RegisterHandler<VotePlayer.Request>(RequestedVotePlayer);
        }

        private void InitializeClient() {
            NetworkClient.RegisterHandler<GetRoomDetailData.Response>(ResponseGetRoomDetailData);

            NetworkClient.RegisterHandler<ApplyPlayerName.Response>(ResponseApplyPlayerName);
            NetworkClient.RegisterHandler<CreateRoom.Response>(ResponseCreateRoom);
            NetworkClient.RegisterHandler<GetRoomList.Response>(ResponseGetRoomList);
            NetworkClient.RegisterHandler<JoinRoom.Response>(ResponseJoinRoom);
            NetworkClient.RegisterHandler<StartGame.Response>(ResponseStartGame);
            NetworkClient.RegisterHandler<VotePlayer.Response>(ResponseVotePlayer);

            NetworkClient.RegisterHandler<ChangeHost.SendPlayer>(ReceiveUpdateMember);
            NetworkClient.RegisterHandler<StartGame.SendRoom>(ReceiveStartGame);
            NetworkClient.RegisterHandler<TimeOver.SendRoom>(ReceiveTimeOver);
            NetworkClient.RegisterHandler<VotePlayer.SendRoom>(ReceiveVotePlayer);
        }

        #region Request Apply Player Name

        /// <summary>
        /// プレイヤー名を適用する。
        /// </summary>
        public void RequestApplyPlayerName(ApplyPlayerName.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<ApplyPlayerName.Response> OnApplyPlayerNameResponseEvent;

        private void ResponseApplyPlayerName(NetworkConnection connection, ApplyPlayerName.Response response) {
            OnApplyPlayerNameResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// プレイヤー名を適用する。
        /// </summary>
        private void RequestedApplyPlayerName(NetworkConnection connection, ApplyPlayerName.Request request) {
            var msg = new ApplyPlayerName.Response();

            try {
                var identity = connection.identity;
                var netId = identity.netId;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedApplyPlayerName] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = ApplyPlayerName.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                var playerData = playerDataHolder.GetPlayerData(connection.identity);
                playerData.ApplyPlayerName(request.PlayerName);

                msg.Result = ApplyPlayerName.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedApplyPlayerName] 予期せぬエラーが発生しました\nNetId : {0}",
                    connection.identity.netId);
                msg.Result = ApplyPlayerName.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Request Get Room List

        /// <summary>
        /// 部屋一覧を取得する。
        /// </summary>
        public void RequestGetRoomList() {
            NetworkClient.connection.Send(new GetRoomList.Request());
        }

        public event Action<GetRoomList.Response> OnGetRoomListResponseEvent;

        private void ResponseGetRoomList(NetworkConnection connection, GetRoomList.Response response) {
            OnGetRoomListResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 部屋一覧を取得する。
        /// </summary>
        private void RequestedGetRoomList(NetworkConnection connection, GetRoomList.Request request) {
            var msg = new GetRoomList.Response();

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedGetRoomList] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = GetRoomList.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                var roomDataList = roomDataHolder
                    .GetAllRoomData()
                    .Where(d => !d.IsPlaying)
                    .Where(d => !d.IsFullMember)
                    .OrderBy(d => d.DateTime)
                    .Select(d => d.CreateRoomSimpleData())
                    .ToList();

                msg.Result = GetRoomList.Result.Succeed;
                msg.RoomDataList = roomDataList;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedGetRoomList] 予期せぬエラーが発生しました\nNetId : {0}", connection.identity.netId);
                msg.Result = GetRoomList.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Request Create Room

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        public void RequestCreateRoom(CreateRoom.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<CreateRoom.Response> OnCreateRoomResponseEvent;

        private void ResponseCreateRoom(NetworkConnection connection, CreateRoom.Response response) {
            OnCreateRoomResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 部屋を作成する。
        /// </summary>
        private void RequestedCreateRoom(NetworkConnection connection, CreateRoom.Request request) {
            var msg = new CreateRoom.Response();

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedCreateRoom] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = CreateRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                var netId = identity.netId;

                // 既に部屋のホストになっているかどうかチェック
                if (roomDataHolder.ExistRoomByHostPlayer(netId)) {
                    Debug.LogWarningFormat("[RequestedCreateRoom] 指定されたプレイヤーは既に部屋のホストになっています\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = CreateRoom.Result.FailureMultipleRoomHost;
                    connection.Send(msg);
                    return;
                }

                var constArg = new ConstArg();
                constArg.RoomName = request.RoomName;
                constArg.MaxMemberNum = defaultMaxRoomMemberNum;

                var variableArg = new VariableArg();
                variableArg.GameTime = defaultGameTime;
                variableArg.WolfNum = defaultWolfNum;

                roomDataHolder.CreateRoomData(playerDataHolder, connection, constArg, variableArg);
                msg.Result = CreateRoom.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedCreateRoom] 予期せぬエラーが発生しました\nNetId : {0}", connection.identity.netId);
                msg.Result = CreateRoom.Result.FailureMultipleRoomHost;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Send Update Room Member

        /// <summary>
        /// サーバから部屋のメンバー更新を通知する。
        /// </summary>
        private void SendRoomUpdateMember(RoomData roomData) {
            var roomDetailData = roomData.CreateRoomDetailData();
            foreach (var member in roomData.GetAllMember()) {
                var msg = new RoomUpdate.SendRoom();
                msg.RoomData = roomDetailData;
                msg.IsHost = member.identity.netId == roomData.HostNetId;
                member.Send(msg);
            }
        }

        public event Action<ChangeHost.SendPlayer> OnUpdateMemberReceiveEvent;

        private void ReceiveUpdateMember(NetworkConnection connection, ChangeHost.SendPlayer data) {
            OnUpdateMemberReceiveEvent?.Invoke(data);
        }

        #endregion

        #region Request Join Room

        /// <summary>
        /// 部屋に参加する。
        /// </summary>
        public void RequestJoinRoom(JoinRoom.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<JoinRoom.Response> OnJoinRoomResponseEvent;

        private void ResponseJoinRoom(NetworkConnection connection, JoinRoom.Response response) {
            OnJoinRoomResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 部屋に参加する。
        /// </summary>
        private void RequestedJoinRoom(NetworkConnection connection, JoinRoom.Request request) {
            var msg = new JoinRoom.Response();
            RoomData roomData;

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                var netId = identity.netId;

                // プレイヤーが既に部屋に入っているかどうかチェック
                if (roomDataHolder.ExistRoomByContainPlayer(netId)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] プレイヤーは既にいずれかの部屋に入っています\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailureAlreadyJoinRoom;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByGuid(request.RoomGuid)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 存在しない部屋が指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailureNonExistRoom;
                    connection.Send(msg);
                    return;
                }

                roomData = roomDataHolder.GetRoomDataByGuid(request.RoomGuid);

                // 部屋が満員かどうかチェック
                if (roomData.IsFullMember) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 部屋は既に定員です\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailureFullMember;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 既にゲームを開始しています\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                var result = roomData.JoinRoom(connection);
                if (!result) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 入室に失敗しました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = JoinRoom.Result.FailureJoinRoom;
                    connection.Send(msg);
                    return;
                }

                msg.Result = JoinRoom.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedJoinRoom] 予期せぬエラーが発生しました\nNetId : {0}", connection.identity.netId);
                msg.Result = JoinRoom.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomUpdateMember(roomData);
        }

        #endregion

        #region Request Leave Room

        /// <summary>
        /// 部屋から抜ける。
        /// </summary>
        public void RequestLeaveRoom(LeaveRoom.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<LeaveRoom.Response> OnLeaveRoomResponseEvent;

        private void ResponseLeaveRoom(NetworkConnection connection, LeaveRoom.Response response) {
            OnLeaveRoomResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 部屋から抜ける。
        /// </summary>
        private void RequestedLeaveRoom(NetworkConnection connection, LeaveRoom.Request request) {
            var msg = new LeaveRoom.Response();
            uint netId;
            RoomData roomData;

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = LeaveRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                netId = identity.netId;

                // プレイヤーが部屋にいるかどうかチェック
                if (!roomDataHolder.ExistRoomByContainPlayer(netId)) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] プレイヤーはいずれの部屋にも入っていません\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = LeaveRoom.Result.FailureNoJoinRoom;
                    connection.Send(msg);
                    return;
                }

                roomData = roomDataHolder.GetRoomDataByContainPlayer(netId);
                var result = roomData.LeaveRoom(connection);
                if (!result) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] 退室に失敗しました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = LeaveRoom.Result.FailureLeaveRoom;
                    connection.Send(msg);
                    return;
                }

                msg.Result = LeaveRoom.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedLeaveRoom] 予期せぬエラーが発生しました\nNetId : {0}", connection.identity.netId);
                msg.Result = LeaveRoom.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            // 部屋が空いたら部屋を消す
            if (roomData.MemberNum < 1) {
                roomDataHolder.RemoveRoomData(roomData.RoomGuid);
                return;
            }

            // ホストが退室したらホストを変える
            if (netId == roomData.HostNetId) {
                roomData.ChangeHost();
            }

            SendRoomUpdateMember(roomData);
        }

        #endregion

        #region Request Get Room Detail Data

        /// <summary>
        /// 指定した部屋の詳細な情報を取得する。
        /// </summary>
        public void RequestGetRoomDetailData(GetRoomDetailData.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<GetRoomDetailData.Response> OnGetRoomDetailDataResponseEvent;

        private void ResponseGetRoomDetailData(NetworkConnection connection, GetRoomDetailData.Response response) {
            OnGetRoomDetailDataResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 指定した部屋の詳細な情報を取得する。
        /// </summary>
        private void RequestedGetRoomDetailData(NetworkConnection connection, GetRoomDetailData.Request request) {
            var msg = new GetRoomDetailData.Response();

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedGetRoomDetailData] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = GetRoomDetailData.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByGuid(request.RoomGuid)) {
                    Debug.LogWarningFormat("[RequestedGetRoomDetailData] 存在しない部屋が指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = GetRoomDetailData.Result.FailureNonExistRoom;
                    connection.Send(msg);
                    return;
                }

                var netId = identity.netId;
                var roomData = roomDataHolder.GetRoomDataByContainPlayer(netId);

                msg.Result = GetRoomDetailData.Result.Succeed;
                msg.IsHost = netId == roomData.HostNetId;
                msg.RoomData = roomData.CreateRoomDetailData();
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedGetRoomDetailData] 予期せぬエラーが発生しました\nNetId : {0}",
                    connection.identity.netId);
                msg.Result = GetRoomDetailData.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Request Start Game

        /// <summary>
        /// ゲームを開始する。
        /// </summary>
        public void RequestStartGame(StartGame.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<StartGame.Response> OnStartGameResponseEvent;

        private void ResponseStartGame(NetworkConnection connection, StartGame.Response response) {
            OnStartGameResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// ゲームを開始する。
        /// </summary>
        private void RequestedStartGame(NetworkConnection connection, StartGame.Request request) {
            var msg = new StartGame.Response();
            uint netId;
            RoomData roomData;

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedStartGame] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = StartGame.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                netId = identity.netId;

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByHostPlayer(netId)) {
                    Debug.LogWarningFormat("[RequestedStartGame] 指定したプレイヤーがホストである部屋が存在しません\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = StartGame.Result.FailureNonHost;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                roomData = roomDataHolder.GetRoomDataByHostPlayer(netId);
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedStartGame] 既にゲームを開始しています\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = StartGame.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                roomData.StartGame();

                msg.Result = StartGame.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedStartGame] 予期せぬエラーが発生しました\nNetId : {0}",
                    connection.identity.netId);
                msg.Result = StartGame.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomStartGame(roomData);
            roomData.OnTimeOverEvent += SendRoomTimeOver;
        }

        /// <summary>
        /// サーバからゲーム開始を通知する。
        /// </summary>
        private void SendRoomStartGame(RoomData roomData) {
            foreach (var member in roomData.GetAllMember()) {
                var msg = new StartGame.SendRoom();
                msg.Theme = roomData.GetTheme(member.identity.netId);
                msg.GameTime = roomData.VariableArg.GameTime;
                msg.GameStartDateTime = roomData.GameStartDateTime;
                member.Send(msg);
            }
        }

        public event Action<StartGame.SendRoom> OnStartGameReceiveEvent;

        private void ReceiveStartGame(NetworkConnection connection, StartGame.SendRoom data) {
            OnStartGameReceiveEvent?.Invoke(data);
        }

        #endregion

        #region Send Room Time Over

        /// <summary>
        /// サーバから時間切れを通知する。
        /// </summary>
        private void SendRoomTimeOver(RoomData roomData) {
            roomData.OnTimeOverEvent -= SendRoomTimeOver;
            foreach (var member in roomData.GetAllMember()) {
                member.Send(new TimeOver.SendRoom());
            }
        }

        public event Action<TimeOver.SendRoom> OnTimeOverReceiveEvent;

        private void ReceiveTimeOver(NetworkConnection connection, TimeOver.SendRoom data) {
            OnTimeOverReceiveEvent?.Invoke(data);
        }

        #endregion

        #region Request Vote Player

        /// <summary>
        /// 投票する。
        /// </summary>
        public void RequestVotePlayer(VotePlayer.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<VotePlayer.Response> OnVotePlayerResponseEvent;

        private void ResponseVotePlayer(NetworkConnection connection, VotePlayer.Response response) {
            OnVotePlayerResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 投票する。
        /// </summary>
        private void RequestedVotePlayer(NetworkConnection connection, VotePlayer.Request request) {
            var msg = new VotePlayer.Response();
            uint netId;
            RoomData roomData;

            try {
                var identity = connection.identity;

                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(identity)) {
                    Debug.LogWarningFormat("[RequestedVotePlayer] 存在しないプレイヤーが指定されました\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = VotePlayer.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                netId = identity.netId;

                // プレイヤーが部屋にいるかどうかチェック
                if (!roomDataHolder.ExistRoomByContainPlayer(netId)) {
                    Debug.LogWarningFormat("[RequestedVotePlayer] プレイヤーはいずれの部屋にも入っていません\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = VotePlayer.Result.FailureNoJoinRoom;
                    connection.Send(msg);
                    return;
                }

                roomData = roomDataHolder.GetRoomDataByContainPlayer(netId);

                // ゲームを開始しているかどうかチェック
                if (!roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedVotePlayer] ゲームはまだ開始していません\nNetId : {0}",
                        connection.identity.netId);
                    msg.Result = VotePlayer.Result.FailureNoPlaying;
                    connection.Send(msg);
                    return;
                }

                roomData.VotePlayer(netId, request.VoteForwardPlayerNetId);

                msg.Result = VotePlayer.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedVotePlayer] 予期せぬエラーが発生しました\nNetId : {0}", connection.identity.netId);
                msg.Result = VotePlayer.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomVotePlayer(roomData, netId);
        }

        /// <summary>
        /// サーバから投票を通知する。
        /// </summary>
        private void SendRoomVotePlayer(RoomData roomData, uint voteOriginPlayerNetId) {
            foreach (var member in roomData.GetAllMember()) {
                var msg = new VotePlayer.SendRoom();
                msg.VoteOriginPlayerNetId = voteOriginPlayerNetId;
                member.Send(msg);
            }
        }

        public event Action<VotePlayer.SendRoom> OnVotePlayerReceiveEvent;

        private void ReceiveVotePlayer(NetworkConnection connection, VotePlayer.SendRoom data) {
            OnVotePlayerReceiveEvent?.Invoke(data);
        }

        #endregion
    }
}