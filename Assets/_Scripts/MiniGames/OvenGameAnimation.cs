using System;
using UnityEngine;

public class OvenGameAnimation : MonoBehaviour
{
    private Animator ovenAnimator;

    public static event Action<bool> OnOvenMiniGameState;

    #region Consts
    private const string OvenOpenTrigger = "OpenOven";
    private const string OvenCloseTrigger = "CloseOven";
    private const string OvenOffTrigger = "OvenOff";
    private const string OvenCookTrigger = "Cook";
    private const string OvenBurnSTrigger = "BurnSmall";
    private const string OvenBurnBTrigger = "BurnBig";

    private const int OvenCloseTriggerCode = -2521;
    #endregion

    #region Events Sub
    private void OnEnable()
    {
        BarController.OnBarClickedOvenAnim += BarController_OnBarClickedOvenAnim;
        BarController.OnOvenOnOffAnim += BarController_OnOvenOnOffAnim;
    }


    private void OnDisable()
    {
        BarController.OnBarClickedOvenAnim -= BarController_OnBarClickedOvenAnim;
        BarController.OnOvenOnOffAnim -= BarController_OnOvenOnOffAnim;

    }
    #endregion
    private void BarController_OnOvenOnOffAnim(bool obj)
    {
        if (obj)
        {
            ovenAnimator.ResetTrigger(OvenOffTrigger);
            ovenAnimator.SetTrigger(OvenOpenTrigger);
        }
        else
        {
            ovenAnimator.ResetTrigger(OvenOpenTrigger);
            ovenAnimator.SetTrigger(OvenOffTrigger);
            OnOvenMiniGameState?.Invoke(false);
        }
    }
    private void BarController_OnBarClickedOvenAnim(float grade)
    {
        if (grade == OvenCloseTriggerCode)
        {
            OnOvenMiniGameState?.Invoke(true);
            ovenAnimator.SetTrigger(OvenCloseTrigger);
            return;
        }
        switch(grade)
        {
            case 100:
                Instantiate(GameObjs.Instance.GetPS_SuccessEffect(),new Vector3(18.5f,-11f,0),Quaternion.Euler(-90,0,0)).SetActive(true); //TODO? Maybe move to Effects summon script?
                ovenAnimator.SetTrigger(OvenCookTrigger);
                break;
            case 80:
                ovenAnimator.SetTrigger(OvenBurnSTrigger);
                break;
            default:
                ovenAnimator.SetTrigger(OvenBurnBTrigger);
                break;
        }
    }
    
    void Start()
    {
       ovenAnimator = GetComponent<Animator>(); 
    }
}
