using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UITestFrameworkSamples.Tests
{
    /// <summary>
    /// メインメニューのUIテスト
    ///
    /// ■ テストの命名規則
    /// [テスト対象]_[条件]_[期待結果] の形式で記述
    /// 例: StartButton_WhenClicked_ShouldStartGame
    ///
    /// ■ AAAパターン
    /// Arrange（準備）: テストに必要なオブジェクトや状態を準備
    /// Act（実行）: テスト対象の操作を実行
    /// Assert（検証）: 期待される結果を検証
    ///
    /// ■ テストの原則
    /// - 各テストは完全に独立して実行可能
    /// - 1つのテストで1つの事柄のみを検証（単一責任）
    /// - テストごとに新しい状態から開始（SetUp/TearDown活用）
    /// </summary>
    public class MainMenuTests : UITestFixture
    {
        [SetUp]
        public void SetUp()
        {
            panelSize = new Vector2(1920, 1080);
            CreateMenuUI();
        }

        [TearDown]
        public void TearDown()
        {
            rootVisualElement.Clear();
        }

        [Test]
        public void StartButton_WhenClicked_ShouldStartGame()
        {
            Debug.Log("=== StartButton_WhenClicked_ShouldStartGame 開始 ===");

            // Arrange
            var controller = new MainMenuController(rootVisualElement);
            var startButton = rootVisualElement.Q<Button>("start-button");
            simulate.FrameUpdate();
            Debug.Log($"準備完了: スタートボタンを取得しました（name: {startButton.name}）");

            Assert.IsFalse(controller.IsGameStarted, "事前条件: ゲームは開始されていない");

            // Act
            simulate.Click(startButton);
            Debug.Log("スタートボタンをクリックしました");

            // Assert
            Assert.IsTrue(controller.IsGameStarted, "ゲームが開始されるはずです");
            Debug.Log($"検証成功: ゲームが開始されました（IsGameStarted: {controller.IsGameStarted}）");
            Debug.Log("=== StartButton_WhenClicked_ShouldStartGame 成功 ===");
        }

        [Test]
        public void MainMenu_WhenGameStarts_ShouldBeHidden()
        {
            Debug.Log("=== MainMenu_WhenGameStarts_ShouldBeHidden 開始 ===");

            // Arrange
            var controller = new MainMenuController(rootVisualElement);
            var startButton = rootVisualElement.Q<Button>("start-button");
            var mainMenuContainer = rootVisualElement.Q<VisualElement>("main-menu-container");
            simulate.FrameUpdate();
            Debug.Log("準備完了: メインメニューコンテナとスタートボタンを取得しました");

            Debug.Log($"初期状態: メインメニューの表示状態 = {mainMenuContainer.style.display.value}");

            // Act
            simulate.Click(startButton);
            simulate.FrameUpdate();
            Debug.Log("スタートボタンをクリックし、画面を更新しました");

            // Assert
            Debug.Log($"クリック後: メインメニューの表示状態 = {mainMenuContainer.style.display.value}");
            Assert.AreEqual(DisplayStyle.None, mainMenuContainer.style.display.value,
                "メインメニューが非表示になるはずです");
            Debug.Log("検証成功: メインメニューが非表示になりました");
            Debug.Log("=== MainMenu_WhenGameStarts_ShouldBeHidden 成功 ===");
        }

        [Test]
        public void MainMenu_OnInitialization_ShouldHaveAllRequiredButtons()
        {
            // Arrange & Act
            simulate.FrameUpdate();

            // Assert
            AssertButtonExists("start-button", "ゲームを始める");
            AssertButtonExists("continue-button", "続きから");
            AssertButtonExists("settings-button", "設定");
            AssertButtonExists("credits-button", "クレジット");
            AssertButtonExists("quit-button", "終了");
        }

        [Test]
        public void StartButton_WhenClickedMultipleTimes_ShouldProcessOnlyOnce()
        {
            // Arrange
            var controller = new MainMenuController(rootVisualElement);
            var startButton = rootVisualElement.Q<Button>("start-button");
            var clickCount = 0;
            controller.OnGameStart += () => clickCount++;
            simulate.FrameUpdate();

            // Act
            simulate.Click(startButton);
            simulate.Click(startButton);
            simulate.Click(startButton);

            // Assert
            Assert.AreEqual(1, clickCount, "イベントは一度だけ処理されるはずです");
        }

        #region ヘルパーメソッド

        private void AssertButtonExists(string buttonName, string expectedText)
        {
            var button = rootVisualElement.Q<Button>(buttonName);
            Assert.IsNotNull(button, $"{buttonName}が存在するはずです");
            Assert.AreEqual(expectedText, button.text,
                $"{buttonName}のテキストが'{expectedText}'であるはずです");
        }

        private void CreateMenuUI()
        {
            var mainMenuContainer = new VisualElement();
            mainMenuContainer.name = "main-menu-container";
            mainMenuContainer.style.width = Length.Percent(100);
            mainMenuContainer.style.height = Length.Percent(100);

            var content = new VisualElement();
            content.name = "content";
            content.style.width = Length.Percent(100);
            content.style.height = Length.Percent(100);
            content.style.alignItems = Align.Center;
            content.style.justifyContent = Justify.Center;

            var title = new Label("SAMPLE GAME");
            title.name = "game-title";
            title.style.fontSize = 72;
            title.style.color = new Color(0.17f, 0.17f, 0.17f);
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 60;
            content.Add(title);

            var buttonContainer = new VisualElement();
            buttonContainer.name = "button-container";
            buttonContainer.style.alignItems = Align.Center;
            buttonContainer.style.width = 400;

            CreateButton(buttonContainer, "start-button", "ゲームを始める");
            CreateButton(buttonContainer, "continue-button", "続きから");
            CreateButton(buttonContainer, "settings-button", "設定");
            CreateButton(buttonContainer, "credits-button", "クレジット");
            CreateButton(buttonContainer, "quit-button", "終了");

            content.Add(buttonContainer);
            mainMenuContainer.Add(content);
            rootVisualElement.Add(mainMenuContainer);
        }

        private void CreateButton(VisualElement parent, string name, string text)
        {
            var button = new Button();
            button.name = name;
            button.text = text;
            button.style.width = 350;
            button.style.height = 60;
            button.style.marginBottom = 15;
            button.style.fontSize = 24;
            parent.Add(button);
        }

        #endregion
    }
}