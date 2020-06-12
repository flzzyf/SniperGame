using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Ball : Missile {
    [Header("外圈半径")]
    [Range(.01f, .15f)]
    public float radius_Outer = .02f;
    [Header("内圈半径是外圈的百分比")]
    [Range(0, 1)]
    public float radius_InsidePercent = .5f;

    [Header("内外颜色")]
    public Color color_Inside = Color.white;
    public Color color_Outside = Color.black;

    public Transform gfx_Inner, gfx_Outer;

    //玩家检测器
    public CollideDetector playerDetector;
    public CollideDetector missileDetector;

    protected override void Init() {
        base.Init();

        missileDetector.missile = this;

        playerDetector.AddOnTriggerEnterCallback((tr) => {
            if (tr.GetComponent<Cross>().Hit(transform)) {
                Destroy(gameObject);
            }
        });
    }

    private void OnValidate() {
        gfx_Inner.localScale = Vector3.one * radius_Outer * radius_InsidePercent * 2;
        gfx_Outer.localScale = Vector3.one * radius_Outer * 2;

        gfx_Inner.GetComponent<SpriteRenderer>().color = color_Inside;
        gfx_Outer.GetComponent<SpriteRenderer>().color = color_Outside;

        playerDetector.GetComponent<CircleCollider2D>().radius = radius_Outer * radius_InsidePercent;
        missileDetector.GetComponent<CircleCollider2D>().radius = radius_Outer;
    }

    public override float OuterRadius() {
        return radius_Outer;
    }

    public override void Kick(Vector2 dir) {
        base.Kick(dir);

        missileDetector.AddOnTriggerEnterCallback((tr) => {
            GameManager.instance.ModifyFocusValue(2);
            tr.GetComponent<CollideDetector>().missile.TakeDamage(2);
        });
    }
}
