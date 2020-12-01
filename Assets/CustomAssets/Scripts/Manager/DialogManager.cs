using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Dialog;
using UnityEngine;

namespace Manager {
    public class DialogManager : SingletonMonoBehavior<DialogManager> {
        [SerializeField]
        private Transform dialogRoot;

        [SerializeField]
        private List<DialogBase> dialogPrefabList;

        private Dictionary<Type, LinkedList<DialogBase>> dialogPool;

        private DialogBase GetDialogPrefab<T>() where T : DialogBase {
            var dialogType = typeof(T);
            return dialogPrefabList.FirstOrDefault(d => d.GetType() == dialogType);
        }

        public DialogBase GetDialog<T>() where T : DialogBase {
            var dialogType = typeof(T);
            if (dialogPool.TryGetValue(dialogType, out var pool)) {
                if (pool == null) {
                    pool = new LinkedList<DialogBase>();
                    dialogPool[dialogType] = pool;
                }
            } else {
                pool = new LinkedList<DialogBase>();
                dialogPool.Add(dialogType, pool);
            }

            var dialog = pool.FirstOrDefault(d => !d.IsShowing);
            if (dialog == null) {
                // 一つも使えるものがないなら複製する
                var prefab = GetDialogPrefab<T>();
                if (prefab == null) {
                    return null;
                }

                dialog = Instantiate(prefab);
                dialog.transform.SetParent(dialogRoot);
                pool.AddLast(dialog);
            }

            return dialog;
        }
    }
}