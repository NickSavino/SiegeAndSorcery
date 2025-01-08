using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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
}
