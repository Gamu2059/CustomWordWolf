using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title.RoomList {
    public class JoinRoomElementView : MonoBehaviour {
        [SerializeField]
        private Text roomNameText;

        [SerializeField]
        private Text roomMemberNumText;

        [SerializeField]
        private Button joinRoomButton;

        public IObservable<Unit> JoinRoomButtonObservable => joinRoomButton.OnClickAsObservable();

        public void Initialize(ConnectRoomData roomData) {
            roomNameText.text = roomData.RoomName;
            roomMemberNumText.text = $"{roomData.MemberNum}人";
        }
    }
}