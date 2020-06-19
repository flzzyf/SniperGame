using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SniperAlertManager : Singleton<SniperAlertManager>{
    public Slider slider_Alert;

    public float alertIncreaseRatePerSecond = .1f;

    private void Awake() {
        alertPanelStartingPos = alertPanel.rectTransform.anchoredPosition;
    }

    private void Start() {
        SetAlertValue(0);
    }

    private void Update() {
        if (GameManager.instance.gameWin)
            return;

        if (isAlerting && !isShowingHidingPanel) {
            ModifyAlertValue(alertIncreaseRatePerSecond * alertValueMax * Time.deltaTime);

            if(alertValue >= 100) {
                GameManager.instance.GameLose();

                alertValue = 0;
            }
        }

        if (isAlerting && Input.GetKeyDown("c")) {
            GameManager.instance.KillAllMissile();
            ShowHidingPanel(hidingPanel.gameObject.activeSelf == false);
        }
    }

    #region 警戒值

    float alertValue;
    const float alertValueMax = 100;

    void SetAlertValue(float value) {
        alertValue = Mathf.Clamp(value, 0, alertValueMax);

        slider_Alert.value = alertValue / alertValueMax;
    }

    void ModifyAlertValue(float modifyValue) {
        SetAlertValue(alertValue + modifyValue);
    }

    #endregion

    #region 隐蔽窗口

    public Image hidingPanel;

    float startingY = -600;

    bool isShowingHidingPanel;

    void ShowHidingPanel(bool show) {
        isShowingHidingPanel = show;

        hidingPanel.gameObject.SetActive(show);

        if (show) {
            hidingPanel.rectTransform.anchoredPosition = new Vector2(0, startingY);
            hidingPanel.rectTransform.DOAnchorPosY(0, .6f);
        }
    }

    #endregion

    #region 警戒窗口

    public Image alertPanel;

    Vector2 alertPanelStartingPos;

    void ShowAlertPanel(bool show) {
        alertPanel.gameObject.SetActive(show);
        if (show) {
            float offset = 300;
            alertPanel.rectTransform.anchoredPosition = new Vector2(alertPanelStartingPos.x, alertPanelStartingPos.y + offset);
            alertPanel.rectTransform.DOAnchorPosY(alertPanelStartingPos.y, .5f);
        }
    }

    #endregion

    #region 警戒模式

    public bool isAlerting;

    public void EnterAlert(float duration) {
        StartCoroutine(EnterAlertCor(duration));
    }
    IEnumerator EnterAlertCor(float duration) {
        isAlerting = true;

        ShowAlertPanel(true);

        yield return new WaitForSeconds(duration);

        ShowAlertPanel(false);

        isAlerting = false;

        ShowHidingPanel(false);
    }

    #endregion

}
