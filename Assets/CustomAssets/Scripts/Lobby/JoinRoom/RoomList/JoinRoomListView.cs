using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom.RoomList {
    /// <summary>
    /// 部屋一覧UIのViewコンポーネント。
    /// </summary>
    public class JoinRoomListView :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        [SerializeField]
        private GameObject noRoomTextObj;

        /// <summary>
        /// Viewを初期化する。
        /// </summary>
        public void Initialize() {
            SetActiveNoRoom(true);
        }

        /// <summary>
        /// 部屋がないというテキストオブジェクトの表示/非表示を切り替える。
        /// </summary>
        public void SetActiveNoRoom(bool isActive) {
            noRoomTextObj.SetActive(isActive);
        }
    }
}