using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;
namespace _Scripts.Controllers
{
    public class StructurePlacementController : MonoBehaviour
    {
        KeyCode[] numericKeys =
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

        [SerializeField]
        List<BuildableStructure> _selectedStructures = new List<BuildableStructure>();
        
        List<BuildableStructure> _placedStructures  = new List<BuildableStructure>();
        
        BuildableStructure _selectedStructure;
        BuildableStructure _structurePreview;

        ConnectionCandidate _previewCandidate;
        ConnectionCandidate _targetCandidate;

        BuildingConnector _connectorPreview;
        TowerConnectionSurface _connectorPreviewSurface;
        
        bool _hasSnapCandidate;

        /*
        * Serialized Fields
        */
        [SerializeField]
        List<BuildableStructure> structures;
        
        [SerializeField]
        Collider placementSurface;

        [SerializeField]
        float rotationSpeed = 90;
        
        private float _placementYaw;

        [SerializeField]
        float snapDistance = 1.5f;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _selectedStructure = null;

            _selectedStructures.Clear();

            for (int i = 0; i < numericKeys.Length && i < structures.Count; i++)
            {
                _selectedStructures.Add(structures[i]);
            }
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
            UpdatePlacement();
            ConfirmPlacement();
        }

        void GetStructureSelection()
        {
            for (int i = 0; i < numericKeys.Length && i < structures.Count; i++)
            {
                if (!Input.GetKeyDown(numericKeys[i]))
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

        void UpdatePlacement()
        {
            _hasSnapCandidate = false;
            _previewCandidate = default;
            _targetCandidate = default;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!placementSurface.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (_structurePreview != null)
                {
                    _structurePreview.gameObject.SetActive(false);
                    ClearConnectorPreview();
                }
                
                return;
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
            
            _hasSnapCandidate = FindSnapCandidate(out _previewCandidate, out _targetCandidate);

            if (_hasSnapCandidate)
            {
                _previewCandidate = AlignPreview(
                    _previewCandidate,
                    _targetCandidate
                );
            }
            UpdateConnectorPreview();
        }

        bool FindSnapCandidate(
    out ConnectionCandidate previewCandidate,
    out ConnectionCandidate targetCandidate)
{
    ConnectionCandidate bestPreview = default;
    ConnectionCandidate bestTarget = default;

    bool found = false;
    float closestDistanceSquared = snapDistance * snapDistance;

    foreach (var building in _placedStructures)
    {
        if (building == null ||
            building == _structurePreview ||
            !building.isActiveAndEnabled ||
            !building.IsPlaced())
        {
            continue;
        }

        // Cases where the preview has fixed sockets.
        foreach (var source in _structurePreview.Sockets)
        {
            if (!IsAvailable(source))
            {
                continue;
            }

            var sourceCandidate = new ConnectionCandidate(source);

            // Wall → wall.
            foreach (var target in building.Sockets)
            {
                if (IsAvailable(target) && source.CanConnectTo(target))
                {
                    Consider(
                        sourceCandidate,
                        new ConnectionCandidate(target)
                    );
                }
            } // End target socket loop.

            // Wall → tower.
            var targetSurface = building.ConnectionSurface;

            if (targetSurface != null &&
                targetSurface.isActiveAndEnabled &&
                targetSurface.TryGetCandidate(
                    source.transform.position,
                    out var towerCandidate))
            {
                Consider(sourceCandidate, towerCandidate);
            }
        } // End preview socket loop.

        // Tower → wall.
        var previewSurface = _structurePreview.ConnectionSurface;

        if (previewSurface == null ||
            !previewSurface.isActiveAndEnabled)
        {
            continue;
        }

        foreach (var target in building.Sockets)
        {
            if (!IsAvailable(target))
            {
                continue;
            }

            if (previewSurface.TryGetCandidate(
                target.transform.position,
                out var towerCandidate))
            {
                Consider(
                    towerCandidate,
                    new ConnectionCandidate(target)
                );
            }
        }
    }

    previewCandidate = bestPreview;
    targetCandidate = bestTarget;
    return found;

    void Consider(
        ConnectionCandidate source,
        ConnectionCandidate target)
    {
        if (source.Owner == null ||
            target.Owner == null ||
            source.Owner == target.Owner)
        {
            return;
        }

        float distanceSquared =
            (source.Position - target.Position).sqrMagnitude;

        if (distanceSquared >= closestDistanceSquared)
        {
            return;
        }

        closestDistanceSquared = distanceSquared;
        bestPreview = source;
        bestTarget = target;
        found = true;
    }
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
            if (!Input.GetMouseButtonDown(0) ||
                _structurePreview == null ||
                !_structurePreview.gameObject.activeSelf)
            {
                return;
            }

            if (_hasSnapCandidate)
            {
                BuildingSocket source = _previewCandidate.ExistingSocket;
                BuildingSocket target = _targetCandidate.ExistingSocket;

                // Recheck existing sockets before creating anything.
                if ((!_previewCandidate.IsDynamic && !IsAvailable(source)) ||
                    (!_targetCandidate.IsDynamic && !IsAvailable(target)))
                {
                    return;
                }

                BuildingConnector createdConnector = null;

                if (_previewCandidate.IsDynamic)
                {
                    createdConnector =
                        _previewCandidate.ConnectionSurface.CreatePermanent(
                            _previewCandidate
                        );

                    source = createdConnector.Socket;
                }
                else if (_targetCandidate.IsDynamic)
                {
                    createdConnector =
                        _targetCandidate.ConnectionSurface.CreatePermanent(
                            _targetCandidate
                        );

                    target = createdConnector.Socket;
                }

                if (source == null || target == null ||
                    !source.TryConnect(target))
                {
                    // Roll back a connector if connecting failed.
                    if (createdConnector != null)
                    {
                        createdConnector.Socket.Owner.UnregisterSocket(
                            createdConnector.Socket
                        );

                        createdConnector.gameObject.SetActive(false);
                        Destroy(createdConnector.gameObject);
                    }

                    return;
                }
            }

            _structurePreview.Place();
            _placedStructures.Add(_structurePreview);

            _structurePreview = null;
            ClearConnectorPreview();

            _hasSnapCandidate = false;
            _previewCandidate = default;
            _targetCandidate = default;
        }
        
        public void RemoveStructureSelection()
        {
            ClearConnectorPreview();

            if (_structurePreview != null)
            {
                _structurePreview.gameObject.SetActive(false);
                Destroy(_structurePreview.gameObject);
            }

            _structurePreview = null;
            _selectedStructure = null;
            _hasSnapCandidate = false;
            _previewCandidate = default;
            _targetCandidate = default;
            _placementYaw = 0f;
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

        static bool IsAvailable(BuildingSocket socket)
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
                // Wall-to-wall connections don't need a connector.
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
