using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceStartTouch : Touch_Indication
{
    private GameObject touchCurrPos;

    private const string currTouchTag = "CurrPosition";
    private Transform _transform;

    [SerializeField]
    private GameObject sliceLine;

    private Camera mainCam;
    private LineAnimation _lineAnimation;
    void Start()
    {
        mainCam = Camera.main;
        touchCurrPos = GameObject.FindGameObjectWithTag(currTouchTag);
        _transform = transform;

        _lineAnimation = Instantiate(sliceLine, Utils.ScreenToWorld3D(mainCam, _transform.position), Quaternion.identity).GetComponent<LineAnimation>();
        StartCoroutine(WaitForNextFrame());
    }

    private void Update()
    {
        _lineAnimation.SetTopPoint(Utils.ScreenToWorld3D(mainCam, touchCurrPos.transform.position) + new Vector3(0, .25f, 0));
    }

    private IEnumerator WaitForNextFrame()
    {
        yield return new WaitForEndOfFrame();
        _lineAnimation.SetEnabled(true);
        _lineAnimation.SetTopBotPoint(Utils.ScreenToWorld3D(mainCam, _transform.position) + new Vector3(0, .25f, 0), Utils.ScreenToWorld3D(mainCam, touchCurrPos.transform.position) + new Vector3(0, .25f, 0));
    }
}
