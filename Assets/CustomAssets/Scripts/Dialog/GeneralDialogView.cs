using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog {
    public class GeneralDialogView : DialogViewBase {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Button okButton;

        public IObservable<Unit> OkObservable => okButton.OnClickAsObservable();

        public void SetTitle(string title) {
            titleText.text = title;
        }

        public void SetMessage(string message) {
            messageText.text = message;
        }

        public override async UniTask ShowAsync() {
            await base.ShowAsync();
            gameObject.SetActive(true);
        }

        public override async UniTask HideAsync() {
            gameObject.SetActive(false);
            await base.HideAsync();
        }
    }
}