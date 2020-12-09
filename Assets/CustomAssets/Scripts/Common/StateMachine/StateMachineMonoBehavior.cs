using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common {
    /// <summary>
    /// ステートマシンを持つMonoBehavior。
    /// </summary>
    /// <typeparam name="TState">ステートマシンで扱うデータ構造</typeparam>
    public class StateMachineMonoBehavior<TState> :
            // MonoBehaviorを継承する
            MonoBehaviour,
            // Initializeメソッドを持っている
            Initializable
        // 条件は、ステートマシンで扱うデータ構造が列挙型であること
        where TState : Enum {
        /// <summary>
        /// ステートマシン。
        /// </summary>
        protected StateMachine<TState> StateMachine { get; private set; }

        /// <summary>
        /// ステートと関連付けられたインスタンスの一覧。
        /// </summary>
        protected Dictionary<TState, IStateChangeable> StateDictionary { get; private set; }

        /// <summary>
        /// 初期化する。
        /// </summary>
        public void Initialize() {
            StateMachine = new StateMachine<TState>();
            StateDictionary = new Dictionary<TState, IStateChangeable>();
            StateMachine.OnChangeStateEvent += OnChangeState;

            OnInitialize();
        }

        /// <summary>
        /// ステート戦時の処理。
        /// </summary>
        /// <param name="nextState">変更先ステート</param>
        /// <param name="oldState">変更前ステート</param>
        /// <param name="arg">変更先に渡す引数</param>
        /// <param name="isBack">戻る場合かどうか</param>
        private void OnChangeState(TState nextState, TState oldState, IChangeStateArg arg, bool isBack) {
            // 内部でUniTaskの処理を呼び出す
            ChangeStateAsync().Forget();

            async UniTask ChangeStateAsync() {
                // 一覧の中に変更前ステートに対応するインスタンスがあれば呼び出す
                if (StateDictionary.ContainsKey(oldState)) {
                    await StateDictionary[oldState].StateOutAsync();
                }

                // 一覧の中に変更先ステートに対応するいんすたんすがあれば呼び出す
                if (StateDictionary.ContainsKey(nextState)) {
                    await StateDictionary[nextState].StateInAsync(arg, isBack);
                }
            }
        }

        protected virtual void OnInitialize() {
        }
    }
}