using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Setup : MonoBehaviour {

    [SerializeField]
    private Button serverButton;

    [SerializeField]
    private Button clientButton;

    private void Start() {
        serverButton.onClick.AddListener(NetworkManager.singleton.StartServer);
        clientButton.onClick.AddListener(NetworkManager.singleton.StartClient);
    }
}