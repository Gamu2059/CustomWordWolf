using System;
using System.Collections.Generic;
using System.Linq;
using ConnectData;
using Mirror;
using UniRx;
using UnityEngine;

namespace ManagedData {
    public enum RoomState {
        ReadyGame,
        PlayGame,
    }

    public class ConstArg {
        public Guid RoomGuid;
        public string RoomName;
        public int MaxMemberNum;
        public int GameTimeResolution;
        public int WolfNumResolution;
        public Vector2Int GameTimeRange;
        public Vector2Int WolfNumRange;
        public List<(string, string)> ThemeUnitList;
    }

    public class VariableArg {
        public int GameTime;
        public int WolfNum;
    }

    public class RoomData {
        private PlayerDataHolder playerDataHolder;

        private ThemeBuilder themeBuilder;
        private Dictionary<int, DateTime> memberDictionary;
        private List<int> wolfMemberList;
        private Dictionary<int, int> voteDictionary;

        private string peopleTheme;
        private string wolfTheme;

        private IDisposable gameTimeDisposable;

        public DateTime DateTime { get; }
        public int HostConnectionId { get; private set; }
        public RoomState State { get; private set; }

        public ConstArg ConstArg { get; }

        public VariableArg VariableArg { get; }

        public Guid RoomGuid => ConstArg.RoomGuid;

        public DateTime GameStartDateTime { get; private set; }

        public bool IsPlaying => State == RoomState.PlayGame;
        public int MemberNum => memberDictionary.Count;

        public bool IsFullMember => MemberNum >= ConstArg.MaxMemberNum;

        public bool IsLowerLimitGameTime => VariableArg.GameTime <= ConstArg.GameTimeRange.x;
        public bool IsUpperLimitGameTime => VariableArg.GameTime >= ConstArg.GameTimeRange.y;
        public bool IsLowerLimitWolfNum => VariableArg.WolfNum <= ConstArg.WolfNumRange.x;
        public bool IsUpperLimitWolfNum => VariableArg.WolfNum >= ConstArg.WolfNumRange.y;

        public event Action<RoomData> OnTimeOverEvent;

        public RoomData(PlayerDataHolder playerDataHolder, NetworkConnection connection, ConstArg constArg,
            VariableArg variableArg) {
            this.playerDataHolder = playerDataHolder;

            this.DateTime = DateTime.UtcNow;
            this.HostConnectionId = connection.connectionId;
            this.State = RoomState.ReadyGame;
            this.ConstArg = constArg;
            this.VariableArg = variableArg;

            themeBuilder = new ThemeBuilder(constArg.ThemeUnitList);
            memberDictionary = new Dictionary<int, DateTime>();
            wolfMemberList = new List<int>();
            voteDictionary = new Dictionary<int, int>();

            JoinRoom(connection);
        }

        public bool ContainMember(int connectionId) {
            return memberDictionary.ContainsKey(connectionId);
        }

        public bool JoinRoom(NetworkConnection joinedPlayerConnection) {
            var connectionId = joinedPlayerConnection.connectionId;
            if (ContainMember(connectionId)) {
                return false;
            }

            memberDictionary.Add(connectionId, DateTime.Now);
            return true;
        }

        public bool LeaveRoom(NetworkConnection leftPlayerConnection) {
            var connectionId = leftPlayerConnection.connectionId;
            if (!ContainMember(connectionId)) {
                return false;
            }

            memberDictionary.Remove(connectionId);
            return true;
        }

        public bool ChangeHost() {
            if (MemberNum < 1) {
                return false;
            }

            var firstMember = memberDictionary.OrderBy(m => m.Value).First();
            HostConnectionId = firstMember.Key;
            return true;
        }

        public bool ChangeGameTime(int changeForward) {
            if (changeForward == 0) {
                return false;
            }

            var value = VariableArg.GameTime;
            var resolution = ConstArg.GameTimeResolution;
            var range = ConstArg.GameTimeRange;

            value += changeForward > 0 ? resolution : -resolution;
            value = Mathf.Max(Mathf.Min(value, range.y), range.x);
            if (value == VariableArg.GameTime) {
                return false;
            }

            VariableArg.GameTime = value;
            return true;
        }

        public bool ChangeWolfNum(int changeForward) {
            if (changeForward == 0) {
                return false;
                ;
            }

            var value = VariableArg.WolfNum;
            var resolution = ConstArg.WolfNumResolution;
            var range = ConstArg.WolfNumRange;

            value += changeForward > 0 ? resolution : -resolution;
            value = Mathf.Max(Mathf.Min(value, range.y), range.x);
            if (value == VariableArg.WolfNum) {
                return false;
            }

            VariableArg.WolfNum = value;
            return true;
        }

        public void StartGame() {
            State = RoomState.PlayGame;
            wolfMemberList.Clear();
            voteDictionary.Clear();

            (peopleTheme, wolfTheme) = themeBuilder.BuildTheme();
            wolfMemberList = new List<int>(
                memberDictionary.Keys
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(VariableArg.WolfNum)
            );
            GameStartDateTime = DateTime.UtcNow;
            gameTimeDisposable = Observable
                .Timer(TimeSpan.FromSeconds(VariableArg.GameTime))
                .Subscribe(_ => {
                    StopGame();
                    OnTimeOverEvent?.Invoke(this);
                });
        }

        public void StopGame() {
            gameTimeDisposable?.Dispose();
            gameTimeDisposable = null;

            State = RoomState.ReadyGame;
            voteDictionary.Clear();
            wolfMemberList.Clear();
        }

        public string GetTheme(int connectionId) {
            return wolfMemberList.Contains(connectionId) ? wolfTheme : peopleTheme;
        }

        public void VotePlayer(int voteOriginPlayerConnectionId, int voteForwardPlayerConnectionId) {
            if (voteDictionary.ContainsKey(voteOriginPlayerConnectionId)) {
                voteDictionary[voteOriginPlayerConnectionId] = voteForwardPlayerConnectionId;
            } else {
                voteDictionary.Add(voteOriginPlayerConnectionId, voteForwardPlayerConnectionId);
            }
        }

        public IEnumerable<NetworkConnection> GetAllMemberConnection() {
            return memberDictionary.Keys
                .Select(id => playerDataHolder.GetPlayerData(id))
                .Select(data => data.Connection);
        }

        public IEnumerable<NetworkConnection> GetAllMemberConnectionOrderByAccess() {
            return memberDictionary
                .OrderBy(m => m.Value)
                .Select(m => playerDataHolder.GetPlayerData(m.Key))
                .Select(data => data.Connection);
        }

        public RoomSimpleData CreateRoomSimpleData() {
            var playerData = playerDataHolder.GetPlayerData(HostConnectionId);
            Debug.Log(playerData.PlayerName);
            return new RoomSimpleData {
                RoomGuid = RoomGuid,
                RoomName = ConstArg.RoomName,
                HostName = playerData.PlayerName,
                MaxMemberNum = ConstArg.MaxMemberNum,
                MemberNum = MemberNum,
            };
        }

        public RoomDetailData CreateRoomDetailData() {
            var playerDataList = memberDictionary
                .OrderBy(p => p.Value)
                .Select(p => new PlayerSimpleData {
                    PlayerConnectionId = p.Key,
                    PlayerName = playerDataHolder.GetPlayerData(p.Key).PlayerName
                }).ToList();

            return new RoomDetailData {
                RoomGuid = RoomGuid,
                RoomName = ConstArg.RoomName,
                MaxMemberNum = ConstArg.MaxMemberNum,
                PlayerDataList = playerDataList,
            };
        }
    }
}