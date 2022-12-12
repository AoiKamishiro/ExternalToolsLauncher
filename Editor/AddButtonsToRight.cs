using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
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
                        string file = p.Path.Replace("{ProjectPath}", dirPath).Replace("{ProjectName}", projName).Replace("{SlnName}", slnName);
                        string args = p.Args.Replace("{ProjectPath}", dirPath).Replace("{ProjectName}", projName).Replace("{SlnName}", slnName);
                        Process process = new Process();
                        process.StartInfo.FileName = file;
                        process.StartInfo.Arguments = args;
                        process.Start();
                    }
                }
            }
        }
    }
}