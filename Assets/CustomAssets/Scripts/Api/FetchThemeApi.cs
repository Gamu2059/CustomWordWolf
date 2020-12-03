using System;
using ConnectData;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;
using Unity.RemoteConfig;
using UnityEngine;

namespace Api {
    public class FetchThemeApi {
        public struct UserAttributes {
        }

        public struct AppAttributes {
        }

        private bool isReserveResponse;
        private FetchTheme.Response response;

        public async UniTask<FetchTheme.Response> Request() {
            isReserveResponse = false;
            ConfigManager.FetchCompleted += ApplyRemoteSettings;
            ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
            Debug.Log(111);
            await UniTask.WaitUntil(() => isReserveResponse);
            return response;
        }
        private void ApplyRemoteSettings(ConfigResponse configResponse) {
            response = new FetchTheme.Response();
            switch (configResponse.requestOrigin) {
                case ConfigOrigin.Default:
                    response.Result = FetchTheme.Result.Failure;
                    break;
                case ConfigOrigin.Cached:
                case ConfigOrigin.Remote:
                    response.Result = FetchTheme.Result.Succeed;
                    response.ThemeData = GetThemeData();
                    break;
            }

            isReserveResponse = true;
        }

        private ThemeData GetThemeData() {
            var theme = ConfigManager.appConfig.GetString("Theme", null);
            return JsonUtility.FromJson<ThemeData>(theme);
        }
    }
}