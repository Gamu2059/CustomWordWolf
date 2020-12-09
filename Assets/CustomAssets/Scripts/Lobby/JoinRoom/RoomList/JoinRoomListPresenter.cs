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
    /// <summary>
    /// 部屋一覧UIのPresenterコンポーネント。
    /// </summary>
    public class JoinRoomListPresenter :
        // MonoBehaviorを継承する
        MonoBehaviour,
        // Initializeメソッドを持っている
        Initializable {
        /// <summary>
        /// 部屋一覧UIのView
        /// </summary>
        [SerializeField]
        private JoinRoomListView view;

        /// <summary>
        /// リスト用のプレハブ
        /// </summary>
        [SerializeField]
        private JoinRoomElementPresenter elementPrefab;

        /// <summary>
        /// リストのレイアウトグループ
        /// </summary>
        [SerializeField]
        private VerticalLayoutGroup layoutGroup;

        /// <summary>
        /// 部屋に参加するボタンを押した時のアクション。
        /// </summary>
        public event Action<Guid> OnClickJoinRoomEvent;

        /// <summary>
        /// リスト要素の一覧。
        /// </summary>
        private List<JoinRoomElementPresenter> elements;

        /// <summary>
        /// 部屋一覧UIを初期化する。
        /// </summary>
        public void Initialize() {
            // Viewも初期化する
            view.Initialize();
            elements = new List<JoinRoomElementPresenter>();
        }

        /// <summary>
        /// 部屋一覧を更新する。
        /// </summary>
        /// <returns></returns>
        public async UniTask UpdateRoomListAsync() {
            // キャッシュがある場合は、最初に削除しておく
            if (elements.Count > 0) {
                elements.ForEach(e => Destroy(e.gameObject));
                elements.Clear();
            }

            view.SetActiveNoRoom(true);

            // サーバと通信して参加可能な部屋一覧を取得する
            var response = await new RoomListApi().Request(new GetRoomList.Request());
            if (response.Result != GetRoomList.Result.Succeed) {
                return;
            }

            if (response.RoomDataList.Count < 1) {
                return;
            }

            view.SetActiveNoRoom(false);

            // 部屋一覧データを用いてリスト要素を生成する
            foreach (var roomData in response.RoomDataList) {
                var element = Instantiate(elementPrefab);
                element.transform.SetParent(layoutGroup.transform);
                element.Initialize(roomData);
                element.OnClickJoinRoomEvent += guid => OnClickJoinRoomEvent?.Invoke(guid);
                elements.Add(element);
            }

            layoutGroup.CalculateLayoutInputVertical();
        }
    }
}