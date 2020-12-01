using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

namespace UI.Button {
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

        [SerializeField]
        private RectTransform frontFrame;

        [SerializeField]
        private Image frontImage;
        
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

        private void Start() {
            TweenFrontFrame(normalFrontOffset);
            TweenFrontColor(normalFrontColor, tweenDuration);
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
            frontFrame.DOAnchorPosY(targetY, duration).SetEase(easeType).SetLink(gameObject);
        }

        private void TweenFrontColor(Color targetColor, float duration = 0) {
            frontImage.DOColor(targetColor, duration).SetEase(easeType).SetLink(gameObject);
        }
    }
}