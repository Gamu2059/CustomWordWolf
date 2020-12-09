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
    /// <summary>
    /// 部屋作成UIのViewコンポーネント。
    /// </summary>
    public class CreateRoomView :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        [SerializeField]
        private InputField createRoomNameInputField;

        [SerializeField]
        private CustomButton applyButton;

        [SerializeField]
        private CustomButton backButton;

        /// <summary>
        /// InputField上の部屋名を取得する。
        /// </summary>
        public string RoomName => createRoomNameInputField.text;

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
            createRoomNameInputField.OnValueChangedAsObservable().Subscribe(OnEditNameChanged).AddTo(gameObject);
        }

        /// <summary>
        /// 表示処理。
        /// </summary>
        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            applyButton.Show();
            backButton.Show();

            createRoomNameInputField.text = String.Empty;
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
        /// <param name="roomName">変更された部屋名</param>
        private void OnEditNameChanged(string roomName) {
            var isValidRoomName =
                roomName != null &&
                !string.IsNullOrEmpty(roomName) &&
                !string.IsNullOrWhiteSpace(roomName);

            applyButton.Button.interactable = isValidRoomName;
        }
    }
}