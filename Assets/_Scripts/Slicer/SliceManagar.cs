using System.Collections;
using UnityEngine;
using System;

public class SliceManagar : MonoBehaviour
{
    #region Consts
    private const string slicedTag = "Sliced";
    private const string sliceableTag = "Sliceable";
    #endregion 

    private GameObjs gameObjs;

    public static event Action<float, bool, GameObject[]> OnFinishedSlicing;
    public static event Action<float> OnBestGradeSlice;

    private bool finishedSlicing = false;
    private bool levelFinished = false;

    private GameObject[] slicesArr;

    #region slice related vars
    private int currSlices; // current number of slices
    private int maxSlicesNum; // maximum number of slices player can make
    private int slicesNum; // number of slices to spawn
    private int highestIndexSlice;
    #endregion

    #region Slice Grade
    private float finalGrade;
    private float passingGrade;
    private (float, int)[] grades;
    private bool gradeAdded = false;
    private bool perfectSlice = false;
    public float DeducePrestanatge { get; set; } // between 0-1
    #endregion

    private GameObject slicePrefab;
    private GameObject sliceablePrefab;

    public void SetStartLevelVars()
    {
        this.finishedSlicing = false;
        this.levelFinished = false;
        gradeAdded = false;
        slicesArr = new GameObject[0];
        ResetCurrSlicesNum();
        ResetMaxSlicesNum();
        ResetFinalGrade();
    }
    private void Awake()
    {
        gameObjs = GameObjs.Instance;
        slicePrefab = gameObjs.GetIntendedCutPlane();
    }

    #region Event Sub
    private void OnEnable()
    {
        PlaneAccuracy.OnSliceAcuuracy += PlaneAccuracy_OnSliceAcuuracy;
        SlicePlaneController.OnSliceInstantiate += SlicePlaneController_OnSliceInstantiate;
    }


    private void OnDisable()
    {
        SlicePlaneController.OnSliceInstantiate -= SlicePlaneController_OnSliceInstantiate;
        PlaneAccuracy.OnSliceAcuuracy -= PlaneAccuracy_OnSliceAcuuracy;
    }
    #endregion

    #region Event Methods
    private void SlicePlaneController_OnSliceInstantiate()
    {
        IncCurrSlicesNum();
        gradeAdded = false;
    }
    private void PlaneAccuracy_OnSliceAcuuracy(float finalGrade, int sliceIndex)
    {
        grades[sliceIndex] = (finalGrade, sliceIndex);
        if (sliceIndex == GetHighestIndexSlice() && !gradeAdded)
        {
            ArrangeGrades();
            BestGradeSlice();
            SetHighestIndexSlice();
        }
    }
    #endregion

    #region Array Sets and Checks
    private int GetHighestIndexSlice() => this.highestIndexSlice;
    private void SetHighestIndexSlice()
    {
        int h = 0;
        for (int i = 0; i < slicesNum; i++)
            if (slicesArr[i].gameObject.activeSelf && i > h)
                h = i;
        this.highestIndexSlice = h;
    }
    private bool CheckSlicesLeft() // returns true if there are slices left and false if not
    {
        for (int i = 0; i < slicesNum; i++)
            if (slicesArr[i].gameObject.activeSelf)
                return true;
        return false;
    }
    #endregion

