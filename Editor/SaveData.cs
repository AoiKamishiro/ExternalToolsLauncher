using System;
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
			public bool Show;
			public string Name, Path, Args, Icon;

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
					if (_iconTexture != null && AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_iconTexture)) != Icon) { _iconTexture = null; }
					if (_iconTexture == null) { _iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(Icon)); }
					return _iconTexture;
				}
			}
		}
	}
}
