using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.PlayerList;
using UnityEngine;
using UniRx;

namespace Game {
    public class GamePresenter : MonoBehaviour {
        [SerializeField]
        private GameView view;

        [SerializeField]
        private PlayerListPresenter playerListPresenter;

        private void Start() {
            Initialize();
        }

        private void Initialize() {
            view.Initialize();
            SetViewEvents();
            InitializeChildPresenter();
            SetChildPresenterEvents();
        }

        private void SetViewEvents() {
            view.LeaveRoomButtonObservable
                .Subscribe(_ => OnRoomLeave())
                .AddTo(gameObject);
            view.StartGameButtonObservable
                .Subscribe(_ => OnGameStart())
                .AddTo(gameObject);
            view.BackRoomButtonObservable
                .Subscribe(_ => OnRoomBack())
                .AddTo(gameObject);
        }

        private void InitializeChildPresenter() {
            playerListPresenter.Initialize();
        }

        private void SetChildPresenterEvents() {
        }

        private void OnRoomLeave() {
        }

        private void OnGameStart() {
        }

        private void OnRoomBack() {
        }
    }
}