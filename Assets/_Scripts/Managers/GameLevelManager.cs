using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-1)]
public class GameLevelManager : Singelton<GameLevelManager>
{
    #region Consts
    private const string sliceLevelSOName = "SliceLevel";
    private const string miniGameLevelSOName = "MiniGameLevelSO";
    private const string sliceableTag = "Sliceable";
    private const string intendedCutTag = "Intended_Cut";
    private readonly Vector3 PanPosition = new(15, 3, 0);
    private readonly Vector3 OvenPosition = new(18.5f, -1, -9);
    #endregion

    private GameManager gameManager;

    private List<GameObject> slicedObjsParents;

    private float[] finalGradeing;

    private float currLevelTimer = 0;

    private GameStates currGameState;

    #region Events
    public static event Action<float> OnFinishedLevel;
    public static event Action<string> OnCountDownTimer;
    #endregion

    #region Current Level
    [SerializeField]
    private LevelDetailsSO currLevel;
    private Queue<string> levelOrder;
    private int currLevelPartNum;
    private string currLevelPart;

    #endregion

    #region Slice
    private int sliceLevelNum;
    private Queue<SliceLevel> sliceLevels;
    private SliceLevel currSliceLevel;
    private SliceLevelManager _sliceLevelManager;
    #endregion

    #region MiniGame
    private int miniGameLevelNum;
    private Queue<MiniGameLevelSO> miniGameLevels;
    private MiniGameLevelSO currMiniGamesLevel;
    private MiniGameManager _miniGameManager;
    #endregion

    #region Events Sub
    private void OnEnable()
    {
        SliceLevelManager.OnFinishedSlicingLevel_GameMaster += SliceLevelManager_OnFinishedSlicingLevel;
        SliceLevelManager.OnFinishedSlicing_GoParent += SliceLevelManager_OnFinishedSlicing_GoParent;
        SliceLevelManager.OnFinishedMoving += SliceLevelManager_OnFinishedMoving;

        MiniGameManager.OnFinishAllMinigames += MiniGameManager_OnFinishAllMinigames;
        OvenGameAnimation.OnOvenMiniGameState += OvenGameAnimation_OnOvenMiniGameState;
    }


    private void OnDisable()
    {
        SliceLevelManager.OnFinishedSlicingLevel_GameMaster -= SliceLevelManager_OnFinishedSlicingLevel;
        SliceLevelManager.OnFinishedSlicing_GoParent -= SliceLevelManager_OnFinishedSlicing_GoParent;
        SliceLevelManager.OnFinishedMoving -= SliceLevelManager_OnFinishedMoving;

        MiniGameManager.OnFinishAllMinigames -= MiniGameManager_OnFinishAllMinigames;
        OvenGameAnimation.OnOvenMiniGameState -= OvenGameAnimation_OnOvenMiniGameState;
    }
    #endregion

    #region Events Methods
    private void SliceLevelManager_OnFinishedSlicingLevel(float grade)
    {
        GameInputManager.Instance.SetCanGetInput(false);
        StartCoroutine(DelayDestroyObjsLeft_SliceGame());

        finalGradeing[currLevelPartNum - 1] = grade;
        PlayLevel();
    }
    private void MiniGameManager_OnFinishAllMinigames(float grade)
    {
        GameInputManager.Instance.SetCanGetInput(false);

        finalGradeing[currLevelPartNum - 1] = grade;
        PlayLevel();
    }
    private void SliceLevelManager_OnFinishedSlicing_GoParent(GameObject obj)
    {
        slicedObjsParents.Add(obj);
    }
    private void SliceLevelManager_OnFinishedMoving()
    {
        SetActiveGOParents(false);
    }

    #endregion

    #region Slice Parent Methods
    private void SetParentGOParents(GameObject parent)
    {
        if (slicedObjsParents.Count == 0)
            return;
        foreach (GameObject go in slicedObjsParents)
            go.transform.SetParent(parent.transform, true);
    }
    private void RemoveParentGOParents()
    {
        if (slicedObjsParents.Count == 0)
            return;
        foreach (GameObject go in slicedObjsParents)
            go.transform.SetParent(null, true);
    }
    private void SetActiveGOParents(bool set)
    {
        if (slicedObjsParents.Count == 0)
            return;
        foreach (GameObject go in slicedObjsParents)
            go.SetActive(set);
    }

