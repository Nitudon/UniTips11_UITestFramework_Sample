using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.TestFramework;

namespace UITestFrameworkSamples.Tests
{
    /// <summary>
    /// インベントリシステムのUIテスト
    ///
    /// ■ テストの命名規則
    /// [テスト対象]_[条件]_[期待結果] の形式で記述
    /// 例: AddItem_WhenInventoryIsEmpty_ShouldAddSuccessfully
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
    public class InventoryTests : UITestFixture
    {
        private InventoryController controller;

        [SetUp]
        public void SetUp()
        {
            panelSize = new Vector2(1920, 1080);
            CreateInventoryUI();
            controller = new InventoryController(rootVisualElement);
            simulate.FrameUpdate();
        }

        [TearDown]
        public void TearDown()
        {
            rootVisualElement.Clear();
            controller = null;
        }

        [Test]
        public void AddItem_WhenInventoryIsEmpty_ShouldAddSuccessfully()
        {
            Debug.Log("=== AddItem_WhenInventoryIsEmpty_ShouldAddSuccessfully 開始 ===");

            // Arrange
            var item = new InventoryItem("Apple", "A fresh red apple");
            Assert.AreEqual(0, controller.ItemCount, "事前条件: インベントリは空");
            Debug.Log("準備完了: インベントリは空の状態です");

            // Act
            bool result = controller.AddItem(item);
            Debug.Log($"アイテムを追加しました（結果: {result}）");

            // Assert
            Assert.IsTrue(result, "アイテムの追加が成功するはずです");
            Assert.AreEqual(1, controller.ItemCount, "アイテム数が1になるはずです");
            Assert.Contains(item, controller.Items, "アイテムがリストに含まれるはずです");
            Debug.Log($"検証成功: アイテム数={controller.ItemCount}、リストに含まれる={controller.Items.Contains(item)}");
            Debug.Log("=== AddItem_WhenInventoryIsEmpty_ShouldAddSuccessfully 成功 ===");
        }

        [Test]
        public void AddItem_WhenInventoryIsFull_ShouldReturnFalse()
        {
            Debug.Log("=== AddItem_WhenInventoryIsFull_ShouldReturnFalse 開始 ===");

            // Arrange
            for (int i = 0; i < 8; i++)
            {
                controller.AddItem(new InventoryItem($"Item{i}"));
            }
            Assert.IsTrue(controller.IsFull, "事前条件: インベントリが満杯");
            Debug.Log($"準備完了: インベントリに8個のアイテムを追加しました（満杯: {controller.IsFull}）");

            // Act
            var newItem = new InventoryItem("NewItem");
            bool result = controller.AddItem(newItem);
            Debug.Log($"9個目のアイテム追加を試行しました（結果: {result}）");

            // Assert
            Assert.IsFalse(result, "満杯時の追加は失敗するはずです");
            Assert.AreEqual(8, controller.ItemCount, "アイテム数は変わらないはずです");
            Assert.IsFalse(controller.Items.Contains(newItem), "新規アイテムは追加されないはずです");
            Debug.Log($"検証成功: アイテム数={controller.ItemCount}、追加失敗={!result}");
            Debug.Log("=== AddItem_WhenInventoryIsFull_ShouldReturnFalse 成功 ===");
        }

        [Test]
        public void ItemSlot_WhenClicked_ShouldSelectItem()
        {
            Debug.Log("=== ItemSlot_WhenClicked_ShouldSelectItem 開始 ===");

            // Arrange
            var item = new InventoryItem("Key", "An old rusty key");
            controller.AddItem(item);
            var itemSlot = rootVisualElement.Q<Button>("item-slot-0");
            simulate.FrameUpdate();
            Debug.Log("準備完了: アイテムを追加し、スロットを取得しました");

            // Act
            simulate.Click(itemSlot);
            simulate.FrameUpdate();
            Debug.Log("アイテムスロットをクリックしました");

            // Assert
            Assert.AreEqual(item, controller.SelectedItem, "クリックしたアイテムが選択されるはずです");
            Assert.IsTrue(itemSlot.ClassListContains("selected"), "選択状態のCSSクラスが付与されるはずです");
            Debug.Log($"検証成功: 選択アイテム={controller.SelectedItem.Name}、CSSクラス付与={itemSlot.ClassListContains("selected")}");
            Debug.Log("=== ItemSlot_WhenClicked_ShouldSelectItem 成功 ===");
        }

