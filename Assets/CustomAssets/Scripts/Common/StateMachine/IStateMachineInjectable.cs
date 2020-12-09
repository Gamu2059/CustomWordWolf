using System;

namespace Common {
    /// <summary>
    /// ステートマシンを受け取るメソッドを定義するインタフェース。
    /// </summary>
    /// <typeparam name="TState">受け取るステートマシンで扱うデータ構造</typeparam>
    public interface IStateMachineInjectable<TState>
        // 条件は、ステートマシンで扱うデータ構造が列挙型であること
        where TState : Enum {
        void InjectStateMachine(StateMachine<TState> stateMachine);
    }
}