using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Rigidbody _rigidBody;
    private NavMeshAgent _navMeshAgent;
    private Camera _camera;

    public Vector3 _destination;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (_destination != null)
        {
            _navMeshAgent.SetDestination(_destination);
        }
    }
}
