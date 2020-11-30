using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Api;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Title.RoomList {
    public class JoinRoomListPresenter : MonoBehaviour {
        [SerializeField]
        private JoinRoomElementPresenter elementPrefab;

        [SerializeField]
        private VerticalLayoutGroup layoutGroup;

        public event Action<Guid> OnRoomDecided;

        private List<JoinRoomElementPresenter> elements;

        public void Initialize() {
            elements = new List<JoinRoomElementPresenter>();
        }

        public async UniTaskVoid UpdateList() {
            if (elements.Count > 0) {
                elements.ForEach(e => Destroy(e.gameObject));
                elements.Clear();
            }

            var roomListApi = new RoomListApi();
            var roomListResponse = await roomListApi.Request(new ConnectData.RoomList.Request());
            if (roomListResponse.Result != ConnectData.RoomList.Result.Succeed) {
                return;
            }

            foreach (var roomData in roomListResponse.RoomDataList) {
                var element = Instantiate(elementPrefab);
                element.transform.SetParent(transform);
                element.Initialize(roomData);
                element.OnRoomDecided += guid => OnRoomDecided?.Invoke(guid);
                elements.Add(element);
            }

            layoutGroup.CalculateLayoutInputVertical();
        }
    }
}