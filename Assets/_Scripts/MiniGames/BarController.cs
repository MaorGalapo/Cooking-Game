using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;


public class BarController : Singelton<BarController>
{
    private GameObjs _gameObjs;
    private Canvas mainMiniGameCanvas;

    private Color[] colors;

    #region Events
    public static event Action<float> OnBarClickedPanFlip; // Game Event
    public static event Action<float> OnBarClickedOvenAnim; // Game Event
    public static event Action<bool> OnOvenOnOffAnim;
    public static event Action<string, Color> OnBarClicked_UI; // UI event
    public static event Action<float> OnBarClicked_Grade;
    #endregion

    private Action miniGameMethod;

    private GameInputManager inputManager;

    private string currMiniGame;
    private bool canGetInput = true, gotFirstInput = false;
    private bool ovenPress = false;
    private float[] barSizes;
    private float[] passingGrades;

    #region BarImages
    //[SerializeField]
    private Image okBar, greatBar, perfectBar, playerBar, superBar;
    #endregion

    #region Consts
    private const string okBarName = "okBar", greatBarName = "greatBar", perfectBarName = "perfectBar", playerBarName = "playerBar", superBarName = "SuperBar";

    private const string ovenMiniGame = "ovengame", panMiniGame = "pangame";
    private const float sizeMult = 5;// X*2/10 == X*5
    private const float dirChange = .6f; //dist dir will change

    private const int OvenCloseTriggerCode = -2521;
    #region Defaults
    private readonly float[] defaultPassingGrades = { 98.2f, 90f, 80f, 55f };
    private readonly float[] defaultBarSizes = { 1.3f, .6f, .3f, .1f };
    private const float defaultSpeed = 20;
    #endregion

    #endregion

    #region changing vars
    private float speed = 50f;

    private float maxX, minX;
    // private float maxY, minY;
    private float currX;//, currY;


    #endregion


    #region Events sub
    private void OnEnable()
    {
        inputManager.OnStartTouch += InputManager_OnStartTouch;
        inputManager.OnEndTouch += InputManager_OnEndTouch;
        MiniGameManager.OnFinishAllMinigames += MiniGameManager_OnFinishAllMinigames;
        MiniGameManager.OnFinishMinigame += MiniGameManager_OnFinishMinigame;
    }


    private void OnDisable()
    {
        inputManager.OnStartTouch -= InputManager_OnStartTouch;
        inputManager.OnEndTouch -= InputManager_OnEndTouch;
        MiniGameManager.OnFinishAllMinigames -= MiniGameManager_OnFinishAllMinigames;
        MiniGameManager.OnFinishMinigame -= MiniGameManager_OnFinishMinigame;
    }
    #endregion

    #region Delay Methods
    //private void DelayInput()
    //{
    //    StartCoroutine(DelayInputCoroutine(1));
    //}

    private IEnumerator DelayInputCoroutine(float sec)
    {
        canGetInput = false;
        GameLevelTimerManager.SetPauseLevelTimer(true);
        yield return new WaitForSeconds(sec);
        OnBarClickedOvenAnim?.Invoke(OvenCloseTriggerCode);
        yield return new WaitForSeconds(.25f);
        GameLevelTimerManager.SetPauseLevelTimer(false);
        canGetInput = true;
    }
    private void DelayNextLevelPart(float grade)
    {
        StartCoroutine(DelayNextLevelPartCoroutine(1.4f, grade));
    }
    private IEnumerator DelayNextLevelPartCoroutine(float sec, float grade)
    {
        GameLevelTimerManager.SetPauseLevelTimer(true);
        canGetInput = false;
        yield return new WaitForSeconds(sec);
        OnBarClicked_Grade?.Invoke(grade);
        SetUpBarSizes(barSizes[0], barSizes[1], barSizes[2], barSizes[3]);
        GameLevelTimerManager.SetPauseLevelTimer(false);
    }
    #endregion

    #region setUp_Methods

    #region Public Methods
    public void SetCanGetInputTrueDelay(float delay)
    {
        StartCoroutine(DelayInputCoroutine(delay));
    }
    public void SetCanGetInput(bool input) => this.canGetInput = input;
    public void SetUpGrades(float[] passingGrades)
    {
        if (passingGrades.Length != 4)
        {
            this.passingGrades = defaultPassingGrades;
            return;
        }
        this.passingGrades = passingGrades;
    }

