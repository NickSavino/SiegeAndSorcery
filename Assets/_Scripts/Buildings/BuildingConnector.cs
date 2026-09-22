using UnityEngine;

public class BuildingConnector : MonoBehaviour
{
    [SerializeField]
    BuildingSocket socket;
    
    public BuildingSocket Socket => socket;
    
    public Vector3 SocketOffset => transform.InverseTransformPoint(socket.transform.position);

    public void AlignSocket(Vector3 position, Quaternion rotation)
    {
        Quaternion localSocketRotation = Quaternion.Inverse(transform.rotation) * socket.transform.rotation;

        transform.rotation = rotation * Quaternion.Inverse(localSocketRotation);
        
        transform.position += position - socket.transform.position;
    }
}
