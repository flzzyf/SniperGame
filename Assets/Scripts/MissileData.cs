using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Missile")]
public class MissileData : ScriptableObject {
    [Header("速度")]
    public float speed;
    [Header("生命值")]
    public int hp = 1;

    [Header("内外半径")]
    public float radius_Inside = .03f;
    public float radius_Outside = .07f;
    [Header("内外颜色")]
    public Color color_Inside = Color.white;
    public Color color_Outside = Color.black;
}