    public void SetUpSpeed(float speed)
    {
        if (speed <= 20)
        {
            this.speed = defaultSpeed;
            return;
        }
        this.speed = speed;
    }

    #endregion

    #region Private Methods
    public void SetBarSizesVar(float[] barSizes)
    {
        if (barSizes.Length != 4)
        {
            this.barSizes = defaultBarSizes;
            return;
        }
        this.barSizes = barSizes;
    }
    private void SetUpSprites()
    {
        okBar.sprite = _gameObjs.GetOkBarSprite();
        greatBar.sprite = _gameObjs.GetGreatBarSprite();
        perfectBar.sprite = _gameObjs.GetPerfectBarSprite();
        playerBar.sprite = _gameObjs.GetPlayerBarSprite();
        superBar.sprite = _gameObjs.GetSuperBarSprite();
    }
    private void SetUpBarGameObjs()
    {
        okBar = GameObject.Find(okBarName).GetComponent<Image>();
        greatBar = GameObject.Find(greatBarName).GetComponent<Image>();
        perfectBar = GameObject.Find(perfectBarName).GetComponent<Image>();
        playerBar = GameObject.Find(playerBarName).GetComponent<Image>();
        superBar = GameObject.Find(superBarName).GetComponent<Image>();
    }

    private void SetBarSizeX(Image bar, float size)
    {
        bar.rectTransform.localScale = new Vector3(size, bar.rectTransform.localScale.y, bar.rectTransform.localScale.y);
    }

    private void SetBarSPositionPan(Image bar, Image parentBar)
    {

        (float, float) minMaxX = getMinMaxX(bar, parentBar);

        bar.rectTransform.localPosition = new Vector3(UnityEngine.Random.Range(minMaxX.Item1, minMaxX.Item2), bar.rectTransform.localPosition.y, bar.rectTransform.localPosition.z);
    }

    private void SetBarSPositionOven(Image bar, Image parentBar)
    {
        (float, float) minMaxX = getMinMaxX(bar, parentBar);

        bar.rectTransform.localPosition = new Vector3(minMaxX.Item2, bar.rectTransform.localPosition.y, bar.rectTransform.localPosition.z);
    }

    private void SetPlayerBarPositionX(Image bar)
    {
        SetMinMaxX();

        bar.rectTransform.localPosition = new Vector3(minX, bar.rectTransform.localPosition.y, bar.rectTransform.localPosition.z);
        currX = maxX;
    }
    private void SetMinMaxX()
    {
        (float, float) minMaxX = getMinMaxX(playerBar, okBar);

        minX = minMaxX.Item1;
        maxX = minMaxX.Item2;
    }
    private void SetUpBarSizes(float okBarSize, float greatBarSize, float perfectBarSize, float playerBarSize)
    {
        SetBarSizeX(okBar, okBarSize);
        SetBarSizeX(greatBar, greatBarSize);
        SetBarSizeX(perfectBar, perfectBarSize);

        SetBarSizeX(playerBar, playerBarSize);
        SetBarSizeX(superBar, playerBarSize);
    }

    //private void setPlayerBarPositionY((float, float) minMaxY, Image bar)
    //{
    //    minY = minMaxY.Item1;
    //    maxY = minMaxY.Item2;

    //    bar.rectTransform.localPosition = new Vector3(bar.rectTransform.localPosition.x, minY, bar.rectTransform.localPosition.z);
    //    currY = maxY;
    //}
    #endregion

    #endregion

    #region Unity Methods
    private void Awake()
    {
        inputManager = GameInputManager.Instance;
        _gameObjs = GameObjs.Instance;
        SetUpBarGameObjs();
        colors = _gameObjs.GetMiniGameTextColors();
        mainMiniGameCanvas = GetComponentInChildren<Canvas>();
    }
    void Start()
    {
        EnableUI();
        SetUpSprites();
    }
    void Update()
    {
        if (canGetInput)
            miniGameMethod?.Invoke();

    }
    #endregion

