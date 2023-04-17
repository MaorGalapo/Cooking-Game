using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1)]
public class GameInputManager : Singelton<GameInputManager>
{
    #region Events

    public delegate void StartTouch(Vector3 position, float time);
    public event StartTouch OnStartTouch;

    public delegate void EndTouch(Vector3 position, float time);
    public event EndTouch OnEndTouch;

    #endregion
    private bool canGetInput;

    private GameObjs gameObjs;
    private GameObject playerTouchPos;

    private PlayerControls _playerControls;
    private GameLevelManager _gameLevelManager;
    //private Camera mainCamera;
    private bool firstTouch = true;

    private const string PauseButtonTag = "PauseButtonTag";

    #region Enable Disable
    private void OnEnable()
    {
        _playerControls?.Enable();
    }
    private void OnDisable()
    {
        _playerControls?.Disable();
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        gameObjs = GameObjs.Instance;
        //mainCamera = Camera.main;
    }
    void Start()
    {
        _gameLevelManager = GameLevelManager.Instance;
        canGetInput = false;
        _playerControls.Touch.PrimaryTouch.started += ctx => StartTouchPrimary(ctx);
        _playerControls.Touch.PrimaryTouch.canceled += ctx => EndTouchPrimary(ctx);

    }
    private void Update()
    {
        if (canGetInput && _playerControls.Touch.PrimaryTouch.inProgress)
        {
            if (playerTouchPos != null)
            {
                playerTouchPos.transform.position = _playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
            }
        }
    }
    #endregion

    #region Delay Methods
    IEnumerator FirstTouchWait(InputAction.CallbackContext context)
    {
        yield return new WaitForEndOfFrame();
        TouchStartUIPOS();
        OnStartTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
    }
    IEnumerator StartTouchWait(InputAction.CallbackContext context)
    {
        yield return new WaitForEndOfFrame();
        if (canGetInput && !CheckPointerOverPauseButton() && _gameLevelManager.GetCurrGameState() != GameStates.Paused)
        {
            //if (firstTouch)
            //{
            //    firstTouch = false;
            //    StartCoroutine(FirstTouchWait(context));
            //}
            //else
            {
                TouchStartUIPOS();
                OnStartTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
            }
        }
    }
    IEnumerator EndTouchWait(InputAction.CallbackContext context)
    {
        yield return new WaitForEndOfFrame();
        if (canGetInput && !CheckPointerOverPauseButton() && _gameLevelManager.GetCurrGameState() != GameStates.Paused)
        {
            this.playerTouchPos = null;
            OnEndTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }
    #endregion

    #region Start End TouchPrimary
    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        StartCoroutine(StartTouchWait(context));
        //Debug.Log("<color=blue>Touch Fired</color>" +Time.time);
        //if (canGetInput&&!EventSystem.current.IsPointerOverGameObject())
        //{
        //    if (firstTouch)
        //    {
        //        firstTouch = false;
        //        StartCoroutine(FirstTouchWait(context));
        //    }
        //    else
        //    {
        //        TouchStartUIPOS();
        //        OnStartTouch?.Invoke(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        //    }
        //}
    }
    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        StartCoroutine(EndTouchWait(context));
        //if (canGetInput && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    this.playerTouchPos = null;
        //    OnEndTouch?.Invoke(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        //}
    }
    #endregion

    private void PosTouchPrimary(InputAction.CallbackContext context)
    {
        //Debug.Log(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
        OnEndTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private bool CheckPointerOverPauseButton()
    {
        return EventSystem.current.IsPointerOverGameObject() &&
     EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.CompareTag(PauseButtonTag);
    }
    private void TouchStartUIPOS() => playerTouchPos = Instantiate(gameObjs.GetTouchPos_UI_Prefab(), _playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), Quaternion.identity, Level_UI_Manager.Instance.GetComponent<Transform>());

    #region Public Methods
    public void SetCanGetInput(bool input) => this.canGetInput = input;
    public bool GetCanGetInput() => this.canGetInput;
    #endregion

}

