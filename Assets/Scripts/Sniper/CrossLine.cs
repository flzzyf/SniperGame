using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossLine : MonoBehaviour {
    //和中点距离
    public float distance;

    //抖动幅度
    public float magnitute = 1;

    public float speed = 2;
    bool moving;

    Camera cam;

    private void Awake() {
        cam = Camera.main;
    }

    void MoveTo(Vector2 pos) {
        StartCoroutine(MoveToCor(pos));
    }
    IEnumerator MoveToCor(Vector2 pos) {
        moving = true;

        while(Vector2.Distance(transform.position, pos) > speed * Time.fixedDeltaTime) {
            Vector2 dir = (pos - (Vector2)transform.position).normalized;
            transform.Translate(dir * speed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        moving = false;
    }

    private void Update() {
        if (!moving) {
            float offsetMagnitude = .1f;
            Vector2 randomOffset = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * offsetMagnitude;

            Vector2 dir = (transform.position - cam.transform.position).normalized;
            Vector2 pos = (Vector2)cam.transform.position + dir * distance;

            Debug.Log(dir.x + "," + dir.y);

            MoveTo(pos + randomOffset);
        }
    }
}
