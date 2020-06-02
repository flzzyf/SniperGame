using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : Singleton<GameManager> {
    public Cross cross;
    public AimCircle aimCircle;

    //命中率
    public static float chances;

    public Text text_Chances;
    public Slider slider_Chances;

    AimState aimState;
    AimState lastAimState;

    public Color color_Outside;
    public Color color_Crossing;
    public Color color_Inside;

    public static SniperData SniperData;
    public SniperData sniperData;

    private void Awake() {
        SniperData = sniperData;

        aimCircle.SetRadius(sniperData.aimRatePhase1Radius);

        missilesInCircle = new List<Missile>();

        StartCoroutine(AutoGenerateMissiles());

        StartCoroutine(GenerateObstacles());

        InitFocusValue();
    }

    void Update() {
        //如果准心在目标范围内
        Circle targetCircle = new Circle { center = aimCircle.transform.position, radius = aimCircle.transform.localScale.x / 2 };
        Circle crossCircle = new Circle { center = cross.transform.position, radius = cross.outerCircleRadius };
        aimState = AimState.Outside;

        if (targetCircle.IsPointInCircle(cross.transform.position)) {
            aimState = AimState.Crossing;
            chances += Time.deltaTime * sniperData.aimRateMultiplier;
        }

        if (targetCircle.GetCircleRelation(crossCircle) == CircleRelations.Contain) {
            aimState = AimState.Inside;
            chances += Time.deltaTime * sniperData.aimRateMultiplier;
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

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    //消除扩散圈内所有飞弹
        //    KillMissilesInCircle();
        //}

        //if (Input.GetKeyDown(KeyCode.F)) {
        //    GenerateMissile();
        //}

        //DetectMissilesInCircle();

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
    }

    IEnumerator GenerateObstacles() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(8, 14));

            GenerateObstacle();
        }
    }

    public GameObject gameWinWindow;
    bool gameWin;

    IEnumerator GameWin() {
        Time.timeScale = 0;

        gameWinWindow.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        gameWin = true;
    }

    #region 命中率

    //设置命中率文本
    public void SetChances(float chances) {
      
        //命中率满
        if(!gameWin && chances >= 100) {
            StartCoroutine(GameWin());

            return;
        }
        
        //最多100
        GameManager.chances = Mathf.Clamp(chances, 0, 100);

        text_Chances.text = string.Format("稳定率 : {0}%", (chances).ToString("f0"));
        slider_Chances.value = chances / 100;

        //设置瞄准圈半径
        float phase = GetPhase(chances, sniperData.aimRatePhase1Percnet * 100, sniperData.aimRatePhase2Percnet * 100);
        //Debug.Log(phase);
        float radius = sniperData.aimRatePhase1Radius + (sniperData.aimRatePhase2Radius - sniperData.aimRatePhase1Radius) * phase;

        aimCircle.SetRadius(radius);
    }

    public void ModifyChances(float modify) {
        SetChances(chances + modify);
    }

    //获取命中率阶段
    float GetPhase(float chances, float min, float max) {
        float phase = (Mathf.Clamp(chances, min, max) - min) / (max - min);

        return phase;
    }

    #endregion

    #region 生成弹幕

    public Missile missilePrefab;

    public void GenerateMissile() {
        Vector2 pos = GenerateOutsidePoint();
        Missile missile = Instantiate(missilePrefab, pos, Quaternion.identity, Camera.main.transform);

        float offsetValue = .1f;
        Vector2 randomOffset = new Vector2(Random.Range(-offsetValue, offsetValue), Random.Range(-offsetValue, offsetValue));
        Vector2 targetPoint = (Vector2)cross.transform.position + randomOffset;
        Vector2 dir = (targetPoint - pos).normalized;
        missile.MoveToward(dir);

        //尺寸随机
        float size = Random.Range(1, 3);
        float speed = sniperData.missileSpeed / size;
        missile.transform.localScale = Vector3.one * size;
        missile.speed = speed;
    }

    float missileGenerateRate {
        get {
            return sniperData.missileRate + chances * sniperData.missileRateBoostPerAimRate;
        }
    }

    IEnumerator AutoGenerateMissiles() {
        while (true) {
            yield return new WaitForSeconds(1 / missileGenerateRate);

            GenerateMissile();
        }
    }

    //获取一个屏幕外的点
    Vector2 GenerateOutsidePoint() {
        float x;
        float y;
        int index = Random.Range(0, 5);
        if (index == 0) {
            x = -ScreenSize.x / 2 - .1f;
            y = Random.Range(-ScreenSize.y / 2, ScreenSize.y / 2);
        }
        else if (index == 1) {
            x = ScreenSize.x / 2 + .1f;
            y = Random.Range(-ScreenSize.y / 2, ScreenSize.y / 2);
        }
        else if (index == 2) {
            y = -ScreenSize.y / 2 - .1f;
            x = Random.Range(-ScreenSize.x / 2, ScreenSize.x / 2);
        } else {
            y = ScreenSize.y / 2 + .1f;
            x = Random.Range(-ScreenSize.x / 2, ScreenSize.x / 2);
        }



        return (Vector2)Camera.main.transform.position +  new Vector2(x, y);
    }

    Vector2 ScreenSize {
        get {
            Camera cam = Camera.main;

            float ratio = (float)Screen.width / Screen.height;

            return new Vector2(cam.orthographicSize * ratio, cam.orthographicSize) * 2;
        }
    }

    #endregion

    #region 消除弹幕

    public LayerMask missileMask;

    public float focusValueToKill = 4;

    //消除圈内弹幕
    void KillMissilesInCircle() {
        if(focusValue < focusValueToKill) {
            return;
        }

        ModifyFocusValue(-focusValueToKill);

        var colliders = Physics2D.OverlapCircleAll(cross.transform.position, cross.outerCircleRadius, missileMask);

        HashSet<Missile> missileSet = new HashSet<Missile>();

        for (int i = 0; i < colliders.Length; i++) {
            Missile missile = colliders[i].GetComponent<Missile>();

            missileSet.Add(missile);
        }

        foreach (var item in missileSet) {
            //如果相交
            Circle crossCircle = new Circle { center = cross.transform.position, radius = cross.outerCircleRadius };
            Circle missileCircle = new Circle { center = item.transform.position, radius = item.GetComponent<CircleCollider2D>().radius * item.transform.localScale.x };
            if (crossCircle.GetCircleRelation(missileCircle) == CircleRelations.Intersect) {
                ModifyChances(2);

                Vector2 dir = (item.transform.position - cross.transform.position).normalized;
                item.Kick(dir);
            } else {
                //item.Die();
            }

        }
    }

    List<Missile> missilesInCircle;

    //void DetectMissilesInCircle() {
    //    var colliders = Physics2D.OverlapCircleAll(cross.transform.position, cross.outerCircleRadius, missileMask);

    //    foreach (var item in colliders) {
    //        if (!missilesInCircle.Contains(item.GetComponent<Missile>())) {
    //            item.GetComponent<Missile>().SetAlpha(.1f, .1f);

    //            missilesInCircle.Add(item.GetComponent<Missile>());
    //        }
    //    }

    //    Circle targetCircle = new Circle { center = aimCircle.transform.position, radius = aimCircle.transform.localScale.x / 2 };
    //    for (int i = missilesInCircle.Count - 1; i >= 0; i--) {
    //        Missile missile = missilesInCircle[i];
    //        Circle missileCircle = new Circle { center = missile.transform.position, radius = missile.GetComponent<CircleCollider2D>().radius };
    //        if(targetCircle.GetCircleRelation(missileCircle) != CircleRelations.Intersect) {
    //            missile.SetAlpha(1f, .1f);
    //        }
    //    }
    //}

    #endregion

    #region 障碍物

    public Vector2 obstaclePos;
    public Obstacle obstaclePrefab;

    void GenerateObstacle() {
        Obstacle obstacle = Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity);
    }

    #endregion

    public float backgroundMoveSpeed = .03f;

    public Transform background;
    public Target target;

    //背景移动
    void BackgroundMove(Vector2 dir) {

    }

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
        if(focusValue < focusValueMax) {
            ModifyFocusValue(focusRegenRate * Time.deltaTime);
        } else if(focusValue > focusValueMax) {
            SetFocusValue(focusValueMax);
        }
    }

    void SetFocusValue(float value) {
        focusValue = value;

        slider_Focus.value = value / focusValueMax;
    }

    void ModifyFocusValue(float modify) {
        SetFocusValue(focusValue + modify);

    }

    #endregion

    #region 集中

    //集中键
    KeyCode[] focusKeys = { KeyCode.Space, KeyCode.Mouse0 };

    //集中中
    bool isFocusing;

    //目前集中的时间
    float focusTime;

    void UpdateFocus() {
        //按下
        foreach (var item in focusKeys) {
            if (Input.GetKeyDown(item)) {

                StartFocus();

                return;
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


        }
    }

    //开始集中
    void StartFocus() {
        Debug.Log("开始集中");

        isFocusing = true;

        //扣除耐力
        ModifyFocusValue(-5);

    }

    //结束集中
    void EndFocus() {
        Debug.Log("结束集中");

        isFocusing = false;

    }

    //保持集中
    void StayFocus() {
        Debug.Log("保持集中");

        focusTime += Time.deltaTime;

    }

    //单击集中
    void FocusClick() {
        Debug.Log("单击集中");

        focusTime = 0;
    }

    #endregion

}

public enum AimState { Outside, Crossing, Inside}
