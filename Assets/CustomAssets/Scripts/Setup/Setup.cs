using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirror;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Setup : MonoBehaviour {

    [SerializeField]
    private bool isDebug;

    [SerializeField]
    private string address;
    
    [SerializeField]
    private Button serverButton;

    [SerializeField]
    private Button clientButton;

    private void Start() {
#if !UNITY_SERVER
        NetworkManager.singleton.networkAddress = isDebug ? "localhost" : address;
        serverButton.OnClickAsObservable().Subscribe(_ => NetworkManager.singleton.StartServer());
        clientButton.OnClickAsObservable().Subscribe(_ => NetworkManager.singleton.StartClient());
#endif
    }
}