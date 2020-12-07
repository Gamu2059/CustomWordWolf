using UnityEngine;
using UnityEngine.UI;

namespace Game.Result {
    public class ResultElementView : MonoBehaviour {
        [SerializeField]
        private Text playerNameText;

        [SerializeField]
        private Color peopleColor;

        [SerializeField]
        private Color wolfColor;

        public void Initialize(string playerName, bool isWolf) {
            playerNameText.text = playerName;
            playerNameText.color = isWolf ? wolfColor : peopleColor;

            // なぜかスケールが変わるので1にする
            transform.localScale = Vector3.one;
        }
    }
}