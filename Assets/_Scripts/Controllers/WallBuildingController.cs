using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class WallBuildingController : MonoBehaviour
{



    [SerializeField]
    private float MAX_WALL_LENGTH;

    [SerializeField]
    private Material _opaqueMaterial;

    [SerializeField]
    private Material _transMaterial;

    [SerializeField]
    private GameObject _postPrefab;

    [SerializeField]
    private GameObject _panelPrefab;

    private List<GameObject> _currentPosts;
    private List<GameObject> _currentPanels;

    private bool _initialPostPlaced;

    private bool _placingPost;
    private bool _instedPost;

    private bool _modeActive;

    private bool _placingPanel;
    private bool _instedPanel;

    private Vector3 _nextPostPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentPosts = new List<GameObject>();
        _currentPanels = new List<GameObject>();
        _placingPost = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetBuildModeActive(false);
        }
        if (_modeActive)
        {
            if (_placingPost)
            {
                PlacePost();
            }
            else if (_placingPanel)
            {
                PlacePanel();
            }
        }
    }

    void PlacePanel()
    {
        GameObject panel;
        if (!_instedPanel)
        {
            panel = Instantiate(_panelPrefab);
            DisabledMode(panel);
            Vector3 lastPostPos = _currentPosts[_currentPosts.Count - 1].transform.position;
            panel.transform.position = new Vector3(lastPostPos.x, panel.transform.position.y, lastPostPos.z);
            _currentPanels.Add(panel);
            _instedPanel = true;
        }
        else
        {
            int index = _currentPanels.Count - 1;   // want to reference object that already exists in list

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {  
                
                // reset panel position to post
                Vector3 lastPostPos = _currentPosts[_currentPosts.Count - 1].transform.position;
                _currentPanels[index].transform.position = new Vector3(lastPostPos.x, lastPostPos.y, lastPostPos.z);

                GameObject hitObject = hit.collider.gameObject; // get the gameobject that the cursor is on
                Vector3 distanceVector;
                if (hitObject.name.StartsWith(_postPrefab.name)) {      // if it is a post, snap the wall to it
                    distanceVector = hitObject.transform.position - _currentPanels[index].transform.position;
                }
                else {
                    distanceVector = hit.point - _currentPanels[index].transform.position;
                }


                if (distanceVector.magnitude > MAX_WALL_LENGTH)
                {
                    distanceVector = distanceVector.normalized * MAX_WALL_LENGTH;
                }
                // get distance and rotation, set rotation
       

                distanceVector.y = 0;      // do not want to rotate up or down

                _nextPostPosition = _currentPanels[index].transform.position + distanceVector;
           //     _nextPostPosition.y = hit.point.y;

                Vector3 currentScale = _currentPanels[index].transform.localScale;
                Quaternion rotation = Quaternion.LookRotation(distanceVector, Vector3.up);
                _currentPanels[index].transform.rotation = rotation;

                // scale panel along Z-axis so it reaches mouse cursor
                _currentPanels[index].transform.localScale = new Vector3(currentScale.x, currentScale.y, distanceVector.magnitude);
                

                //TODO: If we make all models with pivots at the edge of a wall, then this will not be needed
                Vector3 translateVector = new Vector3(0, 0, distanceVector.magnitude) / 2;  // need to translate along Z-axis (if pivot is at center)
                _currentPanels[index].transform.Translate(translateVector);

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0))     // automatically place post if chain has started
                {
                    EnabledMode(_currentPanels[index]);
                    _placingPanel = false;
                    _instedPanel = false;
                    _placingPost = true;
                    _instedPost = false;
                    if (!_initialPostPlaced)
                    {
                        _initialPostPlaced = true;
                    }
                }
            }
        }
    }

    void PlacePost()
    {
        GameObject post;
        if (!_instedPost)
        {
            post = Instantiate(_postPrefab);
            DisabledMode(post);
            _currentPosts.Add(post);
            _instedPost = true;
        }

        int index = _currentPosts.Count - 1;   // want to reference object that already exists in list

        if (!_initialPostPlaced)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {   // _currentPosts[0] is initial post
                _currentPosts[index].transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }
        else
        {
            _currentPosts[index].transform.position = _nextPostPosition;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if ((Input.GetMouseButtonDown(0)))   // left click, check if over UI
        {
            EnabledMode(_currentPosts[index]);
            _placingPanel = true;
            _instedPanel = false;
            _placingPost = false;
            _instedPost = false;
            if (!_initialPostPlaced)
            {
                _initialPostPlaced = true;
            }
        }
    }

    void DisabledMode(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = _transMaterial;
        obj.GetComponent<Collider>().enabled = false;
    }

    void EnabledMode(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = _opaqueMaterial;
        obj.GetComponent<Collider>().enabled = true;
    }
    
    public void ToggleActive()
    {
        _modeActive = !_modeActive;
        if (!_modeActive)
        {
            _initialPostPlaced = false;
            if (_instedPanel)
            {
                Destroy(_currentPanels.LastOrDefault());
                _instedPanel = false;
            }
            else if (_instedPost)
            {
                Destroy(_currentPosts.LastOrDefault());
                _instedPost = false;
            }
        }
    }

    public void SetBuildModeActive(bool isActive)
    {
        _modeActive = isActive;

        if (!_modeActive)
        {
            _initialPostPlaced = false;
            if (_instedPanel)
            {
                Destroy(_currentPanels.LastOrDefault());
                _instedPanel = false;
            }
            else if (_instedPost)
            {
                Destroy(_currentPosts.LastOrDefault());
                _instedPost = false;
            }
        } else
        {
            _placingPost = true;
        }
    }

    public bool IsBuildModeActive()
    {
        return _modeActive;
    }
}
