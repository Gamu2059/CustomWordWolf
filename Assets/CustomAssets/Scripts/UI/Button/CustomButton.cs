using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

namespace UI.Button {
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler {
        [Serializable]
        private class ColorSet {
            public Color BaseColor;
            public Color BackColor;
            public Color FrontColor;
            public Color ContentColor;
        }

        [Header("Refer")]
        [SerializeField]
        private Graphic baseGraphic;

        [SerializeField]
        private Graphic backGraphic;

        [SerializeField]
        private Graphic frontGraphic;

        [SerializeField]
        private Graphic contentGraphic;

        [Header("User Events")]
        [SerializeField]
        private UnityEngine.UI.Button selfButton;

        [SerializeField]
        private RectTransform frontFrame;

        [SerializeField]
        private float normalFrontOffset = 20;

        [SerializeField]
        private float highlightFrontOffset = 15;

        [SerializeField]
        private float clickFrontOffset = 0;

        [SerializeField]
        private Color normalFrontColor;

        [SerializeField]
        private Color highlightFrontColor;

        [SerializeField]
        private float tweenDuration = 0.1f;

        [SerializeField]
        private Ease easeType;

        [Header("Interactable")]
        [SerializeField]
        private ColorSet validColor;

        [SerializeField]
        private ColorSet invalidColor;

        public UnityEngine.UI.Button Button => selfButton;

        private ReactiveProperty<bool> onInteractable;

        private void Start() {
            Show();
            onInteractable = new ReactiveProperty<bool>(Button.IsInteractable());
            Button
                .ObserveEveryValueChanged(b => b.IsInteractable())
                .Subscribe(x => onInteractable.Value = x)
                .AddTo(gameObject);
            onInteractable
                .Subscribe(x => TweenColor(x ? validColor : invalidColor, tweenDuration))
                .AddTo(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            TweenFrontFrame(highlightFrontOffset, tweenDuration);
            TweenFrontColor(highlightFrontColor, tweenDuration);
        }

        public void OnPointerExit(PointerEventData eventData) {
            TweenFrontFrame(normalFrontOffset, tweenDuration);
            TweenFrontColor(normalFrontColor, tweenDuration);
        }

        public void OnPointerDown(PointerEventData eventData) {
            TweenFrontFrame(clickFrontOffset, tweenDuration);
        }

        public void OnPointerUp(PointerEventData eventData) {
            TweenFrontFrame(highlightFrontOffset, tweenDuration);
        }

        private void TweenFrontFrame(float targetY, float duration = 0) {
            if (!Button.IsInteractable()) {
                return;
            }

            frontFrame.DOAnchorPosY(targetY, duration).SetEase(easeType).SetLink(gameObject);
        }

        private void TweenFrontColor(Color targetColor, float duration = 0) {
            if (!Button.IsInteractable()) {
                return;
            }

            frontGraphic.DOColor(targetColor, duration).SetEase(easeType).SetLink(gameObject);
        }

        private void TweenColor(ColorSet colorSet, float duration = 0) {
            var baseC = colorSet.BaseColor;
            var backC = colorSet.BackColor;
            var frontC = colorSet.FrontColor;
            var contentC = colorSet.ContentColor;
            baseC.a = baseGraphic.color.a;
            backC.a = backGraphic.color.a;
            frontC.a = frontGraphic.color.a;
            contentC.a = contentGraphic.color.a;

            DOTween.Sequence()
                .Append(baseGraphic.DOColor(baseC, duration).SetEase(easeType))
                .Join(backGraphic.DOColor(backC, duration).SetEase(easeType))
                .Join(frontGraphic.DOColor(frontC, duration).SetEase(easeType))
                .Join(contentGraphic.DOColor(contentC, duration).SetEase(easeType))
                .SetLink(gameObject);
        }

        public void Show() {
            TweenFrontFrame(normalFrontOffset);
            TweenFrontColor(normalFrontColor, tweenDuration);
        }
    }
}