using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Slice Level", menuName = "Sliceable/SliceableLevel")]
public class SliceLevel : ScriptableObject
{
    public SliceableObject[] Sliceable_Objs;
    public float levelPassingGrade;
    public float levelTime;
}
