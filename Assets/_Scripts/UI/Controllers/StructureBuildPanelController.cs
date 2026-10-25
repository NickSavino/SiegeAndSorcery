using System;
using _Scripts.Controllers;
using _Scripts.UI.Components;
using UnityEngine;
namespace _Scripts.UI.Controllers
{
    public class StructureBuildPanelController : MonoBehaviour
    {
        [SerializeField]
        StructureBuildPanel panel;

        [SerializeField]
        BuildableStructurePlacementController placementController;

        void OnEnable()
        {
            panel.PlacementRequested += placementController.SelectStructure;
            panel.CancelRequested += placementController.CancelPlacement;

            placementController.SelectionChanged += panel.SetSelectedStructure;
            
            panel.SetSelectedStructure(placementController.SelectedStructure);
        }

        void OnDisable()
        {
            panel.PlacementRequested -= placementController.SelectStructure;
            panel.CancelRequested -= placementController.CancelPlacement;
            
            placementController.SelectionChanged -= panel.SetSelectedStructure;
        }

        void Start()
        {
            panel.Populate(placementController.AvailableStrucutres);
        }
    }
}