        [Test]
        public void ItemDetail_WhenItemSelected_ShouldShowDetails()
        {
            // Arrange
            var item = new InventoryItem("Stone", "A smooth round stone");
            controller.AddItem(item);
            var nameLabel = rootVisualElement.Q<Label>("item-name");
            var descLabel = rootVisualElement.Q<Label>("item-description");

            // Act
            controller.SelectItem(0);
            simulate.FrameUpdate();

            // Assert
            Assert.AreEqual("Stone", nameLabel.text, "アイテム名が表示されるはずです");
            Assert.AreEqual("A smooth round stone", descLabel.text, "説明文が表示されるはずです");
        }

        [Test]
        public void UseButton_WhenClicked_ShouldRemoveItem()
        {
            Debug.Log("=== UseButton_WhenClicked_ShouldRemoveItem 開始 ===");

            // Arrange
            var item = new InventoryItem("Bottle", "An empty glass bottle");
            controller.AddItem(item);
            controller.SelectItem(0);

            var useButton = rootVisualElement.Q<Button>("use-button");
            bool itemUsed = false;
            controller.OnItemUsed += (usedItem) => itemUsed = true;
            simulate.FrameUpdate();
            Debug.Log("準備完了: アイテムを追加し、選択しました");

            // Act
            simulate.Click(useButton);
            simulate.FrameUpdate();
            Debug.Log("Useボタンをクリックしました");

            // Assert
            Assert.IsTrue(itemUsed, "アイテム使用イベントが発火するはずです");
            Assert.AreEqual(0, controller.ItemCount, "アイテムが削除されるはずです");
            Assert.IsNull(controller.SelectedItem, "選択状態がクリアされるはずです");
            Debug.Log($"検証成功: イベント発火={itemUsed}、アイテム数={controller.ItemCount}、選択クリア={controller.SelectedItem == null}");
            Debug.Log("=== UseButton_WhenClicked_ShouldRemoveItem 成功 ===");
        }

        [Test]
        public void DiscardButton_WhenClicked_ShouldRemoveItem()
        {
            // Arrange
            var item = new InventoryItem("Coin", "A shiny gold coin");
            controller.AddItem(item);
            controller.SelectItem(0);

            var discardButton = rootVisualElement.Q<Button>("discard-button");
            simulate.FrameUpdate();

            // Act
            simulate.Click(discardButton);
            simulate.FrameUpdate();

            // Assert
            Assert.AreEqual(0, controller.ItemCount, "アイテムが削除されるはずです");
            Assert.IsNull(controller.SelectedItem, "選択状態がクリアされるはずです");
        }

        [Test]
        public void CloseButton_WhenClicked_ShouldHideInventory()
        {
            // Arrange
            var closeButton = rootVisualElement.Q<Button>("close-button");
            var inventoryContainer = rootVisualElement.Q<VisualElement>("inventory-container");
            simulate.FrameUpdate();

            Assert.AreEqual(DisplayStyle.Flex, inventoryContainer.style.display.value,
                "事前条件: インベントリが表示されている");

            // Act
            simulate.Click(closeButton);
            simulate.FrameUpdate();

            // Assert
            Assert.AreEqual(DisplayStyle.None, inventoryContainer.style.display.value,
                "インベントリが非表示になるはずです");
        }

        [Test]
        public void EmptySlot_WhenClicked_ShouldClearSelection()
        {
            // Arrange
            var item = new InventoryItem("Apple");
            controller.AddItem(item);
            controller.SelectItem(0);
            Assert.IsNotNull(controller.SelectedItem, "事前条件: アイテムが選択されている");

            var emptySlot = rootVisualElement.Q<Button>("item-slot-1");
            simulate.FrameUpdate();

            // Act
            simulate.Click(emptySlot);
            simulate.FrameUpdate();

            // Assert
            Assert.IsNull(controller.SelectedItem, "選択がクリアされるはずです");
            var nameLabel = rootVisualElement.Q<Label>("item-name");
            Assert.AreEqual("選択されていません", nameLabel.text, "詳細表示がリセットされるはずです");
        }

