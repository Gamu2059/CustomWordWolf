using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class TitleGroupView :
        MonoBehaviour,
        Initializable {
        #region Inspector

        [Header("Refer")]
        [SerializeField]
        private CustomButton startButton;

        [SerializeField]
        private Image titleImage;

        [SerializeField]
        private Image darkCutImage;

        [Header("Param")]
        [SerializeField]
        private float fadeInDuration = 1;

        #endregion

        #region Field & Property

        public IObservable<Unit> StartObservable => startButton.Button.OnClickAsObservable();

        private RectTransform titleRectT;

        #endregion

        public void Initialize() {
            titleRectT = titleImage.transform as RectTransform;
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
            startButton.Show();

            darkCutImage.color = Color.black;
            darkCutImage.raycastTarget = true;
            var tween = darkCutImage.DOColor(Color.clear, fadeInDuration).SetEase(Ease.Linear);
            await UniTask.WaitWhile(() => tween.IsPlaying());
            darkCutImage.raycastTarget = false;
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }
    }
}