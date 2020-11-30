using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GameView : MonoBehaviour {
        [SerializeField]
        private Button leaveRoomButton;

        public IObservable<Unit> LeaveRoomButtonObservable => leaveRoomButton.OnClickAsObservable();

        [SerializeField]
        private Button startGameButton;

        public IObservable<Unit> StartGameButtonObservable => startGameButton.OnClickAsObservable();

        [SerializeField]
        private Button backRoomButton;

        public IObservable<Unit> BackRoomButtonObservable => backRoomButton.OnClickAsObservable();

        [SerializeField]
        private Text remainTimeText;

        [SerializeField]
        private Text themeText;

        public void Initialize() {
            SetReadyGameUI();
            SetActiveStartButton(false);
        }

        public void SetActiveStartButton(bool isActive) {
            startGameButton.gameObject.SetActive(isActive);
        }

        public void SetTheme(string theme) {
            themeText.text = $"テーマは{theme}です";
        }

        public void SetRemainTime(int remainTime) {
            remainTimeText.text = $"残り時間 {remainTime}";
        }

        public void SetReadyGameUI() {
            remainTimeText.gameObject.SetActive(false);
            themeText.gameObject.SetActive(false);
            backRoomButton.gameObject.SetActive(false);
        }

        public void SetStartGameUI() {
            remainTimeText.gameObject.SetActive(true);
            themeText.gameObject.SetActive(true);
            backRoomButton.gameObject.SetActive(false);
        }

        public void SetTimeOverGameUI() {
            remainTimeText.gameObject.SetActive(false);
            themeText.gameObject.SetActive(false);
            backRoomButton.gameObject.SetActive(true);
        }
    }
}