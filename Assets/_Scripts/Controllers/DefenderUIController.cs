using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class DefenderUIController : MonoBehaviour
{

    private WallBuildingController _wallBuilder;

    private StructurePlacementController _structurePlacementController;

    public StructureName selectedStructure = StructureName.None;

    private bool _controlsEnabled = false;

    private List<UnitController> _selectedUnits;

    public void Start()
    {
        _selectedUnits = new List<UnitController>();
       // TryGetComponent(out _structurePlacementController);
       // TryGetComponent(out _wallBuilder);
    }

    private void Update() {
        SelectUnitClick();
        ChangeUnitsDestination();
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