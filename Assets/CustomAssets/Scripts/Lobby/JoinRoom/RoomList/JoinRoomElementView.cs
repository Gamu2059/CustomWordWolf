using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom.RoomList {
    /// <summary>
    /// 部屋一覧要素UIのViewコンポーネント。
    /// </summary>
    public class JoinRoomElementView : MonoBehaviour {
        [SerializeField]
        private Text roomNameText;

        [SerializeField]
        private Text hostNameText;

        [SerializeField]
        private Text roomMemberNumText;

        [SerializeField]
        private CustomButton joinRoomButton;

        /// <summary>
        /// 部屋参加ボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> JoinRoomObservable => joinRoomButton.Button.OnClickAsObservable();

        /// <summary>
        /// Viewを初期化する。
        /// </summary>
        /// <param name="roomData">部屋データ</param>
        public void Initialize(RoomSimpleData roomData) {
            roomNameText.text = roomData.RoomName;
            hostNameText.text = roomData.HostName;
            roomMemberNumText.text = $"{roomData.MemberNum}/{roomData.MaxMemberNum}人";

            // なぜか分からないけどスケールがおかしくなることがあるので1にする
            transform.localScale = Vector3.one;
        }
    }
}