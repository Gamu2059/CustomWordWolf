using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CustomAssets.Scripts.Game {
    public class GameGroupView :
        MonoBehaviour,
        Initializable {
        public void Initialize() {
        }

        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }
    }
}