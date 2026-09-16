using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class SquadController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<UnitController> _units;

    [field: SerializeField]
    public UnitController _unitType;

    [field: SerializeField]
    public int _squadSize = 12;

    [field: SerializeField]
    public GameObject _unitTypeTest;

    [field: SerializeField]
    public int SIZE_ROWS = 3;

    [field: SerializeField]
    public int SIZE_COLS = 4;

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
        SetUnitsDestination();
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
            units.Add(Instantiate(_unitTypeTest));
        }
        return units;
    }

    int IndexConvert(int i, int j) {
        return (i * SIZE_COLS) + j;
    }

    void SetUnitsDestination() {
        if (Input.GetMouseButtonDown(1)) {
           // RaycastHit hit;
           // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);

            float width = SIZE_COLS * pixelsPerUnit;
            float height = SIZE_ROWS * pixelsPerUnit;



            Vector3 origin = Input.mousePosition - new Vector3(width / 2f, height / 2f, 0);
            Vector3 iter = new Vector3(origin.x, origin.y, origin.z);

            GameObject cont = null;
            for (int i = 0; i < SIZE_ROWS; ++i) {
                for (int j = 0; j < SIZE_COLS; ++j) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(iter);
                    if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post

                        Vector3 destination = hit.point;
                        //      destination.y = spheres[0].gameObject.transform.position.y;  // keep y axis unaffected
                        // foreach (GameObject cont in spheres) {

                        cont = spheres[IndexConvert(i, j)];
                            
                           cont.transform.position = destination;

                       // }
                    }
                    iter += new Vector3(pixelsPerUnit * cont.transform.lossyScale.x, 0, 0);
                }
                iter += new Vector3(0, pixelsPerUnit * cont.transform.lossyScale.y, 0);
                iter.x = origin.x;
            }
        }
    }

    void SetUnitsDestinationScaled() {
        if (Input.GetMouseButtonDown(1)) {
            Vector3 anchor = Input.mousePosition;

            if (Input.GetMouseButton(1)) {

                Vector3 pivot = Input.mousePosition;

                ROW_COL rowsCols = getOptimalShape(anchor, pivot);

                // RaycastHit hit;
                // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);

                float width = SIZE_COLS * pixelsPerUnit;
                float height = SIZE_ROWS * pixelsPerUnit;



                Vector3 origin = Input.mousePosition - new Vector3(width / 2f, height / 2f, 0);
                Vector3 iter = new Vector3(origin.x, origin.y, origin.z);

                GameObject cont = null;
                for (int i = 0; i < SIZE_ROWS; ++i) {
                    for (int j = 0; j < SIZE_COLS; ++j) {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(iter);
                        if (Physics.Raycast(ray, out hit)) {   // _currentPosts[0] is initial post

                            Vector3 destination = hit.point;
                            //      destination.y = spheres[0].gameObject.transform.position.y;  // keep y axis unaffected
                            // foreach (GameObject cont in spheres) {

                            cont = spheres[IndexConvert(i, j)];

                            cont.transform.position = destination;

                            // }
                        }
                        iter += new Vector3(pixelsPerUnit * cont.transform.lossyScale.x, 0, 0);
                    }
                    iter += new Vector3(0, pixelsPerUnit * cont.transform.lossyScale.y, 0);
                    iter.x = origin.x;
                }
            }
        }
    }


    ROW_COL getOptimalShape(Vector3 anchor, Vector3 pivot) {
        float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);
        float xDistance = Math.Abs(anchor.x - pivot.x);

        float pixelsPerSphere = pixelsPerUnit * _unitTypeTest.transform.lossyScale.x;



        int numRows = Math.Min(1,  (int) (xDistance / pixelsPerSphere));
        int numCols = 0;
        for (int i = numRows; i >= 1; ++i) {
            if (_squadSize % i == 0) {
                numCols = _squadSize / i;
                numRows = i;
                break;
            }
        }

        return new ROW_COL ( numRows, numCols);
    }

}
