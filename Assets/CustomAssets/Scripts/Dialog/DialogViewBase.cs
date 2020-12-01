using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialog {
    public class DialogViewBase : MonoBehaviour, Initializable {
        public void Initialize() {
        }

        public virtual async UniTask ShowAsync() {
        }

        public virtual async UniTask HideAsync() {
        }
    }
}