using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollideDetector : MonoBehaviour {
    public Action<Transform> onTriggerEnter;

    public Missile missile;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("enter:" + collision.name);

        onTriggerEnter?.Invoke(collision.transform);
    }

    public void AddOnTriggerEnterCallback(Action<Transform> callback) {
        onTriggerEnter = callback;
    }

    public void RemoveOnTriggerEnterCallback() {
        onTriggerEnter = null;
    }
}
