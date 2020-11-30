using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Title {
    public class EditPlayerNameWindowPresenter : MonoBehaviour {
        [SerializeField]
        private EditPlayerNameWindowView view;

        public event Action<string> OnPlayerNameEdited;

        public void Initialize() {
            view.Initialize();
            view.CloseButtonObservable.Subscribe(_ => Hide()).AddTo(gameObject);
            view.ApplyButtonObservable.Subscribe(_ => OnPlayerNameApplied()).AddTo(gameObject);
            Hide();
        }

        public void Show(string playerName) {
            view.Show(playerName);
        }

        public void Hide() {
            view.Hide();
        }

        private void OnPlayerNameApplied() {
            Hide();
            OnPlayerNameEdited?.Invoke(view.PlayerName);
        }
    }
}