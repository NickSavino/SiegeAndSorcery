using UnityEngine;

public class DefenderUIController : MonoBehaviour
{

    private WallBuildingController _wallBuilder;

    private StructurePlacementController _structurePlacementController;

    public StructureName selectedStructure = StructureName.None;

    private bool _controlsEnabled = false;

    public void Start()
    {
        TryGetComponent(out _structurePlacementController);
        TryGetComponent(out _wallBuilder);
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