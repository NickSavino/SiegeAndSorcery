using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class StructureManager : MonoBehaviour
{

    private List<StructureController> _activeStructures;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _activeStructures = new List<StructureController>();
        AddAllInitialStructures();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddStructure(StructureController controller)
    {
        _activeStructures.Add(controller);
    }


    public void RemoveStructure(StructureController controller)
    {
        _activeStructures.Remove(controller);   // will be inefficient later on im sure
    }

    private void AddAllInitialStructures()
    {
        StructureController[] defaults = GameObject.FindObjectsByType<StructureController>(FindObjectsSortMode.None);
        foreach (StructureController controller in defaults)
        {
            _activeStructures.Add(controller);
        }
    }

    public StructureController FindNearestEnemyStructure(Vector3 position, int team)
    {
        StructureController closest = null;
        float minDistance = 0f;

        List<StructureController> enemyStructures = _activeStructures.FindAll(x => x._team != team);

        if (enemyStructures.Count == 0)
        {
            return null;                    // no enemy structures 
        }

        // otherwise

        foreach (StructureController structure in enemyStructures)
        {
            float distance = (structure.gameObject.transform.position - position).magnitude;
            if (distance < minDistance || closest == null)
            {
                closest = structure;
                minDistance = distance;
            }
        }
        return closest;
    }


    public static StructureManager GetStructureManager()
    {
        return GameObject.Find(STRUCTS_NAMES.STRUCTURE_MANAGER).GetComponent<StructureManager>();
    }
}
