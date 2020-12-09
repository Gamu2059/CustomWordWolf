using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lobby {
    /// <summary>
    /// ロビーに属するUIを管理する概念のViewコンポーネント。
    /// </summary>
    public class LobbyGroupView :
        MonoBehaviour,
        Initializable {
        /// <summary>
        /// ロビーグループのViewを初期化する。
        /// </summary>
        public void Initialize() {
        }

        /// <summary>
        /// ロビーグループ全体の表示処理。
        /// </summary>
        public async UniTask ShowAsync() {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// ロビーグループ全体の非表示処理。
        /// </summary>
        public async UniTask HideAsync() {
            gameObject.SetActive(false);
        }
    }
}