        #region ヘルパーメソッド

        private void CreateInventoryUI()
        {
            var container = new VisualElement();
            container.name = "inventory-container";
            container.style.width = Length.Percent(100);
            container.style.height = Length.Percent(100);
            container.style.backgroundColor = new Color(0.16f, 0.16f, 0.24f);

            var header = CreateHeader();
            container.Add(header);

            var content = new VisualElement();
            content.name = "content";
            content.style.flexDirection = FlexDirection.Row;
            content.style.flexGrow = 1;
            content.style.paddingTop = 20;
            content.style.paddingBottom = 20;
            content.style.paddingLeft = 20;
            content.style.paddingRight = 20;

            var itemGrid = CreateItemGrid();
            content.Add(itemGrid);

            var itemDetail = CreateItemDetail();
            content.Add(itemDetail);

            container.Add(content);
            rootVisualElement.Add(container);
        }

        private VisualElement CreateHeader()
        {
            var header = new VisualElement();
            header.name = "header";
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.height = 60;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 20;

            var title = new Label("インベントリ");
            title.name = "inventory-title";
            title.style.fontSize = 28;
            title.style.color = Color.white;
            header.Add(title);

            var closeButton = new Button();
            closeButton.name = "close-button";
            closeButton.text = "×";
            closeButton.style.width = 40;
            closeButton.style.height = 40;
            closeButton.style.fontSize = 24;
            header.Add(closeButton);

            return header;
        }

        private VisualElement CreateItemGrid()
        {
            var gridContainer = new VisualElement();
            gridContainer.name = "item-grid-container";
            gridContainer.style.flexGrow = 2;
            gridContainer.style.marginRight = 20;

            var grid = new VisualElement();
            grid.name = "item-grid";
            grid.style.flexDirection = FlexDirection.Row;
            grid.style.flexWrap = Wrap.Wrap;

            for (int i = 0; i < 8; i++)
            {
                var slot = new Button();
                slot.name = $"item-slot-{i}";
                slot.AddToClassList("item-slot");
                slot.AddToClassList("empty-slot");
                slot.style.width = 100;
                slot.style.height = 100;
                slot.style.marginTop = 8;
            slot.style.marginBottom = 8;
            slot.style.marginLeft = 8;
            slot.style.marginRight = 8;
                slot.style.backgroundColor = new Color(0.12f, 0.16f, 0.22f);

                var label = new Label();
                label.name = $"item-label-{i}";
                slot.Add(label);

                grid.Add(slot);
            }

            gridContainer.Add(grid);
            return gridContainer;
        }

        private VisualElement CreateItemDetail()
        {
            var detail = new VisualElement();
            detail.name = "item-detail";
            detail.style.flexGrow = 1;
            detail.style.backgroundColor = new Color(0.12f, 0.16f, 0.22f);
            detail.style.paddingTop = 20;
            detail.style.paddingBottom = 20;
            detail.style.paddingLeft = 20;
            detail.style.paddingRight = 20;
            detail.style.borderTopLeftRadius = 10;
            detail.style.borderTopRightRadius = 10;
            detail.style.borderBottomLeftRadius = 10;
            detail.style.borderBottomRightRadius = 10;

            var nameLabel = new Label("選択されていません");
            nameLabel.name = "item-name";
            nameLabel.style.fontSize = 24;
            nameLabel.style.color = Color.white;
            nameLabel.style.marginBottom = 15;
            detail.Add(nameLabel);

            var descLabel = new Label("");
            descLabel.name = "item-description";
            descLabel.style.fontSize = 16;
            descLabel.style.color = new Color(0.82f, 0.84f, 0.86f);
            descLabel.style.flexGrow = 1;
            detail.Add(descLabel);

            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.justifyContent = Justify.SpaceAround;

            var useButton = new Button();
            useButton.name = "use-button";
            useButton.text = "使用";
            useButton.style.width = 120;
            useButton.style.height = 45;
            buttonContainer.Add(useButton);

            var discardButton = new Button();
            discardButton.name = "discard-button";
            discardButton.text = "破棄";
            discardButton.style.width = 120;
            discardButton.style.height = 45;
            buttonContainer.Add(discardButton);

            detail.Add(buttonContainer);
            return detail;
        }

        #endregion
    }
}