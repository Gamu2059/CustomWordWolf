using System;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class PlayView :
        MonoBehaviour,
        Initializable {
        [SerializeField]
        private Text gameTimeText;

        [SerializeField]
        private Text themeText;

        [SerializeField]
        private CustomButton backReadyButton;

        public IObservable<Unit> BackReadyObservable => backReadyButton.Button.OnClickAsObservable();

        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            backReadyButton.Show();
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        public void SetGameTime(int remainTime) {
            gameTimeText.text = $"残り時間：{remainTime}秒";
        }

        public void SetTheme(string theme) {
            themeText.text = $"テーマ：{theme}";
        }
    }
}