using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Def : Missile {
    [Header("外圈半径")]
    [Range(.01f, .15f)]
    public float radius_Outer = .02f;
    [Header("内圈半径是外圈的百分比")]
    [Range(0, 1)]
    public float radius_InsidePercent = .5f;

    protected override void OnValidated() {
        base.OnValidated();

        gfx_Inner.localScale = Vector3.one * radius_Outer * radius_InsidePercent * 2;
        gfx_Outer.localScale = Vector3.one * radius_Outer * 2;

        playerDetector.GetComponent<CircleCollider2D>().radius = radius_Outer * radius_InsidePercent;
        missileDetector.GetComponent<CircleCollider2D>().radius = radius_Outer;
    }

    public override float OuterRadius() {
        return radius_Outer;
    }
}
