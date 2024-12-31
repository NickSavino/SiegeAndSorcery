using UnityEngine;
using System.Collections.Generic;

public class WallBuilding : MonoBehaviour
{


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

    private bool _placingPanel;
    private bool _instedPanel;


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

        if (_placingPost)
        {
            placePost();
        }
        else if (_placingPanel)
        {
            placePanel();
        }
    }



    void placePanel()
    {
        GameObject panel;
        if (!_instedPanel)
        {
            panel = Instantiate(_panelPrefab);
            disabledMode(panel);
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
                _currentPanels[index].transform.position = new Vector3(lastPostPos.x, _currentPanels[index].transform.position.y, lastPostPos.z);

                // get distance and rotation, set rotation
                Vector3 distanceVector = hit.point - _currentPanels[index].transform.position;
                distanceVector.y = 0;      // do not want to rotate up or down
                Vector3 currentScale = _currentPanels[index].transform.localScale;
                Quaternion rotation = Quaternion.LookRotation(distanceVector, Vector3.up);
                _currentPanels[index].transform.rotation = rotation;

                // scale panel along Z-axis so it reaches mouse cursor
                _currentPanels[index].transform.localScale = new Vector3(currentScale.x, currentScale.y, distanceVector.magnitude);
                

                //TODO: If we make all models with pivots at the edge of a wall, then this will not be needed
                Vector3 translateVector = new Vector3(0, 0, distanceVector.magnitude) / 2;  // need to translate along Z-axis (if pivot is at center)
                _currentPanels[index].transform.Translate(translateVector);

                if (Input.GetMouseButtonDown(0))     // automatically place post if chain has started
                {
                    enabledMode(_currentPanels[index]);
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

    void placePost()
    {
        GameObject post;
        if (!_instedPost)
        {
            post = Instantiate(_postPrefab);
            disabledMode(post);
            _currentPosts.Add(post);
            _instedPost = true;
        }

        int index = _currentPosts.Count - 1;   // want to reference object that already exists in list

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {   // _currentPosts[0] is initial post
            _currentPosts[index].transform.position = new Vector3(hit.point.x, _currentPosts[index].transform.position.y, hit.point.z);
        }

        if (Input.GetMouseButtonDown(0) || _initialPostPlaced)    // left click
        {
            enabledMode(_currentPosts[index]);
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


    void disabledMode(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = _transMaterial;
        obj.GetComponent<Collider>().enabled = false;
    }

    void enabledMode(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = _opaqueMaterial;
        obj.GetComponent<Collider>().enabled = true;
    }

}
