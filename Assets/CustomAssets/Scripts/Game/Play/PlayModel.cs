using System;
using ConnectData;
using UniRx;

namespace Game {
    public class PlayModel {
        public PlayArg PlayArg { get; private set; }

        private ReactiveProperty<int> gameTime;
        private Subject<Unit> timerOver;

        public IObservable<int> GameTimeObservable => gameTime;

        public IObservable<Unit> TimeOverObservable => timerOver;

        private IDisposable gameTimeDisposable;
        private IDisposable timeOverDisposable;

        public PlayModel() {
            gameTime = new ReactiveProperty<int>();
            timerOver = new Subject<Unit>();
        }

        public void StartGame(PlayArg data) {
            PlayArg = data;
            gameTime.Value = data.StartGameData.GameTime;
            gameTimeDisposable = Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Subscribe(elapsedTime => gameTime.Value = data.StartGameData.GameTime - (int) elapsedTime);
            timeOverDisposable = GameTimeObservable
                .Where(t => t <= 0)
                .Subscribe(_ => {
                    timerOver.OnNext(Unit.Default);
                });
        }

        public void Dispose() {
            timeOverDisposable?.Dispose();
            timeOverDisposable = null;
            gameTimeDisposable?.Dispose();
            gameTimeDisposable = null;
        }
    }
}