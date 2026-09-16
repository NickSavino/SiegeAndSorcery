using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
namespace _Scripts.Controllers
{
    public class StructurePlacementController : MonoBehaviour
    {
        /*
        * Constants
        */
        const int IGNORE_RAYCAST_LAYER = 2; // Physics.IgnoreRaycastLayer is 4, but in GameObject this layer is 2...
        const int IGNORE_DEFAULT_LAYER = 0; // Default = 0
       
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

        List<BuildableStructure> _placedStructures  = new List<BuildableStructure>();
        
        BuildableStructure _selectedStructure;
        BuildableStructure _structurePreview;

        BuildingSocket _previewSocket;
        BuildingSocket _targetSocket;
        
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
            for (int i = 0; i < numericKeys.Length; i++)
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
            }
        }

        void UpdatePlacement()
        {
            _previewSocket = null;
            _targetSocket = null;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!placementSurface.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (_structurePreview != null)
                {
                    _structurePreview.gameObject.SetActive(false);
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
            
            FindSnapCandidate();

            if (_targetSocket != null)
            {
                AlignPreview();
            }
            
        }

        void FindSnapCandidate()
        {
            float closestDistanceSquared = snapDistance * snapDistance;

            foreach (var building in _placedStructures)
            {
                if (building == null ||
                    !building.isActiveAndEnabled ||
                    !building.IsPlaced())
                {
                    continue;
                }

                foreach (var target in building.Sockets)
                {
                    if (target == null || !target.isActiveAndEnabled)
                    {
                        continue;
                    }

                    foreach (var source in _structurePreview.Sockets)
                    {
                        if (source == null ||
                            !source.isActiveAndEnabled ||
                            !source.CanConnectTo(target))
                        {
                            continue;
                        }

                        float distanceSquared = (source.transform.position - target.transform.position).sqrMagnitude;

                        if (distanceSquared < closestDistanceSquared)
                        {
                            closestDistanceSquared = distanceSquared;
                            _previewSocket = source;
                            _targetSocket = target;
                        }
                    }
                }
            }
        }

        void AlignPreview()
        {
            var root = _structurePreview.transform;

            float angle = Vector3.SignedAngle(
                _previewSocket.transform.forward,
                -_targetSocket.transform.forward,
                Vector3.up);

            root.rotation = Quaternion.AngleAxis(angle, Vector3.up) * root.rotation;
            
            root.position += _targetSocket.transform.position - _previewSocket.transform.position;
        }

        void ConfirmPlacement()
        {
            if (!Input.GetMouseButtonDown(0) || _structurePreview == null || !_structurePreview.gameObject.activeSelf)
            {
                return;
            }

            if (_targetSocket != null && !_previewSocket.TryConnect(_targetSocket))
            {
                return;
            }
            
            _structurePreview.Place();
            _placedStructures.Add(_structurePreview);

            _structurePreview = null;
            _previewSocket = null;
            _targetSocket = null;
        }
        
        public void RemoveStructureSelection()
        {
            if (_structurePreview != null)
            {
                Destroy(_structurePreview.gameObject);
            }
            
            _structurePreview = null;
            _selectedStructure = null;
            _previewSocket = null;
            _targetSocket = null;
            _placementYaw = 0f;
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
        
    }
}