    #region general mini game funcs
    private (float, float) getMinMaxX(Image bar, Image parentBar)
    {
        //float maxX, minX;
        float size, posX;

        size = parentBar.rectTransform.lossyScale.x;
        posX = parentBar.rectTransform.localPosition.x;

        return (posX - size * sizeMult + bar.rectTransform.lossyScale.x * sizeMult, posX + size * sizeMult - bar.rectTransform.lossyScale.x * sizeMult);
    }
    private void dirControllerX()
    {
        if (maxX - playerBar.rectTransform.localPosition.x <= dirChange)
            currX = minX;
        if (minX - playerBar.rectTransform.localPosition.x >= -dirChange)
            currX = maxX;
    }
    private bool RectOverlap(Rect firstRect, Rect secondRect)
    {
        if (firstRect.x + firstRect.width * 0.5f < secondRect.x - secondRect.width * 0.5f)
        {
            return false;
        }
        if (secondRect.x + secondRect.width * 0.5f < firstRect.x - firstRect.width * 0.5f)
        {
            return false;
        }
        if (firstRect.y + firstRect.height * 0.5f < secondRect.y - secondRect.height * 0.5f)
        {
            return false;
        }
        if (secondRect.y + secondRect.height * 0.5f < firstRect.y - firstRect.height * 0.5f)
        {
            return false;
        }
        return true;
    }
    private Rect GetWorldSapceRect(RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }
    private float GetRectDistance(RectTransform R1, RectTransform R2)
    {
        float x = Math.Abs(R1.localPosition.x) - Math.Abs(R2.localPosition.x);
        float y = Math.Abs(R1.localPosition.y) - Math.Abs(R2.localPosition.y);

        return Math.Abs(x);
    }
    private float CheckOverLap()
    {

        RectTransform playerBarRect_ = playerBar.rectTransform;
        RectTransform superBarRect_ = superBar.rectTransform;
        RectTransform perfectBarRect_ = perfectBar.rectTransform;
        RectTransform greatBarRect_ = greatBar.rectTransform;
        RectTransform okBarRect_ = okBar.rectTransform;

        Rect playerBarRect = GetWorldSapceRect(playerBarRect_);

        float playerDistance = GetRectDistance(playerBarRect_, superBarRect_);

        float grade = 100 - playerDistance; // grade is the distance from super bar with some adjustments

        if (RectOverlap(playerBarRect, GetWorldSapceRect(superBarRect_)))
        {
            OnBarClicked_UI?.Invoke("Super!", colors[0]);
            if (grade >= this.passingGrades[0])
                return 100;
            return grade;
        }

        if (RectOverlap(playerBarRect, GetWorldSapceRect(perfectBarRect_)))
        {
            OnBarClicked_UI?.Invoke("perfect!", colors[1]);
            if (grade > this.passingGrades[1])
                return (grade - this.passingGrades[1]) * .5f + this.passingGrades[1];
            return grade;
        }

        if (RectOverlap(playerBarRect, GetWorldSapceRect(greatBarRect_)))
        {
            OnBarClicked_UI?.Invoke("great", colors[2]);

            if (grade > this.passingGrades[2])
                return (grade - this.passingGrades[2]) * .5f + this.passingGrades[2];
            return grade;
        }

        if (RectOverlap(playerBarRect, GetWorldSapceRect(okBarRect_)))
        {
            OnBarClicked_UI?.Invoke("bad..", colors[3]);

            if (grade > this.passingGrades[3])
                return (grade - this.passingGrades[3]) * .5f + this.passingGrades[3];
            return grade;

        }
        return grade;
    }

    private float OnBarClickedSetupNextLevel()
    {
        //DelayInput();
        float grade = CheckOverLap();
        DelayNextLevelPart(grade);
        return grade;
    }

    //private (float, float) getMinMaxY(Image bar, Image parentBar)
    //{
    //    float maxY, minY, size, posY;

    //    size = parentBar.rectTransform.lossyScale.y;
    //    posY = parentBar.rectTransform.localPosition.y;

    //    return (posY - size * sizeMult + bar.rectTransform.lossyScale.y * sizeMult, posY + size * sizeMult - bar.rectTransform.lossyScale.y * sizeMult);
    //}
    //private void dirControllerY()
    //{
    //    if (maxY - playerBar.rectTransform.localPosition.y <= dirChange)
    //        currY = minY;
    //    if (minY - playerBar.rectTransform.localPosition.y >= -dirChange)
    //        currY = maxY;
    //}

