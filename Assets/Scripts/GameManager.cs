using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public enum AimState { Outside, Crossing, Inside }

public class GameManager : Singleton<GameManager> {
    public Cross cross;
    public AimCircle aimCircle;

    AimState aimState;
    AimState lastAimState;

    public Color color_Outside;
    public Color color_Crossing;
    public Color color_Inside;

    public static SniperData sniperData {
        get {
            return instance.SniperData;
        }
    }
    public SniperData SniperData;

    Camera cam;

    public LayerMask layer_Player;
    public LayerMask layer_Missile;

    private void Awake() {
        cam = Camera.main;

        aimCircle.SetRadius(sniperData.aimRatePhase1Radius);

        InitFocusValue();

        Action generateLevel = null;
        generateLevel = () => {
            StartCoroutine(GenerateLevels(sniperData.levels, () => {
                generateLevel();
            }));
        };

        generateLevel();
    }

    void Update() {
        //如果准心在目标范围内
        Circle targetCircle = new Circle { center = aimCircle.transform.position, radius = aimCircle.transform.localScale.x / 2 };
        Circle crossCircle = new Circle { center = cross.transform.position, radius = cross.outerCircleRadius };
        aimState = AimState.Outside;

        if (targetCircle.GetCircleRelation(crossCircle) == CircleRelations.Contain) {
            aimState = AimState.Inside;
            chances += Time.deltaTime * sniperData.aimRateMultiplier * sniperData.aimRateFocusPerSecond;
        }

        //准心在十字线圈内
        Circle crossLineCircle = new Circle { center = crossLine.transform.position, radius = crossLine.crossLineCircleRadius };
        if (crossLineCircle.GetCircleRelation(crossCircle) == CircleRelations.Contain) {
            chances += Time.deltaTime * sniperData.aimRateMultiplier * sniperData.aimRateCorssLinePerSecond;
        }

        SetChances(chances);

        if(lastAimState != aimState) {
            lastAimState = aimState;

            if (aimState == AimState.Crossing) {
                aimCircle.SetColor(color_Crossing);
            }
            else if(aimState == AimState.Outside) {
                aimCircle.SetColor(color_Outside);
            } else if (aimState == AimState.Inside) {
                aimCircle.SetColor(color_Inside);
            }
        }

        UpdateFocusValue();

        //游戏胜利后点击任意键重开
        if(gameWin && Input.anyKeyDown) {
            Time.timeScale = 1;

            gameWin = false;

            chances = 0;

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        //专注
        UpdateFocus();

        if (Input.GetKeyDown("f")) {
            StartCoroutine(GenerateLevels(sniperData.levels));
        }
    }

    #region 胜利界面

    public GameObject gameWinWindow;
    bool gameWin;

    IEnumerator GameWin() {
        Time.timeScale = 0;

        gameWinWindow.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        gameWin = true;
    }

    #endregion

    #region 命中率

    //命中率
    public static float chances;

    //命中率UI
    public Text text_Chances;
    public Slider slider_Chances;

    //设置命中率文本
    public void SetChances(float chances) {
      
        //命中率满
        if(!gameWin && chances >= 100) {
            StartCoroutine(GameWin());

            return;
        }
        
        //最多100
        chances = Mathf.Clamp(chances, 0, 100);
        GameManager.chances = chances;

        text_Chances.text = string.Format("稳定率 : {0}%", (chances).ToString("f0"));
        slider_Chances.value = chances / 100;

        //设置瞄准圈半径
        float phase = GetPhase(chances, sniperData.aimRatePhase1Percnet * 100, sniperData.aimRatePhase2Percnet * 100);
        //Debug.Log(phase);
        float radius = sniperData.aimRatePhase1Radius + (sniperData.aimRatePhase2Radius - sniperData.aimRatePhase1Radius) * phase;

        //设置瞄准圈半径
        aimCircle.SetRadius(radius);
    }

    public CrossLine crossLine;

    public void ModifyChances(float modify) {
        SetChances(chances + modify);
    }

    //获取命中率阶段
    float GetPhase(float chances, float min, float max) {
        float phase = (Mathf.Clamp(chances, min, max) - min) / (max - min);

        return phase;
    }

    public void Hit() {
        if(isFocusing) {
            if(focusValue > 30) {
                ModifyFocusValue(-30);
            } else {
                //耐力不足30%也可以抵消一次攻击

                //扩散圈变为最大
                cross.SetOuterCirclePercent(1);

                //暂时禁用集中直到回到50%
                focusDisbled = true;
                //强制结束集中
                EndFocus();

            }
        } else {
            ModifyChances(-5);
        }
    }

    #endregion

    #region 生成弹幕

    [Serializable]
    public struct MissileTypes {
        public Missile missile_Small;
        public Missile missile_Middle;
        public Missile missile_Big;
        public Missile def_Small;
        public Missile def_Middle;
        public Missile def_Large;
        public Missile bomb;
        public Missile spread;
        public Missile bouncy;
        public Missile tracer;
    }

    public MissileTypes missileType;

    List<Missile> missileList = new List<Missile>();

    public void GenerateMissile(Vector2 pos, MissileType type, Vector2 dir = default) {
        Missile missilePrefab;
        if(type == MissileType.大型) {
            missilePrefab = missileType.missile_Big;
        }
        else if(type == MissileType.中型){
            missilePrefab = missileType.missile_Middle;
        } else if (type == MissileType.防御型小) {
            missilePrefab = missileType.def_Small;
        } else if (type == MissileType.防御型中) {
            missilePrefab = missileType.def_Middle;
        } else if (type == MissileType.防御型大) {
            missilePrefab = missileType.def_Large;
        } else if (type == MissileType.引爆型) {
            missilePrefab = missileType.bomb;
        } else if (type == MissileType.分裂型) {
            missilePrefab = missileType.spread;
        } else if (type == MissileType.反弹型) {
            missilePrefab = missileType.bouncy;
        } else if (type == MissileType.跟踪型) {
            missilePrefab = missileType.tracer;
        } else {
            missilePrefab = missileType.missile_Small;
        }

        Missile missile = Instantiate(missilePrefab, pos, Quaternion.identity, Camera.main.transform);
        missileList.Add(missile);
        missile.onDestory = () => {
            missileList.Remove(missile);
        };

        if(dir == default) {
            Vector2 targetPoint = cam.transform.position;
            dir = (targetPoint - pos).normalized;
        }
        missile.MoveToward(dir);
    }

    //生成大型障碍物
    //IEnumerator GenerateObstacles() {
    //    while (true) {
    //        yield return new WaitForSeconds(Random.Range(8, 14));

    //        GenerateObstacle();
    //    }
    //}

    //生成关卡
    IEnumerator GenerateLevels(SniperLevel[] levels, Action onComplete = null) {
        Queue<Action> actionQueue = new Queue<Action>();

        foreach (var item in levels) {
            actionQueue.Enqueue(() => {
                GenerateWaves(item.waves, () => {
                    if(actionQueue.Count > 0) {
                        actionQueue.Dequeue().Invoke();
                    } else {
                        onComplete?.Invoke();
                    }
                });
            });
        }

        actionQueue.Dequeue().Invoke();

        yield return null;
    }

    //生成各个波次
    void GenerateWaves(Wave[] waves, Action onComplete = null) {
        Queue<Action> waveGenerateQueue = new Queue<Action>();

        foreach (var item in waves) {
            waveGenerateQueue.Enqueue(() => {
                StartCoroutine(GenerateWave(item, () => {
                    if (waveGenerateQueue.Count > 0) {
                        waveGenerateQueue.Dequeue().Invoke();
                    }
                }));
            });
        }

        waveGenerateQueue.Enqueue(() => {
            onComplete?.Invoke();
        });

        waveGenerateQueue.Dequeue().Invoke();
    }

    //生成波次
    IEnumerator GenerateWave(Wave wave, Action onComplete = null) {
        float angle = wave.startAngle;
        for (int i = 0; i < wave.number; i++) {
            GenerateMissile(GetMissileGeneratePoint(angle), wave.missile);

            angle += wave.increasement;

            if(wave.interval > 0)
                yield return new WaitForSeconds(wave.interval);
        }

        yield return new WaitForSeconds(wave.waitTime);

        onComplete?.Invoke();
    }

    Vector2 GetMissileGeneratePoint(float angle) {
        float radius = Mathf.Sqrt(Mathf.Pow(ScreenSize.x, 2) + Mathf.Pow(ScreenSize.y, 2)) / 2;
        radius += .2f;

        Vector2 offset = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;

        return (Vector2)cam.transform.position + offset;
    }

    #endregion

    #region 消除弹幕

    public LayerMask missileMask;

    public float focusValueToKill = 4;

    //获取扩散圈上的飞弹
    List<Missile> GetMissilesOnCircle() {
        List<Missile> missileList = new List<Missile>();

        foreach (var item in Physics2D.OverlapCircleAll(cross.transform.position, cross.outerCircleRadius, missileMask)) {
            Missile missile = item.GetComponent<CollideDetector>().missile;

            Circle crossCircle = new Circle { center = cross.transform.position, radius = cross.outerCircleRadius };
            Circle missileCircle = new Circle { center = missile.transform.position, radius = missile.OuterRadius() };
            if (crossCircle.GetCircleRelation(missileCircle) == CircleRelations.Intersect) {
                missileList.Add(missile);
            }
        }


        return missileList;
    }

    //获取圈内的飞弹
    List<Missile> GetMissilesInCircle(Vector2 pos, float radius) {
        HashSet<Missile> missileSet = new HashSet<Missile>();

        foreach (var item in Physics2D.OverlapCircleAll(pos, radius, missileMask)) {
            Missile missile = item.GetComponent<CollideDetector>().missile;

            if (!missileSet.Contains(missile)) {
                Circle crossCircle = new Circle { center = pos, radius = radius };
                Circle missileCircle = new Circle { center = missile.transform.position, radius = missile.OuterRadius() };
                if (crossCircle.GetCircleRelation(missileCircle) != CircleRelations.Separate) {
                    missileSet.Add(missile);
                }

                missileSet.Add(missile);
            }
        }

        return missileSet.ToList();
    }

    //消除扩散圈上的飞弹
    void KillMissiles() {
        //消除与扩散圈相交的飞弹
        var missiles = GetMissilesOnCircle();
        while (missiles.Count > 0) {
            missiles[0].TakeDamage(1);

            ModifyFocusValue(2);

            missiles.RemoveAt(0);
        }
    }

    //反弹扩散圈上的飞弹
    void KickMissiles() {
        //消除与扩散圈相交的飞弹
        var missiles = GetMissilesOnCircle();
        while (missiles.Count > 0) {
            Vector2 dir = (missiles[0].transform.position - cross.transform.position).normalized;
            missiles[0].Kick(dir);

            missiles.RemoveAt(0);
        }
    }

    public void KillAllMissile() {
        while(missileList.Count > 0) {
            missileList[0].Die();
        }
    }

    public void OnMissileDie(Missile missile) {
        missileList.Remove(missile);
    }

    #endregion

    #region 障碍物

    public Vector2 obstaclePos;
    public Obstacle obstaclePrefab;

    void GenerateObstacle() {
        Obstacle obstacle = Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity);
    }

