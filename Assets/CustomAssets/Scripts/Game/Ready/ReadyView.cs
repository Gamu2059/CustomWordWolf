using System;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;

namespace Game.Ready {
    public class ReadyView :
        MonoBehaviour,
        Initializable {
        [SerializeField]
        private CustomButton leaveRoomButton;

        [SerializeField]
        private CustomButton startGameButton;

        public IObservable<Unit> LeaveRoomObservable => leaveRoomButton.Button.OnClickAsObservable();
        public IObservable<Unit> StartGameObservable => startGameButton.Button.OnClickAsObservable();

        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            leaveRoomButton.Show();
            startGameButton.Show();
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        public void SetActiveStartGameButton(bool isActive) {
            startGameButton.gameObject.SetActive(isActive);
        }
    }
}