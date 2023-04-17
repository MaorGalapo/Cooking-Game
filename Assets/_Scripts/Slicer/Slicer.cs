using UnityEngine;
using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;

public class Slicer : MonoBehaviour
{
    public Material materialAfterSlice;
    private (Material, Material) objMaterials;
    private LayerMask sliceMask;

    public bool isFreeze = true;

    private const int SliceableLayer = 6;
    private const string Sliceable = "Sliceable";
    private const int framesLag = 5;

    private int countFrames;
    public bool isTouched;

    private IntersectionPoints topBotPoint;
    private Vector3 intersectionVector;

    public static event Action<Vector3, Vector3> OnCut;
    private void Awake()
    {
        sliceMask = LayerMask.GetMask(Sliceable);
        countFrames = -framesLag;
        isTouched = true;
    }

    private void Start()
    {
        topBotPoint = GetComponent<IntersectionPoints>();
        StartCoroutine(WaitForFrame());
    }
    private IEnumerator WaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        SliceInstantiate();
    }
    private void SliceInstantiate()
    {
        if (isTouched == true && Time.frameCount - countFrames >= framesLag)
        {
            countFrames = Time.frameCount;
            isTouched = false;

            Collider[] objectsToBeSliced = Physics.OverlapBox(transform.position, new Vector3(1, 0.1f, 0.1f), transform.rotation, sliceMask);

            foreach (Collider objectToBeSliced in objectsToBeSliced)
            {
                if (objectToBeSliced.GetComponent<SliceableMaterial>() != null)
                {
                    SliceableMaterial thisObj = objectToBeSliced.GetComponent<SliceableMaterial>();
                    objMaterials = thisObj.GetMaterials();
                    materialAfterSlice = thisObj.GetSlicedMatCurr();
                }
                else
                    materialAfterSlice = null;

                // sliced object using plane transform and rotation [ref SliceObject()]
                SlicedHull slicedObject = SliceObject(objectToBeSliced.gameObject, materialAfterSlice);
                if (slicedObject != null)
                {
                    // creates lower and upper hulls from sliced object
                    GameObject upperHullGameobject = slicedObject.CreateUpperHull(objectToBeSliced.gameObject, materialAfterSlice);
                    GameObject lowerHullGameobject = slicedObject.CreateLowerHull(objectToBeSliced.gameObject, materialAfterSlice);

                    // gives proper name to upper and lower hull
                    upperHullGameobject.name = $"{objectToBeSliced.name}_{upperHullGameobject.name}";
                    lowerHullGameobject.name = $"{objectToBeSliced.name}_{lowerHullGameobject.name}";

                    // sets upper and lower hulls position to be as same as object sliced
                    upperHullGameobject.transform.position = objectToBeSliced.transform.position + transform.up * .02f;
                    lowerHullGameobject.transform.position = objectToBeSliced.transform.position - transform.up * .02f;

                    // creates an empty gameobject that has the same name as object cut to be upper and lower hulls parent to create hiarchy
                    //GameObject SlicedParent = new GameObject(objectToBeSliced.name);
                    //SlicedParent.transform.position = objectToBeSliced.gameObject.transform.position;
                    //Transform SlicedParentTransform_ = SlicedParent.transform;

                    Destroy(objectToBeSliced.gameObject);

                    // makes objects phisical by adding rigidbody and meshcollider (also setting cut section material, sets parent and adding costume script) [ref MakeItPhysical()]
                    MakeItPhysical(upperHullGameobject);//, SlicedParentTransform_);
                    MakeItPhysical(lowerHullGameobject);//, SlicedParentTransform_);

                }
            }
        }
        Destroy(gameObject);
    }

    // adds to obj meshcollider, rigidbody and costume script that holds cut section material and sets the obj to be vhild of transform parent
    private void MakeItPhysical(GameObject obj)//, Transform parent)
    {
        obj.AddComponent<MeshCollider>().convex = true;
        obj.GetComponent<MeshCollider>().enabled = false;
        obj.AddComponent<Rigidbody>();
        if (isFreeze)
            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.AddComponent<SliceableMaterial>().SetMaterials(objMaterials);
        obj.layer = SliceableLayer;
        obj.tag = Sliceable;
        // obj.transform.parent = parent;

    }

    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        //Debug.Log($"Position {transform.position} and rotation {transform.up}");
        intersectionVector = topBotPoint.GetIntersectionVector();
        OnCut?.Invoke(intersectionVector, transform.up);
        return obj.Slice(transform.position, transform.up, crossSectionMaterial); // slices object using plane position and rotatation
    }


}
