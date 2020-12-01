using System;
using Common;
using UniRx;

namespace Lobby.JoinRoom {
    public class JoinRoomModel {

        private ReactiveProperty<string> playerName;
        public IObservable<string> PlayerNameObservable => playerName;
        public string PlayerName => playerName.Value;
        
        public JoinRoomModel() {
            var playerName = PlayerPrefsManager.PlayerName;
            this.playerName = new ReactiveProperty<string>(playerName);
        }

        public void SetPlayerName(string playerName) {
            PlayerPrefsManager.PlayerName = playerName;
            this.playerName.Value = playerName;
        }
    }
}