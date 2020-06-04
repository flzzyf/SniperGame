using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Missile : MonoBehaviour {
    [HideInInspector]
    public float speed = 4;

    Vector2 dir;
    Camera cam;

    //被弹回
    bool isKickBack;

    public ParticleSystem particle_MissileExplode;

    public void MoveToward(Vector2 dir) {
        this.dir = dir;
    }

    private void Awake() {
        cam = Camera.main;

        speed = GameManager.SniperData.missileSpeed;
    }

    private void Update() {
        //离屏幕太远自动清除
        if(Vector2.Distance(cam.transform.position, transform.position) > 2) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if(dir != default) {
            transform.Translate(dir * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isKickBack) {
            
        } else {
            if (!collision.collider.CompareTag("Player")) {
                return;
            }

            if (collision.collider.GetComponent<Cross>().Hit(transform)) {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isKickBack) {
            if (collision.CompareTag("Missile")) {
                GameManager.instance.ModifyFocusValue(2);

                collision.GetComponent<Missile>().Die();
            }
        }
    }

    //变化透明度
    public void SetAlpha(float alpha, float duration) {
        foreach (var item in GetComponentsInChildren<SpriteRenderer>()) {
            item.DOFade(alpha, duration);
        }
    }

    //踢回
    public void Kick(Vector2 dir) {
        this.dir = dir;

        speed *= 3;

        isKickBack = true;
    }

    public void Die() {
        Instantiate(particle_MissileExplode.gameObject, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
