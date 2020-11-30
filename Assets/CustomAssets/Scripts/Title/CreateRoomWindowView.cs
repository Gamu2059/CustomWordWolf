using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class CreateRoomWindowView : MonoBehaviour {

        [SerializeField]
        private InputField createRoomNameInputField;

        public string RoomName => createRoomNameInputField.text;
        
        [SerializeField]
        private Button createRoomButton;

        public IObservable<Unit> CreateRoomButtonObservable => createRoomButton.OnClickAsObservable();

        [SerializeField]
        private Button closeButton;

        public IObservable<Unit> CloseButtonObservable => closeButton.OnClickAsObservable();

        public void Initialize() {
            createRoomNameInputField.OnValueChangedAsObservable().Subscribe(OnEditNameChanged).AddTo(gameObject);
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void OnEditNameChanged(string roomName) {
            var isValidName =
                roomName != null &&
                !string.IsNullOrEmpty(roomName) &&
                !string.IsNullOrWhiteSpace(roomName);

            createRoomButton.interactable = isValidName;
        }
    }
}
