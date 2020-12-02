using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using ConnectData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayerList {
    public class PlayerListPresenter : MonoBehaviour, Initializable {
        [SerializeField]
        private PlayerElementPresenter elementPrefab;

        [SerializeField]
        private GridLayoutGroup layoutGroup;

        private Dictionary<int, PlayerElementPresenter> elementDictionary;

        public void Initialize() {
            elementDictionary = new Dictionary<int, PlayerElementPresenter>();
        }

        /// <summary>
        /// 初回のデータ注入はこちらを呼び出す
        /// </summary>
        public void SetMember(RoomDetailData data) {
            // 残っているキャッシュを全削除する
            foreach (var element in elementDictionary) {
                Destroy(element.Value.gameObject);
            }

            elementDictionary.Clear();

            foreach (var playerData in data.PlayerDataList) {
                var element = Instantiate(elementPrefab);
                element.transform.SetParent(layoutGroup.transform);
                element.Initialize(playerData);
                elementDictionary.Add(playerData.PlayerConnectionId, element);
            }

            // 新規作成する

            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.CalculateLayoutInputHorizontal();
        }

        /// <summary>
        /// 2回目以降のデータ注入はこちらを呼び出す
        /// </summary>
        public void UpdateMember(RoomDetailData data) {
            // キャッシュはあるが更新データがない時は削除する
            var removeDictionary = new Dictionary<int, PlayerElementPresenter>(elementDictionary);
            foreach (var element in elementDictionary) {
                var existData = data.PlayerDataList.Any(d => d.PlayerConnectionId == element.Key);
                if (!existData) {
                    Destroy(element.Value.gameObject);
                    removeDictionary.Remove(element.Key);
                }
            }

            elementDictionary = removeDictionary;

            // キャッシュはないが更新データがある時は新規作成する
            foreach (var playerData in data.PlayerDataList) {
                var existData = elementDictionary.ContainsKey(playerData.PlayerConnectionId);
                if (!existData) {
                    var element = Instantiate(elementPrefab);
                    element.transform.SetParent(layoutGroup.transform);
                    element.Initialize(playerData);
                    elementDictionary.Add(playerData.PlayerConnectionId, element);
                }
            }

            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.CalculateLayoutInputHorizontal();
        }
    }
}