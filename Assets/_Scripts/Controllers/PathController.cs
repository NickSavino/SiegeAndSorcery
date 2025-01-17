using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PathController : MonoBehaviour

{

    private Vector3 _anchor;
    private LineRenderer _activePath;
    private LineRenderer _tempPath;
    private List<Vector3> _activePoints;
    private List<Vector3> _tempPoints;

    [field: SerializeField] public bool _isActive { get; set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Find(STRUCTS_NAMES.ACTIVE_PATH).TryGetComponent<LineRenderer>(out _activePath);
        transform.Find(STRUCTS_NAMES.TEMP_PATH).TryGetComponent<LineRenderer>(out _tempPath);
        _activePoints = new List<Vector3>();
        _tempPoints = new List<Vector3>();

        _anchor = transform.root.Find(STRUCTS_NAMES.SPAWN_POINT).transform.position;    // spawn points position is _anchor
        _tempPath.positionCount = 0;    // temp path should start with nothing

        _activePath.positionCount = 0;  // full path as well should start with nothing
        _isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        AddTempPoint();

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

                _tempPoints[_tempPoints.Count - 1] = nextPoint;     
                _tempPath.SetPosition(_tempPoints.Count, nextPoint); // tempPoints is always one point behind

                if (_isActive && Input.GetMouseButtonDown(0))
                {
                    _tempPoints.Add(nextPoint);
                    ++_tempPath.positionCount;
                }
            }
        }
    }


    public void Activate()
    {
        _isActive = true;
        _tempPoints.Add(_anchor);
        _tempPath.positionCount = 2;
        _tempPath.SetPosition(0, _anchor);


        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 _next = new Vector3(hit.point.x, _anchor.y, hit.point.z);
            _tempPath.SetPosition(1, _next);
        }
    }


    public void Deactivate()
    {
        _isActive = false;
        _tempPoints.Clear();
        _tempPath.positionCount = 0;        // clear it
    }



    
}
