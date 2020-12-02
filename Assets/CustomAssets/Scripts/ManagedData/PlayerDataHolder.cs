using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace ManagedData {
    public class PlayerDataHolder {
        private Dictionary<int, PlayerData> playerDictionary;

        private void CheckDictionary() {
            if (playerDictionary == null) {
                playerDictionary = new Dictionary<int, PlayerData>();
            }
        }

        public bool ExistPlayerData(int connectionId) {
            CheckDictionary();
            return playerDictionary.ContainsKey(connectionId);
        }

        public bool ExistPlayerData(NetworkConnection connection) {
            return ExistPlayerData(connection.connectionId);
        }

        public PlayerData GetPlayerData(int connectionId) {
            if (ExistPlayerData(connectionId)) {
                return playerDictionary[connectionId];
            }

            return null;
        }

        public PlayerData GetPlayerData(NetworkConnection connection) {
            return GetPlayerData(connection.connectionId);
        }

        public void CreatePlayerData(NetworkConnection connection, string playerName) {
            var connectionId = connection.connectionId;
            if (!ExistPlayerData(connectionId)) {
                playerDictionary.Add(connectionId, new PlayerData(connection, playerName));
            }
        }

        public void RemovePlayerData(NetworkConnection connection) {
            var connectionId = connection.connectionId;
            if (ExistPlayerData(connectionId)) {
                playerDictionary.Remove(connectionId);
            }
        }
    }
}