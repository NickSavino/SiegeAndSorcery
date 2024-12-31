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
    private bool _initialPlaced;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentPosts = new List<GameObject>();
        _currentPanels = new List<GameObject>();
        _initialPlaced = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }

        if (!_initialPlaced)
        {
            placeInitialPost();
        }
    }





    void placeInitialPost()
    {
        GameObject initialPost;
        if (_currentPosts.Count  == 0)
        {
            initialPost = Instantiate(_postPrefab);
            initialPost.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Blue_Trans");
            initialPost.GetComponent<CapsuleCollider>().enabled = false;
            _currentPosts.Add(initialPost);
        }


        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {   // _currentPosts[0] is initial post
            _currentPosts[0].transform.position = new Vector3(hit.point.x, _currentPosts[0].transform.position.y, hit.point.z);
        }
    }
}
