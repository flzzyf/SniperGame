using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [Header("速度")]
    public float speedMultipiler = 1;
    [Header("生命值")]
    public int hp = 1;

    [Header("内外颜色")]
    public Color color_Inside = Color.white;
    public Color color_Outside = Color.black;

    //玩家检测器
    public CollideDetector playerDetector;
    public CollideDetector missileDetector;

    public Transform gfx_Inner, gfx_Outer;

    public virtual float OuterRadius() {
        return 0;
    }

    private void Awake() {
        cam = Camera.main;

        Init();
    }

    protected virtual void Init() {
        missileDetector.missile = this;

        playerDetector.AddOnTriggerEnterCallback((tr) => {
            if (tr.GetComponent<Cross>().Hit(transform)) {
                Die(false);
            }
        });
    }

    private void Update() {
        //离屏幕太远自动清除
        if (Vector2.Distance(cam.transform.position, transform.position) > 2) {
            Remove();
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() {

    }

    private void FixedUpdate() {
        UpdateMove();
    }

    #region 移动

    Vector2 dir;

    [Header("朝向移动方向")]
    public bool faceTargetDir;

    public void MoveToward(Vector2 dir) {
        this.dir = dir;

        if(faceTargetDir)
            FaceTarget2D(transform, dir);
    }

    void FaceTarget2D(Transform _origin, Vector3 dir) {
        _origin.up = dir.normalized;
    }

    void UpdateMove() {
        if (dir != default) {
            transform.Translate(dir * speedMultipiler * GameManager.sniperData.missileSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    #endregion

    #region 踢回

    //被弹回
    bool isKickBack;

    public float missileKickSpeedMultiplier {
        get {
            return GameManager.sniperData.missileKickSpeedMultiplier;
        }
    }

    //踢回
    public virtual void Kick(Vector2 dir) {
        this.dir = dir;

        speedMultipiler *= missileKickSpeedMultiplier;

        isKickBack = true;
    }

    #endregion

    #region 离开镜头自动清除

    Camera cam;

    #endregion

    #region 被击中

    [Header("被击中后变形")]
    public bool transformWhenHit;
    [Header("被击中后变形成")]
    public MissileType transformToWhenHit;

    [Header("无敌")]
    public bool invincible;

    //被击中
    public virtual void TakeDamage(int damage) {
        if (invincible)
            return;

        hp -= damage;

        if(hp <= 0) {
            //被击中后会变成其他飞弹
            if (transformWhenHit) {
                //生成其他飞弹
                GameManager.instance.GenerateMissile(transform.position, transformToWhenHit, dir);
            }

            //自毁
            Die();
        }
    }

    public Action onDestory;

    bool isDead;

    //死亡
    public virtual void Die(bool triggerDeathrattle = true) {
        if (isDead) {
            return;
        }

        isDead = true;

        //移出列表
        GameManager.instance.OnMissileDie(this);

        //生成死亡粒子
        Instantiate(GameManager.sniperData.particle_MissileExplode.gameObject, transform.position, Quaternion.identity);

        Remove();

        if (triggerDeathrattle) {
            Deathrattle();
        }
    }

    //亡语效果
    public virtual void Deathrattle() {

    }

    public void Remove() {
        Destroy(gameObject);
        onDestory?.Invoke();
    }

    #endregion

    private void OnValidate() {
        gfx_Inner.GetComponentInChildren<SpriteRenderer>().color = color_Inside;
        gfx_Outer.GetComponentInChildren<SpriteRenderer>().color = color_Outside;

        OnValidated();
    }

    protected virtual void OnValidated() {

    }
}
