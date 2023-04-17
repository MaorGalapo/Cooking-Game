using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class CameraController : Singelton<CameraController>
{
    #region Contst
    private const float SlicingFOV = 13f;
    private static readonly Vector3 SlicingPos = new Vector3(0, 15, 0);
    private static readonly Vector3 SlicingRotation = new Vector3(90, 0, 0);

    private const float PanGameFOV = 31f;
    private static readonly Vector3 PanGamePos = new Vector3(15, 30, -3);
    private static readonly Vector3 PanGameRotation = new Vector3(90, 0, 0);

    private const float OvenGameFOV = 40f;
    private static readonly Vector3 OvenGamePos = new Vector3(18.5f, -2, -60);
    private static readonly Vector3 OvenGameRotation = new Vector3(0, 0, 0);
    #endregion

    private GameObject gameObject_;
    private Transform transform_;
    private Camera camera_;

    [SerializeField]
    private AnimationCurve curve;
    private void Awake()
    {
        this.gameObject_ = gameObject;
        this.transform_ = transform;
        this.camera_ = GetComponent<Camera>();
    }
    #region Coroutines
    private IEnumerator LerpFOV(float fov)
    {
        while (true)
        {
            if (camera_.fieldOfView + .5f >= fov)
            {
                camera_.fieldOfView = fov;
                yield return null;
                break;
            }
            Mathf.Lerp(camera_.fieldOfView, fov, Time.deltaTime * .5f);
        }
    }
    #endregion
    private void ChangePositionAnim(float fov, Vector3 Pos, Vector3 Rotation)
    {
        LeanTween.move(gameObject_, Pos, .7f).setEase(LeanTweenType.animationCurve);
        LeanTween.rotate(gameObject_, Rotation, .7f).setEase(curve);
        camera_.fieldOfView = fov;

    }
    public void SetSlicingPosition()
    {
        ChangePositionAnim(SlicingFOV, SlicingPos, SlicingRotation);
    }
    public void SetPanGamePosition()
    {
        ChangePositionAnim(PanGameFOV, PanGamePos, PanGameRotation);
    }
    public void SetOvenGamePosition()
    {
        ChangePositionAnim(OvenGameFOV, OvenGamePos, OvenGameRotation);
    }

    #region Hard Sets
    //private void ChangePositionHard(float fov, Vector3 Pos, Vector3 Rotation)
    //{
    //    camera_.transform.position = Pos;
    //    camera_.transform.Rotate(Rotation);
    //    camera_.fieldOfView = fov;

    //}
    //public void SetSlicingPositionHard()
    //{
    //    ChangePositionHard(SlicingFOV, SlicingPos, SlicingRotation);
    //}
    //public void SetPanGamePositionHard()
    //{
    //    ChangePositionHard(PanGameFOV, PanGamePos, PanGameRotation);
    //}
    //public void SetOvenGamePositionHard()
    //{
    //    ChangePositionHard(OvenGameFOV, OvenGamePos, OvenGameRotation);
    //}
    #endregion\

}
