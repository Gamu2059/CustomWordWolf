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
    public partial class CustomNetworkManager : NetworkManager {
        #region Inspector

        [SerializeField]
        private string titleScene;

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

        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
            Debug.Log("OnClientConnect");
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);
            Debug.Log("OnClientDisconnect");
        }

        public override void OnClientError(NetworkConnection conn, int errorCode) {
            base.OnClientError(conn, errorCode);
            Debug.Log("OnClientError");
        }

        public override void OnClientNotReady(NetworkConnection conn) {
            base.OnClientNotReady(conn);
            Debug.Log("OnClientNotReady");
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation,
            bool customHandling) {
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
            Debug.Log("OnClientChangeScene");
        }

        public override void OnClientSceneChanged(NetworkConnection conn) {
            base.OnClientSceneChanged(conn);
            Debug.Log("OnClientSceneChanged");
        }

        #endregion

        #region Start & Stop callbacks

        public override void OnStartServer() {
            Debug.Log("OnStartServer");
            InitializeServer();
        }

        public override void OnStartClient() {
            Debug.Log("OnStartClient");
            InitializeClient();
        }

        public override void OnStopServer() {
            Debug.Log("OnStopServer");
        }

        public override void OnStopClient() {
            Debug.Log("OnStopClient");
        }

        #endregion

        private void InitializeServer() {
            playerDataHolder = new PlayerDataHolder();
            roomDataHolder = new RoomDataHolder();

            NetworkServer.RegisterHandler<LoadGameScene.Request>(RequestedLoadGameScene);

            NetworkServer.RegisterHandler<ApplyPlayerName.Request>(RequestedApplyPlayerName);
            NetworkServer.RegisterHandler<CreateRoom.Request>(RequestedCreateRoom);
            NetworkServer.RegisterHandler<RoomList.Request>(RequestedRoomList);
            NetworkServer.RegisterHandler<JoinRoom.Request>(RequestedJoinRoom);

            NetworkServer.RegisterHandler<StartGame.Request>(RequestedStartGame);
            NetworkServer.RegisterHandler<VotePlayer.Request>(RequestedVotePlayer);
        }

        private void InitializeClient() {
            NetworkClient.RegisterHandler<LoadGameScene.Response>(ResponseLoadGameScene);

            NetworkClient.RegisterHandler<ApplyPlayerName.Response>(ResponseApplyPlayerName);
            NetworkClient.RegisterHandler<CreateRoom.Response>(ResponseCreateRoom);
            NetworkClient.RegisterHandler<RoomList.Response>(ResponseRoomList);
            NetworkClient.RegisterHandler<JoinRoom.Response>(ResponseJoinRoom);
            NetworkClient.RegisterHandler<StartGame.Response>(ResponseStartGame);
            NetworkClient.RegisterHandler<VotePlayer.Response>(ResponseVotePlayer);

            NetworkClient.RegisterHandler<ChangeHost.SendPlayer>(ReceiveChangeHost);
            NetworkClient.RegisterHandler<StartGame.SendRoom>(ReceiveStartGame);
            NetworkClient.RegisterHandler<TimeOver.SendRoom>(ReceiveTimeOver);
            NetworkClient.RegisterHandler<VotePlayer.SendRoom>(ReceiveVotePlayer);

            SceneManager.LoadScene(titleScene);
        }

        #region Server Request

        /// <summary>
        /// サーバから部屋のホスト変更を通知する。
        /// </summary>
        private void SendPlayerChangeHost(NetworkConnection connection) {
            connection.Send(new ChangeHost.SendPlayer());
        }

        /// <summary>
        /// サーバからゲーム開始を通知する。
        /// </summary>
        private void SendRoomStartGame(RoomData roomData, int remainTime, DateTime countStartDateTime) {
            var memberList = roomData.GetAllMemberList();
            var wolfMember = memberList.OrderBy(_ => Guid.NewGuid()).First();
            var theme = themeList.OrderBy(_ => Guid.NewGuid()).First();

            foreach (var member in memberList) {
                var data = new StartGame.SendRoom {
                    Theme = member == wolfMember ? theme.Item1 : theme.Item2,
                    RemainTime = remainTime,
                    CountStartDateTime = countStartDateTime,
                };
                member.Send(data);
            }
        }

        /// <summary>
        /// サーバから時間切れを通知する。
        /// </summary>
        private void SendRoomTimeOver(RoomData roomData) {
            foreach (var member in roomData.GetAllMember()) {
                member.Send(new TimeOver.SendRoom());
            }
        }

        /// <summary>
        /// サーバから投票を通知する。
        /// </summary>
        private void SendRoomVotePlayer(RoomData roomData, uint voteOriginPlayerNetId) {
            foreach (var member in roomData.GetAllMember()) {
                member.Send(new VotePlayer.SendRoom {VoteOriginPlayerNetId = voteOriginPlayerNetId});
            }
        }

        #endregion

        #region Server Response

        /// <summary>
        /// クライアントがゲームシーンを呼び出した時の処理。
        /// </summary>
        private void RequestedLoadGameScene(NetworkConnection connection, LoadGameScene.Request request) {
            var identity = connection.identity;
            var netId = identity.netId;
            var roomData = roomDataHolder.GetRoomDataByContainPlayer(netId);
            if (roomData == null) {
                connection.Send(new LoadGameScene.Response {Result = LoadGameScene.Result.Failure});
                return;
            }

            var response = new LoadGameScene.Response {
                Result = LoadGameScene.Result.Succeed,
                IsHost = netId == roomData.CurrentHostNetId,
            };
            connection.Send(response);
        }

        /// <summary>
        /// クライアントから名前の適用をリクエストされた時の処理。
        /// </summary>
        private void RequestedApplyPlayerName(NetworkConnection connection, ApplyPlayerName.Request request) {
            var playerData = playerDataHolder.GetPlayerData(connection.identity);
            if (playerData == null) {
                Debug.LogError("存在しないプレイヤーです");
                connection.Send(new ApplyPlayerName.Response {Result = ApplyPlayerName.Result.Failure});
                return;
            }

            playerData.UpdatePlayerName(request.PlayerName);
            Debug.Log(request.PlayerName);

            connection.Send(new ApplyPlayerName.Response {Result = ApplyPlayerName.Result.Succeed});
        }

        /// <summary>
        /// クライアントから部屋作成をリクエストされた時の処理。
        /// </summary>
        private void RequestedCreateRoom(NetworkConnection connection, CreateRoom.Request request) {
            var identity = connection.identity;
            var netId = identity.netId;
            if (roomDataHolder.ExistRoomByHostPlayer(netId)) {
                // 同一プレイヤーは部屋を立てらない
                connection.Send(new CreateRoom.Response {Result = CreateRoom.Result.Failure});
                return;
            }

            var playerData = playerDataHolder.GetPlayerData(identity);
            var guid = Guid.NewGuid();
            while (roomDataHolder.ExistRoomByGuid(guid)) {
                guid = Guid.NewGuid();
            }

            var roomData = roomDataHolder.CreateRoomData(guid, request.RoomName, playerData.PlayerName, netId);
            if (roomData == null) {
                connection.Send(new CreateRoom.Response {Result = CreateRoom.Result.Failure});
                return;
            }

            roomData.JoinRoom(connection);
            connection.Send(new CreateRoom.Response
                {Result = CreateRoom.Result.Succeed, CreatedRoomData = roomData.CreateConnectRoomData()});
        }

        /// <summary>
        /// クライアントから部屋一覧の取得をリクエストされた時の処理。
        /// </summary>
        private void RequestedRoomList(NetworkConnection connection, RoomList.Request request) {
            // TODO:既にゲーム開始している部屋は除く
            var roomDataList = roomDataHolder
                .GetAllRoomData()
                .OrderBy(d => d.DateTime)
                .Select(d => d.CreateConnectRoomData())
                .ToList();
            connection.Send(new RoomList.Response {Result = RoomList.Result.Succeed, RoomDataList = roomDataList});
        }

        /// <summary>
        /// クライアントから部屋参加をリクエストされた時の処理。
        /// </summary>
        private void RequestedJoinRoom(NetworkConnection connection, JoinRoom.Request request) {
            if (!roomDataHolder.ExistRoomByGuid(request.RoomGuid)) {
                connection.Send(new JoinRoom.Response {Result = JoinRoom.Result.NotExist});
                return;
            }

            // TODO:部屋で既にゲーム開始している場合は入れない

            var roomData = roomDataHolder.GetRoomDataByGuid(request.RoomGuid);
            roomData.JoinRoom(connection);
            connection.Send(new JoinRoom.Response
                {Result = JoinRoom.Result.Succeed, JoinedRoomData = roomData.CreateConnectRoomData()});
        }

        /// <summary>
        /// クライアントからゲーム開始をリクエストされた時の処理。
        /// </summary>
        private void RequestedStartGame(NetworkConnection connection, StartGame.Request request) {
            var identity = connection.identity;
            var netId = identity.netId;
            var roomData = roomDataHolder.GetRoomDataByHostPlayer(netId);
            if (roomData == null) {
                connection.Send(new StartGame.Response {Result = StartGame.Result.Failure});
                return;
            }

            var remainTime = 10;
            var countStartDateTime = DateTime.UtcNow;
            roomData.StartGame();
            connection.Send(new StartGame.Response {Result = StartGame.Result.Succeed});
            SendRoomStartGame(roomData, remainTime, countStartDateTime);

            Observable.Timer(TimeSpan.FromSeconds(remainTime)).Subscribe(_ => {
                // タイムオーバー処理
                SendRoomTimeOver(roomData);
            }).AddTo(gameObject);
        }

        /// <summary>
        /// クライアントから投票をリクエストされた時の処理。
        /// </summary>
        private void RequestedVotePlayer(NetworkConnection connection, VotePlayer.Request request) {
            var identity = connection.identity;
            var netId = identity.netId;
            var roomData = roomDataHolder.GetRoomDataByContainPlayer(netId);
            if (roomData == null) {
                connection.Send(new VotePlayer.Response {Result = VotePlayer.Result.Failure});
                return;
            }

            roomData.VotePlayer(netId, request.VoteForwardPlayerNetId);
            connection.Send(new VotePlayer.Response {Result = VotePlayer.Result.Succeed});
            SendRoomVotePlayer(roomData, netId);
        }

        #endregion

        #region Client Request

        /// <summary>
        /// ゲームシーンを読み込んだ時に送信する。
        /// </summary>
        public void RequestLoadGameScene(LoadGameScene.Request request) {
            NetworkClient.connection.Send(request);
        }

        /// <summary>
        /// プレイヤー名を適用する。
        /// </summary>
        public void RequestApplyName(ApplyPlayerName.Request request) {
            NetworkClient.connection.Send(request);
        }

        /// <summary>
        /// 部屋作成をリクエストする。
        /// この部屋はMirrorで用意されたServer/Clientの関係ではなく仮想的な部屋である。
        /// 部屋を立てたクライアントは部屋のホストとなる。
        /// </summary>
        public void RequestCreateRoom(CreateRoom.Request request) {
            NetworkClient.connection.Send(request);
        }

        /// <summary>
        /// 部屋一覧の取得をリクエストする。
        /// </summary>
        public void RequestRoomList() {
            NetworkClient.connection.Send(new RoomList.Request());
        }

        /// <summary>
        /// 部屋参加をリクエストする。
        /// </summary>
        public void RequestJoinRoom(JoinRoom.Request request) {
            NetworkClient.connection.Send(request);
        }

        /// <summary>
        /// ゲーム開始をリクエストする。
        /// </summary>
        public void RequestStartGame(StartGame.Request request) {
            NetworkClient.connection.Send(request);
        }

        /// <summary>
        /// 投票をリクエストする。
        /// </summary>
        public void RequestVotePlayer(VotePlayer.Request request) {
            NetworkClient.connection.Send(request);
        }

        #endregion

        #region Client Response

        public event Action<LoadGameScene.Response> OnLoadGameSceneResponse;

        private void ResponseLoadGameScene(NetworkConnection connection, LoadGameScene.Response response) {
            OnLoadGameSceneResponse?.Invoke(response);
        }

        public event Action<ApplyPlayerName.Response> OnApplyPlayerNameResponse;

        private void ResponseApplyPlayerName(NetworkConnection connection, ApplyPlayerName.Response response) {
            OnApplyPlayerNameResponse?.Invoke(response);
        }

        public event Action<CreateRoom.Response> OnCreateRoomResponse;

        private void ResponseCreateRoom(NetworkConnection connection, CreateRoom.Response response) {
            OnCreateRoomResponse?.Invoke(response);
        }

        public event Action<ChangeHost.SendPlayer> OnChangeHostReceived;

        private void ReceiveChangeHost(NetworkConnection connection, ChangeHost.SendPlayer data) {
            OnChangeHostReceived?.Invoke(data);
        }

        public event Action<RoomList.Response> OnRoomListResponse;

        private void ResponseRoomList(NetworkConnection connection, RoomList.Response response) {
            OnRoomListResponse?.Invoke(response);
        }

        public event Action<JoinRoom.Response> OnJoinRoomResponse;

        private void ResponseJoinRoom(NetworkConnection connection, JoinRoom.Response response) {
            OnJoinRoomResponse?.Invoke(response);
        }

        public event Action<StartGame.Response> OnStartGameResponse;

        private void ResponseStartGame(NetworkConnection connection, StartGame.Response response) {
            OnStartGameResponse?.Invoke(response);
        }

        public event Action<StartGame.SendRoom> OnStartGameReceived;

        private void ReceiveStartGame(NetworkConnection connection, StartGame.SendRoom data) {
            OnStartGameReceived?.Invoke(data);
        }

        public event Action<TimeOver.SendRoom> OnTimeOverReceived;

        private void ReceiveTimeOver(NetworkConnection connection, TimeOver.SendRoom data) {
            OnTimeOverReceived?.Invoke(data);
        }

        public event Action<VotePlayer.Response> OnVotePlayerResponse;

        private void ResponseVotePlayer(NetworkConnection connection, VotePlayer.Response response) {
            OnVotePlayerResponse?.Invoke(response);
        }

        public event Action<VotePlayer.SendRoom> OnVotePlayerReceived;

        private void ReceiveVotePlayer(NetworkConnection connection, VotePlayer.SendRoom data) {
            OnVotePlayerReceived?.Invoke(data);
        }

        #endregion
    }
}