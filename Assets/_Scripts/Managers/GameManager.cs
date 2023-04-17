using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingeltonPersistent<GameManager>
{
    private LevelDetailsSO currPlayLevel;

    public LevelDetailsSO GetCurrPlayLevel()=>this.currPlayLevel;
    private void SetGameSettings()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }

    #region Consts
    private const string GamePlaySceneName = "GameScene";
    private const string LevelMenueSceneName = "LevelMenu";
    #endregion

    #region Events Sub
    private void OnEnable()
    {
        LevelStartButton.OnLevelStart += LevelStartButton_OnLevelStart;
        PauseMenu.OnQuitLevel += PauseMenu_OnQuitLevel;
    }


    private void OnDisable()
    {
        LevelStartButton.OnLevelStart -= LevelStartButton_OnLevelStart;
        PauseMenu.OnQuitLevel -= PauseMenu_OnQuitLevel;
    }
    #endregion
    private void PauseMenu_OnQuitLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(LevelMenueSceneName);
    }

    private void LevelStartButton_OnLevelStart(LevelDetailsSO obj)
    {
        currPlayLevel = obj;
        SceneManager.LoadScene(GamePlaySceneName);
    }
    private void Start()
    {
        SetGameSettings();
    }
}
