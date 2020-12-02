using System;
using Common;
using UI.Button;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Option {
    public class OptionBase : MonoBehaviour {
        [SerializeField]
        protected Text optionValueText;

        [SerializeField]
        protected CustomButton decrementButton;

        [SerializeField]
        protected CustomButton incrementButton;

        public IObservable<Unit> DecrementObservable => decrementButton.Button.OnClickAsObservable();
        public IObservable<Unit> IncrementObservable => incrementButton.Button.OnClickAsObservable();

        public void SetActiveOperatorButton(bool isActive) {
            decrementButton.gameObject.SetActive(isActive);
            incrementButton.gameObject.SetActive(isActive);
        }

        public virtual void UpdateOption(IOptionMessage option) {
            decrementButton.Button.interactable = !option.IsLowerLimit;
            incrementButton.Button.interactable = !option.IsUpperLimit;
        }
    }
}