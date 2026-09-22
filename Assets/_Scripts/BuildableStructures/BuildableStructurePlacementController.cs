using System.Collections.Generic;
using _Scripts.BuildableStructures;
using UnityEngine;
namespace _Scripts.Controllers
{
    public class BuildableStructurePlacementController : MonoBehaviour
    {
        readonly KeyCode[] _numericKeys =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        readonly List<BuildableStructure> _placedStructures  = new List<BuildableStructure>();
        
        readonly BuildableStructureSnapSolver _snapSolver = new BuildableStructureSnapSolver();
        
        BuildableStructure _selectedStructure;
        BuildableStructure _structurePreview;

        ConnectionCandidate _previewCandidate;
        ConnectionCandidate _targetCandidate;

        BuildableStructureConnector _connectorPreview;
        BuildableStructureConnectionSurface _connectorPreviewSurface;
        
        bool _hasSnapCandidate;

        private float _placementYaw;
        
        /*
        * Serialized Fields
        */
        [SerializeField]
        List<BuildableStructure> structures;
        
        [SerializeField]
        Collider placementSurface;

        [SerializeField]
        float rotationSpeed = 90;
        
        [SerializeField]
        float snapDistance = 1.5f;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _selectedStructure = null;
        }

        // Update is called once per frame
        void Update()
        {
            GetStructureSelection();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RemoveStructureSelection();
                return;
            }
            
            if (_selectedStructure == null)
            {
                return;
            }
            
            RotateStructure();

            if (!TryUpdatePlacement())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ConfirmPlacement();
            }
        }

        void GetStructureSelection()
        {
            for (int i = 0; i < _numericKeys.Length && i < structures.Count; i++)
            {
                if (!Input.GetKeyDown(_numericKeys[i]))
                {
                    continue;
                }
                
                RemoveStructureSelection();

                if (i < structures.Count)
                {
                    _selectedStructure = structures[i];
                }

                return;
            }
        }

        bool TryUpdatePlacement()
        {
            ClearSnapCandidate();
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!placementSurface.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (_structurePreview != null)
                {
                    _structurePreview.gameObject.SetActive(false);
                }
                
                ClearConnectorPreview();
                
                return false;
            }

            if (_structurePreview == null)
            {
                _structurePreview = Instantiate(_selectedStructure);
            }
            
            _structurePreview.gameObject.SetActive(true);
            
            _structurePreview.transform.SetPositionAndRotation(
                hit.point,
                Quaternion.Euler(0f, _placementYaw, 0f)
                );
            
            _hasSnapCandidate = _snapSolver.TryFindSnapCandidate(
                _structurePreview, 
                _placedStructures, 
                snapDistance, 
                out _previewCandidate, 
                out _targetCandidate);

            if (_hasSnapCandidate)
            {
                _previewCandidate = AlignPreview(
                    _previewCandidate,
                    _targetCandidate
                );
            }
            UpdateConnectorPreview();
            return true;
        }


        void ClearSnapCandidate()
        {
            _hasSnapCandidate = false;
            _previewCandidate = default;
            _targetCandidate = default;
        }
        

        ConnectionCandidate AlignPreview(ConnectionCandidate source, ConnectionCandidate target)
        {
            var root = _structurePreview.transform;

            Vector3 localSourcePosition = root.InverseTransformPoint(source.Position);
            Quaternion localSourceRotation = Quaternion.Inverse(root.rotation) * source.Rotation;

            
            float angle = Vector3.SignedAngle(
                source.Forward,
                -target.Forward,
                Vector3.up
            );

            root.rotation = Quaternion.AngleAxis(angle, Vector3.up) * root.rotation;

            root.position += target.Position - root.TransformPoint(localSourcePosition);

            if (source.IsDynamic)
            {
                return new ConnectionCandidate(source.ConnectionSurface, root.TransformPoint(localSourcePosition), root.rotation * localSourceRotation);
            }

            return new ConnectionCandidate(source.ExistingSocket);
        }

        void ConfirmPlacement()
        {
            if (_structurePreview == null || !_structurePreview.gameObject.activeSelf)
            {
                return;
            }

            if (!TryCommitConnection())
            {
                return;
            }
            
            _structurePreview.Place();
            _placedStructures.Add(_structurePreview);

            _structurePreview = null;
            
            ClearConnectorPreview();
            ClearSnapCandidate();
        }

        bool TryCommitConnection()
        {
            if (!_hasSnapCandidate)
            {
                return true;
            }

            var source = _previewCandidate.ExistingSocket;
            var target = _targetCandidate.ExistingSocket;

            if ((!_previewCandidate.IsDynamic && !IsAvailable(source)) || (!_targetCandidate.IsDynamic && !IsAvailable(target)))
            {
                return false;
            }

            BuildableStructureConnector createdConnector = null;

            if (_previewCandidate.IsDynamic)
            {
                createdConnector = _previewCandidate.ConnectionSurface.CreatePermanent(_previewCandidate);
                
                source = createdConnector.Socket;
            }
            else if (_targetCandidate.IsDynamic)
            {
                createdConnector = _targetCandidate.ConnectionSurface.CreatePermanent(_targetCandidate);

                target = createdConnector.Socket;
            }

            if (source != null &&
                target != null &&
                source.TryConnect(target))
            {
                return true;
            }

            if (createdConnector != null)
            {
                createdConnector.Socket.Owner.UnregisterSocket(createdConnector.Socket);
                
                createdConnector.gameObject.SetActive(false);
                
                Destroy(createdConnector.gameObject);
            }

            return false;
        }
        
        void RemoveStructureSelection()
        {
            ClearConnectorPreview();

            if (_structurePreview != null)
            {
                _structurePreview.gameObject.SetActive(false);
                Destroy(_structurePreview.gameObject);
            }

            _structurePreview = null;
            _selectedStructure = null;
            _placementYaw = 0f;
            
            ClearSnapCandidate();
        }

        void OnDisable()
        {
            RemoveStructureSelection();
        }

        void RotateStructure()
        {
            float direction = 0f;

            if (Input.GetKey(KeyCode.Q))
            {
                direction += 1f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                direction -= 1f;
            }

            _placementYaw += direction * rotationSpeed * Time.deltaTime;
        }

        static bool IsAvailable(BuildableStructureSocket socket)
        {
            return socket != null
                && socket.isActiveAndEnabled
                && socket.Owner != null
                && !socket.IsOccupied;
        }
        
        void UpdateConnectorPreview()
        {
            if (!_hasSnapCandidate)
            {
                ClearConnectorPreview();
                return;
            }

            ConnectionCandidate candidate;

            if (_previewCandidate.IsDynamic)
            {
                candidate = _previewCandidate;
            }
            else if (_targetCandidate.IsDynamic)
            {
                candidate = _targetCandidate;
            }
            else
            {
                ClearConnectorPreview();
                return;
            }

            if (_connectorPreview == null ||
                _connectorPreviewSurface != candidate.ConnectionSurface)
            {
                ClearConnectorPreview();

                _connectorPreviewSurface = candidate.ConnectionSurface;
                _connectorPreview =
                    _connectorPreviewSurface.CreatePreview(candidate);
            }

            _connectorPreview.AlignSocket(
                candidate.Position,
                candidate.Rotation
            );
        }

        void ClearConnectorPreview()
        {
            if (_connectorPreview != null)
            {
                _connectorPreview.gameObject.SetActive(false);
                Destroy(_connectorPreview.gameObject);
            }

            _connectorPreview = null;
            _connectorPreviewSurface = null;
        }
    }
}
