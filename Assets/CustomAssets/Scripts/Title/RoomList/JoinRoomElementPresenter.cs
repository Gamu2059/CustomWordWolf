using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Title.RoomList {
    public class JoinRoomElementPresenter : MonoBehaviour {
        [SerializeField]
        private JoinRoomElementView view;

        public event Action<Guid> OnRoomDecided;

        public void Initialize(ConnectRoomData roomData) {
            view.Initialize(roomData);
            view.JoinRoomButtonObservable.Subscribe(_ => OnRoomDecided?.Invoke(roomData.RoomGuid)).AddTo(gameObject);
        }
    }
}