using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UITestFrameworkSamples
{
    /// <summary>
    /// インベントリアイテム
    /// </summary>
    public class InventoryItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public InventoryItem(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
    
    /// <summary>
    /// インベントリシステムのコントローラー
    /// </summary>
    public class InventoryController
    {
        // プロパティ
        public List<InventoryItem> Items => new List<InventoryItem>(items);
        public InventoryItem SelectedItem => selectedItem;
        public int ItemCount => items.Count;
        public bool IsFull => items.Count >= MaxSlots;
        public bool IsVisible
        {
            get
            {
                var container = rootElement.Q("inventory-container");
                return container?.style.display.value != DisplayStyle.None;
            }
        }
        
        private readonly VisualElement rootElement;
        private readonly List<InventoryItem> items;
        private InventoryItem selectedItem;
        private Button selectedSlot;
        private const int MaxSlots = 8;

        // イベント
        public event Action<InventoryItem> OnItemUsed;
        public event Action<InventoryItem> OnItemSelected;

        public InventoryController(VisualElement root)
        {
            rootElement = root;
            items = new List<InventoryItem>();
            Initialize();
        }

        private void Initialize()
        {
            // 閉じるボタン
            var closeButton = rootElement.Q<Button>("close-button");
            closeButton?.RegisterCallback<ClickEvent>(evt => CloseInventory());

            // アイテムスロットの初期化
            for (int i = 0; i < MaxSlots; i++)
            {
                var slot = rootElement.Q<Button>($"item-slot-{i}");
                if (slot != null)
                {
                    int slotIndex = i;
                    slot.RegisterCallback<ClickEvent>(evt => OnSlotClicked(slotIndex));
                }
            }

            // アクションボタン
            var useButton = rootElement.Q<Button>("use-button");
            var discardButton = rootElement.Q<Button>("discard-button");

            useButton?.RegisterCallback<ClickEvent>(evt => UseSelectedItem());
            discardButton?.RegisterCallback<ClickEvent>(evt => DiscardSelectedItem());

            // Pick Upボタン
            var pickupButton = rootElement.Q<Button>("pickup-button");
            pickupButton?.RegisterCallback<ClickEvent>(evt => PickUpItem());

            // 初期状態ではボタンを無効化
            SetActionButtonsEnabled(false);
        }

        /// <summary>
        /// アイテムを追加
        /// </summary>
        public bool AddItem(InventoryItem item)
        {
            if (items.Count >= MaxSlots)
            {
                return false;
            }

            items.Add(item);
            UpdateSlot(items.Count - 1);
            return true;
        }

        /// <summary>
        /// アイテムを削除
        /// </summary>
        public bool RemoveItem(InventoryItem item)
        {
            int index = items.IndexOf(item);
            if (index < 0) return false;

            items.RemoveAt(index);
            UpdateAllSlots();

            if (selectedItem == item)
            {
                ClearSelection();
            }

            return true;
        }

        /// <summary>
        /// スロットがクリックされた時
        /// </summary>
        private void OnSlotClicked(int slotIndex)
        {
            if (slotIndex >= items.Count)
            {
                ClearSelection();
                return;
            }

            SelectItem(slotIndex);
        }

        /// <summary>
        /// アイテムを選択
        /// </summary>
        public void SelectItem(int slotIndex)
        {
            if (slotIndex >= items.Count) return;

            // 前の選択を解除
            if (selectedSlot != null)
            {
                selectedSlot.RemoveFromClassList("selected");
            }

            // 新しい選択
            selectedItem = items[slotIndex];
            var slot = rootElement.Q<Button>($"item-slot-{slotIndex}");
            if (slot != null)
            {
                slot.AddToClassList("selected");
                selectedSlot = slot;
            }

            // 詳細を更新
            UpdateItemDetail(selectedItem);
            SetActionButtonsEnabled(true);
            OnItemSelected?.Invoke(selectedItem);
        }

        /// <summary>
        /// 選択をクリア
        /// </summary>
        private void ClearSelection()
        {
            if (selectedSlot != null)
            {
                selectedSlot.RemoveFromClassList("selected");
                selectedSlot = null;
            }
            selectedItem = null;
            UpdateItemDetail(null);
            SetActionButtonsEnabled(false);
        }

        /// <summary>
        /// アイテム詳細を更新
        /// </summary>
        private void UpdateItemDetail(InventoryItem item)
        {
            var nameLabel = rootElement.Q<Label>("item-name");
            var descLabel = rootElement.Q<Label>("item-description");

            if (item != null)
            {
                nameLabel.text = item.Name;
                descLabel.text = item.Description;
            }
            else
            {
                nameLabel.text = "選択されていません";
                descLabel.text = "";
            }
        }

        /// <summary>
        /// スロットを更新
        /// </summary>
        private void UpdateSlot(int index)
        {
            var slot = rootElement.Q<Button>($"item-slot-{index}");
            var label = rootElement.Q<Label>($"item-label-{index}");

            if (slot == null) return;

            if (index < items.Count)
            {
                var item = items[index];
                slot.RemoveFromClassList("empty-slot");
                label.text = item.Name;
            }
            else
            {
                slot.AddToClassList("empty-slot");
                label.text = "";
            }
        }

        /// <summary>
        /// 全スロットを更新
        /// </summary>
        private void UpdateAllSlots()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                UpdateSlot(i);
            }
        }

        /// <summary>
        /// アクションボタンの有効/無効を設定
        /// </summary>
        private void SetActionButtonsEnabled(bool enabled)
        {
            var useButton = rootElement.Q<Button>("use-button");
            var discardButton = rootElement.Q<Button>("discard-button");

            useButton?.SetEnabled(enabled);
            discardButton?.SetEnabled(enabled);
        }

        /// <summary>
        /// 選択中のアイテムを使用
        /// </summary>
        public void UseSelectedItem()
        {
            if (selectedItem == null) return;

            OnItemUsed?.Invoke(selectedItem);
            RemoveItem(selectedItem);
        }

        /// <summary>
        /// 選択中のアイテムを破棄
        /// </summary>
        public void DiscardSelectedItem()
        {
            if (selectedItem == null) return;
            RemoveItem(selectedItem);
        }

        /// <summary>
        /// フィールドからアイテムを拾う
        /// </summary>
        public void PickUpItem()
        {
            if (IsFull) return;

            string[] itemNames = { "Apple", "Bottle", "Coin", "Key", "Stone" };
            string[] descriptions = {
                "A fresh red apple",
                "An empty glass bottle",
                "A shiny gold coin",
                "An old rusty key",
                "A smooth round stone"
            };

            int index = items.Count % itemNames.Length;
            AddItem(new InventoryItem(itemNames[index], descriptions[index]));
        }

        /// <summary>
        /// インベントリを閉じる
        /// </summary>
        public void CloseInventory()
        {
            var container = rootElement.Q("inventory-container");
            if (container != null)
            {
                container.style.display = DisplayStyle.None;
            }
        }
    }
}