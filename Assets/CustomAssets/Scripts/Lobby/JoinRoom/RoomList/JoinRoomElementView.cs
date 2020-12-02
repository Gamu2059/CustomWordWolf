using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom.RoomList {
    public class JoinRoomElementView : MonoBehaviour {
        [SerializeField]
        private Text roomNameText;

        [SerializeField]
        private Text hostNameText;

        [SerializeField]
        private Text roomMemberNumText;

        [SerializeField]
        private CustomButton joinRoomButton;

        public IObservable<Unit> JoinRoomObservable => joinRoomButton.Button.OnClickAsObservable();

        public void Initialize(RoomSimpleData roomData) {
            roomNameText.text = roomData.RoomName;
            hostNameText.text = roomData.HostName;
            roomMemberNumText.text = $"{roomData.MemberNum}/{roomData.MaxMemberNum}人";

            // なぜか分からないけどスケールがおかしくなることがあるので1にする
            transform.localScale = Vector3.one;
        }
    }
}