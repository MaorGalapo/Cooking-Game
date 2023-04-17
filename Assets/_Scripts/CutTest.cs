using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using System;

public class CutTest : MonoBehaviour
{
    public Vector3 cutPlain;
    public Vector3 cutDir;
    [SerializeField]
    private bool cut = false;
    [SerializeField]
    private float cutPresantage = 0f;

    [SerializeField]
    private GameObject plane;

    public static event Action<Vector3, Vector3> OnCut;
    private void Update()
    {
        if (cut )//&& cutPresantage != 0 && cutPresantage != 1)
        {
            cut = false;

            float minX = transform.position.x - (transform.localScale.x * .5f), maxX = transform.localScale.x * .5f + transform.position.x;

            //float cutPoint = minX + cutPresantage * transform.localScale.x;
            float cutPoint = plane.transform.position.x;

            //cutPlain = new Vector3(transform.position.x + cutPoint, transform.position.y, transform.position.z);

            cutPlain = plane.transform.position;
            cutDir = plane.transform.up;
            

            SlicedHull slicedObject = gameObject.Slice(cutPlain, cutDir);
            if (slicedObject != null)
            {
                GameObject upperHullGameobject = slicedObject.CreateUpperHull(gameObject.gameObject);
                GameObject lowerHullGameobject = slicedObject.CreateLowerHull(gameObject.gameObject);

                upperHullGameobject.name = $"{gameObject.name}_{upperHullGameobject.name}";
                lowerHullGameobject.name = $"{gameObject.name}_{lowerHullGameobject.name}";

                upperHullGameobject.transform.position = gameObject.transform.position;
                lowerHullGameobject.transform.position = gameObject.transform.position;

                GameObject SlicedParent = new GameObject(gameObject.name);
                Transform SlicedParentTransform_ = SlicedParent.transform;

                MakeItPhysical(upperHullGameobject, SlicedParentTransform_);
                MakeItPhysical(lowerHullGameobject, SlicedParentTransform_);

                Destroy(gameObject);
                OnCut?.Invoke(cutPlain, RotationToVector3(transform));
            }

        }
        //else
        //    if (cut && (cutPresantage == 0 || cutPresantage == 1))
        //{
        //    cut = false;
        //    Debug.Log("<color=red>Cant cut like that</color>");
        //}
    }
    private void MakeItPhysical(GameObject obj, Transform parent)
    {
        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>().useGravity = false;
        obj.transform.parent = parent;

    }
    private Vector3 RotationToVector3(Transform T)
    {
        return new Vector3(T.eulerAngles.x, T.eulerAngles.y, T.eulerAngles.z);
    }
}
