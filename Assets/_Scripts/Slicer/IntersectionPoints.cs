using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionPoints : MonoBehaviour
{
    private const string SliceableTag = "Sliceable";

    [SerializeField]
    private Transform topPoint;
    [SerializeField]
    private Transform botPoint;

    private (Vector3, Vector3) GetIntersectionPoints()
    {
        Vector3 topIntersectionPoint = new Vector3(-100, -100, -100);
        Vector3 botIntersectionPoint = new Vector3(-100, -100, -100);

        Vector3 topPosition = topPoint.position;
        Vector3 botPosition = botPoint.position;

        Ray topRay = new(topPosition, botPosition - topPosition);
        Ray botRay = new(botPosition, topPosition - botPosition);

        RaycastHit[] hits;

        hits = Physics.RaycastAll(topRay, Vector3.Distance(topPosition, botPosition));

        if (hits != null)
        {
            ArrangeHitsByDistance(hits);
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.transform.CompareTag(SliceableTag))
                {
                    topIntersectionPoint = raycastHit.point;
                    break;
                }
            }
        }

        hits = Physics.RaycastAll(botRay, Vector3.Distance(topPosition, botPosition));

        if (hits != null)
        {
            ArrangeHitsByDistance(hits);
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.transform.CompareTag(SliceableTag))
                {
                    botIntersectionPoint  = raycastHit.point;
                    break;
                }
            }
        }
#if UNITY_EDITOR
        Debug.DrawRay(topPosition, (botPosition - topPosition) * .5f, Color.cyan, 10);
        Debug.DrawRay(botPosition, (topPosition - botPosition) * .5f, Color.magenta, 10);
        topPoint.position = topIntersectionPoint;
        botPoint.position = botIntersectionPoint;
#endif
        return (topIntersectionPoint, botIntersectionPoint);
    }
    private void ArrangeHitsByDistance(RaycastHit[] hits)
    {
        RaycastHit[] tempHits = hits;
        (float, int)[] distanceIndexArray = new (float, int)[hits.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            distanceIndexArray[i].Item1 = hits[i].distance;
            distanceIndexArray[i].Item2 = i;
        }

        Utils.QuickSortHighToLow(distanceIndexArray);

        for (int i = 0; i < distanceIndexArray.Length; i++)
        {
            hits[i] = tempHits[distanceIndexArray[i].Item2];
        }
    }
    public Vector3 GetIntersectionVector()
    {
        (Vector3, Vector3) topBotpoint = GetIntersectionPoints();
        Vector3 topPoint = topBotpoint.Item1, botPoint = topBotpoint.Item2;
        return new Vector3(Mathf.Abs(topPoint.x) - Mathf.Abs(botPoint.x), Mathf.Abs(topPoint.y) - Mathf.Abs(botPoint.y), Mathf.Abs(topPoint.z) - Mathf.Abs(botPoint.z));
    }

    public (Vector3, Vector3) GetTopBotPoints()
    {
        return (topPoint.position, botPoint.position);
    }

}
