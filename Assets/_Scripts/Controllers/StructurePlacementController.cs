using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.InputSystem.LowLevel;
using System.Linq;
using UnityEngine.EventSystems;

public class StructurePlacementController : MonoBehaviour
{
    /*
     * Constants
     */
    private int NUMERIC_OFFSET = 49;    // ALPHA1 is 49, for keyboard number click selection
    private const int IGNORE_RAYCAST_LAYER = 2;         // Physics.IgnoreRaycastLayer is 4, but in GameObject this layer is 2...
    private const int IGNORE_DEFAULT_LAYER = 0;         // Default = 0

    private KeyCode[] numericKeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    /*
         * Serialized Fields
   */
    [SerializeField]
    private List<ModelMaterial> _structures;

    private Dictionary<StructureName, ModelMaterial> _structuresDictionary;

    [SerializeField]
    private ModelMaterial _selectedStructure;

    [SerializeField]
    private float ROTATION_SENSITIVITY;

    /*
     *   struct, GameObject with required mayerials
     */
    [System.Serializable]
    private class ModelMaterial      
    {
        public StructureName structureName;
        public GameObject model;
        public Material opaqueMaterial;
        public Material transMaterial;
        public Material invalidMaterial;

        public ModelMaterial(GameObject model, Material opaqueMaterial, Material transMaterial, Material invalidMaterial)
        {
            this.model = model;
            this.opaqueMaterial = opaqueMaterial;
            this.transMaterial = transMaterial;
            this.invalidMaterial = invalidMaterial;
        }

        public ModelMaterial(ModelMaterial sample)
        {
            this.transMaterial = sample.transMaterial;
            this.opaqueMaterial = sample.opaqueMaterial;
            this.model = sample.model;
            this.invalidMaterial = sample.invalidMaterial;
        }
    };

    /*
     * General fields
     */
    private int _selectedIndex;     // selected struct key number
    private GameObject instedObj;   // currently instantiated game object / structed
    private bool _insted;           // flag indicated if current structure has been instantiated

    private bool _showedOnce;       // when placing a rotated item with right mouse button clicked,
                                    // we need to make sure the next one is placed at cursor

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selectedStructure = null;
        _selectedIndex = 0;

        _structuresDictionary = _structures.ToDictionary(x => x.structureName, x => x);
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlacement();
        DeselectAll();
        RotateStructure();
        ConfirmPlacement();
    }

    void SelectTypeKeyClick()
    {
        bool wasKeyClicked = false;
        foreach (KeyCode numer in numericKeys)
        {
            if (Input.GetKeyDown(numer))
            {
                _selectedIndex = (int)numer - NUMERIC_OFFSET;
                wasKeyClicked = true;
            }
        }
        if (wasKeyClicked)      // only do this if a numerical key was clicked
        {
            if (_selectedIndex >= _structures.Count)
            {
                _selectedIndex = -1;
                _selectedStructure = null;
                _showedOnce = false;
            }
            else
            {
                Destroy(instedObj); // disregard current thing we were trying to place
                _insted = false;
                _selectedStructure = _structures[_selectedIndex];
                _showedOnce = false;
            }
        }

    }

    public StructureName SetType(StructureName structureName)
    {
        Destroy(instedObj);
        if (!_structuresDictionary.ContainsKey(structureName) || structureName == StructureName.None)
        {
            _selectedIndex = -1;
            _selectedStructure = null;
            _showedOnce = false;
            return StructureName.None;
        } 
        
        _insted = false;
        _selectedStructure = _structuresDictionary[structureName];
        _showedOnce = false;
        return structureName;
    }

    void ShowPlacement()
    {
        if (_selectedStructure?.model != null)     // only show placements if we have selected a type of structure to build
        {
            if (!_insted)
            {
                instedObj = Instantiate(_selectedStructure.model);
                SetDisabledMode(instedObj);
                _insted = true;
            }
            if (!IsRotating() || !_showedOnce)       // if not holding down right click
            {
                _showedOnce = true;

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {   // _currentPosts[0] is initial post
                    instedObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                    if (CheckInvalidPlacement())
                    {
                        SetInvalidMode(instedObj);
                    }
                    else
                    {
                        SetDisabledMode(instedObj);
                    }
                }
            }
        }

    }

    void ConfirmPlacement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!CheckInvalidPlacement())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {   
              if (Input.GetMouseButtonDown(0) )
                {   if (!IsRotating())
                    {
                        instedObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    }
                    SetEnabledMode(instedObj);
                    instedObj = null;       // do not target this gameobject anymore!
                    _insted = false;        // instantiate a new one if we want, but keep this type selected
                    _showedOnce = false;
                }
            }
        }
    }

    void DeselectAll()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(instedObj);
            _insted = false;        
            _selectedStructure = null;
            _showedOnce = false;
        }
    }

    void SetDisabledMode(GameObject obj)
    {
        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = _selectedStructure.transMaterial;
        }

        obj.layer = IGNORE_RAYCAST_LAYER;
        obj.tag = TAGS_STRUCTS.UNTAGGED;

        foreach (Collider childCollider in obj.GetComponentsInChildren<Collider>())
        {
            childCollider.gameObject.layer = IGNORE_RAYCAST_LAYER;
        }

    }

    void SetEnabledMode(GameObject obj)
    {
        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = _selectedStructure.opaqueMaterial;
        }

        obj.layer = IGNORE_DEFAULT_LAYER;
        obj.tag = TAGS_STRUCTS.INVALID_PLACEMENT;

        foreach (Collider childCollider in obj.GetComponentsInChildren<Collider>())
        {
            childCollider.gameObject.layer = obj.layer = IGNORE_DEFAULT_LAYER;
            ;
        }
    }

    void SetInvalidMode(GameObject obj)
    {
        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = _selectedStructure.invalidMaterial;
        }
    }

    bool CheckInvalidPlacement()
    {
        foreach (Collider thisCollider in instedObj.GetComponentsInChildren<Collider>())
        {
            Collider[] hitColliders = Physics.OverlapBox(instedObj.transform.position, instedObj.transform.localScale);
            foreach (Collider thatCollider in hitColliders)
            {

                if (thatCollider.gameObject.tag.Equals(TAGS_STRUCTS.INVALID_PLACEMENT))
                {
                    return true;
                }
            }
        }
        return false;
    }


    void RotateStructure()
    {
        if (Input.GetMouseButton(1))    // right click
        {
            float delta = Input.GetAxis("Mouse X") * ROTATION_SENSITIVITY;
            instedObj.transform.Rotate(new Vector3(0, delta, 0));
        }
    }

    bool IsRotating()
    {
        return Input.GetMouseButton(1);
    }
}
