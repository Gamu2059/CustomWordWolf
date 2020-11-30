using UnityEngine;

namespace Common {
    public class PlayerPrefsManager {

        private const string PlayerNameKey = "PlayerName";
        
        public static string PlayerName {
            get {
                return PlayerPrefs.GetString(PlayerNameKey, null);
            }
            set {
                PlayerPrefs.SetString(PlayerNameKey, value);
            }
        }
    }
}