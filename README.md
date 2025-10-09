# Unity UI Test Framework Sample Project

Unity 6.3におけるUI Test Frameworkのサンプルプロジェクトです。

## 概要

このプロジェクトは、Unity UI Test Frameworkを使用したUI自動テストの実装例を提供します。
UI Toolkitで構築されたUIのテストケースを含んでいます。

## 動作環境

- Unity 6.3.0f1以降
- UI Test Framework 1.0.0
- Unity Test Framework 1.5.1

## プロジェクト構成

```
Assets/
├── UITestFrameworkSamples/
│   ├── Runtime/
│   │   ├── MainMenu/          # メインメニューのサンプル実装
│   │   └── Inventory/         # インベントリのサンプル実装
│   ├── Tests/
│   │   ├── Editor/            # Editorモードテスト
│   │   └── Runtime/           # Runtimeモードテスト
│   ├── Scenes/                # サンプルシーン
│   └── Resources/             # UI Test Frameworkの設定ファイル
```

## セットアップ

1. このリポジトリをクローン
2. Unity 6.3.0f1以降でプロジェクトを開く
3. Package Managerで必要なパッケージが自動的にインストールされます

## サンプルの実行方法

### シーンの実行

1. `Assets/UITestFrameworkSamples/Scenes/MainMenuSampleScene.unity` を開く
2. Playボタンを押してサンプルUIを確認

### テストの実行

1. Test Runnerウィンドウを開く（Window > General > Test Runner）
2. PlayModeタブまたはEditModeタブを選択
3. Run Allボタンでテストを実行

## サンプル一覧

### MainMenu（メインメニュー）

UI Toolkitで実装されたメインメニューのサンプルです。

- **シーン**: `MainMenuSampleScene.unity`
- **テスト**: `Assets/UITestFrameworkSamples/Tests/Editor/MainMenuTests.cs`

### Inventory（インベントリ）

UI Toolkitで実装されたインベントリシステムのサンプルです。

- **シーン**: `InventorySampleScene.unity`
- **テスト**: `Assets/UITestFrameworkSamples/Tests/Editor/InventoryTests.cs`

## 注意事項

### Game Viewの解像度設定

このサンプルはFullHD（1920x1080）の解像度を想定して作成されています。
**Game Viewの解像度を1920x1080に設定してください。**

1. Game Viewの左上のドロップダウンから「1920x1080」を選択
2. 1920x1080が選択肢にない場合は、「+」ボタンから追加してください
