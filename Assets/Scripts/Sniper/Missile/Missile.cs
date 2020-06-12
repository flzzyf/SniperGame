using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [Header("速度")]
    public float speedMultipiler = 1;
    [Header("生命值")]
    public int hp = 1;

    public virtual float OuterRadius() {
        return 0;
    }

    private void Awake() {
        cam = Camera.main;

        Init();
    }

    protected virtual void Init() {

    }

    private void Update() {
        //离屏幕太远自动清除
        if (Vector2.Distance(cam.transform.position, transform.position) > 2) {
            Remove();
        }
    }

    private void FixedUpdate() {
        UpdateMove();
    }

    #region 移动

    Vector2 dir;

    public void MoveToward(Vector2 dir) {
        this.dir = dir;
    }

    void UpdateMove() {
        if (dir != default) {
            transform.Translate(dir * speedMultipiler * GameManager.sniperData.missileSpeed * Time.fixedDeltaTime);
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

    //被击中后变形的
    public MissileType transformToWhenHit;
    public bool transformWhenHit;

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

    //死亡
    public virtual void Die() {
        Instantiate(GameManager.sniperData.particle_MissileExplode.gameObject, transform.position, Quaternion.identity);

        Remove();
    }

    public void Remove() {
        Destroy(gameObject);
        onDestory?.Invoke();
    }

    #endregion

}
