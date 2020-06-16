using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Spread : Missile {
    public float xScale = .6f;

    public float width = .1f;
    public float innerPercent = .5f;

    public override float OuterRadius() {
        return width * xScale / 2;
    }

    protected override void OnValidated() {
        base.OnValidated();

        gfx_Outer.localScale = new Vector3(width * xScale, width, 1);
        gfx_Inner.localScale = new Vector3(width * xScale, width, 1) * innerPercent;

        playerDetector.GetComponent<CircleCollider2D>().radius = width * xScale * innerPercent / 2;
        missileDetector.GetComponent<CircleCollider2D>().radius = width * xScale / 2;
    }

    [Header("被消除后生成的弹幕类型")]
    public MissileType generateMissileType;
    [Header("被消除后生成的弹幕数量")]
    public int generateMissileNum = 6;

    public override void Deathrattle() {
        base.Deathrattle();

        for (int i = 0; i < generateMissileNum; i++) {
            float angle = i * 60;
            Vector2 offset = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

            GameManager.instance.GenerateMissile(transform.position, generateMissileType, offset);
        }

    }
}
