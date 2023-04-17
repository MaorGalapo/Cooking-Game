using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class SliceableMaterial : MonoBehaviour
{
    [SerializeField]
    private Material slicedMatHorz;
    [SerializeField]
    private Material slicedMatVert;
    private Material slicedMatCurr;

    #region Get
    public Material GetSlicedMatHorz() => this.slicedMatHorz;
    public Material GetSlicedMatVert() => this.slicedMatVert;
    public Material GetSlicedMatCurr() => this.slicedMatCurr;
    public (Material, Material) GetMaterials() => (this.slicedMatHorz, this.slicedMatVert);
    #endregion

    #region Set
    public void SetSlicedMatHorz(Material slicedMatHorz) => this.slicedMatHorz = slicedMatHorz;
    public void SetSlicedMatVert(Material slicedMatVer) => this.slicedMatVert = slicedMatVer;
    public void SetSlicedMatCurr(Material slicedMatCurr) => this.slicedMatCurr = slicedMatCurr;
    public void SetMaterials((Material, Material) materials) => (this.slicedMatHorz, this.slicedMatVert) = materials;
    #endregion

    #region eventSub
    private void OnEnable()
    {
        SlicePlaneController.OnSliceInstantiateSliceAngle += SlicePlaneController_OnSliceInstantiateSliceAngle;
    }
    private void OnDisable()
    {
        SlicePlaneController.OnSliceInstantiateSliceAngle -= SlicePlaneController_OnSliceInstantiateSliceAngle;
    }

    #endregion
    private void SlicePlaneController_OnSliceInstantiateSliceAngle(bool angle)
    {
        if (angle)
        {
            SetSlicedMatCurr(GetSlicedMatHorz());
        }
        else
        {
            SetSlicedMatCurr(GetSlicedMatVert());
        }
    }
    private void Start()
    {
        StartCoroutine(SpawnWait());
    }
    IEnumerator SpawnWait()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<MeshCollider>().enabled = true;
    }
}
