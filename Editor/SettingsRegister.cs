using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
{
    /// <summary>
    /// Preference 内で設定を変更する為のクラス
    /// </summary>
    public static class SettingsResiter
    {
        /// <summary>
        /// 設定を保存する為のEditorPrefsのキー
        /// </summary>
        private const string PREFS_KEY = "KM_ETL_SETTINGS";
        /// <summary>
        /// アイコン画像が保存されたフォルダのGUID値
        /// </summary>
        private const string ICON_DIR_GUID = "7ee47ee6d736af5448d6f8ad22f5f511";
        /// <summary>
        /// デフォルトの設定値が保存されているファイルのGUID値
        /// </summary>
        private const string DEFAULT_DATA_GUID = "465f1a32ea1ceb34e969bbd94417a4dc";

        /// <summary>
        /// デフォルトの設定値
        /// </summary>
        private static SaveData DefaultSaveData
        {
            get => FromJsonFile(AssetDatabase.GUIDToAssetPath(DEFAULT_DATA_GUID));
        }
        /// <summary>
        /// EditorPrefsに保存されている設定値
        /// </summary>
        internal static SaveData SaveData
        {
            get
            {
                string load = EditorPrefs.GetString(PREFS_KEY, string.Empty);
                if (string.IsNullOrEmpty(load))
                {
                    return DefaultSaveData;
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

        private static Dictionary<string, string> _icons;
        /// <summary>
        /// アイコン画像のGUID値とファイル名の辞書
        /// </summary>
        private static Dictionary<string, string> Icons
        {
            get
            {
                if (_icons == null)
                {
                    _icons = new Dictionary<string, string>();
                    foreach (string guid in AssetDatabase.FindAssets($"t: {nameof(Texture2D)}", new string[] { AssetDatabase.GUIDToAssetPath(ICON_DIR_GUID) }))
                    {
                        _icons.Add(guid, Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)).Proper());
                    }
                }
                return _icons;
            }
        }

        /// <summary>
        /// バックアップ関連の表示の有無
        /// </summary>
        private static bool showExportSettings = false;

        /// <summary>
        /// Preference 内に設定項目を追加します。
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Preferences/External Tools Launcher", SettingsScope.User)
            {
                label = "External Tools Launcher",
                keywords = new string[] { "External Tools Launcher", "External", "Tools", "Launcher" },
                guiHandler = x => OnGUI()
            };
        }

        /// <summary>
        /// GUIの表示を行います
        /// </summary>
        private static void OnGUI()
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
            EditorGUILayout.HelpBox("{ProjectPath}はプロジェクトのフォルダのパスを返します。\n{ProjectName}はプロジェクトのフォルダ名を返します。\n{SelectPath}は選択されたアセットのパスを返します。\n{SlnName}はソリューションファイル名を返します。", MessageType.Info);
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
                    ToJsonFile(savePath, saveData);
                }
                if (GUILayout.Button("Import Profiles"))
                {
                    string loadPath = EditorUtility.OpenFilePanel("Open", "Assets", "json");
                    saveData = FromJsonFile(loadPath);
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
                saveData.Profiles[i].Name = EditorGUILayout.TextField("Profile Name", saveData.Profiles[i].Name);
                EditorGUI.indentLevel++;
                saveData.Profiles[i].Show = EditorGUILayout.Toggle("Visiblity", saveData.Profiles[i].Show);

                EditorGUILayout.BeginHorizontal();
                saveData.Profiles[i].Path = EditorGUILayout.TextField("Path", saveData.Profiles[i].Path);
                if (GUILayout.Button("Load", GUILayout.Width(100)))
                {
#if UNITY_EDITOR_OSX
                    string path = EditorUtility.OpenFilePanelWithFilters("Open", "Assets", new string[] {"All File",""});
#else
                    string path = EditorUtility.OpenFilePanelWithFilters("Open", "Assets", new string[] {"All File","*"});
#endif
                    if (!string.IsNullOrEmpty(path)) saveData.Profiles[i].Path = path;
                }
                EditorGUILayout.EndHorizontal();

                saveData.Profiles[i].Args = EditorGUILayout.TextField("Arguments", saveData.Profiles[i].Args);

                saveData.Profiles[i].UseExternalIcon = EditorGUILayout.Toggle("Use External Icon", saveData.Profiles[i].UseExternalIcon);
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(saveData.Profiles[i].UseExternalIcon);
                int iconOrder = EditorGUILayout.Popup("Internal Icon", GetIconOrder(saveData.Profiles[i].Icon), Icons.Values.ToArray());
                saveData.Profiles[i].Icon = Icons.Keys.ToArray()[iconOrder];
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(!saveData.Profiles[i].UseExternalIcon);
                EditorGUILayout.BeginHorizontal();
                saveData.Profiles[i].ExternalIconPath = EditorGUILayout.TextField("External Icon", saveData.Profiles[i].ExternalIconPath);
                if (GUILayout.Button("Load Icon", GUILayout.Width(100)))
                {
                    string path = EditorUtility.OpenFilePanelWithFilters("Open", "Assets", new string[] { "Image File", "png,jpg,psd" });
                    if (!string.IsNullOrEmpty(path)) saveData.Profiles[i].ExternalIconPath = path;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;


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
                    deleteGuid = saveData.Profiles[i].Uuid;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(64), GUILayout.Width(64));
                EditorGUILayout.LabelField(new GUIContent(saveData.Profiles[i].IconTexture), GUILayout.Height(64), GUILayout.Width(64));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                saveData.Profiles[i].LastChanged = System.DateTimeOffset.UtcNow;
            }
            if (!string.IsNullOrEmpty(deleteGuid))
            {
                saveData.Profiles = saveData.Profiles.Where(p => p.Uuid != deleteGuid).ToArray();
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

        /// <summary>
        /// アイコン画像のGUIDからアイコン辞書のインデックスを返します。
        /// </summary>
        /// <param name="guid">アイコン画像のGUID値</param>
        /// <returns>アイコン辞書内のインデックス</returns>
        private static int GetIconOrder(string guid)
        {
            int order = 0;
            foreach (KeyValuePair<string, string> icon in Icons)
            {
                if (icon.Key == guid) { return order; }
                order++;
            }
            return 0;
        }

        /// <summary>
        /// 与えられた文字列の一文字目を大文字にた値を返します。
        /// </summary>
        /// <param name="txt">一文字目を大文字にしたい文字列</param>
        /// <returns>一文字目を大文字した文字列</returns>
        private static string Proper(this string txt)
        {
            char[] _array = txt.ToCharArray();
            char up = char.ToUpper(_array[0]);
            _array[0] = up;
            return new string(_array);
        }

        /// <summary>
        /// Jsonファイルに保存された設定値を取得します。
        /// </summary>
        /// <param name="path">Jsonファイルのパス</param>
        /// <returns>Jsonファイルから読み取られた設定値</returns>
        /// <exception cref="JsonReaderException">Jsonファイルの読み取りに失敗した場合</exception>
        private static SaveData FromJsonFile(string path)
        {
            StreamReader rader = new StreamReader(path);
            string json = rader.ReadToEnd();
            rader.Close();
            try
            {
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data;
            }
            catch
            {
                Debug.LogError("ETL Error: Incorrect Json EE.");
                throw new JsonReaderException("ETL Error: Incorrect Json.");
            }
        }

        /// <summary>
        /// 設定値をJsonファイルに保存します。
        /// </summary>
        /// <param name="path">Jsonファイルのパス</param>
        /// <param name="saveData">保存する設定値</param>
        private static void ToJsonFile(string path, SaveData saveData)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Encoding enc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer = new StreamWriter(path, false, enc);
                writer.WriteLine(JsonUtility.ToJson(saveData));
                writer.Close();
            }
        }
    }
}