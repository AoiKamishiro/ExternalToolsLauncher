using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
{
    [InitializeOnLoad]
    public static class SettingsResiter
    {
        private const string DEFAULT_VALUE = "{\"Profiles\":[{\"Show\":true,\"Name\":\"VSCode\",\"Path\":\"code\",\"Args\":\"\\\"{ProjectPath}\\\"\",\"Icon\":\"fa1bd4f685d92654cac4088f0697676b\"},{\"Show\":true,\"Name\":\"Visual Studio\",\"Path\":\"C:\\\\Program Files\\\\Microsoft Visual Studio\\\\2022\\\\Professional\\\\Common7\\\\IDE\\\\devenv.exe\",\"Args\":\"\\\"{ProjectPath}/{SlnName}\\\"\",\"Icon\":\"48ffc9dc2bd7d9144923a0ae241d542a\"},{\"Show\":true,\"Name\":\"Jetbrains Rider\",\"Path\":\"C:\\\\Program Files\\\\JetBrains\\\\JetBrains Rider 2022.3\\\\bin\\\\rider64.exe\",\"Args\":\"\\\"{ProjectPath}/{SlnName}\\\"\",\"Icon\":\"b7a9754d051cade459986006582aabdc\"},{\"Show\":true,\"Name\":\"PowerShell\",\"Path\":\"wt\",\"Args\":\"-d \\\"{ProjectPath}\\\" -p \\\"PowerShell\\\"\",\"Icon\":\"d845b85b2b27fe948a1860b5c7c6a4f7\"},{\"Show\":true,\"Name\":\"Github\",\"Path\":\"https://github.com/AoiKamishiro/ExternalToolsLauncher\",\"Args\":\"\",\"Icon\":\"3c916a1640d0eaa4ea1e50e8a5a6c528\"}]}";
        private const string PREFS_KEY = "KM_ETL_SETTINGS";
        private const string ICON_DIR_GUID = "7ee47ee6d736af5448d6f8ad22f5f511";

        internal static SaveData SaveData
        {
            get
            {
                string load = EditorPrefs.GetString(PREFS_KEY, DEFAULT_VALUE);
                if (string.IsNullOrEmpty(load))
                {
                    return new SaveData();
                }
                SaveData saveData = JsonUtility.FromJson<SaveData>(load);
                return saveData;
            }
            set
            {
                string save = JsonUtility.ToJson(value);
                EditorPrefs.SetString(PREFS_KEY, save);
            }
        }
        private static SaveData DefaultSaveData
        {
            get => JsonUtility.FromJson<SaveData>(DEFAULT_VALUE);
        }
        private static readonly string[] IconName = Array.Empty<string>();
        private static readonly string[] IconGUIDs = Array.Empty<string>();
        private static bool showExportSettings = false;

        private static int GUIDtoIconOrder(string guid)
        {
            int order = 0;
            foreach (string d in IconGUIDs)
            {
                if (d == guid) { return order; }
                order++;
            }
            return 0;
        }

        static SettingsResiter()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new string[] { AssetDatabase.GUIDToAssetPath(ICON_DIR_GUID) }))
            {
                IconName = IconName.Append(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)).ToUpper(0)).ToArray();
                IconGUIDs = IconGUIDs.Append(guid).ToArray();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/External Tools Launcher", SettingsScope.User)
            {
                label = "External Tools Launcher",
                keywords = new HashSet<string>(new[] { "External Tools Launcher", "External", "Tools", "Launcher" }),
                guiHandler = searchContext =>
                {
                    SaveData saveData = SaveData;
                    string deleteGuid = string.Empty;
                    int orderUpIndex = -1;
                    int orderDownIndex = -1;

                    EditorGUILayout.Space(16);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("エディタ上部に任意のアプリの起動ボタンを追加します。");
                    if (GUILayout.Button("Github リポジトリはこちら。", EditorStyles.linkLabel))
                    {
                        Application.OpenURL("https://github.com/AoiKamishiro/ExternalToolsLauncher");
                    }
                    EditorGUILayout.HelpBox("{ProjectPath}はプロジェクトのフォルダのパスを返します。\n{SlnName}はソリューションファイル名を返します。", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Profile"))
                    {
                        SaveData.Profile newProfile = new SaveData.Profile
                        {
                            Show = true
                        };
                        saveData.Profiles = saveData.Profiles.Append(newProfile).ToArray();
                    }
                    if (GUILayout.Button("Reset to Default"))
                    {
                        saveData = DefaultSaveData;
                    }
                    EditorGUILayout.EndHorizontal();
                    showExportSettings = EditorGUILayout.Foldout(showExportSettings, "Backup Settings");
                    if (showExportSettings)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Export Profiles"))
                        {
                            string savePath = EditorUtility.SaveFilePanel("Save", "Assets", "ETLSettings", "json");
                            if (!string.IsNullOrEmpty(savePath))
                            {
                                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                                StreamWriter writer = new StreamWriter(savePath, false, enc);
                                writer.WriteLine(JsonUtility.ToJson(saveData));
                                writer.Close();
                            }
                        }
                        if (GUILayout.Button("Import Profiles"))
                        {
                            string loadPath = EditorUtility.OpenFilePanel("Open", "Assets", "json");
                            StreamReader rader = new StreamReader(loadPath);
                            string json = rader.ReadToEnd();
                            rader.Close();
                            try
                            {
                                SaveData importedData = JsonUtility.FromJson<SaveData>(json);
                                saveData = importedData;
                            }
                            catch
                            {
                                Debug.LogError("ETL Error: Incorrect Json.");
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space(8);

                    for (int i = 0; i < saveData.Profiles.Length; i++)
                    {
                        EditorGUILayout.Space(8);

                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.BeginVertical();
                        saveData.Profiles[i].Name = EditorGUILayout.DelayedTextField("Profile Name", saveData.Profiles[i].Name);
                        EditorGUI.indentLevel++;
                        saveData.Profiles[i].Show = EditorGUILayout.Toggle("Visiblity", saveData.Profiles[i].Show);

                        EditorGUILayout.BeginHorizontal();
                        saveData.Profiles[i].Path = EditorGUILayout.DelayedTextField("Path", saveData.Profiles[i].Path);
                        if (GUILayout.Button("Load", GUILayout.Width(100)))
                        {
                            string path = EditorUtility.OpenFilePanel("Open", "Assets", "*");
                            if (!string.IsNullOrEmpty(path)) saveData.Profiles[i].Path = path;
                        }
                        EditorGUILayout.EndHorizontal();

                        saveData.Profiles[i].Args = EditorGUILayout.DelayedTextField("Arguments", saveData.Profiles[i].Args);
                        int iconOrder = EditorGUILayout.Popup("Icon", GUIDtoIconOrder(saveData.Profiles[i].Icon), IconName);
                        saveData.Profiles[i].Icon = IconGUIDs[iconOrder];
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(i == 0);
                        if (GUILayout.Button("Up"))
                        {
                            orderUpIndex = i;
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.BeginDisabledGroup(i == saveData.Profiles.Length - 1);
                        if (GUILayout.Button("Down"))
                        {
                            orderDownIndex = i;
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.Space(32);
                        if (GUILayout.Button("Delete Profile", GUILayout.Width(100)))
                        {
                            deleteGuid = saveData.Profiles[i].Guid;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(64), GUILayout.Width(64));
                        EditorGUILayout.LabelField(new GUIContent(saveData.Profiles[i].IconTexture), GUILayout.Height(64), GUILayout.Width(64));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();
                    }
                    if (!string.IsNullOrEmpty(deleteGuid))
                    {
                        saveData.Profiles = saveData.Profiles.Where(p => p.Guid != deleteGuid).ToArray();
                        deleteGuid = string.Empty;
                    }
                    if (orderUpIndex != -1)
                    {
                        IEnumerable<SaveData.Profile> profiles = Enumerable.Empty<SaveData.Profile>();
                        for (int i = 0; i < saveData.Profiles.Length; i++)
                        {
                            if (i == orderUpIndex - 1)
                            {
                                profiles = profiles.Append(saveData.Profiles[i + 1]);
                            }
                            else if (i == orderUpIndex)
                            {
                                profiles = profiles.Append(saveData.Profiles[i - 1]);
                            }
                            else
                            {
                                profiles = profiles.Append(saveData.Profiles[i]);
                            }
                        }
                        saveData.Profiles = profiles.ToArray();
                        orderUpIndex = -1;
                    }
                    if (orderDownIndex != -1)
                    {
                        IEnumerable<SaveData.Profile> profiles = Enumerable.Empty<SaveData.Profile>();
                        for (int i = 0; i < saveData.Profiles.Length; i++)
                        {
                            if (i == orderDownIndex)
                            {
                                profiles = profiles.Append(saveData.Profiles[i + 1]);
                            }
                            else if (i == orderDownIndex + 1)
                            {
                                profiles = profiles.Append(saveData.Profiles[i - 1]);
                            }
                            else
                            {
                                profiles = profiles.Append(saveData.Profiles[i]);
                            }
                        }
                        saveData.Profiles = profiles.ToArray();
                        orderDownIndex = -1;
                    }
                    SaveData = saveData;
                }
            };
            return provider;
        }
        private static string ToUpper(this string self, int no = 0)
        {
            if (no > self.Length)
            {
                return self;
            }

            char[] _array = self.ToCharArray();
            char up = char.ToUpper(_array[no]);
            _array[no] = up;
            return new string(_array);
        }
    }
}
