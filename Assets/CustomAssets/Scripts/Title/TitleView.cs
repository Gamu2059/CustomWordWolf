using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour {
    
    [Header("Name")]
    
    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Button editNameButton;

    [Header("Room")]

    [SerializeField]
    private Button createRoomButton;

    [SerializeField]
    private Button joinRoomButton;

    [Header("Edit Name Window")]

    [SerializeField]
    private GameObject editNameWindow;
    
    [SerializeField]
    private InputField editNameInputField;

    [SerializeField]
    private Button editNameOkButton;

    [Header("Create Room Window")]

    [SerializeField]
    private GameObject createRoomWindow;
    
    [SerializeField]
    private InputField createRoomNameInputField;

    [SerializeField]
    private Button createRoomOkButton;

    [Header("Join Room Window")]

    [SerializeField]
    private GameObject joinRoomWindow;
    
    [SerializeField]
    
    
    private void Start() {
    }
}
