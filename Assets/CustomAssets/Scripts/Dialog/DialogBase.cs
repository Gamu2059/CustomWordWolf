using System;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialog {
    public class DialogBase : MonoBehaviour, Initializable, IDisposable {
        private bool isClicked;
        private int clickId;
        public bool IsShowing { get; private set; }

        public virtual void Initialize() {
        }

        public async void Dispose() {
            OnDispose();
            await HideAsync();
        }

        public async UniTask<int> ShowAsync(IDialogArg arg = null) {
            IsShowing = true;
            isClicked = false;
            clickId = -1;

            OnShow(arg);

            await UniTask.WaitUntil(() => isClicked);
            return clickId;
        }

        public async UniTask HideAsync() {
            OnHide();
            IsShowing = false;
        }

        protected void Click(int id) {
            isClicked = true;
            clickId = id;
        }

        protected virtual void OnShow(IDialogArg arg = null) {
        }

        protected virtual void OnHide() {
        }

        protected virtual void OnDispose() {
        }
    }
}