    #endregion

    #region Oven Game Methods
    private void OvenMiniGameController()
    {
        if (ovenPress)
        {
            Vector3 playerPos = playerBar.rectTransform.localPosition;
            float num = playerPos.x - minX; // num is distance from minX
            if (num < 7) // if dist us smaller than 7 give a higher multi so it bar wont go too slow
                num *= 0.1f;
            else
                num *= 0.05f; // give multi so bar wont go too fast
            if (num == 0) // for starting position
                num = .7f;
            playerBar.rectTransform.localPosition = Vector3.MoveTowards(playerPos, new Vector3(currX, playerPos.y, playerPos.z), Time.deltaTime * speed * num);

        }
    }
    private void SetBarsOven()
    {
        SetBarSPositionOven(greatBar, okBar);
        SetBarSPositionPan(perfectBar, greatBar);
        SetBarSPositionPan(superBar, perfectBar);
    }
    public void setUpOvenGame()
    {
        OnOvenOnOffAnim?.Invoke(true);

        miniGameMethod = null;
        currMiniGame = ovenMiniGame;

        SetUpBarSizes(barSizes[0], barSizes[1], barSizes[2], barSizes[3]);

        SetPlayerBarPositionX(playerBar);

        SetBarsOven();

        miniGameMethod += dirControllerX;
        miniGameMethod += OvenMiniGameController;
    }
    #endregion

    #region Pan Game Methods
    private void PanMiniGameController()
    {
        Vector3 playerPos = playerBar.rectTransform.localPosition;

        playerBar.rectTransform.localPosition = Vector3.MoveTowards(playerPos, new Vector3(currX, playerPos.y, playerPos.z), Time.deltaTime * speed);

    }
    private void SetBarsPan()
    {
        SetBarSPositionPan(greatBar, okBar);
        SetBarSPositionPan(perfectBar, greatBar);
        SetBarSPositionPan(superBar, perfectBar);
    }
    public void SetUpPanGame()
    {
        OnOvenOnOffAnim?.Invoke(false);

        miniGameMethod = null;
        currMiniGame = panMiniGame;

        SetUpBarSizes(barSizes[0], barSizes[1], barSizes[2], barSizes[3]);

        SetPlayerBarPositionX(playerBar);

        SetBarsPan();

        miniGameMethod += dirControllerX;
        miniGameMethod += PanMiniGameController;

        canGetInput = true;

    }
    #endregion

    #region Event Methods
    private void MiniGameManager_OnFinishMinigame(float obj)
    {
        StopAllCoroutines();
    }
    private void MiniGameManager_OnFinishAllMinigames(float obj)
    {
        OnOvenOnOffAnim?.Invoke(false);
        StopAllCoroutines();
        canGetInput = false;
        gotFirstInput = false;
    }
    private void InputManager_OnStartTouch(Vector3 _, float __)
    {
        if (canGetInput)
        {
            gotFirstInput = true;
            switch (currMiniGame)
            {
                case panMiniGame:

                    float grade = OnBarClickedSetupNextLevel();

                    OnBarClickedPanFlip?.Invoke(grade); // chance itemes wont fall off

                    SetBarsPan();
                    SetMinMaxX();

                    break;
                case ovenMiniGame: // start the bar movement
                    ovenPress = true;
                    break;
                default:
                    throw new NotImplementedException();

            }
        }
    }
    private void InputManager_OnEndTouch(Vector3 _, float __)
    {
        if (canGetInput && gotFirstInput)
        {
            switch (currMiniGame)
            {
                case panMiniGame:
                    break;
                case ovenMiniGame:

                    float grade = OnBarClickedSetupNextLevel();

                    OnBarClickedOvenAnim?.Invoke(grade);

                    ovenPress = false;

                    SetBarsOven();
                    SetPlayerBarPositionX(playerBar);
                    break;
                default:
                    throw new NotImplementedException();

            }
        }
        gotFirstInput = false;
    }
    #endregion
    public void DisableUI() => this.mainMiniGameCanvas.enabled = false;
    public void EnableUI() => this.mainMiniGameCanvas.enabled = true;
}
