using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    private Camera _camera;

    public GameObject selectedUnit;

    private Vector3 _towerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;

        _towerPos = GameObject.Find("Castle").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            Ray castPoint = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                if (hit.collider != null && hit.collider.gameObject.name.Contains("SpawnPoint"))
                {
                    Vector3 spawnPoint = gameObject.transform.position;
                    spawnPoint.y = 1;
                    GameObject unit = Instantiate(selectedUnit, spawnPoint, new Quaternion());
                    unit.GetComponent<UnitController>()._destination = _towerPos;
                }
            }

        }
    }
}
