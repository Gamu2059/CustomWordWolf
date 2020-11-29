using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager {
    #region Inspector

    [Scene]
    [SerializeField]
    private string titleScene;

    [Scene]
    [SerializeField]
    private string gameScene;

    #endregion

    #region Server System Callbacks

    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");
    }

    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);
        Debug.Log("OnServerReady");
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
        Debug.Log("OnServerAddPlayer");
    }

    public override void OnServerError(NetworkConnection conn, int errorCode) {
        base.OnServerError(conn, errorCode);
        Debug.Log("OnServerError");
    }

    public override void OnServerChangeScene(string newSceneName) {
        base.OnServerChangeScene(newSceneName);
        Debug.Log("OnServerChangeScene");
    }

    public override void OnServerSceneChanged(string sceneName) {
        base.OnServerSceneChanged(sceneName);
        Debug.Log("OnServerSceneChanged");
    }

    #endregion

    #region Client System Callback

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        Debug.Log("OnClientDisconnect");
    }

    public override void OnClientError(NetworkConnection conn, int errorCode) {
        base.OnClientError(conn, errorCode);
        Debug.Log("OnClientError");
    }

    public override void OnClientNotReady(NetworkConnection conn) {
        base.OnClientNotReady(conn);
        Debug.Log("OnClientNotReady");
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation,
        bool customHandling) {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        Debug.Log("OnClientChangeScene");
    }

    public override void OnClientSceneChanged(NetworkConnection conn) {
        base.OnClientSceneChanged(conn);
        Debug.Log("OnClientSceneChanged");
    }

    #endregion

    #region Start & Stop callbacks

    public override void OnStartHost() {
        Debug.Log("OnStartHost");
    }

    public override void OnStartServer() {
        Debug.Log("OnStartServer");
    }

    public override void OnStartClient() {
        Debug.Log("OnStartClient");
        if (!clientLoadedScene) {
            SceneManager.LoadScene(titleScene);
        }
    }

    public override void OnStopServer() {
        Debug.Log("OnStopServer");
    }

    public override void OnStopClient() {
        Debug.Log("OnStopClient");
    }

    public override void OnStopHost() {
        Debug.Log("OnStopHost");
    }

    internal void Hoge() {
    }

    #endregion
}