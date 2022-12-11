﻿using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.unityeditor.externaltoolslauncher
{
	[InitializeOnLoad]
	public static class AddButtonsToRight
	{
		private static readonly string dirPath = Directory.GetParent(Application.dataPath).FullName;
		private static readonly string dirName = Directory.GetParent(Application.dataPath).Name;
		private static readonly string slnName = $"{Directory.GetParent(Application.dataPath).Name}.sln";
		static AddButtonsToRight()
		{
			if (!File.Exists($"{dirPath}/{slnName}"))
			{
				typeof(Editor).Assembly.GetType("UnityEditor.SyncVS").GetMethod("SyncSolution").Invoke(null, null);
			}
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		private static void OnToolbarGUI()
		{
			foreach (SaveData.Profile p in SettingsResiter.SaveData.Profiles)
			{
				if (p.Show && p.IconTexture != null)
				{
					if (Button(p.IconTexture, p.Name))
					{
						StartProcess(p.Path, p.Args);
					}
				}
			}
		}
		private static bool Button(Texture icon, string tooltip)
		{
			return GUILayout.Button(new GUIContent(null, icon, tooltip), "Command");
		}
		private static void StartProcess(string file, string args)
		{
			file = file.Replace("{ProjectPath}", dirPath).Replace("{ProjectName}", dirName).Replace("{SlnName}", slnName);
			args = args.Replace("{ProjectPath}", dirPath).Replace("{ProjectName}", dirName).Replace("{SlnName}", slnName);
			Process process = new Process();
			process.StartInfo.FileName = file;
			process.StartInfo.Arguments = args;
			process.Start();
		}
	}
}