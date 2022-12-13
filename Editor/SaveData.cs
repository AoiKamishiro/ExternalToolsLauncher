using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
{
    /// <summary>
    /// 設定データを保存する為の構造体
    /// </summary>
    [Serializable]
    internal struct SaveData
    {
        public Profile[] Profiles;

        /// <summary>
        /// 個々のプロファイルの構造体
        /// </summary>
        [Serializable]
        public struct Profile
        {
            public bool Show, UseExternalIcon;
            public string Name, Path, Args, Icon, ExternalIconPath;
            public DateTimeOffset LastChanged, LastIconLoaded;

            [NonSerialized]
            private string _uuid;
            /// <summary>
            /// 一時的な識別用のUUID
            /// </summary>
            public string Uuid
            {
                get
                {
                    if (string.IsNullOrEmpty(_uuid)) _uuid = Guid.NewGuid().ToString();
                    return _uuid;
                }
            }

            [NonSerialized]
            private Texture2D _iconTexture;
            /// <summary>
            /// キャッシュされたアイコン画像データ
            /// </summary>
            public Texture2D IconTexture
            {
                get
                {
                    if(_iconTexture==null || (_iconTexture!=null && LastIconLoaded < LastChanged))
                    {
                        if (UseExternalIcon)
                        {
                            if (File.Exists(ExternalIconPath))
                            {
                                _iconTexture = LoadExternalTexture(ExternalIconPath);
                            }
                            if (_iconTexture == null)
                            {
                                _iconTexture = new Texture2D(0, 0);
                            }
                        }
                        else
                        {
                            _iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(Icon));
                        }
                        LastIconLoaded = DateTimeOffset.UtcNow;
                    }

                    return _iconTexture;
                }
            }

            /// <summary>
            /// 与えられたパスから画像を読み込みます。
            /// </summary>
            /// <param name="path">画像ファイルのパス</param>
            /// <returns></returns>
            private Texture2D LoadExternalTexture(string path)
            {
                byte[] values;
                Texture2D texture2D = new Texture2D(1, 1);
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (BinaryReader bin = new BinaryReader(fs))
                    {
                        values = bin.ReadBytes((int)bin.BaseStream.Length);
                    }
                }
                if (texture2D.LoadImage(values))
                {
                    return texture2D;
                }
                return null;
            }
        }
    }
}