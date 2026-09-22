using UnityEngine;

public class CircularConnectionSurface : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    float radius = 7.5f;

    [SerializeField, Range(0f, 180f)]
    float minimumConnectionAngle = 40f;

    [SerializeField]
    BuildableStructureConnector connectorPrefab;

    [SerializeField]
    Transform connectorContainer;
    
    public BuildableStructure Owner { get; private set; }

    public void Initialize(BuildableStructure owner)
    {
        Owner = owner;
    }

    public bool TryGetCandidate(
        Vector3 approachingPosition,
        out ConnectionCandidate candidate)
    {
        candidate = default;

        if (Owner == null || connectorPrefab == null)
        {
            return false;
        }

        Vector3 direction = approachingPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return false;
        }
        
        direction.Normalize();

        if (!HasRoomForConnection(direction))
        {
            return false;
        }

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        Vector3 attachmentPosition = transform.position + direction * radius;

        Vector3 socketPosition = attachmentPosition + rotation * connectorPrefab.SocketOffset;

        candidate = new ConnectionCandidate(
            this,
            socketPosition,
            rotation);

        return true;
    }

    bool HasRoomForConnection(Vector3 direction)
    {
        foreach (var socket in Owner.Sockets)
        {
            if (socket == null)
            {
                continue;
            }

            Vector3 existingDirection = socket.transform.position - transform.position;

            existingDirection.y = 0f;

            if (existingDirection.sqrMagnitude < 0.0001f)
            {
                continue;
            }

            if (Vector3.Angle(direction, existingDirection) < minimumConnectionAngle)
            {
                return false;
            }
        }

        return true;
    }

    public BuildableStructureConnector CreatePreview(ConnectionCandidate candidate)
    {
        var connector = Instantiate(connectorPrefab);

        connector.AlignSocket(candidate.Position, candidate.Rotation);

        connector.Socket.Initialize(Owner);

        foreach (var collider in connector.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        return connector;
    }

    public BuildableStructureConnector CreatePermanent(ConnectionCandidate candidate)
    {
        var connector = Instantiate(connectorPrefab, connectorContainer);
        
        connector.AlignSocket(candidate.Position, candidate.Rotation);
            
        Owner.RegisterSocket(connector.Socket);

        return connector;
    }
}
