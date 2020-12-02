using System;
using System.Collections;
using System.Collections.Generic;
using Api;
using ConnectData;
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

        private GameModel model;

        private IDisposable changeHostApi;
        private IDisposable startGameApi;
        private IDisposable timeOverApi;
        private IDisposable votePlayerApi;

        private void Start() {
            Initialize();
            ConnectInitializationAsync().Forget();
        }

        private void Initialize() {
            model = new GameModel();
            view.Initialize();
            SetViewEvents();
            InitializeChildPresenter();
            SetChildPresenterEvents();
            BindServerReceiveEvents();

            OnRoomBack();
        }

        private void SetViewEvents() {
            view.LeaveRoomButtonObservable
                .Subscribe(_ => OnRoomLeave())
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

        private void BindServerReceiveEvents() {
            // changeHostApi = new UpdateMemberReceiver(OnChangeHostReceived);
            startGameApi = new StartGameReceiver(OnStartGameReceived);
        }

        private async UniTaskVoid ConnectInitializationAsync() {
            var loadGameSceneApi = new GetRoomDetailApi();
            var response = await loadGameSceneApi.Request(new GetRoomDetailData.Request());
            model.SetHost(response.IsHost);
            if (model.IsHost) {
                view.SetActiveStartButton(true);
                view.StartGameButtonObservable
                    .Subscribe(_ => OnGameStartAsync())
                    .AddTo(gameObject);
            }
        }

        private void OnRoomLeave() {
        }

        private async void OnGameStartAsync() {
            var startGameApi = new StartGameApi();
            var response = await startGameApi.Request(new StartGame.Request());
            Debug.Log("ゲーム開始 " + response.Result);
        }

        private void OnRoomBack() {
            model.ReadyGame();
            view.SetReadyGameUI();
        }

        private async void OnPlayerVoteAsync() {
            var votePlayerApi = new VotePlayerApi();
            var response = await votePlayerApi.Request(new VotePlayer.Request {VoteForwardPlayerConnectionId = 1});
            Debug.Log("投票 " + response.Result);
        }

        // private void OnChangeHostReceived(UpdateMember.SendPlayer data) {
        //     model.SetHost(true);
        //     view.SetActiveStartButton(true);
        //     view.StartGameButtonObservable
        //         .Subscribe(_ => OnGameStartAsync())
        //         .AddTo(gameObject);
        // }

        private void OnStartGameReceived(StartGame.SendRoom data) {
            model.StartGame(data);
            view.SetStartGameUI();
            model.RemainTimeObservable.Subscribe(t => view.SetRemainTime(t)).AddTo(gameObject);
            view.SetTheme(data.Theme);

            if (model.IsHost) {
                view.SetActiveStartButton(false);
            }

            timeOverApi = new TimeOverReceiver(OnTimeOverReceived);
            votePlayerApi = new VotePlayerReceiver(OnVotePlayerReceived);
        }

        private void OnTimeOverReceived(TimeOver.SendRoom data) {
            model.TimeOverGame(data);
            view.SetTimeOverGameUI();

            if (model.IsHost) {
                view.SetActiveStartButton(true);
            }
        }

        private void OnVotePlayerReceived(VotePlayer.SendRoom data) {
        }
    }
}