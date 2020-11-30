using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class TitleView : MonoBehaviour {
        [SerializeField]
        private Text playerNameText;

        public string PlayerName => playerNameText.text;

        [SerializeField]
        private Button editPlayerNameButton;

        public IObservable<Unit> EditPlayerNameButtonObservable => editPlayerNameButton.OnClickAsObservable();

        [SerializeField]
        private Button createRoomButton;

        public IObservable<Unit> CreateRoomButtonObservable => createRoomButton.OnClickAsObservable();
        
        [SerializeField]
        private Button joinRoomButton;

        public IObservable<Unit> JoinRoomButtonObservable => joinRoomButton.OnClickAsObservable();
        
        public void Initialize(string playerName) {
            SetPlayerName(playerName);
        }

        public void SetPlayerName(string playerName) {
            playerNameText.text = playerName.Trim();
        }
    }
}