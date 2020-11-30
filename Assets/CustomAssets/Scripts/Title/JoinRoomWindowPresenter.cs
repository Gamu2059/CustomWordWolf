using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Title.RoomList;
using UnityEngine;
using UniRx;
using UnityEngine.PlayerLoop;

namespace Title {
    public class JoinRoomWindowPresenter : MonoBehaviour {
        [SerializeField]
        private JoinRoomWindowView view;

        [SerializeField]
        private JoinRoomListPresenter joinRoomListPresenter;

        public event Action<Guid> OnRoomDecided;

        public void Initialize() {
            view.CloseButtonObservable.Subscribe(_ => Hide()).AddTo(gameObject);
            view.UpdateButtonObservable.Subscribe(_ => joinRoomListPresenter.UpdateList().Forget()).AddTo(gameObject);

            joinRoomListPresenter.Initialize();
            joinRoomListPresenter.OnRoomDecided += guid => OnRoomDecided?.Invoke(guid);

            Hide();
        }

        public void Show() {
            view.Show();
            joinRoomListPresenter.UpdateList().Forget();
        }

        public void Hide() {
            view.Hide();
        }
    }
}