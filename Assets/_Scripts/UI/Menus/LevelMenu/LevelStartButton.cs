using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelStartButton : MonoBehaviour
{
    [SerializeField]
    private LevelDetailsSO buttonLevel;

    private Button buttonStart;

    public static event Action<LevelDetailsSO> OnLevelStart;

    private void Awake()
    {
        buttonStart = GetComponent<Button>();
    }
    private void Start()
    {
        buttonStart.onClick.AddListener(OnClickButton_LevelStart);
    }
    private void OnClickButton_LevelStart()
    {
        OnLevelStart?.Invoke(buttonLevel);
    }
}
