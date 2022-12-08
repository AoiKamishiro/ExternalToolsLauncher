using System;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
{
    [Serializable]
    internal struct SaveData
    {
        public Profile[] Profiles;

        [Serializable]
        public struct Profile
        {
            public bool Show;
            public string Name, Path, Args, Icon;

            [NonSerialized]
            private string _guid;
            public string Guid
            {
                get
                {
                    if (string.IsNullOrEmpty(_guid)) _guid = System.Guid.NewGuid().ToString();
                    return _guid;
                }
            }

            [NonSerialized]
            private Texture2D _iconTexture;

            public Texture2D IconTexture
            {
                get
                {
                    if (_iconTexture != null && AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_iconTexture)) != Icon) { _iconTexture = null; }
                    if (_iconTexture == null) { _iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(Icon)); }
                    return _iconTexture;
                }
            }
        }
    }
}
