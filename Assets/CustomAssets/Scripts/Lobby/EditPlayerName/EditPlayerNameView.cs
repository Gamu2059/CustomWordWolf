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
    /// <summary>
    /// プレイヤー名変更UIのViewコンポーネント。
    /// </summary>
    public class EditPlayerNameView :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        [SerializeField]
        private InputField editPlayerNameInputField;

        [SerializeField]
        private CustomButton applyButton;

        [SerializeField]
        private CustomButton backButton;

        /// <summary>
        /// InputField上のプレイヤー名を取得する。
        /// </summary>
        public string PlayerName => editPlayerNameInputField.text;

        /// <summary>
        /// 適用ボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> ApplyObservable => applyButton.Button.OnClickAsObservable();

        /// <summary>
        /// 戻るボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> BackObservable => backButton.Button.OnClickAsObservable();

        /// <summary>
        /// Viewを初期化する。
        /// </summary>
        public void Initialize() {
            editPlayerNameInputField.OnValueChangedAsObservable().Subscribe(OnChangeEditName).AddTo(gameObject);
        }

        /// <summary>
        /// 表示処理。
        /// </summary>
        public async UniTask ShowAsync(IChangeStateArg arg) {
            gameObject.SetActive(true);
            applyButton.Show();
            backButton.Show();

            // 引数をダウンキャストして値を取得する
            var editPlayerNameArg = arg as EditPlayerNameArg;
            if (editPlayerNameArg == null) {
                Debug.LogError("引数が適切ではありません");
                return;
            }

            editPlayerNameInputField.text = editPlayerNameArg.PlayerName;
        }

        /// <summary>
        /// 非表示処理。
        /// </summary>
        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// InputFieldの文字列が変更された時の処理。
        /// </summary>
        /// <param name="playerName">変更されたプレイヤー名</param>
        private void OnChangeEditName(string playerName) {
            var isValidPlayerName =
                playerName != null &&
                !string.IsNullOrEmpty(playerName) &&
                !string.IsNullOrWhiteSpace(playerName);

            applyButton.Button.interactable = isValidPlayerName;
        }
    }
}