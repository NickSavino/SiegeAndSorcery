using UnityEngine;

public class DefenderUIController : MonoBehaviour
{

    private WallBuildingController _wallBuilder;

    private StructurePlacementController _structurePlacementController;

    public StructureName selectedStructure = StructureName.None;

    public void Start()
    {
        _structurePlacementController = GetComponent<StructurePlacementController>();
        _wallBuilder = GetComponent<WallBuildingController>();
    }

    public void OnWallButtonClick()
    {
        if (selectedStructure != StructureName.None)
        {
            selectedStructure = _structurePlacementController.SetType(StructureName.None);
        }

        _wallBuilder.SetBuildModeActive(!_wallBuilder.IsBuildModeActive());
    }

    public void OnTowerButtonClick()
    {
        if (_wallBuilder.IsBuildModeActive())
        {
            _wallBuilder.SetBuildModeActive(false);
        }

        selectedStructure = _structurePlacementController.SetType(selectedStructure == StructureName.BaseTower ? StructureName.None : StructureName.BaseTower);
    }
}
