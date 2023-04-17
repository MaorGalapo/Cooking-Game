using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAnimScript : MonoBehaviour
{
    private Transform PS_Fire_Alpha;
    private Transform PS_Fire_Add;
    private Transform PS_Fire_Glow;
    private Transform PS_Fire_Sparks;

    private bool animTrigger;

    #region Consts
    private const float EnlargmentFactor = 2.2f;
    #endregion

    private void Awake()
    {
        Transform[] T = GetComponentsInChildren<Transform>();
        PS_Fire_Alpha = T[1];
        PS_Fire_Add = T[2];
        PS_Fire_Glow = T[3];
        PS_Fire_Sparks = T[4];
    }

    #region Events Sub
    private void OnEnable()
    {
        PanFlipAnimation.OnPanFlipStart += PanFlipAnimation_OnPanFlipStart;
    }


    private void OnDisable()
    {
        PanFlipAnimation.OnPanFlipStart -= PanFlipAnimation_OnPanFlipStart;
        StopAllCoroutines();
    }
    #endregion

    #region Event Methods
    private void PanFlipAnimation_OnPanFlipStart(Vector3 obj)
    {
        EnlargeFire();
    }
    #endregion

    private Transform[] GetAllPSTransforms()
    {
        return new Transform[] { PS_Fire_Alpha, PS_Fire_Add, PS_Fire_Glow, PS_Fire_Sparks };
    }

    #region Animation Methods
    /* TORemember :):
     if performace gets effected too much from coroutines
     try to switch to LeenTween */
    private void EnlargeFire()
    {
        animTrigger = true;
        Transform[] PS_TransformArr = GetAllPSTransforms();
        foreach (Transform _transform in PS_TransformArr)
        {
            StartCoroutine(ScaleSize(_transform, _transform.localScale * EnlargmentFactor));
        }
        StartCoroutine(DelayAndDwindle());
    }
    private void DwindleFire()
    {
        if (animTrigger)
        {
            animTrigger = false;
            Transform[] PS_TransformArr = GetAllPSTransforms();
            foreach (Transform _transform in PS_TransformArr)
            {
                StartCoroutine(ScaleSize(_transform, _transform.localScale / EnlargmentFactor));
                StartCoroutine(DelayAndReturnScale(_transform));
            }
        }
    }
    #endregion

    #region Delay Methods
    private IEnumerator DelayAndReturnScale(Transform t)
    {
        yield return new WaitForSeconds(.7f);
        t.localScale = Vector3.one * .1f;
    }
    private IEnumerator DelayAndDwindle()
    {
        yield return new WaitForSeconds(.5f);
        DwindleFire();
    }
    private IEnumerator ScaleSize(Transform t,Vector3 sizeTo)
    {
        while (true)
        {
            yield return new WaitForSeconds(.02f);
            t.localScale = Vector3.Lerp(t.localScale, sizeTo, .16f);
            if (Vector3.Distance(t.localScale, sizeTo) < .05f)
                break;
        }  
        yield return null;
    }
    #endregion

}
