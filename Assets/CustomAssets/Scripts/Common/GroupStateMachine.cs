using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Common {
    public enum GroupState {
        None,
        Title,
        Lobby,
        Game,
        Guide,
    }

    public class GroupStateMachine {
        private GroupState currentState;
        private Stack<GroupState> stateStack;
        public event Action<GroupState, GroupState, IChangeGroupArg> OnChangeState;

        public GroupStateMachine() {
            stateStack = new Stack<GroupState>();
            currentState = GroupState.None;
        }

        public bool RequestChangeState(GroupState requestState, IChangeGroupArg arg = null) {
            if (requestState == currentState) {
                return false;
            }

            stateStack.Push(currentState);
            var tmpState = currentState;
            currentState = requestState;
            OnChangeState?.Invoke(currentState, tmpState, arg);
            return true;
        }
    }
}