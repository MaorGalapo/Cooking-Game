using System;


public static class GameLevelTimerManager
{
    #region Events
    public static Action<float, Action> OnStartLevelTimer;
    public static Action OnEndLevelTimer;
    public static Action<bool> OnPauseLevelTimer;


    #endregion

    public static void SetPauseLevelTimer(bool pause) => OnPauseLevelTimer?.Invoke(pause);
    public static void StartLevelTimer(float time, Action EndMethod = null) => OnStartLevelTimer?.Invoke(time, EndMethod);
    public static void EndLevelTimer() => OnEndLevelTimer?.Invoke();
}
