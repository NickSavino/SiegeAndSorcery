using UnityEngine;
namespace _Scripts.BuildableStructures
{
    public readonly struct ConnectionCandidate
    {
        public BuildableStructure Owner { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
    
        public BuildableStructureSocket ExistingSocket { get; }
        public BuildableStructureConnectionSurface ConnectionSurface { get; }
    
        public Vector3 Forward => Rotation * Vector3.forward;
        public bool IsDynamic => ConnectionSurface != null;

        public ConnectionCandidate(BuildableStructureSocket socket)
        {
            Owner = socket.Owner;
            Position = socket.transform.position;
            Rotation = socket.transform.rotation;

            ExistingSocket = socket;
            ConnectionSurface = null;
        }

        public ConnectionCandidate(
            BuildableStructureConnectionSurface surface,
            Vector3 position,
            Quaternion rotation)
        {
            Owner = surface.Owner;
            Position = position;
            Rotation = rotation;

            ExistingSocket = null;
            ConnectionSurface = surface;
        }
    }
}
