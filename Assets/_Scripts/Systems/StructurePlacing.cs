using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;

public class StructurePlacing : MonoBehaviour
{
    [System.Serializable]
    private class ModelMaterial
    {
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




    [SerializeField]
    private List<ModelMaterial> _structures;

    [SerializeField]
    private ModelMaterial _selectedStructure;

    [SerializeField]
    private float ROTATION_SENSITIVITY;

    private int _selectedIndex;

    private GameObject instedObj;

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

    private int NUMERIC_OFFSET = 49;    // ALPHA1 is 49

    private bool _insted;

    private const int IGNORE_RAYCAST_LAYER = 2;         // Physics.IgnoreRaycastLayer is 4, but in GameObject this layer is 2...

    private const int IGNORE_DEFAULT_LAYER = 0;         // Default = 0



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selectedStructure = _structures[0];
        _selectedIndex = 0;

    }

    // Update is called once per frame
    void Update()
    {
        showPlacement();
        selectTypeKeyClick();
        deselectAll();
        rotateStructure();
        confirmPlacement();

    }

    void selectTypeKeyClick()
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
            }
            else
            {
                Destroy(instedObj); // disregard current thing we were trying to place
                _insted = false;
                _selectedStructure = _structures[_selectedIndex];
            }
        }

    }


    void showPlacement()
    {
        Debug.Log(_selectedStructure);
        if (_selectedStructure != null)     // only show placements if we have selected a type of structure to build
        {


            if (!_insted)
            {
                instedObj = Instantiate(_selectedStructure.model);
                disabledMode(instedObj);
                _insted = true;
            }

            if (!Input.GetMouseButton(1))       // if not holding down right click
            {


                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {   // _currentPosts[0] is initial post
                    instedObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                    if (checkInvalid())
                    {
                        invalidMode(instedObj);
                    }
                    else
                    {
                        disabledMode(instedObj);
                    }
                }
            }
        }

    }


    void confirmPlacement()
    {
        if (!checkInvalid())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {   // _currentPosts[0] is initial post
              if (Input.GetMouseButtonDown(0))
                {
                    instedObj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    enabledMode(instedObj);
                    instedObj = null;       // do not target this gameobject anymore!
                    _insted = false;        // instantiate a new one if we want, but keep this type selected
                }
            }
        }
    }


    void deselectAll()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            instedObj = null;       // do not target this gameobject anymore!
            _insted = false;        
            _selectedStructure = null;
        }
    }




    void disabledMode(GameObject obj)
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

    void enabledMode(GameObject obj)
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

    void invalidMode(GameObject obj)
    {
        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = _selectedStructure.invalidMaterial;
        }

    }

    bool checkInvalid()
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


    void rotateStructure()
    {
        if (Input.GetMouseButton(1))    // right click
        {
            float delta = Input.GetAxis("Mouse X") * ROTATION_SENSITIVITY;
            instedObj.transform.Rotate(new Vector3(0, delta, 0));
        }
    }
}
