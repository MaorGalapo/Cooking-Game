using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FunctionTickTimer
{


    private static List<FunctionTickTimer> activeTimerList;
    private static GameObject initGameObject;

    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionTickTimer_InitGameObject");
            activeTimerList = new List<FunctionTickTimer>();
        }
    }
    public static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string s = "#";
        s += new string(Enumerable.Repeat(chars, length)
           .Select(s => s[random.Next(s.Length)]).ToArray());
        return s;
    }
    public static FunctionTickTimer Create(Action<float> startAction, float timer, float tickInerval = -1, Action endAction = null, string timerName = null)
    {
        InitIfNeeded();
        if (timerName == null)
            timerName = RandomString(5);
        GameObject gameObject = new GameObject("FunctionTickTimer", typeof(MonoBehaviourHook));

        FunctionTickTimer functionTimer = new FunctionTickTimer(startAction, endAction, tickInerval, timer, timerName, gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = functionTimer.Update;

        activeTimerList.Add(functionTimer);

        return functionTimer;
    }

    private static void RemoveTimer(FunctionTickTimer functionTimer)
    {
        InitIfNeeded();
        activeTimerList.Remove(functionTimer);
    }

    public static void StopTimer(string timerName)
    {
        for (int i = 0; i < activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                // Stop this timer
                activeTimerList[i].DestroySelf();
                i--;
            }
        }
    }



    // Dummy class to have access to MonoBehaviour functions
    private class MonoBehaviourHook : MonoBehaviour
    {
        public Action onUpdate;
        private void Update()
        {
            if (onUpdate != null) onUpdate();
        }
    }

    private Action<float> startAction;
    private Action endAction;

    private float timer;
    private float tickInterval;
    float timerElapsed = 0;

    private string timerName;
    private GameObject gameObject;

    private bool isPaused;
    private bool isDestroyed;
    private bool isInitiated = false;

    public string GetName() => this.timerName;
    private FunctionTickTimer(Action<float> startAction, Action endAction, float tickInterval, float timer, string timerName, GameObject gameObject)
    {
        this.startAction = startAction;
        this.endAction = endAction;
        this.timer = timer;
        this.tickInterval = tickInterval;
        this.timerName = timerName;
        this.gameObject = gameObject;
        timerElapsed = 0;
        isInitiated = true;
        isDestroyed = false;
        isPaused = false;
    }

    public void Update()
    {
        if (!isPaused)
        {
            if (isInitiated)
            {
                startAction?.Invoke(timer);
                timerElapsed += Time.deltaTime;
                if (tickInterval != -1)
                {
                    if (timerElapsed > tickInterval)
                    {
                        timer -= tickInterval;
                        startAction?.Invoke(timer);
                        timerElapsed = 0;
                    }
                }
                else
                {
                    timer -= Time.deltaTime;
                    startAction?.Invoke(timer);
                }
                if (timer <= 0 || isDestroyed)
                {
                    endAction?.Invoke();
                    DestroySelf();
                }
            }
        }

    }

    public void DestroySelf()
    {
        isDestroyed = true;
        UnityEngine.Object.Destroy(gameObject);
        RemoveTimer(this);
    }

    public void AddTime(float time) => this.timer += time;

    public void SetPause(bool pasue) => this.isPaused = pasue;
}
