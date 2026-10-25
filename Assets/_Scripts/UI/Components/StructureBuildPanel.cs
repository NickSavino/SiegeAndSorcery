using System;
using System.Collections.Generic;
using _Scripts.BuildableStructures;
using TMPro;
using UnityEngine;
namespace _Scripts.UI.Components
{
    public class StructureBuildPanel : MonoBehaviour
    {
        [SerializeField]
        MenuButton buttonPrefab;

        [SerializeField]
        Transform buttonContainer;

        [SerializeField]
        UnityEngine.UI.Button cancelButton;

        [SerializeField]
        TMP_Text selectionLabel;

        public event Action<BuildableStructure> PlacementRequested;
        public event Action CancelRequested;

        void OnEnable()
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        void OnDisable()
        {
            cancelButton.onClick.RemoveListener(OnCancelClicked);
        }

        public void Populate(IReadOnlyList<BuildableStructure> structures)
        {
            foreach (var structure in structures)
            {
                if (structure == null)
                {
                    continue;
                }

                var button = Instantiate(buttonPrefab, buttonContainer);
                
                button.Initialize(structure.name, () => PlacementRequested?.Invoke(structure));
            }
        }

        public void SetSelectedStructure(BuildableStructure structure)
        {
            selectionLabel.text = structure == null
                ? "Build: None"
                : $"Build: {structure.name}";

            cancelButton.interactable = structure != null;
        }


        void OnCancelClicked()
        {
            CancelRequested?.Invoke();
        }
    }
}
