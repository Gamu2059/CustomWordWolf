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
            
        }

        public void SetTheme(string theme) {
            
        }

        public void SetRemainTime(int remainTime) {
            
        }
    }
}