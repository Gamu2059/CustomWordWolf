using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
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
        private Button joinRoomButton;

        public IObservable<Unit> JoinRoomObservable => joinRoomButton.OnClickAsObservable();

        public void Initialize(ConnectRoomData roomData) {
            roomNameText.text = roomData.RoomName;
            hostNameText.text = roomData.HostName;
            roomMemberNumText.text = $"{roomData.MemberNum}/{roomData.MaxMemberNum}人";
        }
    }
}