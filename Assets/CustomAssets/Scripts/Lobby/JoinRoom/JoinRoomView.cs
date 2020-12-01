using System;
using Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom {
    public class JoinRoomView :
        MonoBehaviour,
        Initializable,
        IStateChangeable {
        [SerializeField]
        private Button backTitleButton;

        [SerializeField]
        private Button editPlayerNameButton;

        [SerializeField]
        private Button updateRoomListButton;

        [SerializeField]
        private Button createRoomButton;

        [SerializeField]
        private Text playerNameText;

        public IObservable<Unit> BackTitleObservable => backTitleButton.OnClickAsObservable();
        public IObservable<Unit> EditPlayerNameObservable => editPlayerNameButton.OnClickAsObservable();
        public IObservable<Unit> UpdateRoomListObservable => updateRoomListButton.OnClickAsObservable();
        public IObservable<Unit> CreateRoomObservable => createRoomButton.OnClickAsObservable();

        public void Initialize() {
        }

        public async UniTask StateInAsync(IChangeStateArg arg) {
            gameObject.SetActive(true);
        }

        public async UniTask StateOutAsync() {
            gameObject.SetActive(false);
        }

        public void SetPlayerName(string playerName) {
            playerNameText.text = playerName;
        }
    }
}