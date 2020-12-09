using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Setup {
    /// <summary>
    /// Setupシーンの管理コンポーネント。
    /// </summary>
    public class Setup : MonoBehaviour {
        [SerializeField]
        private bool isDebug;

        [SerializeField]
        private string address;

        [Scene]
        [SerializeField]
        private string gameScene;

        private void Start() {
            // UNITY_SERVERが定義されていない(ServerBuildがOff)時に実行される
#if !UNITY_SERVER
            NetworkManager.singleton.networkAddress = isDebug ? "localhost" : address;
            SceneManager.LoadScene(gameScene);
#endif
        }
    }
}