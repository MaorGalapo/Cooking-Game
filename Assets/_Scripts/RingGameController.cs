using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class RingGameController : MonoBehaviour
{

    private GameInputManager inputManager;

    private Action miniGameMethod;
    private GameObjs _gameObjs;

    private Image failRing, failRing_2, goodRing, goodRing_2, greatRing, greatRing_2, superRing, playerRing, playerRingOverlapCheck, edgeRing;
    private const string failRingName = "FailRing", goodRingName = "GoodRing", greatRingName = "GreatRing", superRingName = "SuperRing", playerRingName = "PlayerRing", playerRingOverlapName = "PlayerRingOverlap", edgeRingName = "EdgeRing", No2_ext = "_2";

    private bool canGetInput = true, gotFirstInput = false;

    [SerializeField]
    private float timer;

    [SerializeField]
    private float scaleSizeTimer = .1f;

    [SerializeField]
    private float scaleSizeTap = .1f;

    private float maxSize = 11; // not const because may change depending on level (might switch to const or readonly)

    #region Events sub
    private void OnEnable()
    {
        //inputManager.OnStartTouch += InputManager_OnStartTouch;
        //inputManager.OnEndTouch += InputManager_OnEndTouch;
        TapInputManager.OnTapDetected += TapTest_OnTapDetected;
    }


    private void OnDisable()
    {
        //inputManager.OnStartTouch -= InputManager_OnStartTouch;
        //inputManager.OnEndTouch -= InputManager_OnEndTouch;
        TapInputManager.OnTapDetected -= TapTest_OnTapDetected;
    }
    #endregion
    private void SetUpBarGameObjs()
    {
        // failRing = GameObject.Find(failRingName).GetComponent<Image>();
        failRing_2 = GameObject.Find(failRingName + No2_ext).GetComponent<Image>();

        //goodRing = GameObject.Find(goodRingName).GetComponent<Image>();
        goodRing_2 = GameObject.Find(goodRingName + No2_ext).GetComponent<Image>();

        //greatRing = GameObject.Find(greatRingName).GetComponent<Image>();
        greatRing_2 = GameObject.Find(greatRingName + No2_ext).GetComponent<Image>();

        superRing = GameObject.Find(superRingName).GetComponent<Image>();

        playerRing = GameObject.Find(playerRingName).GetComponent<Image>();
        playerRingOverlapCheck = GameObject.Find(playerRingOverlapName).GetComponent<Image>();

        edgeRing = GameObject.Find(edgeRingName).GetComponent<Image>();
    }

    #region OverLap Methods
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

        RectTransform playerRingRect_ = playerRingOverlapCheck.rectTransform;
        RectTransform superRingRect_ = superRing.rectTransform;
        RectTransform greatRingRect_ = greatRing_2.rectTransform;
        RectTransform goodRingRect_ = goodRing_2.rectTransform;
        RectTransform failRingRect_ = failRing_2.rectTransform;
        RectTransform edgeRingRect_ = edgeRing.rectTransform;

        Rect playerRingRect = GetWorldSapceRect(playerRingRect_);

        float playerDistance = GetRectDistance(playerRingRect_, superRingRect_);

        float grade = 100 - playerDistance; // grade is the distance from super bar with some adjustments
        if (RectOverlap(playerRingRect, GetWorldSapceRect(failRingRect_)))
        {
            Debug.Log($"<color=red> | Fail! | </color>");


            //if (grade > this.passingGrades[3])
            //    return (grade - this.passingGrades[3]) * .5f + this.passingGrades[3];
            return grade;

        }
        if (RectOverlap(playerRingRect, GetWorldSapceRect(goodRingRect_)))
        {
            Debug.Log($"<color=yellow> | good | </color>");


            //if (grade > this.passingGrades[2])
            //    return (grade - this.passingGrades[2]) * .5f + this.passingGrades[2];
            return grade;
        }
        if (RectOverlap(playerRingRect, GetWorldSapceRect(greatRingRect_)))
        {
            Debug.Log($"<color=green> | Great | </color>");

            //if (grade > this.passingGrades[1])
            //    return (grade - this.passingGrades[1]) * .5f + this.passingGrades[1];
            return grade;
        }
        if (RectOverlap(playerRingRect, GetWorldSapceRect(superRingRect_)))
        {
            Debug.Log($"<color=cyan> | Super | </color>");
            //if (grade >= this.passingGrades[0])
            //    return 100;
            return grade;
        }
        if (RectOverlap(playerRingRect, GetWorldSapceRect(edgeRingRect_)))
        {
            Debug.Log($"<color=black> | Edge Win! | </color>");


            //if (grade > this.passingGrades[3])
            //    return (grade - this.passingGrades[3]) * .5f + this.passingGrades[3];
            return grade;

        }




        return grade;
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        inputManager = GameInputManager.Instance;
        _gameObjs = GameObjs.Instance;
        SetUpBarGameObjs();
    }
    private void Start()
    {
        canGetInput = true; 
        TapInputManager.Instance.SetCanGetInput(canGetInput);
        //miniGameMethod = CircleMiniGameController;
        StartCoroutine(DelayCircleScaler());
    }
    //void Update()
    //{
    //    if (canGetInput)
    //        miniGameMethod?.Invoke();

    //}
    #endregion
    private void CircleMiniGameController()
    {

    }
    private IEnumerator DelayCircleScaler()//float Time)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            if (playerRing.transform.localScale.x >= 0)
                playerRing.transform.localScale -= new Vector3(scaleSizeTimer, scaleSizeTimer, scaleSizeTimer);
            if (timer <= 0)
            {
                canGetInput = false;
                TapInputManager.Instance.SetCanGetInput(canGetInput);
                CheckOverLap();
                break;
            }
        }

    }
    #region Event Methods
    //private void InputManager_OnStartTouch(Vector3 _, float __)
    //{
    //    if (canGetInput)
    //    {
    //        gotFirstInput = true;
    //    }
    //}
    //private void InputManager_OnEndTouch(Vector3 _, float __)
    //{
    //    if (canGetInput && gotFirstInput)
    //    {
    //    }
    //    gotFirstInput = false;
    //}
    private void TapTest_OnTapDetected()
    {
        if (canGetInput)
        {
            if (playerRing.transform.localScale.x + scaleSizeTap >= maxSize)
            {
                playerRing.transform.localScale = new Vector3(maxSize + .5f, maxSize + .5f, maxSize + .5f);
                return;
            }
            else if (playerRing.transform.localScale.x <= maxSize)// const max size
                playerRing.transform.localScale += new Vector3(scaleSizeTap, scaleSizeTap, scaleSizeTap);
        }
    }
    #endregion

}
