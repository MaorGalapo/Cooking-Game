using UnityEngine;

public class Utils : MonoBehaviour
{
    public const float yDiff = .25f; // y diffirence for touch controlls

    public static float getPersantage(float num, float total) => num * 100 / total; // get persantage of num from total number
    public static Vector3 ScreenToWorld3D(Camera camera, Vector3 position) // get touch position in world position
    {
        Ray ray = camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector3 vec = raycastHit.point;
            vec.y = -yDiff;
            return vec;
        }
        return Vector3.zero;
    }

    #region Cos Sin
    public static float getSin(float angle) => Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (angle))); // get sin of eular deg

    public static float getCos(float angle) => Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (angle)));// get cos of eular deg
    #endregion

    #region Quick Sort

    #region low to high

    public static void QuickSortLowToHigh((float, int)[] array) // wrapper method for quick sort
    {
        QuickSortLowToHigh(array, 0, array.Length - 1);
    }
    private static void QuickSortLowToHigh((float, int)[] arr, int left, int right) 
    {
        int pivot = PetitionLowToHigh(arr, left, right);
        if (left < right)
        {
            if (pivot > 1)
                QuickSortLowToHigh(arr, left, pivot - 1);
            if (pivot + 1 < right)
                QuickSortLowToHigh(arr, pivot + 1, right);
        }
    }
    private static int PetitionLowToHigh((float, int)[] arr, int left, int right) 
    {
        if (arr.Length == left)
            return right;
        float pivot = arr[left].Item1;
        while (true)
        {
            while (pivot < arr[left].Item1)
                left++;
            while (pivot > arr[right].Item1)
                right--;
            if (right > left)
            {
                if (arr[left] == arr[right]) return right;
                (arr[left], arr[right]) = (arr[right], arr[left]);
            }
            else
                return right;
        }
    }
    #endregion

    #region high to low
    public static void QuickSortHighToLow((float, int)[] array) // wrapper method for quick sort
    {
        QuickSortHighToLow(array, 0, array.Length - 1);
    }
    private static void QuickSortHighToLow((float, int)[] arr, int left, int right)
    {
        int pivot = PetitionHighToLow(arr, left, right);
        if (left < right)
        {
            if (pivot > 1)
                QuickSortHighToLow(arr, left, pivot - 1);
            if (pivot + 1 < right)
                QuickSortHighToLow(arr, pivot + 1, right);
        }
    }
    private static int PetitionHighToLow((float, int)[] arr, int left, int right)
    {
        if (arr.Length == left)
            return right;
        float pivot = arr[left].Item1;
        while (true)
        {
            while (pivot > arr[left].Item1)
                left++;
            while (pivot < arr[right].Item1)
                right--;
            if (right > left)
            {
                if (arr[left] == arr[right]) return right;
                (arr[left], arr[right]) = (arr[right], arr[left]);
            }
            else
                return right;
        }
    }
    #endregion

    #endregion

}
