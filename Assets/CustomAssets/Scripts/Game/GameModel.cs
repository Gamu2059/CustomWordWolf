using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using UniRx;
using UnityEngine;

namespace Game {
    public enum GameState {
        ReadyGame,
        PlayGame,
        TimeOverGame,
    }

    public class GameModel {
        public GameState State { get; private set; }

        public bool IsHost { get; private set; }
        
        private ReactiveProperty<int> remainTime;
        public IObservable<int> RemainTimeObservable => remainTime;

        private IDisposable timer;

        public GameModel() {
            ReadyGame();
            remainTime = new ReactiveProperty<int>();
        }

        public void ReadyGame() {
            State = GameState.ReadyGame;
        }

        public void StartGame(StartGame.SendRoom data) {
            State = GameState.PlayGame;
            
            remainTime.Value = data.RemainTime;
            timer = Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Subscribe(elapsedTime => remainTime.Value = data.RemainTime - (int) elapsedTime);
        }

        public void TimeOverGame(TimeOver.SendRoom data) {
            State = GameState.TimeOverGame;
            timer?.Dispose();
        }

        public void SetHost(bool isHost) {
            IsHost = isHost;
        }
    }
}