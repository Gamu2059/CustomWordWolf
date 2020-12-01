using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.CreateRoom {
    public class CreateRoomView :
        MonoBehaviour,
        Initializable {
        [SerializeField]
        private InputField createRoomNameInputField;

        [SerializeField]
        private CustomButton applyButton;

        [SerializeField]
        private CustomButton backButton;

        public string RoomName => createRoomNameInputField.text;
        public IObservable<Unit> ApplyObservable => applyButton.Button.OnClickAsObservable();

        public IObservable<Unit> BackObservable => backButton.Button.OnClickAsObservable();

        public void Initialize() {
            createRoomNameInputField.OnValueChangedAsObservable().Subscribe(OnEditNameChanged).AddTo(gameObject);
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            applyButton.Show();
            backButton.Show();

            createRoomNameInputField.text = String.Empty;
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        private void OnEditNameChanged(string roomName) {
            var isValidName =
                roomName != null &&
                !string.IsNullOrEmpty(roomName) &&
                !string.IsNullOrWhiteSpace(roomName);

            applyButton.Button.interactable = isValidName;
        }
    }
}