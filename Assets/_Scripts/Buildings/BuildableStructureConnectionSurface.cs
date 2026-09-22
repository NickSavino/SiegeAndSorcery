using UnityEngine;
namespace _Scripts.Buildings
{
    public abstract class BuildableStructureConnectionSurface : MonoBehaviour
    {
        [SerializeField]
        protected BuildableStructureConnector connectorPrefab;

        [SerializeField]
        protected Transform connectorContainer;
        
        public BuildableStructure Owner { get; private set; }
        
        public abstract bool TryGetCandidate(Vector3 approachingPosition,
            out ConnectionCandidate candidate);
        
        public void Initialize(BuildableStructure owner)
        {
            Owner = owner;
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
}
