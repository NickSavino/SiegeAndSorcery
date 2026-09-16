using System;
using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;

public class BuildableStructure : MonoBehaviour
{
    [SerializeField]
    public StructureName structureName;
    
    //public Material opaqueMaterial;
    //public Material transMaterial;
    //public Material invalidMaterial;

    public List<BuildingSocket> Sockets;

    bool _isPlaced;

    void Awake()
    {
        foreach (BuildingSocket socket in Sockets)
        {
            if (socket != null)
            {
                socket.Initialize(this);
            }
        }
    }

    public void Place()
    {
        _isPlaced = true;
    }

    public bool IsPlaced()
    {
        return _isPlaced;
    }
}
