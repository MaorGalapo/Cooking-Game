using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlaneAccuracy : MonoBehaviour
{
    #region consts
    private const float distMulty = 90f;
    #endregion

    private IntersectionPoints topBotPoint;

    #region Grading Vars
    private float maxDistGrade = 100;
    private float maxAngleGrade = 100;
    #endregion
    private Vector3 intersectionVector;
    private Vector3 cutDir;
    private const float angleMOE = 10f, distMOE = .15f;

    public int sliceNum = 0;
    public static event Action<float, int> OnSliceAcuuracy;
    void Start()
    {
        topBotPoint = GetComponent<IntersectionPoints>();
        (Vector3, Vector3) topBotPoints = topBotPoint.GetTopBotPoints();
        intersectionVector = topBotPoint.GetIntersectionVector();
        cutDir = transform.up;//RotationToVector3(transform);
    }

    #region eventsSub
    private void OnEnable()
    {
        Slicer.OnCut += CutTest_OnCut;
    }
    private void OnDisable()
    {
        Slicer.OnCut -= CutTest_OnCut;
    }
    #endregion

#if UNITY_EDITOR  // not any use right now
    private Vector3 RotationToVector3(Transform T)
    {
        return new Vector3(T.eulerAngles.x, T.eulerAngles.y, T.eulerAngles.z);
    }
#endif
    private bool checkInMOE(float num, float moe) { return num > -moe && num < moe; } // checks if num is in negative and posetive MOE

    private bool Flip(Vector3 cutDir, Vector3 thisCutDir)
    {
        if (cutDir.x > 0 && thisCutDir.x < 0)
            return true;
        if (thisCutDir.x > 0 && cutDir.x < 0)
            return true;
        if (cutDir.z > 0 && thisCutDir.z < 0)
            return true;
        if (thisCutDir.z > 0 && cutDir.z < 0)
            return true;
        return false;
    }
    private float gradeSlice(float num, float totalGrade) { return totalGrade - num; } // checks if num is in negative and posetive MOE

    private void CutTest_OnCut(Vector3 intersectionVector, Vector3 cutDir)
    {
        if (Flip(cutDir, this.cutDir))
        {
            cutDir *= -1;
        }


        float dist = Vector3.Distance(this.intersectionVector, intersectionVector);

        float angle = Vector3.Angle(this.cutDir, cutDir);

        float angleGrade = Utils.getPersantage(gradeSlice(angle, maxAngleGrade), maxAngleGrade);
        float distGrade = Utils.getPersantage(gradeSlice(dist * distMulty, maxDistGrade), maxDistGrade);

        bool distBool = checkInMOE(dist, distMOE);
        bool angleBool = checkInMOE(angle, angleMOE);

        if (!distBool)
            distGrade -= dist * .5f;
        if (!angleBool)
            angleGrade -= angle * .5f;
        float finalGrade = distGrade * .9f + angleGrade * .1f;

#if UNITY_EDITOR
        //Debug.Log($"<color=magenta>cut plane {intersectionVector}| intended plane {this.intersectionVector} | slice number - {sliceNum}</color>");
        //Debug.Log($"<color=magenta>Dist Grade = {distGrade}/ Angle Grade = {angleGrade}/ Final Grade = {finalGrade}/ Slice Number - {sliceNum}</color>");
        //Debug.Log($"<color=magenta>Dist = {dist * 100}/ Angle = {angle}/ Slice Number - {sliceNum}</color>");
        //Debug.Log($"<Color=magenta> distance: {distBool} ,angle: {angleBool} (num-{sliceNum})</color>");
        //Debug.Log($"<color=yellow>---{sliceNum}------{sliceNum}------{sliceNum}------{sliceNum}------{Time.time}------{sliceNum}------{sliceNum}------{sliceNum}------{sliceNum}---</color>");

        ////Debug.Log($"Angle1 {this.cutDir}, Angle2 {cutDir}");
        ////Debug.Log($"Distance {dist}, Angle {angle}");
#endif
        OnSliceAcuuracy?.Invoke(finalGrade, sliceNum);

    }


}
