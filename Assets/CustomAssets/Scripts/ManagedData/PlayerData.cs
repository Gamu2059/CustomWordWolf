using UnityEngine.PlayerLoop;

namespace ManagedData {
    public class PlayerData {
        public string PlayerName { get; private set; }

        public PlayerData(string playerName) {
            UpdatePlayerName(playerName);
        }

        public void UpdatePlayerName(string playerName) {
            PlayerName = playerName;
        }
    }
}