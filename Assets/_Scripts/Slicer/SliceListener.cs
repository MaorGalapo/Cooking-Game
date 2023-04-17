using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceListener : MonoBehaviour
{
    public Slicer slicer;
    private const int SliceableLatyer = 6;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == SliceableLatyer)
        {
            SliceableMaterial test = other.GetComponent<SliceableMaterial>();
            if (test != null)
            {
                Material material = test.GetSlicedMatCurr();
                slicer.materialAfterSlice = material;
            }
            slicer.isTouched = true;
        }
    }
}
