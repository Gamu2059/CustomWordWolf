using Api;
using Common;
using ConnectData;
using CustomAssets.Scripts.UI.Option;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Game.Ready.Option {
    public class OptionPresenter : MonoBehaviour, Initializable {
        [SerializeField]
        private GameTimeOption gameTimeOption;

        [SerializeField]
        private WolfNumOption wolfNumOption;

        private CompositeDisposable viewDisposable;
        private CompositeDisposable apiDisposable;

        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            SetActiveOperatorButton(false);
            SetViewEvents();
            SetApiEvents();
        }

        public async UniTask HideAsync() {
            DisposeApiEvents();
            DisposeViewEvents();
        }

        private void SetViewEvents() {
            viewDisposable = new CompositeDisposable(
                gameTimeOption.IncrementObservable
                    .Subscribe(_ => ChangeGameTime(1).Forget())
                    .AddTo(gameObject),
                gameTimeOption.DecrementObservable
                    .Subscribe(_ => ChangeGameTime(-1).Forget())
                    .AddTo(gameObject),
                wolfNumOption.IncrementObservable
                    .Subscribe(_ => ChangeWolfNum(1).Forget())
                    .AddTo(gameObject),
                wolfNumOption.DecrementObservable
                    .Subscribe(_ => ChangeWolfNum(-1).Forget())
                    .AddTo(gameObject)
            );
        }

        private void SetApiEvents() {
            apiDisposable = new CompositeDisposable(
                new ChangeGameTimeReceiver(OnChangeGameTime),
                new ChangeWolfNumReceiver(OnChangeWolfNum)
            );
        }

        private void DisposeViewEvents() {
            viewDisposable?.Dispose();
            viewDisposable = null;
        }

        private void DisposeApiEvents() {
            apiDisposable?.Dispose();
            apiDisposable = null;
        }

        private async UniTask ChangeGameTime(int changeForward) {
            var changeGameTimeApi = new ChangeGameTimeApi();
            await changeGameTimeApi.Request(new ChangeGameTime.Request {ChangeForward = changeForward});
        }

        private async UniTask ChangeWolfNum(int changeForward) {
            var changeWolfNumApi = new ChangeWolfNumApi();
            changeWolfNumApi.Request(new ChangeWolfNum.Request {ChangeForward = changeForward});
        }

        public void OnChangeGameTime(ChangeGameTime.SendRoom data) {
            gameTimeOption.UpdateOption(data);
        }

        public void OnChangeWolfNum(ChangeWolfNum.SendRoom data) {
            wolfNumOption.UpdateOption(data);
        }

        public void SetActiveOperatorButton(bool isActive) {
            gameTimeOption.SetActiveOperatorButton(isActive);
            wolfNumOption.SetActiveOperatorButton(isActive);
        }
    }
}