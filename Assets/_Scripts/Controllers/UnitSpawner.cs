using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class UnitSpawner : MonoBehaviour
{


    public GameObject selectedUnit;
    private float _currentTime;

    [SerializeField]
    private float SPAWN_INTERVAL_SECONDS;

    [SerializeField]
    GameObject _destination;

    private Vector3 _spawnPoint;

    private StructureManager _structureManager;
    private PathController _pathController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnPoint = transform.GetChild(0).transform.position;
        _currentTime = 0f;
        _structureManager = StructureManager.GetStructureManager();
        transform.Find(STRUCTS_NAMES.UNIT_PATH).TryGetComponent<PathController>(out _pathController);
    }

    // Update is called once per frame
    void Update()
    {

        TogglePathBuilding();
        if (_destination != null)       // destination was destroyed, need to reset it
        {
            AutoSpawn();
        }
        else
        {
            int team = gameObject.GetComponent<StructureController>()._team;
            StructureController newDestination = _structureManager.FindNearestEnemyStructure(transform.position, team);
            if (newDestination != null) {
                _destination = newDestination.gameObject;
            }
        }
    }

    private void AutoSpawn()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > SPAWN_INTERVAL_SECONDS)
        {
            GameObject unit = Instantiate(selectedUnit);
            unit.transform.position = _spawnPoint;
            unit.GetComponent<UnitController>().SetDestination(_destination);
            _currentTime = 0f;  // reset timer
        }
    }
    

    private void SpawnOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.Equals(GetComponent<Collider>()))          // if I have been clicked
                {
                    Vector3 spawnPoint = gameObject.transform.position;
                    spawnPoint.y = 1;
                    GameObject unit = Instantiate(selectedUnit, spawnPoint, new Quaternion());
                    unit.GetComponent<UnitController>().SetDestination(_destination);
                }
            }
        }
    }

    private void TogglePathBuilding()
    {
        // try activating

        if (!_pathController._isActive)
        {

            if (Input.GetMouseButtonDown(0))
            {


                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Collider myCollider;
                TryGetComponent<Collider>(out myCollider);



                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == myCollider)
                    {
    
                        _pathController.Activate();

                        TryGetComponent<MeshRenderer>(out MeshRenderer renderer);
                        renderer.material.color = Color.cyan;   // just debugging ugly highlighting for now when selected
                    }
                }
            }
        }


        // try deactivating

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                 _pathController.Deactivate();
                MeshRenderer renderer;
                TryGetComponent<MeshRenderer>(out renderer);
                renderer.material.color = Color.white;   // just debugging ugly highlighting for now when selected
            }
        }
    }
}
