using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SliceLevelManager : Singelton<SliceLevelManager>
{
    private SliceLevel currLevel;
    private CameraController camController;

    [SerializeField]
    private GameObject sliceLineRendererCamera;

    #region Events
    public static event Action<float> OnFinishedSlicingLevel_GameMaster;
    public static event Action<float> OnFinishedSlicingLevel;
    public static event Action<float> OnFinishedSlicing_Score;
    public static event Action<GameObject> OnFinishedSlicing_GoParent;
    public static event Action OnFinishedMoving;
    #endregion

    private readonly Vector3 spawnPoint = new Vector3(10, 0, 0);

    private SliceManagar sliceManager;

    #region slicedObjs
    private List<GameObject>[] slicedObjs;
    private List<GameObject> slicedObjsParents = new List<GameObject>();
    #endregion

    #region Grading
    private float levelPassingGrade;
    private int currentSliceable = 0;
    private float finalGrade;
    #endregion


    public void SetSliceLevel(SliceLevel currLevel) => this.currLevel = currLevel;


    #region Events Sub
    private void OnEnable()
    {
        SliceManagar.OnFinishedSlicing += SliceManagar_OnFinishedSlicing;
    }


    private void OnDisable()
    {
        SliceManagar.OnFinishedSlicing -= SliceManagar_OnFinishedSlicing;
    }

    #endregion

    #region Level Finish Methods
    private void FinishedSlicingLevel()
    {
        //OnFinishedSlicingLevel_GameMaster(finalGrade);
        StartCoroutine(WaitForSecondsFinishLevel(.8f));
    }

    private void SliceManagar_OnFinishedSlicing(float finalGrade, bool perfect, GameObject[] slicedObj)
    {
        if (currentSliceable > 0)
        {
            OnFinishedSlicing_Score?.Invoke(finalGrade);
            this.finalGrade += finalGrade;
            this.slicedObjs[currentSliceable - 1] = ConvertArrayToList(slicedObj);
            StartCoroutine(WaitForSecondsFreeze(3));
            StartCoroutine(WaitForSecondsSpawn(3));
        }

    }
    #endregion

    #region Move Methods
    private void FreezeAndMove(List<GameObject> slicedObjsList)
    {
        for (int i = 0; i < slicedObjsList.Count; i++)
        {
            Rigidbody currRb = slicedObjsList[i].GetComponent<Rigidbody>();
            currRb.useGravity = false;
            currRb.constraints = RigidbodyConstraints.FreezeAll;
            currRb.gameObject.transform.parent = slicedObjsParents[currentSliceable - 1].transform;
        }
        StartCoroutine(MoveSliceable(slicedObjsParents[currentSliceable - 1], spawnPoint, .8f));
        OnFinishedSlicing_GoParent?.Invoke(slicedObjsParents[currentSliceable - 1]);
    }

    public void GMStopAllCoroutines()
    {
        StopAllCoroutines();
    }

    IEnumerator MoveSliceable(GameObject sliceable, Vector3 endPoint, float speed)
    {
        if (sliceable == null)
            yield return null;
        Transform sliceTransform = sliceable.transform;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            sliceTransform.position = Vector3.Lerp(sliceTransform.position, endPoint, Time.deltaTime * speed);
            if (Vector3.Distance(sliceTransform.position, endPoint) < 0.05f) //TODO check if object is not in cameras view
                break;
        }
        OnFinishedMoving?.Invoke();
        GameLevelTimerManager.SetPauseLevelTimer(false);
        yield return new WaitForSeconds(1f);
    }
    #endregion

    #region Delay Methods
    IEnumerator WaitForFrameEnableCamera()
    {

        yield return new WaitForEndOfFrame();
        sliceLineRendererCamera.SetActive(true);
    }
    IEnumerator WaitForSecondsFinishLevel(float sec)
    {
        sliceLineRendererCamera.SetActive(false);
        float finalGrade = this.finalGrade / currentSliceable - 1;
        OnFinishedSlicingLevel?.Invoke(finalGrade);
        GameLevelTimerManager.EndLevelTimer();
        yield return new WaitForSeconds(sec);
        OnFinishedSlicingLevel_GameMaster(finalGrade);

    }
    IEnumerator WaitForSecondsFreeze(float sec)
    {
        slicedObjsParents.Add(CreateParent());

        GameLevelTimerManager.SetPauseLevelTimer(true);
        yield return new WaitForSeconds(sec);
        FreezeAndMove(this.slicedObjs[currentSliceable - 1]);
    }
    IEnumerator WaitForSecondsSpawn(float sec)
    {
        GameLevelTimerManager.SetPauseLevelTimer(true);
        yield return new WaitForSeconds(sec);
        GameObject spawndSliceable = SpawnSliceable(currentSliceable, Vector3.zero);
        if (spawndSliceable != null)
        {
            spawndSliceable.transform.position = -spawnPoint;
            StartCoroutine(MoveSliceable(spawndSliceable, Vector3.zero, .8f));
        }
        currentSliceable++;
    }
    IEnumerator DelayFirstLevelPartTimer()
    {
        yield return new WaitForSeconds(4);
        GameLevelTimerManager.StartLevelTimer(currLevel.levelTime, FinishedSlicingLevel);
    }
    IEnumerator DelayFrameStartTimer()
    {
        yield return new WaitForEndOfFrame();
        GameLevelTimerManager.StartLevelTimer(currLevel.levelTime, FinishedSlicingLevel);
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        camController = CameraController.Instance;
        sliceManager = GetComponent<SliceManagar>();
    }

    public void StartSliceLevel()
    {
        StartCoroutine(WaitForFrameEnableCamera());
        finalGrade = 0;
        currentSliceable = 0;
        slicedObjsParents.RemoveRange(0, slicedObjsParents.Count);
        sliceManager.SetStartLevelVars();


        camController.SetSlicingPosition();
        InitialiseSliceLevel();

        if (currLevel.levelTime == 0)
            throw new Exception("Timer is set to nothing");
        if (GameLevelManager.Instance.GetIsFirstLevelPart())
        {
            StartCoroutine(DelayFirstLevelPartTimer());
        }
        else
        {
            GameLevelTimerManager.EndLevelTimer();
            StartCoroutine(DelayFrameStartTimer());
        }
    }

    #endregion

    #region Spawning Methods
    private void InitialiseSliceLevel()
    {
        slicedObjs = new List<GameObject>[currLevel.Sliceable_Objs.Length];
        SpawnFirstSliceable();
    }
    private void SpawnFirstSliceable()
    {
        levelPassingGrade = currLevel.levelPassingGrade;
        // SpawnSliceable(currentSliceable, Vector3.zero);
        GameObject spawndSliceable = SpawnSliceable(currentSliceable, Vector3.zero);
        if (spawndSliceable != null)
        {
            spawndSliceable.transform.position = -spawnPoint;
            StartCoroutine(MoveSliceable(spawndSliceable, Vector3.zero, .8f));
        }
        currentSliceable++;
    }
    private GameObject SpawnSliceable(int currSliceable, Vector3 spawnPos)
    {
        if (currentSliceable >= currLevel.Sliceable_Objs.Length)
        {
            FinishedSlicingLevel();
            return null;
        }

        SliceableObject currObj = currLevel.Sliceable_Objs[currSliceable];
        sliceManager.SetSliceablePrefab(currObj.sliceablePrefab);
        sliceManager.SetPassingGrade(currObj.objPassingGrade);
        GameObject sliceableSpawned = sliceManager.InstantiateSliceableObj(sliceManager.GetSliceablePrefab(), spawnPos);

        if (currObj.numOfRandSlices != 0)
        {
            sliceManager.GenerateSlicesRand(currObj.numOfRandSlices, currObj.numOfRandSlices, currObj.extraSlices, sliceableSpawned);
        }
        else
        {
            (Vector3, Vector3)[] positions = ConvertSliceablePositionToArray(currObj.slicingPositions);
            sliceManager.GenerateSlices(positions, currObj.extraSlices);
        }
        return sliceableSpawned;
    }
    #endregion

    #region Helper Methods
    private GameObject CreateParent()
    {
        GameObject parent = new GameObject($"Sliceable Number {currentSliceable - 1} ");
        parent.transform.position = Vector3.zero;
        parent.transform.rotation = Quaternion.identity;
        return parent;
    }
    private (Vector3, Vector3)[] ConvertSliceablePositionToArray(SliceablePosition[] positions)
    {
        (Vector3, Vector3)[] newPositions = new (Vector3, Vector3)[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            newPositions[i].Item1 = positions[i].position;
            newPositions[i].Item2 = positions[i].rotation;
        }
        return newPositions;
    }
    private List<GameObject> ConvertArrayToList(GameObject[] arr)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < arr.Length; i++)
            list.Add(arr[i]);
        return list;
    }
    #endregion

}
