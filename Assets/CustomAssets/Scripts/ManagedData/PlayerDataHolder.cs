using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace ManagedData {
    public class PlayerDataHolder {
        private Dictionary<NetworkIdentity, PlayerData> playerDictionary;

        private void CheckDictionary() {
            if (playerDictionary == null) {
                playerDictionary = new Dictionary<NetworkIdentity, PlayerData>();
            }
        }

        public void CreatePlayerData(NetworkIdentity identity, string playerName) {
            CheckDictionary();
            if (playerDictionary.ContainsKey(identity)) {
                Debug.LogWarning("既にこのプレイヤーは登録されています");
            } else {
                playerDictionary.Add(identity, new PlayerData(playerName));
            }
        }

        public void RemovePlayerData(NetworkIdentity identity) {
            CheckDictionary();
            playerDictionary.Remove(identity);
        }

        public PlayerData GetPlayerData(NetworkIdentity identity) {
            CheckDictionary();
            if (playerDictionary.TryGetValue(identity, out var playerData)) {
                return playerData;
            }

            return null;
        }

        public PlayerData GetPlayerData(uint netId) {
            CheckDictionary();
            var pair = playerDictionary.FirstOrDefault(p => p.Key.netId == netId);
            return pair.Key == null ? null : pair.Value;
        }

        /// <summary>
        /// プレイヤーが存在するかどうかを取得する。
        /// </summary>
        public bool ExistPlayerData(NetworkIdentity identity) {
            CheckDictionary();
            return playerDictionary.ContainsKey(identity);
        }
    }
}