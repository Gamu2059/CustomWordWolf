using System;
using System.Collections.Generic;
using System.Linq;
using ConnectData;
using Mirror;
using UniRx;

namespace ManagedData {
    public enum RoomState {
        ReadyGame,
        PlayGame,
    }

    public struct ConstArg {
        public Guid RoomGuid;
        public string RoomName;
        public int MaxMemberNum;
        public List<(string, string)> ThemeUnitList;
    }

    public struct VariableArg {
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
        public Guid RoomGuid { get; }
        public string RoomName { get; }
        public int MaxMemberNum { get; }
        public int HostConnectionId { get; private set; }
        public RoomState State { get; private set; }

        public VariableArg VariableArg { get; private set; }

        public DateTime GameStartDateTime { get; private set; }

        public bool IsPlaying => State == RoomState.PlayGame;
        public int MemberNum => memberDictionary.Count;

        public bool IsFullMember => MemberNum >= MaxMemberNum;

        public event Action<RoomData> OnTimeOverEvent;

        public RoomData(PlayerDataHolder playerDataHolder, NetworkConnection connection, ConstArg constArg,
            VariableArg variableArg) {
            this.playerDataHolder = playerDataHolder;

            this.DateTime = DateTime.UtcNow;
            this.RoomGuid = constArg.RoomGuid;
            this.RoomName = constArg.RoomName;
            this.MaxMemberNum = constArg.MaxMemberNum;
            this.State = RoomState.ReadyGame;
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
            return new RoomSimpleData {
                RoomGuid = RoomGuid,
                RoomName = RoomName,
                HostName = playerData?.PlayerName,
                MaxMemberNum = MaxMemberNum,
                MemberNum = MemberNum,
            };
        }

        public RoomDetailData CreateRoomDetailData() {
            var playerDataList = memberDictionary
                .OrderBy(p => p.Value)
                .Select(p => new PlayerSimpleData {
                    PlayerConnectionId = p.Key,
                    PlayerName = playerDataHolder.GetPlayerData(p.Key)?.PlayerName
                }).ToList();

            return new RoomDetailData {
                RoomGuid = RoomGuid,
                RoomName = RoomName,
                MaxMemberNum = MaxMemberNum,
                PlayerDataList = playerDataList,
            };
        }
    }
}