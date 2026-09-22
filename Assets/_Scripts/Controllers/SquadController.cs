using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using Cinemachine.Utility;
using static UnityEngine.UI.Image;
using Unity.VisualScripting;

public class SquadController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<UnitController> _units;

    [field: SerializeField]
    public UnitController _unitType;

    [field: SerializeField]
    public int _squadSize = 48;

    Vector3 anchor = Vector3.zero;

    [field: SerializeField]
    public GameObject _unitTypeTest;

    [field: SerializeField]
    public int SIZE_ROWS = 8;

    [field: SerializeField]
    public int SIZE_COLS = 6;


    [field: SerializeField]
    public int COL_MIN_LEN = 3;

    [field: SerializeField]
    public int UNIT_GAP_SIZE = 2;       // TODO: Make a function of unit's transform size


    

    private List<GameObject> spheres;

    void Start()
    {
        spheres = InitUnits();

 
        
    }

    // Update is called once per framew
    void Update()
    {
        SetUnitsDestinations();
    }



    struct ROW_COL {
        public int rows;
        public int cols;

        public ROW_COL(int r, int c) {
            rows = r;
            cols = c;
        }
    };


    List<GameObject> InitUnits() {
        List<GameObject> units = new List<GameObject>();
        for (int i = 0; i < _squadSize; ++i) {
            units.Add(Instantiate(_unitTypeTest, this.transform, true));
        }
        return units;
    }

    int IndexConvert(int i, int j, ROW_COL rowsCols) {
        return (i * rowsCols.cols) + j;
    }


    /**
     *  Handles sqaud destination selection by users
     * 
     *  Determines if left click (default size) or dynamic sizing
     */

    void SetUnitsDestinations() {

        // Get first mouse position from first clock (top corner of draggable grid)
        if (Input.GetMouseButtonDown(1)) {
            anchor = Input.mousePosition;
        }

        // if holding down
        if (Input.GetMouseButton(1)) {

            // end of draggable grid is CURRENT mouse position this frame
            Vector3 pivot = Input.mousePosition;

            Vector3 diff = (anchor - pivot);

            Vector3 perp = new Vector3(-diff.y, diff.x);

            ROW_COL rowsCols;
            Vector3 origin;

            // case of just a click no hold, default size
            if (diff == Vector3.zero) {
                rowsCols = new ROW_COL(SIZE_ROWS, SIZE_COLS);
                origin = Input.mousePosition;
            }
            else {      // otherwise draggable sizing
                rowsCols = GetOptimalShape(anchor, pivot);
                origin = anchor;
            }

            // world units to screen pixels
            float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);
            float width = rowsCols.cols * pixelsPerUnit;
            float height = rowsCols.rows * pixelsPerUnit;

            // offset, center on cursor
            if (diff == Vector3.zero) {
                origin -= new Vector3(width / 2f, height / 2f, 0);
                CastRaysClicked(origin, rowsCols, pixelsPerUnit);
            }
            else {
                Vector3 iter = new Vector3(origin.x, origin.y, origin.z);

                // Cast the actual rays to get the coords we need to set each destination
                CastRaysDraggableSelection(origin, rowsCols, pixelsPerUnit, diff, perp);
            }
        }
    }


    void CastRaysClicked(Vector3 origin, ROW_COL rowsCols, float pixelsPerUnit) {
        Vector3 iter = new Vector3(origin.x, origin.y, origin.z);

        GameObject cont = null;
        for (int i = 0; i < rowsCols.rows; ++i) {
            for (int j = 0; j < rowsCols.cols; ++j) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(iter);
                if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post

                    Vector3 destination = hit.point;

                    cont = spheres[IndexConvert(i, j, rowsCols)];

                    cont.transform.position = destination;
                }
                iter += new Vector3(pixelsPerUnit * cont.transform.lossyScale.x, 0, 0);
            }
            iter += new Vector3(0, pixelsPerUnit * cont.transform.lossyScale.y, 0);
            iter.x = origin.x;
        }
    }


    void CastRaysDraggableSelection(Vector3 origin, ROW_COL rowsCols, float pixelsPerUnit, Vector3 diff, Vector3 perp) {
        Vector3 iter = new Vector3(origin.x, origin.y, origin.z);

        GameObject cont = null;
        for (int i = 0; i < rowsCols.rows; ++i) {
            iter = origin + (perp.normalized * pixelsPerUnit * i);
            for (int j = 0; j < rowsCols.cols; ++j) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(iter);
                if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post

                    Vector3 destination = hit.point;

                    cont = spheres[IndexConvert(i, j, rowsCols)];

                    cont.transform.position = destination;
                }
                iter += new Vector3(pixelsPerUnit * -diff.normalized.x, pixelsPerUnit * -diff.normalized.y, 0);
            }
        }
    }


    ROW_COL GetOptimalShape(Vector3 anchor, Vector3 pivot) {
        float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);
        float xDistance = Math.Abs(anchor.x - pivot.x) * 4;

        float pixelsPerSphere = pixelsPerUnit * _unitTypeTest.transform.lossyScale.x;



        int numRows = Math.Max(COL_MIN_LEN,  _squadSize - ((int) (xDistance / pixelsPerSphere)));
        int numCols = 0;
        for (int i = numRows; i >= COL_MIN_LEN; --i) {
            if (_squadSize % i == 0) {
                numCols = _squadSize / i;
                numRows = i;
                break;
            }
        }

        return new ROW_COL ( numRows, numCols);
    }

}
