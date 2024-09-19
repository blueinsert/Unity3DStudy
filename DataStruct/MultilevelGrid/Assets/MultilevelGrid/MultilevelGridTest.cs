using bluebean;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultilevelGridTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"sizeofLevel 0 {NativeMultilevelGrid<int>.CellSizeOfLevel(0)}");
        Debug.Log($"sizeofLevel 1 {NativeMultilevelGrid<int>.CellSizeOfLevel(1)}");
        Debug.Log($"sizeofLevel 2 {NativeMultilevelGrid<int>.CellSizeOfLevel(2)}");
        Debug.Log($"sizeofLevel 3 {NativeMultilevelGrid<int>.CellSizeOfLevel(3)}");
        Debug.Log($"sizeofLevel 4 {NativeMultilevelGrid<int>.CellSizeOfLevel(4)}");
        Debug.Log($"sizeofLevel 5 {NativeMultilevelGrid<int>.CellSizeOfLevel(5)}");
        Debug.Log($"sizeofLevel 6 {NativeMultilevelGrid<int>.CellSizeOfLevel(6)}");

        Debug.Log($"levelOfSize 18 {NativeMultilevelGrid<int>.GridLevelForSize(18)}");
        Debug.Log($"levelOfSize 35 {NativeMultilevelGrid<int>.GridLevelForSize(35)}");
        Debug.Log($"levelOfSize 14 {NativeMultilevelGrid<int>.GridLevelForSize(14)}");
        Debug.Log($"levelOfSize 63 {NativeMultilevelGrid<int>.GridLevelForSize(63)}");
        Debug.Log($"levelOfSize 65 {NativeMultilevelGrid<int>.GridLevelForSize(65)}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
