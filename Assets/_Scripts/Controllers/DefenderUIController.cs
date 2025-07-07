using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class DefenderUIController : MonoBehaviour
{


    // For right now, a unit's destination is set to this when a unit
    // is moved to a non-unit or non-structure destination.
    // This makes sure that animation code is untouched for now 
    public static GameObject emptyObject { get; private set; }

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
    public static RectTransform _selectBoxTrans { get; private set; }
    private RectTransform _selectBoxParentTrans;

    // Indicates id of a player. Hardcoded as 0 (knights)
    // for testing
    private int _team = 0;

    void Start()
    {
        TryGetComponent(out _structurePlacementController);
        TryGetComponent(out _wallBuilder);
        emptyObject = new GameObject();
        _selectBoxAnchor = Vector3.zero;
        _selectedUnits = new List<UnitController>();
        RectTransform temp;
        _selectBox.TryGetComponent<RectTransform>(out temp);
        _selectBoxTrans = temp;
        _selectBoxParent.TryGetComponent<RectTransform>(out _selectBoxParentTrans);
    }

    void Update() {
        SelectUnitClick();
        DrawSelectBox();
        ChangeUnitsDestination();

    }


    /// <summary>
    ///     Changes a unit's destination based on right-clicking
    ///     terrain or structure
    ///     
    ///     Could be refactored
    /// </summary>
    void ChangeUnitsDestination() {

        // click right mouse button
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // collided with something
            if (Physics.Raycast(ray, out hit)) {
                // move empty marker to hit point so units face it when running
                emptyObject.transform.position = hit.point;

                // if hit a structure
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

                // if hit a unit
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

                // if just moving unit location
                foreach (UnitController controller in _selectedUnits) {
                    controller.NoDestination();    // nothing to attack
                    NavMeshAgent nav;
                    controller.gameObject.TryGetComponent<NavMeshAgent>(out nav);
                    nav.destination = hit.point;    // unit moves to clicked location
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

        // if pressed escape, clear units
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClearSelectedUnits();
        }
       
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
    

    /// <summary>
    /// Method to handle drawing unit select box
    /// </summary>
    public void DrawSelectBox() {

        // if we have released a select box, make it invisible
        //  (scale = 0)
        if (Input.GetMouseButtonUp(0)) {
            List<UnitController> myUnits;
            bool unitsExist = UnitController.GLOBAL_UNITS.TryGetValue(_team, out myUnits);
            if (unitsExist) {
                foreach (UnitController cont in UnitController.GLOBAL_UNITS[_team]) {
                    if (_selectBoxTrans.rect.Contains(cont.GetSelectionBoxPoint())) {
                        _selectedUnits.Add(cont);
                        cont.SetUnitSelected();
                    }
                }
            }
            _selectBoxTrans.sizeDelta = Vector2.zero;
        }

        // if we have started a select box (clicked down)
        if (Input.GetMouseButtonDown(0)) {
            // get the camera position of the cursor
            Vector3 mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_selectBoxParentTrans, mousePos, null, out _selectBoxAnchor);
        }

        // we are scaling box (holding down)
        if (Input.GetMouseButton(0)) {

            // get camera position of cursor now
            Vector3 mousePos = Input.mousePosition;
            Vector3 localMousePos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_selectBoxParentTrans, mousePos, null, out localMousePos);

            // diffs of anchor and cursor
            float w = localMousePos.x - _selectBoxAnchor.x;
            float h = localMousePos.y - _selectBoxAnchor.y;
            float abs_w = Mathf.Abs(w);
            float abs_h = Mathf.Abs(h);

            // scale box by diff
            _selectBoxTrans.sizeDelta = new Vector3(abs_w, abs_h, 0);

            Vector3 posVec = _selectBoxAnchor - new Vector3(0, abs_h, 0);
            
            // offset w and h if needed (can't render negative w / h)
            if (w < 0) {
                posVec.x -= abs_w;
            }
            if (h > 0) {
                posVec.y += abs_h;
            }
            _selectBoxTrans.anchoredPosition = posVec;
            
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