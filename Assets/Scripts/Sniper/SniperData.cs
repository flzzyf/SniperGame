using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObject/SniperData")]
public class SniperData : ScriptableObject {

    [Header("关卡")]
    public SniperLevel[] levels;

    [Header("十字线区域半径")]
    [Header("十字线")]
    public float crossLineRadius = .08f;

    [Header("集中最大值")]
    [Header("集中")]
    public float focusValueMax = 100;
    [Header("集中每秒恢复量")]
    public float focusRegenRate = 2;

    [Header("进入集中状态消耗的耐力")]
    public float focusValueFocus = 5;
    [Header("集中每秒消耗的耐力")]
    public float focusValuePerSecond = 5;

    [Header("集中且处于判定圈时每秒命中率提升")]
    public float aimRateFocusPerSecond = 2;
    [Header("集中且处于十字线圈时每秒命中率提升")]
    public float aimRateCorssLinePerSecond = 4;

    [Header("集中时的准心移动速度倍率")]
    public float focusSpeedMultiplier = .7f;

    [Header("每秒移动速度（1单位=100像素)")]
    [Header("准心")]
    public float speed = .5f;

    [Header("扩散圈最小半径")]
    public float minOuterCircleRadius = .05f;
    [Header("扩散圈最大半径")]
    public float maxOuterCircleRadius = .2f;

    [Header("缩小速率（每秒百分比）")]
    public float shrinkRatePerSecond = .2f;
    [Header("扩大速率（每秒百分比）")]
    public float spreadRatePerSecond = .33f;


    [Header("命中率1阶段百分比")]
    [Range(0, 1)]
    [Header("判定圈")]
    [Space(20)]
    public float aimRatePhase1Percnet = .3f;
    [Header("命中率2阶段百分比")]
    [Range(0, 1)]
    public float aimRatePhase2Percnet = .7f;

    [Header("命中率1阶段半径")]
    public float aimRatePhase1Radius = .25f;
    [Header("命中率2阶段半径")]
    public float aimRatePhase2Radius = .5f;

    [Header("弹幕基本生成速率")]
    [Header("弹幕")]
    [Space(20)]
    public float missileRate = .5f;

    [Header("每1%命中率增加的弹幕生成速率")]
    public float missileRateBoostPerAimRate = .01f;

    [Header("投射物速度")]
    public float missileSpeed = .5f;

    [Header("投射物被击飞时的速度倍率")]
    public float missileKickSpeedMultiplier = 3;

    [Header("命中率上升倍率")]
    [Header("测试")]
    [Space(20)]
    public float aimRateMultiplier = 2;
}
