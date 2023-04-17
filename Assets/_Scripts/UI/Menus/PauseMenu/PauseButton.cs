using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class PauseButton : MonoBehaviour
{
    public static event Action OnUnPauseLevel_PauseButton;

    [SerializeField]
    private GameObject pauseCanvas;

    private Button pauseButton;

    private bool isPaused;
    private bool didHaveInput;

    private GameInputManager _gameInputManager;
    private GameLevelManager _gameLevelManager;

    #region Events Sub
    private void OnEnable()
    {
        PauseMenu.OnUnPauseLevel_PauseMenu += PauseMenu_OnUnPauseLevel;
    }


    private void OnDisable()
    {
        PauseMenu.OnUnPauseLevel_PauseMenu -= PauseMenu_OnUnPauseLevel;

    }
    #endregion

    private void PauseMenu_OnUnPauseLevel()
    {
        UnPause();
    }
    private void Awake()
    {
        pauseButton = GetComponent<Button>();
    }
    void Start()
    {
        _gameInputManager = GameInputManager.Instance;
        _gameLevelManager=GameLevelManager.Instance;
        pauseButton.onClick.AddListener(PauseGame);
        isPaused = false;
    }

    private void PauseGame()
    {
        if (!isPaused)
        {
            Pause();
        }
        else
        {
            UnPause();
            OnUnPauseLevel_PauseButton?.Invoke();
        }
    }
    private void Pause()
    {
        _gameLevelManager.SetCurrGameState(GameStates.Paused);
        BarController temp = BarController.Instance;
        if(temp != null)
            temp.SetCanGetInput(false);
        Time.timeScale = 0;
        didHaveInput = _gameInputManager.GetCanGetInput();
        _gameInputManager.SetCanGetInput(false);
        Instantiate(pauseCanvas);
        //TODO pasue Audio or lower the volume of it ?(or change to pause menu music)
        isPaused = true;
    }
    private void UnPause()
    {
        _gameLevelManager.SetCurrGameState(GameStates.Running);
        BarController temp = BarController.Instance;
        if (temp != null)
            temp.SetCanGetInput(true);
        _gameInputManager.SetCanGetInput(didHaveInput);
        Time.timeScale = 1f;
        //TODO Unpasue Audio or increase the volume of it ?(or change back to normal music)
        isPaused = false;
    }
}
