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

    public List<BuildingSocket> Sockets = new List<BuildingSocket>();

    [SerializeField]
    TowerConnectionSurface _connectionSurface;

    public TowerConnectionSurface ConnectionSurface => _connectionSurface;
    
    bool _isPlaced;

    void Awake()
    {
        foreach (var socket in Sockets)
        {
            if (socket != null)
            {
                socket.Initialize(this);
            }
        }

        if (_connectionSurface != null)
        {
            _connectionSurface.Initialize(this);
        }
    }

    public void RegisterSocket(BuildingSocket socket)
    {
        if (socket == null || Sockets.Contains(socket))
        {
            return;
        }
        
        socket.Initialize(this);
        Sockets.Add(socket);
    }

    public void UnregisterSocket(BuildingSocket socket)
    {
        if (socket == null)
        {
            return;
        }
        
        socket.Disconnect();
        Sockets.Remove(socket);
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
