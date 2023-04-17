using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectFlip : MonoBehaviour
{

    #region Consts

    private readonly (float, float) xPushForceRange = (-2.2f, 2.2f);
    private readonly (float, float) yPushForceRange = (6f, 8.5f);
    private readonly (float, float) zPushForceRange = (-4f, -7.5f);

    private readonly (float, float) xPushForceRangeFail = (-7f, 7f);
    private readonly (float, float) yPushForceRangeFail = (9f, 15f);
    private readonly (float, float) zPushForceRangeFail = (-10f, -20f);
    #endregion

    private Rigidbody _rb;
    private Transform _transform;
    private Action FlipMethod;

    #region Events Sub

    private void OnEnable()
    {
        PanFlipAnimation.OnPanFlipStart += PanFlipAnimation_OnPanFlipStart;
        PanFlipAnimation.OnPanFlipGrade += PanFlipAnimation_OnPanFlipGrade;
    }


    private void OnDisable()
    {
        PanFlipAnimation.OnPanFlipStart -= PanFlipAnimation_OnPanFlipStart;
        PanFlipAnimation.OnPanFlipGrade -= PanFlipAnimation_OnPanFlipGrade;
    }

    #endregion

    #region Event Methods
    private void PanFlipAnimation_OnPanFlipStart(Vector3 point)
    {
        float force = Mathf.Abs(point.z) - Mathf.Abs(_transform.position.z);
        _rb.AddForce(Vector3.forward * 10 * force);
    }

    private void PanFlipAnimation_OnPanFlipGrade(float obj)
    {

        StartCoroutine(WaitForFixedUpdate(obj));
    }

    #endregion

    #region Delay Methods
    private IEnumerator WaitForFixedUpdate(float grade)
    {
        CheckGrade(grade);

        yield return new WaitForFixedUpdate();

        FlipMethod();
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
    #endregion

    private void CheckGrade(float grade)
    {
        if (grade >= 90)
        {
            FlipMethod = NormalFlip;
            return;
        }

        float x = UnityEngine.Random.Range(grade, 100);

        if (x >= 85)
        {
            FlipMethod = NormalFlip;
        }
        else
        {
            FlipMethod = FailFlip;
        }
    }

    #region Flip Methods
    private void NormalFlip()
    {
        FlipObject(xPushForceRange, yPushForceRange, zPushForceRange);
    }

    private void FailFlip()
    {
        GetComponent<MeshCollider>().isTrigger = true;

        FlipObject(xPushForceRangeFail, yPushForceRangeFail, zPushForceRangeFail);

        StartCoroutine(WaitAndDestroy());
    }

    private void FlipObject((float, float) xRange, (float, float) yRange, (float, float) zRange)
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        float x = UnityEngine.Random.Range(xRange.Item1, xRange.Item2);
        float y = UnityEngine.Random.Range(yRange.Item1, yRange.Item2);
        float z = UnityEngine.Random.Range(zRange.Item1, zRange.Item2);

        _rb.velocity = new Vector3(x, y, z);
    }
    #endregion

    private void Update()
    {
        if(_transform.position.y<-30)
            Destroy(gameObject);
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = transform;
        FlipMethod = NormalFlip;
    }

}
