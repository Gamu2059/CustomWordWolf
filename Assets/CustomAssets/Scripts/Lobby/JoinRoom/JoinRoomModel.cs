using System;
using Common;
using UniRx;

namespace Lobby.JoinRoom {
    /// <summary>
    /// 部屋参加UIのModel。
    /// </summary>
    public class JoinRoomModel {
        /// <summary>
        /// プレイヤー名。
        /// </summary>
        private ReactiveProperty<string> playerNameProperty;

        /// <summary>
        /// プレイヤー名のUniRxストリーム。
        /// </summary>
        public IObservable<string> PlayerNamePropertyObservable => playerNameProperty;

        /// <summary>
        /// プレイヤー名を取得する。
        /// </summary>
        public string PlayerName => playerNameProperty.Value;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public JoinRoomModel() {
            var playerName = PlayerPrefsManager.PlayerName;
            playerNameProperty = new ReactiveProperty<string>(playerName);
        }

        /// <summary>
        /// プレイヤー名をセットする。
        /// </summary>
        /// <param name="playerName">保持したいプレイヤー名</param>
        public void SetPlayerName(string playerName) {
            PlayerPrefsManager.PlayerName = playerName;
            playerNameProperty.Value = playerName;
        }
    }
}