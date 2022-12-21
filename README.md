# External Tools Launcher

## 概要

Unity エディタの上部に任意のアプリの起動ボタンを追加する拡張機能です。  
データは EditorPrefs 内に保存されるため、設定はマシン毎に保存されます。共同開発のプロジェクトでも問題なく活用できます。  
また、データのエクスポート機能もあるため、設定の移行も簡単にできます。

## 使い方

Unity エディタ上部の再生ボタン横に各種機能が追加されます。クリックすることで開けます。

![SampleImage](./Textures/Readme/SampleImage.png)

## 導入

1. Unity のメニューバーから `Edit` -> `Project Settings` を開く
2. Project Settings の左上にある `Package Manager` に、次の内容で `Scoped Registries` を追加
   > `Name` -> `ExternalToolsLauncher`  
   > `URL` -> `https://package.openupm.com`  
   > `Scope(s)` -> `online.kamishiro.externaltoolslauncher`
3. Unity のメニューバーから `Windows` -> `Package Manager` を開く
4. Package Manager の左上にある `+` ボタン右のドロップダウンから、`My Registries` を選択
5. 一覧の中から`External Tools Launcher`を探し、選択した画面の右下の `Install` をクリック

![Image](./Textures/Readme/ProjectSettings.png)

## カスタマイズ

1. Unity のメニューバーから `Editor` -> `Preferences` を開く
2. Preferences ウィンドウの左側から `External Tools Launcher` を開く
3. 各プロファイルの設定が編集できます。
   > `ProfileName` -> プロファイルの名前です。ツールチップにも表示されます。  
   > `Visibility`-> このプロファイルを非表示にできます。  
   > `Path` -> 起動したいアプリケーションのパスを指定します。URL を指定することも可能です。  
   > `Arguments` -> 起動オプションとなる引数を指定します。{projectPath}などの変数を利用できます。  
   > `UseExternalIcon` -> このツールに内蔵されていない画像をアイコンとして使用します。  
   > `Internal Icon` -> このツールに内蔵されたアイコンです。一覧から選択できます。アイコンは今後追加されていく予定です。  
   > `External Icon` -> アイコンとして使用したい外部の画像を指定します。

![SettingsImage](./Textures/Readme/Settings.png)

## 更新

1. Unity のメニューバーから `Windows` -> `Package Manager` を開く
2. Package Manager の左上にある `+` ボタン右のドロップダウンから、`In Project` を選択
3. `Kamishiro Technologies`カテゴリ内の`External Tools launcher`を選択
4. Package Manager の右下にある`Update to x.x.xx`のボタンをクリック

![Update](./Textures/Readme/Update.png)

## ライセンス

[MIT](./LICENSE.md)

### 利用ライブラリ

[Toolbar Extender](https://github.com/marijnz/unity-toolbar-extender/tree/master/Editor)  
[windows-terminal-icons](https://github.com/TheFern2/windows-terminal-icons)
