using System;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom {
    /// <summary>
    /// 部屋参加UIのViewコンポーネント。
    /// </summary>
    public class JoinRoomView :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        [SerializeField]
        private CustomButton backTitleButton;

        [SerializeField]
        private CustomButton editPlayerNameButton;

        [SerializeField]
        private CustomButton updateRoomListButton;

        [SerializeField]
        private CustomButton createRoomButton;

        [SerializeField]
        private Text playerNameText;

        /// <summary>
        /// 戻るボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> BackTitleObservable => backTitleButton.Button.OnClickAsObservable();

        /// <summary>
        /// プレイヤー名変更ボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> EditPlayerNameObservable => editPlayerNameButton.Button.OnClickAsObservable();

        /// <summary>
        /// 部屋一覧更新ボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> UpdateRoomListObservable => updateRoomListButton.Button.OnClickAsObservable();

        /// <summary>
        /// 部屋作成ボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> CreateRoomObservable => createRoomButton.Button.OnClickAsObservable();

        /// <summary>
        /// Viewを初期化する。
        /// </summary>
        public void Initialize() {
        }

        /// <summary>
        /// 表示処理。
        /// </summary>
        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            backTitleButton.Show();
            editPlayerNameButton.Show();
            updateRoomListButton.Show();
            createRoomButton.Show();
        }

        /// <summary>
        /// 非表示処理。
        /// </summary>
        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// プレイヤー名をセットする。
        /// </summary>
        /// <param name="playerName">表示したいプレイヤー名</param>
        public void SetPlayerName(string playerName) {
            playerNameText.text = playerName;
        }
    }
}