using UnityEngine;

public readonly struct ConnectionCandidate
{
    public BuildableStructure Owner { get; }
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }
    
    public BuildingSocket ExistingSocket { get; }
    public TowerConnectionSurface ConnectionSurface { get; }
    
    public Vector3 Forward => Rotation * Vector3.forward;
    public bool IsDynamic => ConnectionSurface != null;

    public ConnectionCandidate(BuildingSocket socket)
    {
        Owner = socket.Owner;
        Position = socket.transform.position;
        Rotation = socket.transform.rotation;

        ExistingSocket = socket;
        ConnectionSurface = null;
    }

    public ConnectionCandidate(
        TowerConnectionSurface surface,
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
