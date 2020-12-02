using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayerList {
    public class PlayerElementView : MonoBehaviour {
        [SerializeField]
        private Text playerNameText;

        [SerializeField]
        private CustomButton voteButton;

        [SerializeField]
        private RectTransform votedRectT;

        public IObservable<Unit> VoteObservable => voteButton.Button.OnClickAsObservable();

        public void Initialize(string playerName) {
            playerNameText.text = playerName;

            // なぜかスケールが変わるので1にする
            transform.localScale = Vector3.one;
        }
    }
}