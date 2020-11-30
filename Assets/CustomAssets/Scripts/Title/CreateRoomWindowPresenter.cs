using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Title {
    public class CreateRoomWindowPresenter : MonoBehaviour {
        [SerializeField]
        private CreateRoomWindowView view;

        public event Action<string> OnRoomNameDecided;

        public void Initialize() {
            view.Initialize();
            view.CloseButtonObservable.Subscribe(_ => Hide()).AddTo(gameObject);
            view.CreateRoomButtonObservable.Subscribe(_ => OnDecideRoomName()).AddTo(gameObject);
            Hide();
        }

        public void Show() {
            view.Show();
        }

        public void Hide() {
            view.Hide();
        }

        private void OnDecideRoomName() {
            Hide();
            OnRoomNameDecided?.Invoke(view.RoomName);
        }
    }
}
