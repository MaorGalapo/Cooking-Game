using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameObjs : Singelton<GameObjs>
{

    #region Effects

    [SerializeField]
    private GameObject PS_SuccessEffect;

    public GameObject GetPS_SuccessEffect() => PS_SuccessEffect;

    #endregion

    #region Touch
    [SerializeField]
    private GameObject touchStart_UI_Prefab;

    public GameObject GetTouchStart_UI_Prefab() => touchStart_UI_Prefab;

    [SerializeField]
    private GameObject touchPos_UI_Prefab;

    public GameObject GetTouchPos_UI_Prefab() => touchPos_UI_Prefab;

    #endregion

    #region UI
    [SerializeField]
    private GameObject textPopup_prefab;

    public GameObject GetTextPopupPrefab() => textPopup_prefab;

    #endregion

    #region Slice

    #region Slicing Planes
    [SerializeField]
    private GameObject intendedCutPlane_Prefab;
    public GameObject GetIntendedCutPlane() => intendedCutPlane_Prefab;


    [SerializeField]
    private GameObject cuttingPlane_Prefab;
    public GameObject GetCuttingPlanePrefab() => cuttingPlane_Prefab;
    #endregion

    [SerializeField]
    private Color[] SlicingGameTextColors;

    public Color[] GetCuttingGameTextColors() => SlicingGameTextColors;
    #endregion

    #region MiniGame

    #region BarSprites
    [SerializeField]
    private Sprite okBar_sprite, greatBar_sprite, perfectBar_sprite, playerBar_sprite, superBar_sprite;

    public Sprite GetOkBarSprite() => okBar_sprite;
    public Sprite GetGreatBarSprite() => greatBar_sprite;
    public Sprite GetPerfectBarSprite() => perfectBar_sprite;
    public Sprite GetPlayerBarSprite() => playerBar_sprite;
    public Sprite GetSuperBarSprite() => superBar_sprite;
    #endregion

    #region Text Popup Colors
    [SerializeField]
    private Color[] MiniGameTextColors;

    public Color[] GetMiniGameTextColors() => MiniGameTextColors;

    #endregion

    #endregion
}
