using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollideDetector : MonoBehaviour {
    public Action onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("enter");
        onTriggerEnter?.Invoke();

    }
}