    #endregion

    #region 专注值（耐力）

    float focusValue;
    float focusValueMax {
        get {
            return sniperData.focusValueMax;
        }
    }
    float focusRegenRate {
        get { return sniperData.focusRegenRate; }
    }

    public Slider slider_Focus;

    void InitFocusValue() {
        SetFocusValue(focusValueMax);
    }

    //更新专注值
    void UpdateFocusValue() {
        if (!isFocusing) {
            //自动回复耐力
            if (focusValue < focusValueMax) {
                ModifyFocusValue(focusRegenRate * Time.deltaTime);
            } else if (focusValue > focusValueMax) {
                SetFocusValue(focusValueMax);
            }
        }
    }

    void SetFocusValue(float value) {
        value = Mathf.Clamp(value, 0, focusValueMax);

        focusValue = value;

        slider_Focus.value = value / focusValueMax;

        if (isFocusing) {
            if (value <= 0) {
                EndFocus();
            }
        }
    }

    public void ModifyFocusValue(float modify) {
        SetFocusValue(focusValue + modify);
    }

    #endregion

    #region 集中

    //集中键
    KeyCode[] focusKeys = { KeyCode.Space, KeyCode.Mouse0 };

    //集中中
    bool isFocusing;

    //触发点击集中过了
    bool isFocusClicked;

