using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
    /// <summary>
    /// シングルトンパターンを実装したMonoBehaviorの基底クラス。
    /// </summary>
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour {
        [SerializeField]
        private bool isDontDestroy;

        public static T Instance { get; private set; }

        private void Awake() {
            if (CheckExistInstance()) {
                Destroy(gameObject);
            } else {
                Instance = GetComponent<T>();
                if (isDontDestroy) {
                    DontDestroyOnLoad(gameObject);
                }

                OnAwake();
            }
        }

        private void OnDestroy() {
            if (Instance == this) {
                OnDestroyed();
                Instance = null;
            }
        }

        public static bool CheckExistInstance() {
            return Instance;
        }

        protected virtual void OnAwake() {
        }

        protected virtual void OnDestroyed() {
        }
    }
}