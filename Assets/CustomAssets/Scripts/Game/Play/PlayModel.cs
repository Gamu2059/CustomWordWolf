using System;
using ConnectData;
using UniRx;

namespace Game {
    public class PlayModel {
        
        public RoomDetailData RoomData { get; private set; }
        
        private ReactiveProperty<int> gameTime;
        public IObservable<int> GameTimeObservable => gameTime;

        private IDisposable timer;

        public PlayModel() {
            gameTime = new ReactiveProperty<int>();
        }

        public void StartGame(PlayArg data) {
            RoomData = data.RoomData;
            gameTime.Value = data.GameTime;
            timer = Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Subscribe(elapsedTime => gameTime.Value = data.GameTime - (int) elapsedTime);
        }

        public void TimeOverGame() {
            timer?.Dispose();
            timer = null;
        }
    }
}