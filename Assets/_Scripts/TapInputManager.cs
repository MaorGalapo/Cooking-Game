using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TapInputManager : Singelton<TapInputManager>
{
    public static event Action OnTapDetected;

    private bool canGetInput;

    public void SetCanGetInput(bool input)=>this.canGetInput = input;
    void Update()
    {
        if (canGetInput)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    OnTapDetected?.Invoke();
                }

            }
        }
    }
}
