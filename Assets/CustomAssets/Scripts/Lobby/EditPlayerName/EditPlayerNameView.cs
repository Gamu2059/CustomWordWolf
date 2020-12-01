using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.EditPlayerName {
    public class EditPlayerNameView :
        MonoBehaviour,
        Initializable {
        [SerializeField]
        private InputField editPlayerNameInputField;

        [SerializeField]
        private CustomButton applyButton;

        [SerializeField]
        private CustomButton backButton;

        public string PlayerName => editPlayerNameInputField.text;
        public IObservable<Unit> ApplyObservable => applyButton.Button.OnClickAsObservable();
        public IObservable<Unit> BackObservable => backButton.Button.OnClickAsObservable();

        public void Initialize() {
            editPlayerNameInputField.OnValueChangedAsObservable().Subscribe(OnEditNameChanged).AddTo(gameObject);
        }

        public async UniTask ShowAsync(IChangeStateArg arg) {
            gameObject.SetActive(true);
            applyButton.Show();
            backButton.Show();

            var editPlayerNameArg = arg as EditPlayerNameArg;
            if (editPlayerNameArg == null) {
                Debug.LogError("引数が適切ではありません");
                return;
            }

            editPlayerNameInputField.text = editPlayerNameArg.PlayerName;
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        private void OnEditNameChanged(string playerName) {
            var isValidName =
                playerName != null &&
                !string.IsNullOrEmpty(playerName) &&
                !string.IsNullOrWhiteSpace(playerName);

            applyButton.Button.interactable = isValidName;
        }
    }
}