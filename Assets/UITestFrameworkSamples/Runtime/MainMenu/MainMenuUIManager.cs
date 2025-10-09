using UnityEngine;
using UnityEngine.UIElements;

namespace UITestFrameworkSamples
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;

        [SerializeField]
        private VisualTreeAsset mainMenuUXML;

        private MainMenuController controller;

        void Start()
        {
            if (mainMenuUXML != null)
            {
                uiDocument.visualTreeAsset = mainMenuUXML;
            }

            controller = new MainMenuController(uiDocument.rootVisualElement);
            controller.OnGameStart += () => Debug.Log("ゲーム開始");
            controller.OnGameQuit += () => Debug.Log("ゲーム終了");
        }
    }
}