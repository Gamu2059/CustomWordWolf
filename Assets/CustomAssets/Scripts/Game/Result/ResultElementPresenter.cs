using UnityEngine;

namespace Game.Result {
    public class ResultElementPresenter : MonoBehaviour {
        [SerializeField]
        private ResultElementView view;

        public void Initialize(string playerName, bool isWolf) {
            view.Initialize(playerName, isWolf);
        }
    }
}