    private void DestroySlicesPrefabs()
    {
        if (slicesArr == null)
            throw new ArgumentNullException();
        for (int i = 0; i < slicesArr.Length; i++)
            Destroy(slicesArr[i]);
    }
    private void UnFreeze()
    {
        if (finishedSlicing)
        {
            CalcFinalGrade();

            DeducePointsForActiveSlices();

            DestroySlicesPrefabs();

            var myObjects = GameObject.FindGameObjectsWithTag(sliceableTag);
            for (int i = 0; i < myObjects.Length; i++)
            {
                myObjects[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                myObjects[i].gameObject.layer = 0;
            }
            StartCoroutine(WaitForFixedUpdateInvoke());
            finishedSlicing = false;
        }
    }
    private (float, float, float, float) GetMinMax(Transform obj)
    {
        float minX = obj.position.x - (obj.localScale.x * .5f), maxX = obj.localScale.x * .5f + obj.position.x;
        float minY = obj.position.z - (obj.localScale.y * .5f), maxY = obj.localScale.y * .5f + obj.position.z;
        return (minX, maxX, minY, maxY);
    }

    #region Grade Methods
    private void ResetFinalGrade() => this.finalGrade = 0;
    public void SetPassingGrade(float num) => this.passingGrade = num;
    public float GetPassingGrade() => this.passingGrade;

    private void BestGradeSlice() // add first grade (after sort is the highest) to final grade unless slice is inactive 
    {
        bool isObjectActive = false;
        for (int i = 0; i < slicesArr.Length && !isObjectActive; i++)
        {
            isObjectActive = slicesArr[grades[i].Item2].gameObject.activeSelf;
            if (isObjectActive)
            {
                if (grades[i].Item1 >= GetPassingGrade())
                {
                    slicesArr[grades[i].Item2].gameObject.SetActive(false);
                }
                finalGrade += grades[i].Item1;
                gradeAdded = true;
                OnBestGradeSlice?.Invoke(grades[i].Item1);
            }
        }
    }

    private void DeducePointsForActiveSlices()
    {
        DeducePrestanatge = .45f; // TODO change that each level changes this for difficulty changes
        bool isObjectActive;
        float startingFinalGrade = this.finalGrade;
        float pointsToDeduce = startingFinalGrade / GetSlicesNum() * DeducePrestanatge;
        for (int i = 0; i < slicesArr.Length; i++)
        {
            isObjectActive = slicesArr[grades[i].Item2].gameObject.activeSelf;
            if (isObjectActive)
            {
                slicesArr[grades[i].Item2].gameObject.SetActive(false);
                this.finalGrade -= pointsToDeduce;
            }
        }
    }

    private void SetGradesArr(int numOfSlices)
    {
        this.grades = new (float, int)[numOfSlices];
    }

    private void ArrangeGrades()
    {
        Utils.QuickSortLowToHigh(grades);
    }

    private void CalcFinalGrade()
    {
        finalGrade /= GetCurrSlicesNum();
        if (perfectSlice && finalGrade + 7 >= GetPassingGrade())
            finalGrade += 7;
        else
            perfectSlice = false;
    }

    #endregion

    #region Generate Slices
    private void SetHighestIndex()
    {
        SetHighestIndexSlice();
        perfectSlice = false;
    }
    private void SetSlicesVars(int slicesNum, int extraSlices)
    {
        levelFinished = false;
        SetSlicesNum(slicesNum);
        SetMaxSlicesNum(GetSlicesNum() + extraSlices);
        SetGradesArr(GetSlicesNum());
        ResetCurrSlicesNum();
        ResetFinalGrade();
    }

    private void GenerateSlicesPosRand(GameObject sliceAbleObj, int slicesNum)
    {
        GameObject sliceable = sliceAbleObj;
        slicesArr = new GameObject[slicesNum];
        for (int i = 0; i < slicesArr.Length; i++)
        {
            slicesArr[i] = Instantiate(slicePrefab, Vector3.zero, Quaternion.identity * Quaternion.Euler(new Vector3(90, 0, UnityEngine.Random.Range(0, 360))));
            (float, float, float, float) minMaxX_MinMaxZ = GetMinMax(sliceable.transform);
            float randX = UnityEngine.Random.Range(minMaxX_MinMaxZ.Item1, minMaxX_MinMaxZ.Item2);
            float randZ = UnityEngine.Random.Range(minMaxX_MinMaxZ.Item3, minMaxX_MinMaxZ.Item4);
            float avarage = (randX + randZ) / 2.5f;
            slicesArr[i].transform.position += new Vector3(avarage, 0, avarage);
            slicesArr[i].GetComponent<PlaneAccuracy>().sliceNum = i;

        }
    }
    private void GenerateSlicesPos(int slicesNum, (Vector3, Vector3)[] positions)
    {
        slicesArr = new GameObject[slicesNum];

        for (int i = 0; i < slicesArr.Length; i++)
        {
            slicesArr[i] = Instantiate(slicePrefab, Vector3.zero, Quaternion.identity * Quaternion.Euler(positions[i].Item2));
            slicesArr[i].transform.position += positions[i].Item1;
            slicesArr[i].GetComponent<PlaneAccuracy>().sliceNum = i;
        }
    }

    public void GenerateSlicesRand(int minSlices, int maxSlices, int extraSlices, GameObject spawndSliceable)
    {
        if (minSlices >= maxSlices)
            maxSlices = minSlices + 1;

        SetSlicesVars(UnityEngine.Random.Range(minSlices, maxSlices), extraSlices);

        GenerateSlicesPosRand(spawndSliceable, GetSlicesNum());

        SetHighestIndex();
    }
    public void GenerateSlices((Vector3, Vector3)[] positions, int extraSlices)
    {
        SetSlicesVars(positions.Length, extraSlices);

        GenerateSlicesPos(GetSlicesNum(), positions);

        SetHighestIndex();
    }
    #endregion

    #region Public Methods (with SetPassingGrade/GenerateSlices/GenerateSlicesRand as exeption)

    #region get/set/reset/inc/dec/comapre  for setMaxSlicesNum, slicesNum and currSlicesNum

    public void SetMaxSlicesNum(int maxSlicesNum) => this.maxSlicesNum = maxSlicesNum;
    public int GetMaxSlicesNum() => this.maxSlicesNum;
    public void IncMaxSlicesNum() => this.maxSlicesNum++;
    public void DecMaxSlicesNum() => this.maxSlicesNum++;
    public void ResetMaxSlicesNum() => SetMaxSlicesNum(0);

    public void SetSlicesNum(int slicesNum) => this.slicesNum = slicesNum;
    public int GetSlicesNum() => this.slicesNum;
    public void IncSlicesNum() => this.slicesNum++;
    public void DecSlicesNum() => this.slicesNum++;
    public void ResetSlicesNum() => SetSlicesNum(0);

    public void SetCurrSlicesNum(int currSlices) => this.currSlices = currSlices;
    public void IncCurrSlicesNum() => this.currSlices++;
    public void DecCurrSlicesNum() => this.currSlices++;
    public int GetCurrSlicesNum() => this.currSlices;
    public void ResetCurrSlicesNum() => SetCurrSlicesNum(0);

    public bool CompareCurrSlicesAndMaxSlices() => GetMaxSlicesNum() == GetCurrSlicesNum();
    public bool CompareCurrSlicesAndSlices() => GetSlicesNum() == GetCurrSlicesNum();
    #endregion

    #region Sliceable Prefab
    public void SetSliceablePrefab(GameObject prefab) => this.sliceablePrefab = prefab;
    public GameObject GetSliceablePrefab() => this.sliceablePrefab;
    public GameObject InstantiateSliceableObj(GameObject sliceablePrefab, Vector3 position) => Instantiate(sliceablePrefab, position, Quaternion.identity);
    #endregion

    #endregion

    #region Delay Methods
    IEnumerator WaitForFixedUpdateInvoke()
    {
        yield return new WaitForFixedUpdate();
        var myObjects = GameObject.FindGameObjectsWithTag(sliceableTag);
        for (int i = 0; i < myObjects.Length; i++)
        {
            myObjects[i].tag = slicedTag;
        }
        OnFinishedSlicing?.Invoke(finalGrade, perfectSlice, myObjects);
    }
    IEnumerator WaitForFixedUpdateUnFreeze()
    {
        yield return new WaitForFixedUpdate();
        finishedSlicing = true;
        UnFreeze();
        ResetSlicesNum();
    }
    #endregion
    void Update()
    {
        if (CompareCurrSlicesAndMaxSlices() || !CheckSlicesLeft())
        {
            if (!levelFinished)
            {
                if (CompareCurrSlicesAndSlices())
                {
                    perfectSlice = true;
                }
                StartCoroutine(WaitForFixedUpdateUnFreeze());
                levelFinished = true;
            }
        }

    }
}
