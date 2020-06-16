using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Tracer : Missile {
    [Header("外圈半径")]
    [Range(.01f, .15f)]
    public float radius_Outer = .02f;
    [Header("内圈半径是外圈的百分比")]
    [Range(0, 1)]
    public float radius_InsidePercent = .5f;

    protected override void Init() {
        base.Init();

        if (followCross) {
            followTarget = GameManager.instance.cross.transform;
        }
    }

    public override float OuterRadius() {
        return radius_Outer;
    }

    protected override void OnValidated() {
        base.OnValidated();

        gfx_Inner.localScale = Vector3.one * radius_Outer * radius_InsidePercent * 2;
        gfx_Outer.localScale = Vector3.one * radius_Outer * 2;

        playerDetector.GetComponent<CircleCollider2D>().radius = radius_Outer * radius_InsidePercent;
        missileDetector.GetComponent<CircleCollider2D>().radius = radius_Outer;
    }

    [Header("追踪准心")]
    public bool followCross = true;
    [Header("转向率")]
    public float rotateSpeed = 200;

    Transform followTarget;

    protected override void OnUpdate() {
        base.OnUpdate();

        if (followCross) {
            Vector2 targetDir = ((Vector2)followTarget.position - (Vector2)transform.position).normalized;
            Vector2 currentDir = transform.up;

            Vector2 dir = Vector2.Lerp(currentDir, targetDir, rotateSpeed / 1000f); ;

            MoveToward(dir);
        }
    }
}
