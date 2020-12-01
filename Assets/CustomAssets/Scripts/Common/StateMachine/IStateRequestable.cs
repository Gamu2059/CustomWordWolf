using System;

namespace Common {
    public interface IStateRequestable<TState, TArg> where TState : Enum where TArg : IChangeStateArg {
        event Func<TState, TArg, bool> OnStateRequestEvent;
    }
}