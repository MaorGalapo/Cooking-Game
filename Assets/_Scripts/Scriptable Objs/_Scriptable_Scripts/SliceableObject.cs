using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Sliceable Object", menuName = "Sliceable/SliceableObject")]
public class SliceableObject : ScriptableObject
{
    public GameObject sliceablePrefab;
    public SliceablePosition[] slicingPositions;
    public float objPassingGrade;
    public int extraSlices;
    public int numOfRandSlices;
}

[System.Serializable]
public class SliceablePosition
{
    public Vector3 position;
    public Vector3 rotation;
}
