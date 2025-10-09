using UnityEngine;
using UnityEngine.UIElements;

namespace UITestFrameworkSamples
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;

        [SerializeField]
        private VisualTreeAsset inventoryUXML;

        private InventoryController controller;

        void Start()
        {
            if (inventoryUXML != null)
            {
                uiDocument.visualTreeAsset = inventoryUXML;
            }

            controller = new InventoryController(uiDocument.rootVisualElement);
        }
    }
}