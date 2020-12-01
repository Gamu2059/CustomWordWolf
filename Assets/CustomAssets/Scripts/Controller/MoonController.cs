using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonController : MonoBehaviour {
    [SerializeField]
    private Transform target;

    private void Update() {
        var angle = transform.localEulerAngles;
        angle.x = target.localEulerAngles.x;
        transform.localEulerAngles = angle;
    }
}