using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PathController : MonoBehaviour

{

    private Vector3 _anchor;
    private LineRenderer _activePath;
    private LineRenderer _tempPath;
    private int DEFAULT_PATH_SIZE = 2;
   // private List<Vector3> _tempPoints;

    [field: SerializeField] public bool _isActive { get; set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Find(STRUCTS_NAMES.ACTIVE_PATH).TryGetComponent<LineRenderer>(out _activePath);
        transform.Find(STRUCTS_NAMES.TEMP_PATH).TryGetComponent<LineRenderer>(out _tempPath);

        _anchor = transform.root.Find(STRUCTS_NAMES.SPAWN_POINT).transform.position;    // spawn points position is _anchor
        _tempPath.positionCount = 0;    // temp path should start with nothing

        _activePath.positionCount = 0;  // full path as well should start with nothing
        _isActive = false;
     
    }

    // Update is called once per frame
    void Update()
    {
   
        AddTempPoint();
        SavePath();



    }



    private void AddTempPoint()
    {
        if (_isActive)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 nextPoint = hit.point;
                nextPoint.y = _anchor.y;

                _tempPath.SetPosition(_tempPath.positionCount - 1, nextPoint); // tempPoints is always one point behind

                if (Input.GetMouseButtonDown(0))        
                {
                    if (_tempPath.positionCount == 2) {
                        _tempPath.SetPosition(_tempPath.positionCount - 1, _anchor);
                    }
                    else {
                        _tempPath.SetPosition(_tempPath.positionCount - 1, nextPoint);
                    }
                    ++_tempPath.positionCount;

                }
            }
        }
    }




    public void Activate()
    {
        ShowActive();
        _isActive = true;
        _tempPath.positionCount = DEFAULT_PATH_SIZE;
        _tempPath.SetPosition(0, _anchor);

    }

    public void SavePath()
    {
        if (_isActive && Input.GetKeyDown(KeyCode.Space))
        {
            _activePath.positionCount = _tempPath.positionCount - 1;
            for (int i = 0; i < _tempPath.positionCount - 1; ++i)
            {
                _activePath.SetPosition(i, _tempPath.GetPosition(i));
            }
        }
    }


    public void Deactivate()
    {
        HideActive();
        _isActive = false;
   //     _tempPoints.Clear();
        _tempPath.positionCount = 0;        // clear it
    }



    private void HideActive()
    {
        _activePath.enabled = false;
    }

    private void ShowActive()
    {
        _activePath.enabled = true;
    }

    public Vector3[] GetPathForUnit() {
        if (_activePath.positionCount > 0) {
            Vector3[] rawPoints = new Vector3[_activePath.positionCount];
            _activePath.GetPositions(rawPoints);


            Vector3[] pathFor = new Vector3[_activePath.positionCount - 1];       // exclude first point, it is same as second (needed for nice line drawing)
            for (int i = 1; i < rawPoints.Length; ++i) {        // skip first
                pathFor[i-1] = rawPoints[i];
            }
            return pathFor;
        }
        return null;
    }


}
