using System;
using System.Collections;
using System.Collections.Generic;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

namespace Title {
    public class TitlePresenter : MonoBehaviour {
        [SerializeField]
        private TitleView view;

        [SerializeField]
        private EditPlayerNameWindowPresenter editPlayerNameWindowPresenter;

        [SerializeField]
        private CreateRoomWindowPresenter createRoomWindowPresenter;

        [SerializeField]
        private JoinRoomWindowPresenter joinRoomWindowPresenter;

        [Header("パラメータ")]
        [SerializeField]
        private string defaultPlayerName = "ほげほげさん";

        [Scene]
        [SerializeField]
        private string gameScene;

        private void Start() {
            Initialize();
        }

        private void Initialize() {
            var playerName = PlayerPrefsManager.PlayerName;
            if (playerName == null) {
                playerName = defaultPlayerName;
            }

            view.Initialize(playerName);
            SetViewEvents();
            InitializeChildPresenter();
            SetChildPresenterEvents();
        }

        private void SetViewEvents() {
            view.EditPlayerNameButtonObservable
                .Subscribe(_ => editPlayerNameWindowPresenter.Show(view.PlayerName))
                .AddTo(gameObject);
            view.CreateRoomButtonObservable
                .Subscribe(_ => createRoomWindowPresenter.Show())
                .AddTo(gameObject);
            view.JoinRoomButtonObservable
                .Subscribe(_ => joinRoomWindowPresenter.Show())
                .AddTo(gameObject);
        }

        private void InitializeChildPresenter() {
            editPlayerNameWindowPresenter.Initialize();
            createRoomWindowPresenter.Initialize();
            joinRoomWindowPresenter.Initialize();
        }

        private void SetChildPresenterEvents() {
            editPlayerNameWindowPresenter.OnPlayerNameEdited += OnPlayerNameEdited;
            createRoomWindowPresenter.OnRoomNameDecided += OnRoomNameDecided;
            joinRoomWindowPresenter.OnRoomDecided += OnJoinRoomDecided;
        }

        private async void OnPlayerNameEdited(string playerName) {
            var applyPlayerNameApi = new ApplyPlayerNameApi();
            var response = await applyPlayerNameApi.Request(new ApplyPlayerName.Request {PlayerName = playerName});
            if (response.Result == ApplyPlayerName.Result.Succeed) {
                PlayerPrefsManager.PlayerName = playerName;
                view.SetPlayerName(playerName);
            }
        }

        private async void OnRoomNameDecided(string roomName) {
            var createRoomApi = new CreateRoomApi();
            var response = await createRoomApi.Request(new CreateRoom.Request {RoomName = roomName});
            if (response.Result == CreateRoom.Result.Succeed) {
                SceneManager.LoadScene(gameScene);
            }
        }

        private async void OnJoinRoomDecided(Guid roomGuid) {
            Debug.Log(3);
            var joinRoomApi = new JoinRoomApi();
            var response = await joinRoomApi.Request(new JoinRoom.Request {RoomGuid = roomGuid});
            if (response.Result == JoinRoom.Result.Succeed) {
                SceneManager.LoadScene(gameScene);
            }
        }
    }
}