    private void SetPositionGOParents(Vector3 pos, bool shouldActive = true)
    {
        if (slicedObjsParents.Count == 0)
            return;
        if (shouldActive)
            SetActiveGOParents(true);
        SetActiveGOParents(true);
        foreach (GameObject go in slicedObjsParents)
            go.transform.position = pos;
    }
    private void FreezeGOParents(bool shouldActive = true)
    {
        if (slicedObjsParents.Count == 0)
            return;
        if (shouldActive)
            SetActiveGOParents(true);
        foreach (GameObject go in slicedObjsParents)
        {
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.useGravity = false;
            }
        }
    }
    private void UnFreezeGOParents(bool shouldActive = true)
    {
        if (slicedObjsParents.Count == 0)
            return;
        if (shouldActive)
            SetActiveGOParents(true);
        foreach (GameObject go in slicedObjsParents)
        {
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
            }
        }
    }
    private void AddObjFlipComponent()
    {
        if (slicedObjsParents.Count == 0)
            return;
        foreach (GameObject go in slicedObjsParents)
        {
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.gameObject.AddComponent<ObjectFlip>();
            }
        }

    }

    private void SetPosUnFreezeGO(Vector3 pos)
    {
        SetPositionGOParents(pos, false);
        UnFreezeGOParents(false);
    }
    private void SetPosFreezeGO(Vector3 pos)
    {
        SetPositionGOParents(pos, false);
        FreezeGOParents(false);
    }
    #endregion

    #region Game Initializing Methods
    private void SetGameSettings()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }
    private void SetManagers()
    {
        _sliceLevelManager = SliceLevelManager.Instance;
        _miniGameManager = MiniGameManager.Instance;
        _sliceLevelManager.gameObject.SetActive(false);
        _miniGameManager.gameObject.SetActive(false);
    }
    private void StartLevel()
    {
        this.currLevelPartNum = 0;
        this.miniGameLevelNum = 0;
        this.sliceLevelNum = 0;
        this.finalGradeing = new float[currLevel.LevelParts.Length];
        PlayLevel();
    }
    #endregion

    #region Start Specific Games Methods
    private void StartSliceLevel()
    {
        _miniGameManager.gameObject.SetActive(false);
        _sliceLevelManager.gameObject.SetActive(true);
        SetSliceLevel();
        _sliceLevelManager.StartSliceLevel();
    }

    #region MiniGame Methods
    public void SetSliceableObjs_PanGame()
    {
        SetPosUnFreezeGO(PanPosition);
        AddObjFlipComponent();
    }
    public void SetSliceableObjs_OvenGame()
    {
        SetPosUnFreezeGO(OvenPosition);
    }
    private void OvenGameAnimation_OnOvenMiniGameState(bool state)
    {
        if (state)
        {
            SetParentGOParents(_miniGameManager.GetTrayPrefab());
            FreezeGOParents();
        }
        else
            RemoveParentGOParents();
    }

    #endregion

    private void StartMiniGameLevel()
    {
        _sliceLevelManager.GMStopAllCoroutines();
        _sliceLevelManager.gameObject.SetActive(false);

        _miniGameManager.gameObject.SetActive(true);
        SetMiniGamesLevel();
        _miniGameManager.StartMiniGamesLevel();
    }
    #endregion

    #region SetLevel Methods
    private void SetSliceLevel() => _sliceLevelManager?.SetSliceLevel(currSliceLevel);
    private void SetMiniGamesLevel() => _miniGameManager?.SetMiniGameLevel(currMiniGamesLevel);
    private void SetLevel()
    {
        sliceLevels = new Queue<SliceLevel>();
        miniGameLevels = new Queue<MiniGameLevelSO>();
        levelOrder = new Queue<string>();

        for (int i = 0; i < currLevel.LevelParts.Length; i++)
        {
            levelOrder.Enqueue(currLevel.LevelParts[i].GetType().Name);
            switch (currLevel.LevelParts[i].GetType().Name)
            {
                case sliceLevelSOName:
                    this.sliceLevels.Enqueue((SliceLevel)currLevel.LevelParts[i]);
                    break;
                case miniGameLevelSOName:
                    this.miniGameLevels.Enqueue((MiniGameLevelSO)currLevel.LevelParts[i]);
                    break;

            }
        }
    }
    #endregion

    #region First level Bool
    private bool isFirstLevelPart;

    public bool GetIsFirstLevelPart() => this.isFirstLevelPart;
    #endregion

    #region GameFlow Methods
    private void PlayLevel()
    {
        if (levelOrder.Count <= 0)
        {
            OnFinishedLevel?.Invoke(CalcFinalGrade());
            return;
        }
        currLevelPart = levelOrder.Dequeue();
        switch (currLevelPart)
        {
            case sliceLevelSOName:
                if (sliceLevels.Count > 0)
                {
                    currSliceLevel = sliceLevels.Dequeue();
                    sliceLevelNum++;
                    currLevelPartNum++;

                    currLevelTimer = currSliceLevel.levelTime;

                    StartSliceLevel();
                    if (isFirstLevelPart)
                    {
                        StartCoroutine(StartGameTimer());
                        isFirstLevelPart = false;
                    }
                    else
                    {
                        StartCoroutine(DelayInput());
                    }
                }
                break;
            case miniGameLevelSOName:
                if (miniGameLevels.Count > 0)
                {
                    currMiniGamesLevel = miniGameLevels.Dequeue();
                    miniGameLevelNum++;
                    currLevelPartNum++;

                    currLevelTimer = currMiniGamesLevel.miniGames[0].levelTime;

                    StartMiniGameLevel();
                    if (isFirstLevelPart)
                    {
                        StartCoroutine(StartGameTimer());
                        isFirstLevelPart = false;
                    }
                    else
                    {
                        StartCoroutine(DelayInput());
                        _miniGameManager.StartMiniGame();
                    }
                }
                break;
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        currGameState = GameStates.Running;
        gameManager = GameManager.Instance;
        SetGameSettings();
        SetManagers();
    }
    void Start()
    {
        isFirstLevelPart = true;
        if (gameManager != null)
            currLevel = gameManager.GetCurrPlayLevel();

        StartCoroutine(StartGameDelay());
    }
    #endregion

    #region Delay Methods
    private IEnumerator DelayDestroyObjsLeft_SliceGame()
    {
        yield return new WaitForSeconds(1.4f);
        var intendentCutsLeft = GameObject.FindGameObjectsWithTag(intendedCutTag);
        foreach (var cut in intendentCutsLeft)
            Destroy(cut);

        var sliceableObjsLeft = GameObject.FindGameObjectsWithTag(sliceableTag);
        foreach (var sliceableObj in sliceableObjsLeft)
            Destroy(sliceableObj);
    }
    private void StartGameLevel()
    {
        StartCoroutine(StartGameLevel_Coroutine());
    }
    private IEnumerator DelayInput()
    {
        yield return new WaitForSeconds(.4f);
        GameInputManager.Instance.SetCanGetInput(true);

    }
    private IEnumerator StartGameLevel_Coroutine()
    {
        yield return new WaitForEndOfFrame();
        SetLevel();
        StartLevel();
        slicedObjsParents = new List<GameObject>();
    }
    private IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(1);
        StartGameLevel();
    }
    private IEnumerator StartGameTimer()
    {
        int x = 4;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (x == 1)
            {
                OnCountDownTimer?.Invoke("Cook!");
                break;
            }
            else
                OnCountDownTimer?.Invoke((x - 1).ToString());
            x--;
        }
        switch (currLevelPart)
        {
            case sliceLevelSOName:
                if (sliceLevels.Count > 0)
                {
                    GameInputManager.Instance.SetCanGetInput(true);
                }
                break;
            case miniGameLevelSOName:
                if (miniGameLevels.Count > 0)
                {
                    GameInputManager.Instance.SetCanGetInput(true);
                    _miniGameManager.StartMiniGame();
                }
                break;
        }
    }

    #endregion

    #region Grade Methods
    private float CalcFinalGrade()
    {
        float finalGrade = 0;
        foreach (float grade in finalGradeing)
            finalGrade += grade;
        finalGrade /= finalGradeing.Length;
        return finalGrade;
    }
    #endregion

    #region Game State Methods
    public GameStates GetCurrGameState() => this.currGameState;
    public void SetCurrGameState(GameStates state) => this.currGameState = state;

    #endregion

}

public enum GameStates
{
    Running,
    Paused
}