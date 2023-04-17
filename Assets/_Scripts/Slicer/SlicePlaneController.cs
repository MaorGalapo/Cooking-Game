using UnityEngine;
using System.Collections.Generic;
using System;

public class SlicePlaneController : MonoBehaviour
{
    private GameObjs gameObjs;

    public static event Action<bool> OnSliceInstantiateSliceAngle;
    public static event Action OnSliceInstantiate;
    private const int SliceableLayer = 6;

    private GameObject cuttingPlane_Prefab;

    private Vector3 startPos = Vector3.zero, endPos = Vector3.zero;

    private Dictionary<(bool, bool), int> CoorAgnlePair = new Dictionary<(bool, bool), int>()
    {
        {(true,true),-1}, // x pos z pos
        {(true,false),-1}, // x pos z neg
        {(false,true),1}, // x neg z pos
        {(false,false),1} // x neg z neg
    };

    private void Awake()
    {
        gameObjs = GameObjs.Instance;
        cuttingPlane_Prefab = gameObjs.GetCuttingPlanePrefab();
    }

    private void OnEnable()
    {
        SwipeDetection.OnSwipePos += SwipeDetection_OnSwipePos;
    }
    private void OnDisable()
    {
        SwipeDetection.OnSwipePos -= SwipeDetection_OnSwipePos;
    }


    private GameObject CreateSlicePlane(Vector3 pos, float angle)
    {
        OnSliceInstantiate?.Invoke();
        GameObject plane = Instantiate(cuttingPlane_Prefab, pos, Quaternion.identity);
        plane.transform.Rotate(new Vector3(90, 0, angle));
        return plane;
    }
    private (Vector3, Vector3) GetTopBotPoint(float angle, float opp, float adj, Transform planeTrans)
    {
        float hypotSize = planeTrans.localScale.x;
        if (angle < 0)
            adj *= -1;
        Vector3 TopPoint = new(planeTrans.position.x - opp* hypotSize, 0, planeTrans.position.z - adj* hypotSize);
        Vector3 BotPoint = new(planeTrans.position.x + opp* hypotSize, 0, planeTrans.position.z + adj* hypotSize);
        return (BotPoint, TopPoint);
    }
    private void SwipeDetection_OnSwipePos(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;

        Ray ray = new Ray(startPos, endPos - startPos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Vector3.Distance(startPos, endPos));
        if (hits != null)
        {
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.transform.gameObject.layer == SliceableLayer)
                {
                    Vector3 vecHitPoint = raycastHit.point;
                    float vecHitAngle = 90 - Vector3.Angle(endPos - startPos, Vector3.forward);

                    SwipeDetectedSlice(vecHitPoint, vecHitAngle);
                    break;
                }
            }
        }
    }

    private void SwipeDetectedSlice(Vector3 pos, float angle)
    {

        angle *= CoorAgnlePair[(startPos.x > endPos.x, startPos.z > endPos.z)]; // setting angle to be positive or negative depending on swipe direction(from pos to neg ect...)

        pos.y += Utils.yDiff;

        OnSliceInstantiateSliceAngle?.Invoke(Mathf.Abs(angle) > 80 && Mathf.Abs(angle) < 110); // for determaning slice direction

        GameObject plane = CreateSlicePlane(pos, angle);

        float opp = Utils.getSin(90 - Mathf.Abs(angle));
        float adj = Utils.getCos(90 - Mathf.Abs(angle));

        var points = GetTopBotPoint(angle, opp, adj, plane.transform);
        Vector3 BotPoint = points.Item1, TopPoint = points.Item2;

        //Debug.Log($"| Opp {opp} | ADJ {adj} | BotPoint {BotPoint} | Top Point {TopPoint} | Point Before Change {plane.transform.position} |");

        Debug.DrawLine(BotPoint, TopPoint, Color.blue, 10);
        if (CoorAgnlePair[(startPos.x > endPos.x, startPos.z > endPos.z)] > 0)
            plane.transform.position = plane.transform.position - (TopPoint - plane.transform.position) / 2;
        else
            plane.transform.position = plane.transform.position - (BotPoint - plane.transform.position) / 2;
    }


}
