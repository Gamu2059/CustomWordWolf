using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.JoinRoom.RoomList {
    public class JoinRoomElementPresenter : MonoBehaviour {
        [SerializeField]
        private JoinRoomElementView view;

        public event Action<Guid> OnJoinRoomDecidedEvent;

        public void Initialize(ConnectRoomData roomData) {
            view.Initialize(roomData);
            view.JoinRoomObservable.Subscribe(_ => OnJoinRoomDecidedEvent?.Invoke(roomData.RoomGuid)).AddTo(gameObject);
        }
    }
}