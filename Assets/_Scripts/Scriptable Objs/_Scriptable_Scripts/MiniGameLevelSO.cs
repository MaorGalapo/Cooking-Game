using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new MiniGame Level", menuName = "MiniGame/MiniGameLevel")]
public class MiniGameLevelSO : ScriptableObject
{
    public MiniGameLevel[] miniGames;
}

[System.Serializable]
public class MiniGameLevel
{
    public string name;
    public float miniGamePassingGrade;
    public float levelTime;
    public BarMiniGameSubLevel[] barLevels;
    public RingMiniGameSubLevel[] ringLevels;
}

[System.Serializable]
public class BarMiniGameSubLevel
{
    public float speed;
    public float[] barSizes;
    public float[] passingGrades;
}

[System.Serializable]
public class RingMiniGameSubLevel
{
    public float speedRingScale;
    public float speedRingTap;
    public float[] ringSizes;
    public float[] passingGrades;
}