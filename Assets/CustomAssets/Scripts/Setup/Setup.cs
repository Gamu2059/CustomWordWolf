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
    private Button serverButton;

    [SerializeField]
    private Button clientButton;

    private void Start() {
        serverButton.OnClickAsObservable().Subscribe(_ => NetworkManager.singleton.StartServer());
        clientButton.OnClickAsObservable().Subscribe( _ => NetworkManager.singleton.StartClient());
    }
}