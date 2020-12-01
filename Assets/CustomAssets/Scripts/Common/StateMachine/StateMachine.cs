using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Common {
    public class StateMachine<T> where T : Enum {
        private T currentState;
        private Stack<T> stateStack;
        public event Action<T, T, IChangeStateArg> OnChangeStateEvent;

        public StateMachine() {
            stateStack = new Stack<T>();
            currentState = default;
        }

        public bool StateRequestChange(T requestState, IChangeStateArg arg = null) {
            if (requestState.Equals(currentState)) {
                return false;
            }

            stateStack.Push(currentState);
            var tmpState = currentState;
            currentState = requestState;
            OnChangeStateEvent?.Invoke(currentState, tmpState, arg);
            return true;
        }
    }
}