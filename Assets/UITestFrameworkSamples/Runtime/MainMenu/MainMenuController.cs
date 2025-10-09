using UnityEngine;
using UnityEngine.UIElements;

namespace UITestFrameworkSamples
{
    public class MainMenuController
    {
        private VisualElement root;

        public bool IsGameStarted { get; private set; }
        public System.Action OnGameStart;
        public System.Action OnGameQuit;

        public MainMenuController(VisualElement rootElement)
        {
            this.root = rootElement;
            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            var startButton = root.Q<Button>("start-button");
            if (startButton != null)
            {
                startButton.clicked += () =>
                {
                    IsGameStarted = true;
                    HideMainMenu();
                    ShowGameScreen();
                    OnGameStart?.Invoke();
                    Debug.Log("Game Started");
                };
            }

            var continueButton = root.Q<Button>("continue-button");
            if (continueButton != null)
            {
                continueButton.clicked += () =>
                {
                    IsGameStarted = true;
                    HideMainMenu();
                    ShowGameScreen();
                    Debug.Log("Continue Game");
                };
            }

            var settingsButton = root.Q<Button>("settings-button");
            if (settingsButton != null)
            {
                settingsButton.clicked += () => Debug.Log("Open Settings");
            }

            var creditsButton = root.Q<Button>("credits-button");
            if (creditsButton != null)
            {
                creditsButton.clicked += () => Debug.Log("Open Credits");
            }

            var quitButton = root.Q<Button>("quit-button");
            if (quitButton != null)
            {
                quitButton.clicked += () =>
                {
                    OnGameQuit?.Invoke();
                    Debug.Log("Quit Game");
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit();
                    #endif
                };
            }
        }

        private void HideMainMenu()
        {
            var mainMenuContainer = root.Q<VisualElement>("main-menu-container");
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
            }
        }

        private void ShowGameScreen()
        {
            var gameScreen = root.Q<VisualElement>("game-screen");
            if (gameScreen != null)
            {
                gameScreen.style.display = DisplayStyle.Flex;
            }
        }
    }
}