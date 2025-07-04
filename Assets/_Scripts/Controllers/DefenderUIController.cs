using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Sirenix.Utilities;
using static System.Net.Mime.MediaTypeNames;

public class DefenderUIController : MonoBehaviour
{

    private WallBuildingController _wallBuilder;

    private StructurePlacementController _structurePlacementController;

    public StructureName selectedStructure = StructureName.None;

    private bool _controlsEnabled = false;

    private List<UnitController> _selectedUnits;

    [SerializeField]
    private GameObject _selectBox;

    [SerializeField]
    private GameObject _selectBoxParent;

    private Vector3 _selectBoxAnchor;

    private RectTransform _selectBoxTrans;

    void Start()
    {
        _selectBoxAnchor = Vector3.zero;
        _selectedUnits = new List<UnitController>();
        _selectBox = GameObject.Find(Constants.UI_SELECT_BOX);
        _selectBox.TryGetComponent<RectTransform>(out _selectBoxTrans);

        // TryGetComponent(out _structurePlacementController);
        // TryGetComponent(out _wallBuilder);
    }

    void Update() {
        SelectUnitClick();
        ChangeUnitsDestination();

    }

    private void LateUpdate() {
        DrawSelectBox();
    }

    /// <summary>
    ///     Changes a unit's destination based on right-clicking
    ///     terrain or structure
    /// </summary>
    void ChangeUnitsDestination() {

        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                StructureController structure = null;
                bool found = hit.collider.gameObject.TryGetComponent<StructureController>(out structure);
                if (found) {
                    foreach (UnitController controller in _selectedUnits) {
                        controller.SetDestination(hit.collider.gameObject);

                        NavMeshAgent nav;
                        controller.gameObject.TryGetComponent<NavMeshAgent>(out nav);
                        nav.destination = structure.transform.position;
                    }
                    return;
                }

                UnitController unit = null;
                found = hit.collider.gameObject.TryGetComponent<UnitController>(out unit);
                if (found) {
                    foreach (UnitController controller in _selectedUnits) {
                        controller.SetDestination(hit.collider.gameObject);

                        NavMeshAgent nav;
                        controller.gameObject.TryGetComponent<NavMeshAgent>(out nav);
                        nav.destination = unit.transform.position;
                    }
                    return;
                }

         
                foreach (UnitController controller in _selectedUnits) {
                    // TODO: Wastes resources, how do we avoild allocation here?
                    GameObject empty = new GameObject();
                    empty.transform.position = hit.point;
                    controller.SetDestination(empty);    // nothing to attack

                    NavMeshAgent nav;
                    controller.gameObject.TryGetComponent<NavMeshAgent>(out nav);
                    nav.destination = hit.point;
                }
    
            }
        }
    }


    /// <summary>
    ///     Deactivates selected units in linear time
    ///     and clears list
    /// </summary>
    private void ClearSelectedUnits() {
        foreach (UnitController controller in _selectedUnits) {
            controller.SetUnitNotSelected();
        }
        _selectedUnits.Clear();
    }


    /// <summary>
    ///     Method to select a single unit based on a mouse-click
    ///     
    ///     Casts a ray from camera to mouse click, gets all units on path,
    ///     selects closest unit.
    /// </summary>
    void SelectUnitClick() {

        // arbitrary, need to determine this better
        float maxDistance = 100f;

       
        if (Input.GetMouseButtonDown(0)) {
            ClearSelectedUnits();
            // get all colliders on vector path
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);

            UnitController closest = null;
            float closestDist = 0f;

            // linear search for closest unit
            foreach (RaycastHit hit in hits)
            {
                UnitController unit;
                hit.collider.gameObject.TryGetComponent<UnitController>(out unit);

                if (unit != null) {
                    if (closest == null) {
                        closest = unit;
                        closestDist = hit.distance;
                    }
                    else {
                        if (hit.distance < closestDist) {
                            closest = unit;
                        }
                    }
                }
            }

            // if found unit do something
            if (closest != null) {
                _selectedUnits.Add(closest);
                closest.SetUnitSelected();
            }
        }
    }
    

    private void SelectBoxAdjustHeight() {

    }

    public void DrawSelectBox() {
        if (Input.GetMouseButtonUp(0)) {
         //   _selectBoxTrans.sizeDelta = Vector2.zero;
        //    _selectBoxTrans.anchoredPosition = Vector2.zero;
        }
        if (Input.GetMouseButtonDown(0)) {

            Vector3 mousePos = Input.mousePosition;
            RectTransform parentTrans;
            _selectBoxParent.TryGetComponent<RectTransform>(out parentTrans);

            RectTransformUtility.ScreenPointToWorldPointInRectangle(parentTrans, mousePos, null, out _selectBoxAnchor);

        }
        if (Input.GetMouseButton(0)) {

            _selectBoxTrans.sizeDelta = new Vector3(100, 100, 0);
            _selectBoxTrans.anchoredPosition = _selectBoxAnchor - new Vector3(0, 100, 0);
            
        }
        Debug.Log(_selectBoxTrans.sizeDelta);
    }

    public void OnWallButtonClick()
    {
        if (selectedStructure != StructureName.None)
        {
            selectedStructure = _structurePlacementController.SetType(StructureName.None);
        }

        if (_controlsEnabled)
        {
            _wallBuilder.SetBuildModeActive(!_wallBuilder.IsBuildModeActive());
        }

    }

    public void OnTowerButtonClick()
    {
        if (_wallBuilder.IsBuildModeActive() && _controlsEnabled)
        {
            _wallBuilder.SetBuildModeActive(false);
        }

        if (_controlsEnabled)
        {
            selectedStructure = _structurePlacementController.SetType(selectedStructure == StructureName.BaseTower ? StructureName.None : StructureName.BaseTower);
        }
    }

    public void EnableControls(bool enable)
    {
        if (enable == false)
        {
            _structurePlacementController.DeselectAll(true);
            _wallBuilder.SetBuildModeActive(false);
        }

        _controlsEnabled = enable;
    }
}