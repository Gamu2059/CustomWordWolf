using System;
using System.Collections;
using System.Collections.Generic;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Lobby.JoinRoom.RoomList {
    /// <summary>
    /// 部屋一覧要素UIのPresenterコンポーネント。
    /// </summary>
    public class JoinRoomElementPresenter : MonoBehaviour {
        /// <summary>
        /// 部屋一覧要素UIのView
        /// </summary>
        [SerializeField]
        private JoinRoomElementView view;

        /// <summary>
        /// 部屋に参加するボタンを押した時のアクション。
        /// </summary>
        public event Action<Guid> OnClickJoinRoomEvent;

        /// <summary>
        /// 部屋一覧要素を初期化する。
        /// </summary>
        /// <param name="roomData">部屋データ</param>
        public void Initialize(RoomSimpleData roomData) {
            // Viewも初期化する
            view.Initialize(roomData);

            // ViewのUniRxイベントを紐づける
            // AddToしているので削除した時点でDisposeされる
            view.JoinRoomObservable.Subscribe(_ => OnClickJoinRoomEvent?.Invoke(roomData.RoomGuid)).AddTo(gameObject);
        }
    }
}