using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
    public class StateMachineMonoBehavior<TState> : MonoBehaviour, Initializable where TState : Enum {
        protected StateMachine<TState> StateMachine { get; private set; }
        protected Dictionary<TState, IStateChangeable> StateDictionary { get; private set; }

        public void Initialize() {
            StateMachine = new StateMachine<TState>();
            StateDictionary = new Dictionary<TState, IStateChangeable>();
            StateMachine.OnChangeStateEvent += OnChangeStateEvent;
            
            OnInitialize();
        }

        private async void OnChangeStateEvent(TState nextState, TState oldState, IChangeStateArg arg, bool isBack) {
            if (StateDictionary.ContainsKey(oldState)) {
                await StateDictionary[oldState].StateOutAsync();
            }

            if (StateDictionary.ContainsKey(nextState)) {
                await StateDictionary[nextState].StateInAsync(arg, isBack);
            }
        }

        protected virtual void OnInitialize() {
        }
    }
}