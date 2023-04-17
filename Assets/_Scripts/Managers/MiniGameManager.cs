using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiniGameManager : Singelton<MiniGameManager>
{
    private MiniGameLevelSO mg_LevelSO;
    private CameraController camController;


    #region Events
    public static event Action<float> OnFinishMinigame;
    public static event Action<float> OnFinishAllMinigames;
    #endregion

    private GameLevelManager _gameLevelManager;
    private BarController _miniGameController;


    private MiniGameLevel currMiniGame;

    private int mg_currSubLevel;
    private int mg_currMiniGame;

    private List<(float, int)> finalGrade;

    #region Consts 
    private const string ovenMiniGame = "ovengame", panMiniGame = "pangame";
    private const string TrayName = "Tray";
    #endregion

    private GameObject trayPreFab;

    public GameObject GetTrayPrefab() => this.trayPreFab;

    public void SetMiniGameLevel(MiniGameLevelSO mg_LevelSO) => this.mg_LevelSO = mg_LevelSO;

    #region Events Sub
    private void OnEnable()
    {
        BarController.OnBarClicked_Grade += BarController_OnBarClicked_Grade;
    }

    private void OnDisable()
    {
        BarController.OnBarClicked_Grade -= BarController_OnBarClicked_Grade;
    }
    #endregion

    #region Events Methods
    private void addGradeToList(float grade)
    {
        if (finalGrade.Count <= mg_currMiniGame)
            finalGrade.Add((grade, 1));
        else
        {
            (float, int) newGrade = finalGrade[mg_currMiniGame];
            newGrade.Item1 += grade;
            newGrade.Item2++;
            finalGrade[mg_currMiniGame] = newGrade;

        }
    }
    private void BarController_OnBarClicked_Grade(float grade)
    {
        addGradeToList(grade);
        NextLevel();
    }

    #endregion

    private void InitializeLevel()
    {
        BarMiniGameSubLevel currLevel = currMiniGame.barLevels[mg_currSubLevel];

        _miniGameController.SetBarSizesVar(currLevel.barSizes);
        _miniGameController.SetUpGrades(currLevel.passingGrades);
        _miniGameController.SetUpSpeed(currLevel.speed);
        _miniGameController.SetCanGetInput(true);

        if (currMiniGame.levelTime == 0)
            throw new Exception("Timer is set to nothing");
        if (GameLevelManager.Instance.GetIsFirstLevelPart())
        {
            StartCoroutine(DelayFirstLevelPartTimer());
        }
        else
        {
            GameLevelTimerManager.EndLevelTimer();
            StartCoroutine(DelayFrameStartTimer());
        }
        switch (currMiniGame.name)
        {
            case panMiniGame:
                _gameLevelManager.SetSliceableObjs_PanGame();
                camController.SetPanGamePosition();
                break;
            case ovenMiniGame:
                _miniGameController.SetCanGetInputTrueDelay(1.4f);
                _gameLevelManager.SetSliceableObjs_OvenGame();
                camController.SetOvenGamePosition();
                break;
        }
    }

    private void InitializeMiniGame()
    {
        if (mg_currMiniGame > 0)
            StartCoroutine(DelayNextMiniGame(mg_currMiniGame - 1));
        else
        {
            currMiniGame = mg_LevelSO.miniGames[mg_currMiniGame];
            ResetLevel();
            InitializeLevel();
            //StartMiniGame();
        }
    }
    private void NextMiniGame()
    {
        IncMiniGame();
        if (this.mg_currMiniGame >= mg_LevelSO.miniGames.Length)
        {
            OnFinishAllMinigames?.Invoke(GetFinalGrade());
            _miniGameController.DisableUI();
            return;
        }
        InitializeMiniGame();
    }
    private void NextLevel()
    {
        float currFinalGrade = GetCurrGrade();
        IncLevel();
        if (this.mg_currSubLevel >= currMiniGame.barLevels.Length)
        {
            OnFinishMinigame?.Invoke(currFinalGrade);
            GameLevelTimerManager.EndLevelTimer();
            NextMiniGame();
            return;
        }
        InitializeLevel();
    }

    #region Inc/Dec/Reset for currLevel and currMiniGame
    private void IncLevel() => mg_currSubLevel++;
    private void IncMiniGame() => mg_currMiniGame++;
    private void DecLevel() => mg_currSubLevel--;
    private void DecMiniGame() => mg_currMiniGame--;
    private void ResetLevel() => mg_currSubLevel = 0;
    private void ResetMiniGame() => mg_currMiniGame = 0;
    #endregion

    #region Coroutines

    private IEnumerator DelayNextMiniGame(float minigame)
    {
        //Display Curr MiniGame Score Nicely
        yield return new WaitForSeconds(1.8f);
        currMiniGame = mg_LevelSO.miniGames[mg_currMiniGame];
        ResetLevel();
        InitializeLevel();
        StartMiniGame();
    }

    IEnumerator DelayFirstLevelPartTimer()
    {
        yield return new WaitForSeconds(4);
        GameLevelTimerManager.StartLevelTimer(currMiniGame.levelTime, StartNextPart_TimerMethod);
    }
    IEnumerator DelayFrameStartTimer()
    {
        yield return new WaitForEndOfFrame();
        GameLevelTimerManager.StartLevelTimer(currMiniGame.levelTime, StartNextPart_TimerMethod);
    }
    #endregion

    #region Setup Methods
    private void SetUpFinalGrade()
    {
        finalGrade = new List<(float, int)>();
    }

    #endregion

    #region Final Grade Methods
    private float GetCurrGrade()
    {
        return finalGrade[mg_currMiniGame].Item1 / finalGrade[mg_currMiniGame].Item2;
    }

    private float GetGradeByPos(int pos) => finalGrade[pos].Item1 / finalGrade[pos].Item2;

    private float GetFinalGrade()
    {
        float finalGrade = 0;
        for (int i = 0; i < this.finalGrade.Count; i++)
        {
            finalGrade += GetGradeByPos(i);
        }
        return finalGrade / this.finalGrade.Count;
    }

    #endregion

    #region Unity Methods
    private void Awake()
    {
        _miniGameController = GetComponent<BarController>();
        camController = CameraController.Instance;
        _gameLevelManager = GameLevelManager.Instance;
    }
    public void StartMiniGamesLevel()
    {
        trayPreFab = GameObject.Find(TrayName);

        SetUpFinalGrade();
        _miniGameController.EnableUI();
        ResetLevel();
        ResetMiniGame();
        InitializeMiniGame();
    }
    public void StartMiniGame()
    {
        switch (currMiniGame.name)
        {
            case panMiniGame:
                _miniGameController.SetUpPanGame();
                break;
            case ovenMiniGame:
                _miniGameController.setUpOvenGame();
                break;
        }
    }
    #endregion
    private void StartNextPart_TimerMethod()
    {
        addGradeToList(0);
        float currFinalGrade = GetCurrGrade();
        IncLevel();
        if (this.mg_currSubLevel >= currMiniGame.barLevels.Length)
        {
            OnFinishMinigame?.Invoke(currFinalGrade);
            GameLevelTimerManager.EndLevelTimer();
            NextMiniGame();
            return;
        }
        InitializeLevel();
    }
}
