using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class EditPlayerNameWindowView : MonoBehaviour {
        [SerializeField]
        private InputField editPlayerNameInputField;

        public string PlayerName => editPlayerNameInputField.text;

        [SerializeField]
        private Button applyButton;

        public IObservable<Unit> ApplyButtonObservable => applyButton.OnClickAsObservable();

        [SerializeField]
        private Button closeButton;

        public IObservable<Unit> CloseButtonObservable => closeButton.OnClickAsObservable();

        public void Initialize() {
            editPlayerNameInputField.OnValueChangedAsObservable().Subscribe(OnEditNameChanged).AddTo(gameObject);
        }

        public void Show(string playerName) {
            editPlayerNameInputField.text = playerName.Trim();
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void OnEditNameChanged(string playerName) {
            var isValidName =
                playerName != null &&
                !string.IsNullOrEmpty(playerName) &&
                !string.IsNullOrWhiteSpace(playerName);

            applyButton.interactable = isValidName;
        }
    }
}