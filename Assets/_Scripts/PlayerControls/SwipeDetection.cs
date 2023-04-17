using System;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDetection : MonoBehaviour
{
    #region Events
    public static event Action<Vector3, Vector3> OnSwipePos;
    #endregion
    [SerializeField]
    private float minDistance = .2f;
    [SerializeField]
    private float maxTime = 1f;

    private Camera mainCam;
    private GameInputManager inputManager;

    private Vector3 startPosition;
    private float startTime;

    private Vector3 endPosition;
    private float endTime;

    private GameObjs gameObjs;
    private void Awake()
    {
        inputManager = GameInputManager.Instance;
        gameObjs= GameObjs.Instance;
    }
    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }
    private void SwipeStart(Vector3 position, float time)
    {
        Instantiate(gameObjs.GetTouchStart_UI_Prefab(), position, Quaternion.identity,Level_UI_Manager.Instance.GetComponent<Transform>());
        startPosition = Utils.ScreenToWorld3D(mainCam, position);
        Debug.DrawLine(startPosition, startPosition + Vector3.right, Color.yellow, 10);
        startTime = time;
    }
    private void SwipeEnd(Vector3 position, float time)
    {
        endPosition = Utils.ScreenToWorld3D(mainCam, position); ;
        Debug.DrawLine(endPosition, endPosition + Vector3.right, Color.cyan, 10);
        endTime = time;
        DetectSwipe();
    }
    private void DetectSwipe()
    {
        if (Vector3.Distance(startPosition, endPosition) >= minDistance &&
            endTime - startTime <= maxTime)
        {

            OnSwipePos?.Invoke(startPosition, endPosition); // invoke slice enevt to notify scripts
        }
#if UNITY_EDITOR
    //    if (Vector3.Distance(startPosition, endPosition) >= minDistance &&
    //endTime - startTime <= maxTime)
    //    {
    //        Debug.Log($"<color=cyan>Swipe! - start pos {startPosition}, end pos {endPosition}, angle ~ {90 - Vector3.Angle(endPosition - startPosition, Vector3.forward)}</color>");
    //        Debug.DrawRay(startPosition, endPosition - startPosition, Color.red, 10);
    //    }
    //    else
    //        if (endTime - startTime <= maxTime)
    //        Debug.Log($"Tap! {endPosition}");
    //    else
    //        Debug.Log("Drag");
#endif
    }
}
