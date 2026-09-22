using UnityEngine;
namespace _Scripts.BuildableStructures
{
    public class BuildableStructureConnector : MonoBehaviour
    {
        [SerializeField]
        BuildableStructureSocket socket;
    
        public BuildableStructureSocket Socket => socket;
    
        public Vector3 SocketOffset => transform.InverseTransformPoint(socket.transform.position);

        public void AlignSocket(Vector3 position, Quaternion rotation)
        {
            Quaternion localSocketRotation = Quaternion.Inverse(transform.rotation) * socket.transform.rotation;

            transform.rotation = rotation * Quaternion.Inverse(localSocketRotation);
        
            transform.position += position - socket.transform.position;
        }
    }
}
