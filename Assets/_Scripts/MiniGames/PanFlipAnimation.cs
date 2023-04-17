using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PanFlipAnimation : MonoBehaviour
{
    public static event Action<Vector3> OnPanFlipStart;
    public static event Action<float> OnPanFlipGrade;

    #region Consts
    private const string FlipFirstPhase = "PanFlipFirstPhase";
    private const string FlipThirdPhase = "PanFlipThirdPhase";
    private const string panFlipTrigger = "Flip";

    #endregion

    private Transform edgeOfPan;

    private float currGrade;
    private bool isGotInput = false; // for performance
    private bool isTriggeredStart = false; // performance
    private Animator panAnimator;


    void Start()
    {
        panAnimator = GetComponent<Animator>();
        panAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        edgeOfPan = GetComponentInChildren<Transform>();

    }

    #region Events Sub

    private void OnEnable()
    {
        BarController.OnBarClickedPanFlip += BarController_OnBarClickedPanFlip;
    }

    private void OnDisable()
    {
        BarController.OnBarClickedPanFlip -= BarController_OnBarClickedPanFlip;
    }

    #endregion
    private void BarController_OnBarClickedPanFlip(float grade)
    {
        currGrade = grade;
        
        isGotInput = true;
        isTriggeredStart = false;

        panAnimator.SetTrigger(panFlipTrigger);
    }
    private void Update()
    {
        if (isGotInput)
        {
            if (!isTriggeredStart && this.panAnimator.GetCurrentAnimatorStateInfo(0).IsName(FlipFirstPhase))
            {
                isTriggeredStart = true;
                OnPanFlipStart?.Invoke(edgeOfPan.position);
            }

            if (this.panAnimator.GetCurrentAnimatorStateInfo(0).IsName(FlipThirdPhase))
            {
                OnPanFlipGrade?.Invoke(currGrade);
                isGotInput = false;
            }
        }
    }
}
