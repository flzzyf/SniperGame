using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public float speed = 1;

    Vector2 dir = Vector2.right;
    Camera cam;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        //离屏幕太远自动清除
        if (Vector2.Distance(cam.transform.position, transform.position) > 3) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) {
            return;
        }

        collision.GetComponent<Cross>().Hit(transform);
    }

    private void FixedUpdate() {
        if (dir != default) {
            transform.Translate(dir * speed * Time.fixedDeltaTime);
        }
    }
}
