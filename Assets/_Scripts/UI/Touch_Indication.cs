using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch_Indication : MonoBehaviour
{

    protected GameInputManager gameInputManager;

    protected void Awake()
    {
        gameInputManager = GameInputManager.Instance;
    }
    #region EventSub
    protected void OnEnable()
    {
        gameInputManager.OnEndTouch += GameInputManager_OnEndTouch;
    }


    protected void OnDisable()
    {
        gameInputManager.OnEndTouch -= GameInputManager_OnEndTouch;
    }
    #endregion
    private void GameInputManager_OnEndTouch(Vector3 position, float time)
    {
        Destroy(gameObject);
    }
}
