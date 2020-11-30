using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class JoinRoomWindowView : MonoBehaviour {
        [SerializeField]
        private Button updateButton;

        public IObservable<Unit> UpdateButtonObservable => updateButton.OnClickAsObservable();
        
        [SerializeField]
        private Button closeButton;

        public IObservable<Unit> CloseButtonObservable => closeButton.OnClickAsObservable();

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}