using UnityEngine.UI;
using UnityEngine;

public class LineAnimation : MonoBehaviour
{
    private Material line_Mat;
    private LineRenderer lineRenderer;

    private (Vector3, Vector3) topBotPoint;
    protected GameInputManager gameInputManager;


    #region EventSub
    private void OnEnable()
    {
        gameInputManager.OnEndTouch += GameInputManager_OnEndTouch;
    }

    private void OnDisable()
    {
        gameInputManager.OnEndTouch -= GameInputManager_OnEndTouch;
    }
    #endregion

    void Awake()
    {
        gameInputManager = GameInputManager.Instance;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = false;
    }

    public void SetTopPoint(Vector3 topPoint) => this.topBotPoint.Item1 = topPoint;
    private void SetBotPoint(Vector3 botPoint) => this.topBotPoint.Item2 = botPoint;

    public void SetEnabled(bool enabled) => lineRenderer.enabled = enabled;

    public void SetTopBotPoint(Vector3 botPoint, Vector3 topPoint)
    {
        SetBotPoint(botPoint);
        SetTopPoint(topPoint);
    }
    void Update()
    {
        lineRenderer.SetPosition(0, this.topBotPoint.Item2);
        lineRenderer.SetPosition(1, this.topBotPoint.Item1);
    }
    private void GameInputManager_OnEndTouch(Vector3 position, float time)
    {
        Destroy(gameObject);
    }
}
