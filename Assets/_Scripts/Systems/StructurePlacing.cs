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

    private GameObject instedObj;

    private bool _insted;

    private const int IGNORE_RAYCAST_LAYER = 2;         // Physics.IgnoreRaycastLayer is 4, but in GameObject this layer is 2...

    private const int IGNORE_DEFAULT_LAYER = 0;         // Default = 0



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selectedStructure = _structures[0];

    }

    // Update is called once per frame
    void Update()
    {

        showPlacement();

    }


    void showPlacement()
    {
        if (!_insted)
        {
            instedObj = Instantiate(_selectedStructure.model);
            disabledMode(instedObj);
            _insted = true;
        }

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

            Debug.Log(checkInvalid());
        }

    }




    void disabledMode(GameObject obj)
    {
   

        foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = _selectedStructure.transMaterial;
        }

        obj.layer = IGNORE_RAYCAST_LAYER;

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

        foreach (Collider renderer in obj.GetComponentsInChildren<Collider>())
        {
            obj.GetComponent<Collider>().enabled = true;
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
}
