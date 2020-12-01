using System;
using UnityEngine;
using UniRx;

namespace Dialog {
    public class GeneralDialogArg : IDialogArg {
        public string Title;
        public string Message;
    }

    public class GeneralDialogPresenter : DialogPresenterBase<GeneralDialogView> {
        private IDisposable okDisposable;

        protected override void OnShow(IDialogArg arg = null) {
            base.OnShow(arg);

            var generalDialogArg = arg as GeneralDialogArg;
            if (generalDialogArg == null) {
                Debug.LogError("引数の型が不適切です " + generalDialogArg.GetType().Name);
                return;
            }

            View.SetTitle(generalDialogArg.Title);
            View.SetMessage(generalDialogArg.Message);

            SetEvents();
        }

        protected override void OnDispose() {
            DisposeEvents();
            base.OnDispose();
        }

        private void SetEvents() {
            okDisposable = View.OkObservable.Subscribe(_ => Click(0));
        }

        private void DisposeEvents() {
            okDisposable?.Dispose();
            okDisposable = null;
        }
    }
}