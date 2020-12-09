using System;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    /// <summary>
    /// タイトルに属するUIを管理する概念のViewコンポーネント。
    /// </summary>
    public class TitleGroupView :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        [SerializeField]
        private CustomButton startButton;

        [SerializeField]
        private Image darkCutImage;

        [SerializeField]
        private float fadeInDuration = 1;

        /// <summary>
        /// スタートボタンのUniRxイベント。
        /// </summary>
        public IObservable<Unit> StartObservable => startButton.Button.OnClickAsObservable();

        /// <summary>
        /// タイトルグループのViewを初期化する。
        /// </summary>
        public void Initialize() {
        }

        /// <summary>
        /// タイトルグループ全体の表示処理。
        /// </summary>
        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            startButton.Show();

            // DOTweenでフェードインさせる
            darkCutImage.color = Color.black;
            darkCutImage.raycastTarget = true;
            var tween = darkCutImage.DOColor(Color.clear, fadeInDuration).SetEase(Ease.Linear);
            await UniTask.WaitWhile(() => tween.IsPlaying());
            darkCutImage.raycastTarget = false;
        }

        /// <summary>
        /// タイトルグループ全体の非表示処理。
        /// </summary>
        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }
    }
}