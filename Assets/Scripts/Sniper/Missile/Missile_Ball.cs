using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Ball : Missile {
    
    [Header("内外半径")]
    [Range(0, 1)]
    public float radius_Inside = .5f;
    //[Range(0, 1)]
    //new public float radius_Outside = 1f;
    [Header("内外颜色")]
    public Color color_Inside = Color.white;
    public Color color_Outside = Color.black;

    public Transform gfx_Inner, gfx_Outer;

    private void OnValidate() {
        gfx_Inner.localScale = Vector3.one * radius_Inside * scale;
        gfx_Outer.localScale = Vector3.one * radius_Outside * scale;

        gfx_Inner.GetComponent<SpriteRenderer>().color = color_Inside;
        gfx_Outer.GetComponent<SpriteRenderer>().color = color_Outside;

        GetComponent<CircleCollider2D>().radius = radius_Outside / 2 * scale;
    }
}
