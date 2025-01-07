using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    private Camera _camera;

    public GameObject selectedUnit;

    private GameObject _towerPos;

    [SerializeField]
    GameObject _destination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
 
            Ray castPoint = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                if (hit.collider != null && hit.collider.gameObject.name.Contains("SpawnPoint") && Vector3.Distance(hit.point, gameObject.transform.position) < 10)
                {
                    Vector3 spawnPoint = gameObject.transform.position;
                    spawnPoint.y = 1;
                    GameObject unit = Instantiate(selectedUnit, spawnPoint, new Quaternion());
                    unit.GetComponent<UnitController>()._destination = _destination;
                }
            }

        }
    }
}
