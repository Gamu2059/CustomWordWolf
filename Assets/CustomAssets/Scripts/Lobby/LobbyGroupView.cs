using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lobby {
    public class LobbyGroupView :
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