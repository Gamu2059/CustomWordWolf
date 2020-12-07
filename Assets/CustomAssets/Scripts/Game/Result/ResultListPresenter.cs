using System.Collections.Generic;
using Common;
using ConnectData;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Game.Result {
    public class ResultListPresenter : MonoBehaviour, Initializable {
        [SerializeField]
        private ResultElementPresenter elementPrefab;

        [SerializeField]
        private LayoutGroup layoutGroup;

        private List<ResultElementPresenter> elements;

        public void Initialize() {
            elements = new List<ResultElementPresenter>();
        }

        public void Refresh() {
            // 残っているキャッシュを全削除する
            if (elements.Count > 0) {
                elements.ForEach(e => Destroy(e.gameObject));
                elements.Clear();
            }
        }

        public void ShowMember(RoomDetailData data, List<int> wolfMemberList, bool isShowWolfMember) {
            Refresh();

            foreach (var playerData in data.PlayerDataList) {
                var isWolf = wolfMemberList.Contains(playerData.PlayerConnectionId);
                if (isShowWolfMember && !isWolf || !isShowWolfMember && isWolf) {
                    continue;
                }

                var element = Instantiate(elementPrefab);
                element.transform.SetParent(layoutGroup.transform);
                element.Initialize(playerData.PlayerName, isWolf);
                elements.Add(element);
            }
        }
    }
}