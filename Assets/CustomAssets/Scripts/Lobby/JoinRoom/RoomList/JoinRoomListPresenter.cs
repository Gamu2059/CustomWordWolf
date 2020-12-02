using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Api;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.JoinRoom.RoomList {
    public class JoinRoomListPresenter : MonoBehaviour, Initializable {
        [SerializeField]
        private JoinRoomListView view;

        [SerializeField]
        private JoinRoomElementPresenter elementPrefab;

        [SerializeField]
        private VerticalLayoutGroup layoutGroup;

        public event Action<Guid> OnJoinRoomDecidedEvent;

        private List<JoinRoomElementPresenter> elements;

        public void Initialize() {
            view.Initialize();
            elements = new List<JoinRoomElementPresenter>();
        }

        public async UniTaskVoid UpdateRoomList() {
            if (elements.Count > 0) {
                elements.ForEach(e => Destroy(e.gameObject));
                elements.Clear();
            }

            view.SetActiveNoRoom(true);

            var roomListApi = new RoomListApi();
            var roomListResponse = await roomListApi.Request(new ConnectData.GetRoomList.Request());
            if (roomListResponse.Result != ConnectData.GetRoomList.Result.Succeed) {
                return;
            }

            if (roomListResponse.RoomDataList.Count < 1) {
                return;
            }

            view.SetActiveNoRoom(false);
            foreach (var roomData in roomListResponse.RoomDataList) {
                var element = Instantiate(elementPrefab);
                element.transform.SetParent(layoutGroup.transform);
                element.Initialize(roomData);
                element.OnJoinRoomDecidedEvent += guid => OnJoinRoomDecidedEvent?.Invoke(guid);
                elements.Add(element);
            }

            layoutGroup.CalculateLayoutInputVertical();
        }
    }
}