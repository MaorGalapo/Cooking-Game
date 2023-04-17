using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameTimer_UI : MonoBehaviour
{
    private event Action OnTimerEnd;
    private const string timerBarName = "TimerBar_fg";
    private RectTransform bar;
    private float barScale;

    private GameObject timerBarCanvas;

    [SerializeField]
    private bool stop = false;
    FunctionTickTimer timer;
    private float timerBarTime;

    #region Evetns Sub
    private void OnEnable()
    {
        GameLevelTimerManager.OnStartLevelTimer += GameLevelTimerManager_OnStartLevelTimer;
        GameLevelTimerManager.OnEndLevelTimer += GameLevelTimerManager_OnEndLevelTimer;
        GameLevelTimerManager.OnPauseLevelTimer += GameLevelTimerManager_OnPauseLevelTimer;
    }


    private void OnDisable()
    {
        GameLevelTimerManager.OnStartLevelTimer -= GameLevelTimerManager_OnStartLevelTimer;
        GameLevelTimerManager.OnEndLevelTimer -= GameLevelTimerManager_OnEndLevelTimer;
        GameLevelTimerManager.OnPauseLevelTimer -= GameLevelTimerManager_OnPauseLevelTimer;
    }
    #endregion

    #region Events Methods
    private void GameLevelTimerManager_OnPauseLevelTimer(bool pasue)
    {
        if (timer != null)
        {
            this.timer.SetPause(pasue);
        }
        else
            Debug.LogError("not game timer to pasue");
    }
    private void GameLevelTimerManager_OnEndLevelTimer()
    {
        Debug.Log("<color=red>Stopped Timer</color>");

        StopGameTimer();
    }

    private void GameLevelTimerManager_OnStartLevelTimer(float time, Action EndMethod)
    {
        StartGameTimer(time, EndMethod);
    }

    #endregion

    #region Unity Methods
    void Start()
    {
        bar = GameObject.Find(timerBarName).GetComponent<RectTransform>();
        timerBarCanvas = GetComponent<GameObject>();
        barScale = bar.localScale.x;
    }
    #endregion

    #region UI Methods
    private void TimerBarCanvas_Enabled(bool enabled) => this.timerBarCanvas.SetActive(enabled);
    #endregion

    #region Timer Methods
    private void StartGameTimer(float time, Action EndMethod)
    {
        if (timer == null)
        {
            timerBarTime = time;
            timer = StartTimer(time, GameTimerMethod, -1, EndMethod);
        }
        else
            Debug.LogError("game timer already running");

    }
    private void StopGameTimer()
    {
        if (timer != null)
        {
            FunctionTickTimer.StopTimer(this.timer.GetName());
            this.timer = null;
        }
        else
            Debug.LogError("not game timer to stop");
    }
    private FunctionTickTimer StartTimer(float timer, Action<float> method, float timerInterval = -1, Action endMethod = null)
    {
        //this.bar.localScale = new Vector3(barScale, bar.localScale.y, bar.localScale.z);
        return FunctionTickTimer.Create(method, timer, timerInterval, endMethod);
    }
    private void GameTimerMethod(float timer)
    {
        if (timer <= 0)
        {
            Debug.Log("<color=red> Stopped bacause of time</color>");
            StopGameTimer();
            OnTimerEnd?.Invoke();
            return;
        }
        //Debug.Log($"Timer || {(timerBarTime - timer) / timerBarTime*100}%");
        //Debug.Log($"Bar Scale || {(barScale - bar.localScale.x) / barScale*100}%");

        bar.localScale = new Vector3(barScale - (barScale * (timerBarTime - timer) / timerBarTime), bar.localScale.y, bar.localScale.z); // UI Bar Scalar
    }
    #endregion
}
