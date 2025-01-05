using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class CubeAI : MonoBehaviour
{

    public NavMeshAgent thisAgent;
    public GameObject destination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisAgent = GetComponent<NavMeshAgent>();
        destination = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        thisAgent.SetDestination(destination.transform.position);
    }
}