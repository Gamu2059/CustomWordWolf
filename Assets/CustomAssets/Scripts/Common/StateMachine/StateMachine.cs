using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Common {
    /// <summary>
    /// ステートマシン。
    /// </summary>
    /// <typeparam name="T">ステートマシンで扱うデータ構造</typeparam>
    public class StateMachine<T>
        // 条件は、データ構造が列挙型であること
        where T : Enum {
        /// <summary>
        /// 現在のステート
        /// </summary>
        private T currentState;

        /// <summary>
        /// ステートのスタック
        /// </summary>
        private Stack<T> stateStack;

        /// <summary>
        /// ステート変更時のアクション。
        /// </summary>
        public event Action<T, T, IChangeStateArg, bool> OnChangeStateEvent;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public StateMachine() {
            stateStack = new Stack<T>();

            // 列挙値の一番最初に定義されている値をセットする
            currentState = default;
        }

        /// <summary>
        /// ステート変更をリクエストする。
        /// </summary>
        /// <param name="requestState">変更先のステート</param>
        /// <param name="arg">変更先に渡す引数</param>
        /// <returns>変更が成功したかどうか</returns>
        public bool RequestChangeState(T requestState, IChangeStateArg arg = null) {
            if (requestState.Equals(currentState)) {
                return false;
            }

            stateStack.Push(currentState);
            var tmpState = currentState;
            currentState = requestState;
            OnChangeStateEvent?.Invoke(currentState, tmpState, arg, false);
            return true;
        }

        /// <summary>
        /// ステートを一つ前に戻す。
        /// </summary>
        /// <param name="arg">変更先に渡す引数</param>
        /// <returns>変更が成功したかどうか</returns>
        public bool RequestBackState(IChangeStateArg arg = null) {
            if (stateStack.Count < 1) {
                return false;
            }

            var tmpState = currentState;
            currentState = stateStack.Pop();
            OnChangeStateEvent?.Invoke(currentState, tmpState, arg, true);
            return true;
        }
    }
}