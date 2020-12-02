using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom.RoomList {
    public class JoinRoomListView : MonoBehaviour, Initializable {
        [SerializeField]
        private GameObject noRoomTextObj;

        public void Initialize() {
            SetActiveNoRoom(true);
        }

        public void SetActiveNoRoom(bool isActive) {
            noRoomTextObj.SetActive(isActive);
        }
    }
}