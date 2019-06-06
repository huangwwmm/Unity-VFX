using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VFX
{
	public static class VFXEditorUtility
	{
		[MenuItem("Custom/VFX/Create Config Editor")]
		private static void CreateConfigTemplate()
		{
			string selectionPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			string configDirectory;
			if (Directory.Exists(selectionPath))
			{
				configDirectory = selectionPath;
			}
			else
			{
				if (File.Exists(selectionPath))
				{
					configDirectory = selectionPath.Substring(0, selectionPath.LastIndexOf('/'));
				}
				else
				{
					configDirectory = "Assets";
				}
			}

			string configPath = string.Format("{0}/{1}.asset", configDirectory, VFXConstants.VFX_CONFIG_EDITOR_FILENAME);
			AssetDatabase.CreateAsset(new VFXConfigEditor(), configPath);

			string configForEditorPath = string.Format("{0}/{1}.asset", configDirectory, VFXConstants.VFX_CONFIG_EDITOR_FOREDITOR_FILENAME);
			AssetDatabase.CreateAsset(new VFXConfigForEditor(), configForEditorPath);

			VFXConfigEditor config = AssetDatabase.LoadAssetAtPath(configPath, typeof(VFXConfigEditor)) as VFXConfigEditor;
			if (config)
			{
				Selection.activeObject = config;
			}
		}
	}
}