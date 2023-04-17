using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Text_Popup : MonoBehaviour
{
    private TMP_Text text;

    private Action effectMethod;
    private Transform transform_;

    private const float moveSpeed = 235f;
    private const float fadeSpeed = 1.25f;

    public static Text_Popup Create(float time,string massage, Color color, float size, GameObject textPrefab, Vector2 pos, Transform parent = null)
    {
        GameObject textPopup = Instantiate(textPrefab);

        Text_Popup textPopupComponent = textPopup.GetComponent<Text_Popup>();
        textPopupComponent.SetUpMassage(massage, color, size);

        textPopupComponent.SetUpPos(parent, pos);

        textPopupComponent.SetUpDestroyTimer(time);

        return textPopupComponent;
    }

    #region Awake and Start
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        transform_ = transform;

    }
    #endregion

    #region Set Ups
    private void SetUpMassage(string textMassage, Color textColor, float size = 260)
    {
        text.fontSize = size;
        text.color = textColor;
        text.outlineColor = textColor - new Color(50,50,50);
        text.text = textMassage;
    }

    private void SetUpPos(Transform parent, Vector2 pos)
    {
        Transform t = transform;
        if (parent != null)
            t.SetParent(parent);
        t.localPosition = pos;
    }

    private void SetUpDestroyTimer(float time)
    {
        StartCoroutine(DestroyTimer(time));
    }

    public void SetMoveUp()
    {
        effectMethod += MoveUp;
    }

    public void SetFade()
    {
        effectMethod += Fade;
    }

    public void Rotate(float r)
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,r);
    }
    #endregion

    #region Effect Methods

    private void MoveUp()
    {
        transform_.position += new Vector3(0, moveSpeed) * Time.deltaTime;
    }

    private void Fade()
    {
        text.alpha-= fadeSpeed * Time.deltaTime;
    }

    #endregion

    private IEnumerator DestroyTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        effectMethod = null;
    }
    private void Update()
    {
        effectMethod();
    }

}
