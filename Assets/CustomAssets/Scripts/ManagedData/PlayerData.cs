using Mirror;
using UnityEngine.PlayerLoop;

namespace ManagedData {
    public class PlayerData {
        public NetworkConnection Connection { get; }
        public string PlayerName { get; private set; }

        public PlayerData(NetworkConnection connection, string playerName) {
            Connection = connection;
            ApplyPlayerName(playerName);
        }

        public void ApplyPlayerName(string playerName) {
            PlayerName = playerName;
        }
    }
}