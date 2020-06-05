using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour {

    Vector2 input;

    Vector2 mousePos;
    Vector2 lastMousePos;

    private void Awake() {
        speed = GameManager.sniperData.speed;
        minOuterCircleRadius = GameManager.sniperData.minOuterCircleRadius;
        maxOuterCircleRadius = GameManager.sniperData.maxOuterCircleRadius;
        shrinkRatePerSecond = GameManager.sniperData.shrinkRatePerSecond;

        shrinkRate = maxOuterCircleRadius * shrinkRatePerSecond;
        spreadRate = maxOuterCircleRadius * spreadRatePerSecond;
    }

    void Start() {
        lastMousePos = Input.mousePosition;

        targetPoint = transform.position;
    }

    void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        //方向键移动
        if(input != default) {
            targetPoint = (Vector2)transform.position + input.normalized * speed * Time.fixedDeltaTime;
        }

        //鼠标移动
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if((Vector2)Input.mousePosition != lastMousePos) {
            lastMousePos = Input.mousePosition;

            targetPoint = mousePos;
        }

        moving = (Vector2)transform.position != targetPoint;
    }

    private void FixedUpdate() {
        if((Vector2)transform.position != targetPoint) {
            //移动
            MoveToward(targetPoint, speed * Time.fixedDeltaTime);
        } 
    }

    #region 准心移动

    public float speed = 3;

    //移动的目标点
    Vector2 targetPoint;

    //移动中
    public bool moving;

    //向目标点移动
    void MoveToward(Vector2 targetPoint, float moveDistance) {
        if (Vector2.Distance(transform.position, targetPoint) <= moveDistance) {
            transform.position = targetPoint;
        } else {
            Vector2 dir = (targetPoint - (Vector2)transform.position).normalized;

            transform.Translate(dir * moveDistance);
        }
    }

    void ApplyForce(Vector2 force) {
        //GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(Force(force, .2f));
    }

    IEnumerator Force(Vector2 force, float duration) {
        for (float i = 0; i < duration; i += Time.fixedDeltaTime) {
            transform.Translate(force * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region 外圆

    public Transform outerCircle;
    [HideInInspector]
    public float outerCircleRadius = .1f;

    float minOuterCircleRadius = .05f;
    float maxOuterCircleRadius = .2f;

    //准心缩小率
    float shrinkRate;
    //扩散率
    float spreadRate;

    float shrinkRatePerSecond = .05f;
    float spreadRatePerSecond {
        get {
            return GameManager.sniperData.spreadRatePerSecond;
        }
    }

    //设置外圆半径
    void SetOuterCricleRadius(float radius) {
        radius = Mathf.Clamp(radius, minOuterCircleRadius, maxOuterCircleRadius);

        outerCircle.localScale = Vector3.one * radius * 2;

        outerCircleRadius = radius;
    }

    //准心扩散
    public void CrossSpread() {
        SetOuterCricleRadius(outerCircleRadius + Time.deltaTime * spreadRate);
    }

    //准心收缩
    public void CrossShrink() {
        SetOuterCricleRadius(outerCircleRadius - Time.deltaTime * shrinkRate);
    }

    //结束专注后准心瞬间扩散
    public void CrossInstantSpread() {
        if(outerCircleRadius < maxOuterCircleRadius * .1f) {
            SetOuterCricleRadius(maxOuterCircleRadius * .1f);
        }
    }

    //设置外圈比例
    public void SetOuterCirclePercent(float percent) {
        SetOuterCricleRadius(maxOuterCircleRadius * percent);
    }

    #endregion

    #region 被命中
    //无敌
    bool isInvincible;

    public bool Hit(Transform hiter) {
        //无敌
        if (isInvincible) {
            return false;
        }

        //被击中
        GameManager.instance.Hit();

        //无敌一段时间
        StartCoroutine(SetInvincible(.6f));

        //弹开
        Vector2 dir = (transform.position - hiter.position).normalized;

        ApplyForce(dir);

        Shaker.Shake(Camera.main.transform, .01f, .02f, .1f);

        return true;
    }

    IEnumerator SetInvincible(float duration) {
        isInvincible = true;

        SetAlpha(.5f, .1f);
        SetScale(.5f, .1f);

        yield return new WaitForSeconds(duration);

        SetAlpha(1f, .1f);
        SetScale(1f, .1f);

        isInvincible = false;
    }

    //变化透明度
    void SetAlpha(float alpha, float duration) {
        foreach (var item in GetComponentsInChildren<SpriteRenderer>()) {
            item.DOFade(alpha, duration);
        }
    }

    public Transform gfx;

    void SetScale(float scale, float duration) {
        gfx.DOScale(scale, duration);
    }

    #endregion


}
