using System;
using Common;
using Cysharp.Threading.Tasks;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Result {
    public class ResultView : MonoBehaviour, Initializable {
        [SerializeField]
        private CustomButton backButton;

        [SerializeField]
        private Text peopleThemeText;

        [SerializeField]
        private Text wolfThemeText;

        public IObservable<Unit> BackObservable => backButton.Button.OnClickAsObservable();

        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }

        public void SetPeopleTheme(string theme) {
            peopleThemeText.text = $"テーマ：{theme}";
        }

        public void SetWolfTheme(string theme) {
            wolfThemeText.text = $"テーマ：{theme}";
        }
    }
}