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
        private Dictionary<NetworkConnection, DateTime> memberDictionary;
        private List<uint> wolfMemberList;
        private Dictionary<uint, uint> voteDictionary;

        private string peopleTheme;
        private string wolfTheme;

        private IDisposable gameTimeDisposable;

        public DateTime DateTime { get; }
        public Guid RoomGuid { get; }
        public string RoomName { get; }
        public int MaxMemberNum { get; }
        public uint HostNetId { get; private set; }
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
            memberDictionary = new Dictionary<NetworkConnection, DateTime>();
            wolfMemberList = new List<uint>();
            voteDictionary = new Dictionary<uint, uint>();

            JoinRoom(connection);
        }

        public bool JoinRoom(NetworkConnection joinedPlayerConnection) {
            if (memberDictionary.ContainsKey(joinedPlayerConnection)) {
                return false;
            }

            memberDictionary.Add(joinedPlayerConnection, DateTime.Now);
            return true;
        }

        public bool LeaveRoom(NetworkConnection leftPlayerConnection) {
            if (!memberDictionary.ContainsKey(leftPlayerConnection)) {
                return false;
            }

            memberDictionary.Remove(leftPlayerConnection);
            return true;
        }

        public bool ChangeHost() {
            var fastJoinMember = memberDictionary.OrderBy(m => m.Value).First();
            if (fastJoinMember.Key == null) {
                return false;
            }

            HostNetId = fastJoinMember.Key.identity.netId;
            return true;
        }

        public void StartGame() {
            State = RoomState.PlayGame;
            wolfMemberList.Clear();
            voteDictionary.Clear();

            (peopleTheme, wolfTheme) = themeBuilder.BuildTheme();
            wolfMemberList = new List<uint>(
                memberDictionary.Keys
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(VariableArg.WolfNum)
                    .Select(k => k.identity.netId)
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

        public string GetTheme(uint netId) {
            return wolfMemberList.Contains(netId) ? wolfTheme : peopleTheme;
        }

        public void VotePlayer(uint voteOriginPlayerNetId, uint voteForwardPlayerNetId) {
            if (voteDictionary.ContainsKey(voteOriginPlayerNetId)) {
                voteDictionary[voteOriginPlayerNetId] = voteForwardPlayerNetId;
            } else {
                voteDictionary.Add(voteOriginPlayerNetId, voteForwardPlayerNetId);
            }
        }

        public IEnumerable<NetworkConnection> GetAllMember() {
            return memberDictionary.Keys;
        }

        public List<NetworkConnection> GetAllMemberList() {
            return GetAllMember().ToList();
        }

        public IEnumerable<NetworkConnection> GetAllMemberOrderByAccess() {
            return memberDictionary
                .OrderBy(m => m.Value)
                .Select(m => m.Key);
        }

        public List<NetworkConnection> GetAllMemberListOrderByAccess() {
            return GetAllMemberOrderByAccess().ToList();
        }

        public bool ContainMember(uint playerNetId) {
            return memberDictionary.Keys.Any(m => m.identity.netId == playerNetId);
        }

        public RoomSimpleData CreateRoomSimpleData() {
            var playerData = playerDataHolder.GetPlayerData(HostNetId);
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
                    PlayerNetId = p.Key.identity.netId,
                    PlayerName = playerDataHolder.GetPlayerData(p.Key.identity.netId)?.PlayerName
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