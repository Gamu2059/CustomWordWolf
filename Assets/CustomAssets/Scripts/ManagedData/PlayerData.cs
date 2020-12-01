using UnityEngine.PlayerLoop;

namespace ManagedData {
    public class PlayerData {
        public string PlayerName { get; private set; }

        public PlayerData(string playerName) {
            ApplyPlayerName(playerName);
        }

        public void ApplyPlayerName(string playerName) {
            PlayerName = playerName;
        }
    }
}