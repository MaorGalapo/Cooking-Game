using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Level", menuName = "Levels/NewLevel")]
public class LevelDetailsSO : ScriptableObject
{
    public ScriptableObject[] LevelParts;
}
