using System;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom {
    public class JoinRoomView :
        MonoBehaviour,
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

        public IObservable<Unit> BackTitleObservable => backTitleButton.Button.OnClickAsObservable();
        public IObservable<Unit> EditPlayerNameObservable => editPlayerNameButton.Button.OnClickAsObservable();
        public IObservable<Unit> UpdateRoomListObservable => updateRoomListButton.Button.OnClickAsObservable();
        public IObservable<Unit> CreateRoomObservable => createRoomButton.Button.OnClickAsObservable();

        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            backTitleButton.Show();
            editPlayerNameButton.Show();
            updateRoomListButton.Show();
            createRoomButton.Show();
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        public void SetPlayerName(string playerName) {
            playerNameText.text = playerName;
        }
    }
}