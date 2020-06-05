using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossLine : MonoBehaviour {
    //和中点距离
    float distance;

    public float maxDistance = .8f;

    //抖动幅度
    public float magnitute = 1;

    public float speed = 2;
    bool moving;

    public float offsetMagnitude = .03f;

    Camera cam;

    private void Awake() {
        cam = Camera.main;

        //移动到最大距离
        Vector2 dir = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;

        transform.position = (Vector2)cam.transform.position + dir * maxDistance;

        distance = maxDistance;
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
            Vector2 randomOffset = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)) * offsetMagnitude;

            Vector2 dir = ((Vector2)transform.position - (Vector2)cam.transform.position).normalized;
            Vector2 pos = (Vector2)cam.transform.position + dir * distance;

            MoveTo(pos + randomOffset);
        }
    }

    //设置和中心的距离百分比
    public void SetDistance(float percent) {
        distance = maxDistance * percent;
    }
}
