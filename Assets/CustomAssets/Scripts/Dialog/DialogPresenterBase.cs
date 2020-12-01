using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialog {
    public class DialogPresenterBase<TViewBase> : DialogBase, Initializable where TViewBase : DialogViewBase {
        protected TViewBase View { get; private set; }

        public override void Initialize() {
            View = GetComponent<TViewBase>();
            if (View == null) {
                Debug.LogWarning("適するViewコンポーネントがアタッチされていません " + typeof(TViewBase).Name);
            } else {
                View.Initialize();
            }
        }
    }
}