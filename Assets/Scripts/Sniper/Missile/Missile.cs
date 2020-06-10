using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [Header("速度")]
    public float speedMultipiler = 1;
    [Header("生命值")]
    public int hp = 1;

    [Header("整体大小")]
    public float scale = .03f;

    private void FixedUpdate() {
        UpdateMove();

        UpdateSearchTarget();
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

    #region 搜索目标

    //被弹回
    bool isKickBack;

    [Range(0, 1)]
    public float radius_Outside = 1f;

    void UpdateSearchTarget() {
        //搜索玩家
        foreach (var item in Physics2D.OverlapCircleAll(transform.position, radius_Outside * scale, GameManager.instance.layer_Player)) {
            if (item.GetComponent<Cross>().Hit(transform)) {
                Destroy(gameObject);
            }
        }

        if (isKickBack) {
            foreach (var item in Physics2D.OverlapCircleAll(transform.position, radius_Outside * scale, GameManager.instance.layer_Missile)) {
                if (item.gameObject != gameObject) {
                    GameManager.instance.ModifyFocusValue(2);
                    item.GetComponent<Missile>().TakeDamage(2);
                }
            }
        }
    }

    #endregion

    #region 踢回

    public float missileKickSpeedMultiplier {
        get {
            return GameManager.sniperData.missileKickSpeedMultiplier;
        }
    }

    //踢回
    public void Kick(Vector2 dir) {
        this.dir = dir;

        speedMultipiler *= missileKickSpeedMultiplier;

        isKickBack = true;
    }

    #endregion

    #region 离开镜头自动清除

    Camera cam;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        //离屏幕太远自动清除
        if (Vector2.Distance(cam.transform.position, transform.position) > 2) {
            Destroy(gameObject);
        }
    }

    #endregion

    #region 被击中

    //被击中后变形的
    public MissileType transformToWhenHit;
    public bool transformWhenHit;

    //被击中
    public void TakeDamage(int damage) {
        //被击中后会变成其他飞弹
        if (transformWhenHit) {
            //生成其他飞弹
            GameManager.instance.GenerateMissile(transform.position, transformToWhenHit, dir);
        }

        //自毁
        Die();
    }

    //死亡
    public void Die() {
        Instantiate(GameManager.sniperData.particle_MissileExplode.gameObject, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    #endregion
}