    //目前集中的时间
    float focusTime;

    //集中被禁用
    bool focusDisbled;

    public float focusValuePerSecond {
        get {
            return sniperData.focusValuePerSecond;
        }
    }
    public float focusValueFocus {
        get {
            return sniperData.focusValueFocus;
        }
    }

    void UpdateFocus() {
        //如果集中没被禁用，按下按键，开始集中
        if (!focusDisbled) {
            foreach (var item in focusKeys) {
                if (Input.GetKeyDown(item)) {

                    StartFocus();

                    return;
                }
            }
        }

        if (isFocusing) {
            bool hasKeyDown = false;
            foreach (var item in focusKeys) {
                if (Input.GetKey(item)) {
                    hasKeyDown = true;

                    break;
                }
            }

            if (!hasKeyDown) {
                EndFocus();
            } else {
                StayFocus();
            }
        } else {
            //准心扩散
            cross.CrossSpread();
        }

        //集中被禁用
        if (focusDisbled) {
            //如果耐力超过50%，结束禁用
            if(focusValue / focusValueMax > .5f) {
                focusDisbled = false;
            }
        }
    }

    //开始集中
    void StartFocus() {
        //Debug.Log("开始集中");

        isFocusing = true;

        //扣除耐力
        ModifyFocusValue(-focusValueFocus);

        //移动速度修改
        cross.speed *= .7f;
    }

