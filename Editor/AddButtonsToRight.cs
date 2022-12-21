using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.externaltoolslauncher
{
    [InitializeOnLoad]
    public static class AddButtonsToRight
    {
        /// <summary>
        /// {ProjectPath}と置換される値。プロジェクトフォルダの絶対パス。
        /// </summary>
        private static readonly string dirPath = Directory.GetParent(Application.dataPath).FullName;
        /// <summary>
        /// {ProjectName}と置換される値。プロジェクトフォルダの名前。
        /// </summary>
        private static readonly string projName = Directory.GetParent(Application.dataPath).Name;
        /// <summary>
        /// {SlnName}と置換される値。ソリューションファイルの名前。
        /// </summary>
        private static readonly string slnName = $"{Directory.GetParent(Application.dataPath).Name}.sln";
        /// <summary>
        /// {SelectPath}と置換される値。選択されたファイパス名。
        /// </summary>
        private static string selectPath;
        static AddButtonsToRight() => Init();

        /// <summary>
        /// 初期化処理を行います。
        /// InitializeOnLoad属性により読み込まれた直後に実行されます。
        /// </summary>
        private static void Init()
        {
            //ソリューションファイルが存在しない場合のみ生成します。
            if (!File.Exists($"{dirPath}/{slnName}"))
            {
                typeof(Editor).Assembly.GetType("UnityEditor.SyncVS").GetMethod("SyncSolution").Invoke(null, null);
            }

            //ツールバーの右側にGUI処理を追加します。
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        }

        /// <summary>
        /// ツールバーの右側に追加されるGUIの描画を行います。
        /// </summary>
        private static void OnRightToolbarGUI()
        {
            foreach (SaveData.Profile p in SettingsResiter.SaveData.Profiles)
            {
                if (p.Show && p.IconTexture != null)
                {
                    if (GUILayout.Button(new GUIContent(null, p.IconTexture, p.Name), "Command"))
                    {
                        string file = GetReplacedString(p.Path);
                        string args = GetReplacedString(p.Args);
                        Process process = new Process();
                        process.StartInfo.FileName = file;
                        process.StartInfo.Arguments = args;
                        process.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 変数を置換した値を返します。
        /// </summary>
        /// <param name="str">置換まえの文字列</param>
        /// <returns></returns>
        private static string GetReplacedString(string str)
        {
            selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            return str.
                Replace("{ProjectPath}", dirPath).
                Replace("{ProjectName}", projName).
                Replace("{SlnName}", slnName).
                Replace("{SelectPath}", selectPath);
        }
    }
}