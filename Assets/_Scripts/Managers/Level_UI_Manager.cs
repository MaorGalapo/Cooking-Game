using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level_UI_Manager : Singelton<Level_UI_Manager>
{
    private GameObjs gameObjs;

    private Color[] colors;

    private GameObject textPopup_prefab;

    private TMP_Text totalScore_txt;
    private const string totaclScoretxt_Name = "Score_Text";

    private void Awake()
    {
        totalScore_txt = gameObject.transform.Find(totaclScoretxt_Name).GetComponent<TMP_Text>();
        totalScore_txt.GetComponent<MonoBehaviour>().enabled = false;
        gameObjs = GameObjs.Instance;
        textPopup_prefab = gameObjs.GetTextPopupPrefab();
        colors = gameObjs.GetCuttingGameTextColors();
    }

    #region Events Sub
    private void OnEnable()
    {
        SliceLevelManager.OnFinishedSlicing_Score += LevelManager_OnFinishedSlicing_Score;
        SliceLevelManager.OnFinishedSlicingLevel += LevelManager_OnFinishedSlicingLevel;
        SliceManagar.OnBestGradeSlice += SliceManagar_OnBestGradeSlice;

        BarController.OnBarClicked_UI += BarController_OnBarClicked;
        MiniGameManager.OnFinishAllMinigames += MiniGameManager_OnFinishAllMinigames;

        GameLevelManager.OnFinishedLevel += GameLevelManager_OnFinishedLevel;
        GameLevelManager.OnCountDownTimer += GameLevelManager_OnCountDownTimer;
    }


    private void OnDisable()
    {
        SliceLevelManager.OnFinishedSlicing_Score -= LevelManager_OnFinishedSlicing_Score;
        SliceLevelManager.OnFinishedSlicingLevel -= LevelManager_OnFinishedSlicingLevel;
        SliceManagar.OnBestGradeSlice -= SliceManagar_OnBestGradeSlice;

        BarController.OnBarClicked_UI -= BarController_OnBarClicked;
        MiniGameManager.OnFinishAllMinigames -= MiniGameManager_OnFinishAllMinigames;

        GameLevelManager.OnFinishedLevel -= GameLevelManager_OnFinishedLevel; 
        GameLevelManager.OnCountDownTimer -= GameLevelManager_OnCountDownTimer;
    }
    #endregion

    #region Text Methods
    private void PopupTextInstantiate(float time, string massage, Color color, float size, Vector2 pos, float rotation = 0)
    {
        Text_Popup textObj = Text_Popup.Create(time, massage, color, size, textPopup_prefab, pos, transform);

        textObj.Rotate(rotation);

        textObj.SetFade();
        textObj.SetMoveUp();
    }
    private void FinalGradeDisplay(float finalGrade)
    {
        totalScore_txt.GetComponent<MonoBehaviour>().enabled = true;
        totalScore_txt.text = finalGrade.ToString();
    }

    #endregion

    #region GameMaster Events
    private void GameLevelManager_OnFinishedLevel(float finalGrade)
    {
        FinalGradeDisplay(finalGrade);
    }
    private void GameLevelManager_OnCountDownTimer(string obj)
    {
        PopupTextInstantiate(3, obj, Color.white, 320, Vector2.zero);
    }
    #endregion

    #region MiniGame Events
    private void BarController_OnBarClicked(string massage, Color massageColor)
    {
        PopupTextInstantiate(3, massage, massageColor, 240, Vector2.zero);
    }
    private void MiniGameManager_OnFinishAllMinigames(float grade)
    {
        PopupTextInstantiate(3, grade.ToString(), Color.white, 240, Vector2.zero); ;
    }
    #endregion

    #region Slicing Events

    private void LevelManager_OnFinishedSlicingLevel(float finalGrade)
    {
        PopupTextInstantiate(5, $"{finalGrade}%", Color.white, 230, Vector2.zero);
    }
    private void LevelManager_OnFinishedSlicing_Score(float grade)
    {
        PopupTextInstantiate(4, GetMassageByGrade(grade), GetColorByGrade(grade), 240, Vector2.zero);
    }
    private void SliceManagar_OnBestGradeSlice(float grade)
    {
        PopupTextInstantiate(3, $"{grade:F0}%", GetColorByGrade(grade), 150, GetRandPos(), GetRandRotation());
    }


    private string GetMassageByGrade(float grade)
    {
        if (grade > 97)
            return "Perfection!";
        else if (grade > 94)
            return "Awesome!";
        else if (grade > 89)
            return "Nice";
        else if (grade > 85)
            return "Cool";
        else return "Failure..";
    }
    private Color GetColorByGrade(float grade)
    {
        if (grade > 97)
            return colors[0];
        else if (grade > 94)
            return colors[1];
        else if (grade > 89)
            return colors[2];
        else if (grade > 85)
            return colors[3];
        else return colors[4];
    }
    #endregion

    #region Random Methods
    private Vector2 GetRandPos()
    {
        float x = Random.Range(-350, 350), y = Random.Range(-600, 400);
        return new Vector2(x, y);
    }
    private float GetRandRotation()
    {
        return Random.Range(-25, 25);
    }
    #endregion

}