    //结束集中
    void EndFocus() {
        //Debug.Log("结束集中");

        //长按0.1秒后，0.3秒内算点击
        if (!isFocusClicked && focusTime < .3f) {
            FocusClick();
        }

        //移动速度修改
        cross.speed /= .7f;

        //瞬间扩散
        cross.CrossInstantSpread();

        //结束集中后，根据集中时间触发不同效果
        if(focusTime >= .3f && focusTime < .5f) {
            KillMissiles();
        }
        else if(focusTime >= .5f && focusTime < 1) {
            var missiles = GetMissilesOnCircle();
            while (missiles.Count > 0) {
                var m2 = GetMissilesInCircle(missiles[0].transform.position, .1f);
                while (m2.Count > 0) {
                    m2[0].TakeDamage(2);

                    m2.RemoveAt(0);
                }
                missiles.RemoveAt(0);
            }
        }
        else if(focusTime >= 1) {
            KickMissiles();
        }

        focusTime = 0;
        isFocusing = false;
        isFocusClicked = false;
    }

    //保持集中
    void StayFocus() {
        //Debug.Log("保持集中");

        focusTime += Time.deltaTime;

        //每秒消耗耐力
        if(focusTime > .3f) {
            ModifyFocusValue(-focusValuePerSecond * Time.deltaTime);
        }

        if (!cross.moving) {
            //准心收缩
            cross.CrossShrink();
        }
    }

    //单击集中
    void FocusClick() {
        //Debug.Log("单击集中");

        isFocusClicked = true;

        focusTime = 0;

        KillMissiles();
    }

    #endregion

    #region （废弃）背景移动

    public float backgroundMoveSpeed = .03f;

    public Transform background;
    public Target target;

    //背景移动
    void BackgroundMove(Vector2 dir) {

    }

    #endregion

    #region Util

    Vector2 ScreenSize {
        get {
            Camera cam = Camera.main;

            float ratio = (float)Screen.width / Screen.height;

            return new Vector2(cam.orthographicSize * ratio, cam.orthographicSize) * 2;
        }
    }

    #endregion

    private void OnDrawGizmos() {
        float radius = Mathf.Sqrt(Mathf.Pow(ScreenSize.x, 2) + Mathf.Pow(ScreenSize.y, 2)) / 2;
        radius += .2f;
        Gizmos.DrawWireSphere(Camera.main.transform.position, radius);
    }

}

