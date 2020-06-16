using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossLine : MonoBehaviour {
    [Header("移动速度")]
    public float speed = 2;
    bool moving;

    [Header("抖动幅度")]
    public float offsetMagnitude = .03f;

    Camera cam;

    public Transform crossLineCircle;

    public float crossLineCircleRadius = .2f;

    private void OnValidate() {
        crossLineCircle.localScale = Vector3.one * crossLineCircleRadius * 2;
    }

    private void Awake() {
        cam = Camera.main;

        //移动到最大距离
        Vector2 dir = new Vector2(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1)).normalized;

        crossLineCircleRadius = GameManager.sniperData.crossLineRadius;
        crossLineCircle.localScale = Vector3.one * crossLineCircleRadius * 2;

        lastMousePos = Input.mousePosition;

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Action randomMove = default;
            
        randomMove = () => {
            Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1)) * offsetMagnitude;
            MoveTo(randomOffset, randomMove);
        };

        randomMove.Invoke();
    }

    Vector2 mousePos;
    Vector2 lastMousePos;

    private void Update() {
        //鼠标移动
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        if ((Vector2)Input.mousePosition != lastMousePos) {
            lastMousePos = Input.mousePosition;

            transform.parent.position = mousePos;
        }
    }

    #region 移动

    void MoveTo(Vector2 offset, Action onComplete = null) {
        StartCoroutine(MoveToCor(offset, onComplete));
    }
    IEnumerator MoveToCor(Vector2 offset, Action onComplete = null) {
        moving = true;
        Vector2 targetPos = mousePos + offset;

        while (Vector2.Distance(transform.position, targetPos) >= speed * Time.fixedDeltaTime) {
            targetPos = mousePos + offset;
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            transform.Translate(dir * speed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        moving = false;

        onComplete?.Invoke();
    }

    #endregion
}
