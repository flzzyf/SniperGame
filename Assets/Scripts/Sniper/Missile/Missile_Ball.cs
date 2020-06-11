using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Ball : Missile {
    
    [Header("内圈半径是外圈的百分比")]
    [Range(0, 1)]
    public float radius_InsidePercent = .5f;
    //[Range(0, 1)]
    //new public float radius_Outside = 1f;
    [Header("内外颜色")]
    public Color color_Inside = Color.white;
    public Color color_Outside = Color.black;

    public Transform gfx_Inner, gfx_Outer;

    private void OnValidate() {
        gfx_Inner.localScale = Vector3.one * radius_Outside * radius_InsidePercent * 2;
        gfx_Outer.localScale = Vector3.one * radius_Outside * 2;

        gfx_Inner.GetComponent<SpriteRenderer>().color = color_Inside;
        gfx_Outer.GetComponent<SpriteRenderer>().color = color_Outside;

        //GetComponent<CircleCollider2D>().radius = radiusWithScale;

        playerDetector.GetComponent<CircleCollider2D>().radius = radius_Outside;
    }

}
