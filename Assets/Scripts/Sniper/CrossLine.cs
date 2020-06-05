using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossLine : MonoBehaviour {
    //和中点距离
    float distance;

    public float maxDistance = .8f;

    public float speed = 2;
    bool moving;

    //抖动幅度
    public float offsetMagnitude = .03f;

    Camera cam;

    [Header("十字线区域")]
    public Transform crossLineCircle;

    public float crossLineCircleRadius = .2f;

    private void OnValidate() {
        crossLineCircle.localScale = Vector3.one * crossLineCircleRadius * 2;
    }

    private void Awake() {
        cam = Camera.main;

        //移动到最大距离
        Vector2 dir = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;

        transform.position = (Vector2)cam.transform.position + dir * maxDistance;

        distance = maxDistance;

        crossLineCircleRadius = GameManager.sniperData.crossLineRadius;
        crossLineCircle.localScale = Vector3.one * crossLineCircleRadius * 2;
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
