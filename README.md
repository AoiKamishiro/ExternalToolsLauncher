# External Tools Launcher
## 概要
Unityエディタの上部に任意のアプリの起動ボタンを追加する拡張機能です。  
データはEditorPrefs内に保存されるため、設定はマシン毎に保存されます。共同開発のプロジェクトでも問題なく活用できます。  
また、データのエクスポート機能もあるため、設定の移行も簡単にできます。  

## 使い方
Unity エディタ上部の再生ボタン横に各種機能が追加されます。クリックすることで開けます。

![SampleImage](./Textures/Readme/SampleImage.png)

## 導入
1. Unityのメニューバーから `Edit` -> `Project Settings` を開く  
2. Project Settings の左上にある `Package Manager` に、以下の内容で `Scoped Registries` を追加  
3. Name -> `ExternalToolsLauncher`, URL -> `https://package.openupm.com`, Scope(s) -> `online.kamishiro.externaltoolslauncher`  
4. Unityのメニューバーから `Windows` -> `Package Manager` を開く  
5. Package Manager の左上にある `+` ボタン右のドロップダウンから、`Custom` を選択  
6. 一覧の中から`External Tools Launcher`を探し、選択した画面の右下の `Install` をクリック  

![Image](./Textures/Readme/ProjectSettings.png)

## カスタマイズ
![SettingsImage](./Textures/Readme/Settings.png)
1. Unityのメニューバーから `Editor` -> `Preferences` を開く  
2. Preferencesウィンドウの左側から `External Tools Launcher` を開く  
3. 各プロファイルの設定が編集できます。  
  
1. ProfileName -> プロファイルの名前です。ツールチップにも表示されます。  
2. Visiblity -> このプロファイルを非表示にできます。  
3. Path -> 起動したいアプリケーションのパスを指定します。URLを指定することも可能です。  
4. Args -> 起動オプションとなる引数を指定します。{projectPath}などの変数を利用できます。  
5. Icon -> 一覧の中からアイコンを選択できます。アイコンは今後追加されていく予定です。  

## ライセンス
[MIT](./LICENSE.md)

### 参考ライブラリ
[Toolbar Extender](https://github.com/marijnz/unity-toolbar-extender/tree/master/Editor)
