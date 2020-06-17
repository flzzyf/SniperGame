using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Wave{
    [Header("弹幕类型")]
    public MissileType missile;
    [Header("生成数量")]
    public int number = 1;

    [Header("初始角度")]
    public float startAngle;
    [Header("每次增加角度")]
    public float increasement = 1;

    [Header("生成间隔时间")]
    public float interval;
    [Header("和下波弹幕间隔时间")]
    public float waitTime;
}

[CreateAssetMenu(menuName = "ScriptableObject/SniperLevel")]
public class SniperLevel : ScriptableObject {
    public Wave[] waves;

    [Header("和下关间隔时间")]
    public float waitTime;
}
