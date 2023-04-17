
using UnityEngine;

public class RotateRectTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 RotartionDir;

    RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.Rotate(RotartionDir);
    }
}
