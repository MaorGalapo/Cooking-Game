using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static event Action OnQuitLevel;
    public static event Action OnUnPauseLevel_PauseMenu;

    private Button quit_btn, continue_btn, reset_btn;

    #region Events Sub
    private void OnEnable()
    {
        PauseButton.OnUnPauseLevel_PauseButton += PauseMenu_OnUnPauseLevel;
    }


    private void OnDisable()
    {
        PauseButton.OnUnPauseLevel_PauseButton -= PauseMenu_OnUnPauseLevel;
    }
    #endregion
    private void PauseMenu_OnUnPauseLevel()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        quit_btn = buttons[0];
        continue_btn = buttons[1];
        reset_btn = buttons[2];

        quit_btn.onClick.AddListener(QuitButtonMethod);
        continue_btn.onClick.AddListener(ContinueButtonMethod);
        reset_btn.onClick.AddListener(ResetButtonMethod);
        
    }

    private void QuitButtonMethod()
    {
        OnUnPauseLevel_PauseMenu?.Invoke();
        OnQuitLevel?.Invoke();
    }
    private void ContinueButtonMethod()
    {
        OnUnPauseLevel_PauseMenu?.Invoke();
        Destroy(gameObject);
    }
    private void ResetButtonMethod()
    {
        OnUnPauseLevel_PauseMenu?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
