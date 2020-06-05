using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Missile : MonoBehaviour {
    public float speedMultipiler = 1;

    Vector2 dir;
    Camera cam;

    //被弹回
    bool isKickBack;

    public ParticleSystem particle_MissileExplode;

    public float innerRadius = 0.05f;
    public float outerRadius {
        get {
            return GetComponent<CircleCollider2D>().radius;
        }
    }

    public void MoveToward(Vector2 dir) {
        this.dir = dir;
    }

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        //离屏幕太远自动清除
        if(Vector2.Distance(cam.transform.position, transform.position) > 2) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if (dir != default) {
            transform.Translate(dir * speedMultipiler * GameManager.sniperData.missileSpeed * Time.fixedDeltaTime);
        }

        //搜索玩家
        foreach (var item in Physics2D.OverlapCircleAll(transform.position, outerRadius, GameManager.instance.layer_Player)) {
            if (item.GetComponent<Cross>().Hit(transform)) {
                Destroy(gameObject);
            }
        }

        if (isKickBack) {
            foreach (var item in Physics2D.OverlapCircleAll(transform.position, outerRadius, GameManager.instance.layer_Missile)) {
                if(item.gameObject != gameObject) {
                    GameManager.instance.ModifyFocusValue(2);
                    item.GetComponent<Missile>().Die();
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

    //变化透明度
    public void SetAlpha(float alpha, float duration) {
        foreach (var item in GetComponentsInChildren<SpriteRenderer>()) {
            item.DOFade(alpha, duration);
        }
    }

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

    public void Die() {
        Instantiate(particle_MissileExplode.gameObject, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

}
