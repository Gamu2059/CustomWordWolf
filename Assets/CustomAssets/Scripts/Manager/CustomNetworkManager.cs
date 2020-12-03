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

        [Range(1, 4)]
        [SerializeField]
        private int defaultWolfNum;

        [SerializeField]
        private int gameTimeResolution = 30;

        [SerializeField]
        private int wolfNumResolution = 1;

        [SerializeField]
        private Vector2Int gameTimeRange = new Vector2Int(60, 600);

        [SerializeField]
        private Vector2Int wolfNumRange = new Vector2Int(1, 4);

        #endregion

        #region Field

        private PlayerDataHolder playerDataHolder;
        private RoomDataHolder roomDataHolder;

        #endregion

        #region Server System Callbacks

        public override void OnServerConnect(NetworkConnection conn) {
            base.OnServerConnect(conn);
            Debug.Log("OnServerConnect");
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            base.OnServerDisconnect(conn);
            Debug.Log("OnServerDisconnect");
            FinalizePlayerDataOnServer(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn) {
            base.OnServerAddPlayer(conn);
            Debug.Log("OnServerAddPlayer");
            InitializePlayerDataOnServer(conn);
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
            NetworkServer.RegisterHandler<LeaveRoom.Request>(RequestedLeaveRoom);

            NetworkServer.RegisterHandler<ChangeGameTime.Request>(RequestedChangeGameTime);
            NetworkServer.RegisterHandler<ChangeWolfNum.Request>(RequestedChangeWolfNum);

            NetworkServer.RegisterHandler<StartGame.Request>(RequestedStartGame);
        }

        private void InitializeClient() {
            NetworkClient.RegisterHandler<GetRoomDetailData.Response>(ResponseGetRoomDetailData);

            NetworkClient.RegisterHandler<ApplyPlayerName.Response>(ResponseApplyPlayerName);
            NetworkClient.RegisterHandler<CreateRoom.Response>(ResponseCreateRoom);
            NetworkClient.RegisterHandler<GetRoomList.Response>(ResponseGetRoomList);
            NetworkClient.RegisterHandler<JoinRoom.Response>(ResponseJoinRoom);
            NetworkClient.RegisterHandler<LeaveRoom.Response>(ResponseLeaveRoom);

            NetworkClient.RegisterHandler<ChangeGameTime.Response>(ResponseChangeGameTime);
            NetworkClient.RegisterHandler<ChangeWolfNum.Response>(ResponseChangeWolfNum);

            NetworkClient.RegisterHandler<StartGame.Response>(ResponseStartGame);

            NetworkClient.RegisterHandler<UpdateMember.SendRoom>(ReceiveUpdateMember);
            NetworkClient.RegisterHandler<ChangeGameTime.SendRoom>(ReceiveChangeGameTime);
            NetworkClient.RegisterHandler<ChangeWolfNum.SendRoom>(ReceiveChangeWolfNum);
            NetworkClient.RegisterHandler<StartGame.SendRoom>(ReceiveStartGame);
        }

        private void InitializePlayerDataOnServer(NetworkConnection connection) {
            try {
                playerDataHolder.CreatePlayerData(connection, "");
            } catch (Exception e) {
                Debug.LogErrorFormat("[InitializePlayerDataOnServer] 予期せぬエラーが発生しました\nid : {0}",
                    connection.connectionId);
                Debug.LogException(e);
            }
        }

        private void FinalizePlayerDataOnServer(NetworkConnection connection) {
            var id = connection.connectionId;
            try {
                // プレイヤーが存在しているかどうかチェックする
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[FinalizePlayerDataOnServer] 存在しないプレイヤーが切断しようとしました\nid : {0}",
                        id);
                    return;
                }

                // 部屋に入っている場合は退室処理を行う
                if (roomDataHolder.ExistRoomByContainPlayer(id)) {
                    RequestedLeaveRoom(connection, new LeaveRoom.Request());
                }

                playerDataHolder.RemovePlayerData(connection);
            } catch (Exception e) {
                Debug.LogErrorFormat("[FinalizePlayerDataOnServer] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
            }
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
            var id = connection.connectionId;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedApplyPlayerName] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = ApplyPlayerName.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                var playerData = playerDataHolder.GetPlayerData(id);
                playerData.ApplyPlayerName(request.PlayerName);

                msg.Result = ApplyPlayerName.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedApplyPlayerName] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
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
            var id = connection.connectionId;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedGetRoomList] 存在しないプレイヤーが指定されました\nid : {0}", id);
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
                Debug.LogErrorFormat("[RequestedGetRoomList] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
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
            var id = connection.connectionId;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedCreateRoom] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = CreateRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 既に部屋のホストになっているかどうかチェック
                if (roomDataHolder.ExistRoomByHostPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedCreateRoom] 指定されたプレイヤーは既に部屋のホストになっています\nid : {0}", id);
                    msg.Result = CreateRoom.Result.FailureMultipleRoomHost;
                    connection.Send(msg);
                    return;
                }

                var constArg = new ConstArg();
                constArg.RoomName = request.RoomName;
                constArg.MaxMemberNum = defaultMaxRoomMemberNum;
                constArg.GameTimeResolution = gameTimeResolution;
                constArg.WolfNumResolution = wolfNumResolution;
                constArg.GameTimeRange = gameTimeRange;
                constArg.WolfNumRange = wolfNumRange;
                constArg.ThemeUnitList = request.ThemeData.Theme.Select(t => (t.Theme1, t.Theme2)).ToList();

                var variableArg = new VariableArg();
                variableArg.GameTime = defaultGameTime;
                variableArg.WolfNum = defaultWolfNum;

                var roomData = roomDataHolder.CreateRoomData(playerDataHolder, connection, constArg, variableArg);
                msg.Result = CreateRoom.Result.Succeed;
                msg.RoomGuid = roomData.RoomGuid;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedCreateRoom] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
                msg.Result = CreateRoom.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Send Room Update Member

        /// <summary>
        /// サーバから部屋のメンバー更新を通知する。
        /// </summary>
        private void SendRoomUpdateMember(RoomData roomData) {
            var roomDetailData = roomData.CreateRoomDetailData();
            foreach (var member in roomData.GetAllMemberConnection()) {
                var msg = new UpdateMember.SendRoom();
                msg.RoomData = roomDetailData;
                msg.IsHost = member.connectionId == roomData.HostConnectionId;
                member.Send(msg);
            }
        }

        public event Action<UpdateMember.SendRoom> OnUpdateMemberReceiveEvent;

        private void ReceiveUpdateMember(NetworkConnection connection, UpdateMember.SendRoom data) {
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
            var id = connection.connectionId;
            RoomData roomData;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // プレイヤーが既に部屋に入っているかどうかチェック
                if (roomDataHolder.ExistRoomByContainPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] プレイヤーは既にいずれかの部屋に入っています\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailureAlreadyJoinRoom;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByGuid(request.RoomGuid)) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 存在しない部屋が指定されました\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailureNonExistRoom;
                    connection.Send(msg);
                    return;
                }

                roomData = roomDataHolder.GetRoomDataByGuid(request.RoomGuid);

                // 部屋が満員かどうかチェック
                if (roomData.IsFullMember) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 部屋は既に定員です\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailureFullMember;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 既にゲームを開始しています\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                var result = roomData.JoinRoom(connection);
                if (!result) {
                    Debug.LogWarningFormat("[RequestedJoinRoom] 入室に失敗しました\nid : {0}", id);
                    msg.Result = JoinRoom.Result.FailureJoinRoom;
                    connection.Send(msg);
                    return;
                }

                msg.Result = JoinRoom.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedJoinRoom] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
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
            var id = connection.connectionId;
            RoomData roomData;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = LeaveRoom.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // プレイヤーが部屋にいるかどうかチェック
                if (!roomDataHolder.ExistRoomByContainPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] プレイヤーはいずれの部屋にも入っていません\nid : {0}", id);
                    msg.Result = LeaveRoom.Result.FailureNoJoinRoom;
                    connection.Send(msg);
                    return;
                }

                roomData = roomDataHolder.GetRoomDataByContainPlayer(id);
                var result = roomData.LeaveRoom(connection);
                if (!result) {
                    Debug.LogWarningFormat("[RequestedLeaveRoom] 退室に失敗しました\nid : {0}", id);
                    msg.Result = LeaveRoom.Result.FailureLeaveRoom;
                    connection.Send(msg);
                    return;
                }

                msg.Result = LeaveRoom.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedLeaveRoom] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
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
            if (id == roomData.HostConnectionId) {
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
            var id = connection.connectionId;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedGetRoomDetailData] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = GetRoomDetailData.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByGuid(request.RoomGuid)) {
                    Debug.LogWarningFormat("[RequestedGetRoomDetailData] 存在しない部屋が指定されました\nid : {0}", id);
                    msg.Result = GetRoomDetailData.Result.FailureNonExistRoom;
                    connection.Send(msg);
                    return;
                }

                var roomData = roomDataHolder.GetRoomDataByContainPlayer(id);
                msg.Result = GetRoomDetailData.Result.Succeed;
                msg.IsHost = id == roomData.HostConnectionId;
                msg.RoomData = roomData.CreateRoomDetailData();
                msg.GameTime = roomData.CreateGameTimeSendData();
                msg.WolfNum = roomData.CreateWolfNumSendData();
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedGetRoomDetailData] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
                msg.Result = GetRoomDetailData.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
            }
        }

        #endregion

        #region Request Change Game Time

        /// <summary>
        /// 制限時間を変更する。
        /// </summary>
        public void RequestChangeGameTime(ChangeGameTime.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<ChangeGameTime.Response> OnChangeGameTimeResponseEvent;

        private void ResponseChangeGameTime(NetworkConnection connection, ChangeGameTime.Response response) {
            OnChangeGameTimeResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 制限時間を変更する。
        /// </summary>
        private void RequestedChangeGameTime(NetworkConnection connection, ChangeGameTime.Request request) {
            Debug.Log("RequestedChangeGameTime");
            var msg = new ChangeGameTime.Response();
            var id = connection.connectionId;
            RoomData roomData;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedChangeGameTime] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = ChangeGameTime.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByHostPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedChangeGameTime] 指定したプレイヤーがホストである部屋が存在しません\nid : {0}", id);
                    msg.Result = ChangeGameTime.Result.FailureNonHost;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                roomData = roomDataHolder.GetRoomDataByHostPlayer(id);
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedChangeGameTime] 既にゲームを開始しています\nid : {0}", id);
                    msg.Result = ChangeGameTime.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                var isSuccess = roomData.ChangeGameTime(request.ChangeForward);
                if (!isSuccess) {
                    msg.Result = ChangeGameTime.Result.NoChange;
                    connection.Send(msg);
                    return;
                }

                msg.Result = ChangeGameTime.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedChangeGameTime] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
                msg.Result = ChangeGameTime.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomChangeGameTime(roomData);
        }

        /// <summary>
        /// サーバから制限時間の変更を通知する。
        /// </summary>
        private void SendRoomChangeGameTime(RoomData roomData) {
            var msg = roomData.CreateGameTimeSendData();
            foreach (var member in roomData.GetAllMemberConnection()) {
                member.Send(msg);
            }
        }

        public event Action<ChangeGameTime.SendRoom> OnChangeGameTimeReceiveEvent;

        private void ReceiveChangeGameTime(NetworkConnection connection, ChangeGameTime.SendRoom data) {
            OnChangeGameTimeReceiveEvent?.Invoke(data);
        }

        #endregion

        #region Request Change Wolf Num

        /// <summary>
        /// 人狼の人数を変更する。
        /// </summary>
        public void RequestChangeWolfNum(ChangeWolfNum.Request request) {
            NetworkClient.connection.Send(request);
        }

        public event Action<ChangeWolfNum.Response> OnChangeWolfNumResponseEvent;

        private void ResponseChangeWolfNum(NetworkConnection connection, ChangeWolfNum.Response response) {
            OnChangeWolfNumResponseEvent?.Invoke(response);
        }

        /// <summary>
        /// 人狼の人数を変更する。
        /// </summary>
        private void RequestedChangeWolfNum(NetworkConnection connection, ChangeWolfNum.Request request) {
            var msg = new ChangeWolfNum.Response();
            var id = connection.connectionId;
            RoomData roomData;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedChangeWolfNum] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = ChangeWolfNum.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByHostPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedChangeWolfNum] 指定したプレイヤーがホストである部屋が存在しません\nid : {0}", id);
                    msg.Result = ChangeWolfNum.Result.FailureNonHost;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                roomData = roomDataHolder.GetRoomDataByHostPlayer(id);
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedChangeWolfNum] 既にゲームを開始しています\nid : {0}", id);
                    msg.Result = ChangeWolfNum.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                var isSuccess = roomData.ChangeWolfNum(request.ChangeForward);
                if (!isSuccess) {
                    msg.Result = ChangeWolfNum.Result.NoChange;
                    connection.Send(msg);
                    return;
                }

                msg.Result = ChangeWolfNum.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedChangeWolfNum] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
                msg.Result = ChangeWolfNum.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomChangeWolfNum(roomData);
        }

        /// <summary>
        /// サーバから人狼の人数の変更を通知する。
        /// </summary>
        private void SendRoomChangeWolfNum(RoomData roomData) {
            var msg = roomData.CreateWolfNumSendData();
            foreach (var member in roomData.GetAllMemberConnection()) {
                member.Send(msg);
            }
        }

        public event Action<ChangeWolfNum.SendRoom> OnChangeWolfNumReceiveEvent;

        private void ReceiveChangeWolfNum(NetworkConnection connection, ChangeWolfNum.SendRoom data) {
            OnChangeWolfNumReceiveEvent?.Invoke(data);
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
            var id = connection.connectionId;
            RoomData roomData;

            try {
                // プレイヤーが存在しているかどうかチェック
                if (!playerDataHolder.ExistPlayerData(id)) {
                    Debug.LogWarningFormat("[RequestedStartGame] 存在しないプレイヤーが指定されました\nid : {0}", id);
                    msg.Result = StartGame.Result.FailureNonExistPlayer;
                    connection.Send(msg);
                    return;
                }

                // 部屋が存在しているかどうかチェック
                if (!roomDataHolder.ExistRoomByHostPlayer(id)) {
                    Debug.LogWarningFormat("[RequestedStartGame] 指定したプレイヤーがホストである部屋が存在しません\nid : {0}", id);
                    msg.Result = StartGame.Result.FailureNonHost;
                    connection.Send(msg);
                    return;
                }

                // ゲームを開始しているかどうかチェック
                roomData = roomDataHolder.GetRoomDataByHostPlayer(id);
                if (roomData.IsPlaying) {
                    Debug.LogWarningFormat("[RequestedStartGame] 既にゲームを開始しています\nid : {0}", id);
                    msg.Result = StartGame.Result.FailurePlaying;
                    connection.Send(msg);
                    return;
                }

                roomData.StartGame();

                msg.Result = StartGame.Result.Succeed;
                connection.Send(msg);
            } catch (Exception e) {
                Debug.LogErrorFormat("[RequestedStartGame] 予期せぬエラーが発生しました\nid : {0}", id);
                Debug.LogException(e);
                msg.Result = StartGame.Result.FailureUnknown;
                msg.Exception = e;
                connection.Send(msg);
                return;
            }

            SendRoomStartGame(roomData);
        }

        /// <summary>
        /// サーバからゲーム開始を通知する。
        /// </summary>
        private void SendRoomStartGame(RoomData roomData) {
            var msg = roomData.CreateStartGameSendData();
            foreach (var member in roomData.GetAllMemberConnection()) {
                msg.Theme = roomData.GetTheme(member.connectionId);
                member.Send(msg);
            }
        }

        public event Action<StartGame.SendRoom> OnStartGameReceiveEvent;

        private void ReceiveStartGame(NetworkConnection connection, StartGame.SendRoom data) {
            OnStartGameReceiveEvent?.Invoke(data);
        }

        #endregion
    }
}