using System;

namespace Common {
    public interface IStateMachineInjectable<TState> where TState : Enum {
        void InjectStateMachine(StateMachine<TState> stateMachine);
    }
}