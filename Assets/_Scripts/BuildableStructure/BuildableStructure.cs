using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;
namespace _Scripts.BuildableStructures
{
    public class BuildableStructure : MonoBehaviour
    {
        [SerializeField]
        public StructureName structureName;
    
        //public Material opaqueMaterial;
        //public Material transMaterial;
        //public Material invalidMaterial;

        public List<BuildableStructureSocket> Sockets = new List<BuildableStructureSocket>();

        [SerializeField]
        BuildableStructureConnectionSurface _connectionSurface;

        public BuildableStructureConnectionSurface ConnectionSurface => _connectionSurface;
    
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

        public void RegisterSocket(BuildableStructureSocket socket)
        {
            if (socket == null || Sockets.Contains(socket))
            {
                return;
            }
        
            socket.Initialize(this);
            Sockets.Add(socket);
        }

        public void UnregisterSocket(BuildableStructureSocket socket)
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
}
