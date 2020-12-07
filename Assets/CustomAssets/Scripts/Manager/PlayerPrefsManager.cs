using UnityEngine;

namespace Common {
    public class PlayerPrefsManager {

        private const string PlayerNameKey = "PlayerName";
        
        public static string PlayerName {
            get {
                return PlayerPrefs.GetString(PlayerNameKey, "こんにちは世界");
            }
            set {
                PlayerPrefs.SetString(PlayerNameKey, value);
            }
        }
    }
}