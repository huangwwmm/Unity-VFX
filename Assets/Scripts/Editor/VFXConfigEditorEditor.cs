using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace VFX
{
	[CustomEditor(typeof(VFXConfigEditor))]
	public class VFXConfigEditorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Generate Class"))
			{
				GenerateClass();
			}
			if (GUILayout.Button("Remove empty or duplicate effects"))
			{
				RemoveEmptyOrDuplicateEffects();
			}

			base.OnInspectorGUI();
		}

		private void GenerateClass()
		{
			VFXConfigEditor config = target as VFXConfigEditor;
			string generateAssetPath = string.Format("{0}{1}.cs", config.GeneratePath, VFXConstants.VFC_CONFIG_CLASS_NAME);
			string generateFullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + generateAssetPath;

			if (!CheckPath(generateFullPath))
			{
				EditorUtility.DisplayDialog("Generate VFX Config", string.Format("{0}\nPath is invalid", generateFullPath), "ok");
				return;
			}

			if (config.VFXAddressableAssetGroup == null
				|| string.IsNullOrEmpty(config.GeneratePath)
				|| config.ConfigTemplate == null)
			{
				EditorUtility.DisplayDialog("Generate VFX Config", "Config editor has null parameter", "ok");
				return;
			}

			RemoveEmptyOrDuplicateEffects();
			List<ForGenerateVFXConfigItem> vfxConfigItems = new List<ForGenerateVFXConfigItem>();
			HashSet<string> vfxTypes = new HashSet<string>();
			List<GameObject> vfxPrefabs = new List<GameObject>();
			for (int iEffect = 0; iEffect < config.Effects.Length; iEffect++)
			{
				GameObject iterEffect = config.Effects[iEffect];
				string assetPath = AssetDatabase.GetAssetPath(iterEffect);
				if (string.IsNullOrEmpty(assetPath))
				{
					Debug.LogError(string.Format("Effect({0}) 不是一个Assets", iEffect), iterEffect);
					continue;
				}
				string assetName = StringUtility.SubFileNameFromPath(assetPath);
				if (!vfxTypes.Add(assetName))
				{
					Debug.LogError(string.Format("Effect({0}) FileName({1})重复", iEffect, assetName), iterEffect);
					continue;
				}

				string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
				AddressableAssetEntry unitPrefabAssetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(assetGuid, config.VFXAddressableAssetGroup);
				string assetKey = VFXConstants.VFX_EFFECT_ASSET_KEY_STARTWITH + assetName;
				unitPrefabAssetEntry.address = assetKey;
				ForGenerateVFXConfigItem vfxConfigItem = new ForGenerateVFXConfigItem();
				vfxConfigItem.Type = assetName;
				vfxConfigItem.ItemConfig = new VFXItemConfig(assetKey);
				vfxConfigItems.Add(vfxConfigItem);
				vfxPrefabs.Add(iterEffect);
			}
			VFXConfigForEditorLoader.Get().EffectPrefabs = vfxPrefabs.ToArray();

			string[] configTemplateLines = config.ConfigTemplate.text.Split(new char[] { '\n' });
			StringBuilder stringBuilder = new StringBuilder();
			for (int iLine = 0; iLine < configTemplateLines.Length; iLine++)
			{
				string iterLine = configTemplateLines[iLine].Replace("\r", "");
				if (iterLine.Contains(VFXConstants.VFX_CONFIG_CLASS_NAME_FLAG))
				{
					stringBuilder.AppendLine(iterLine.Replace(VFXConstants.VFX_CONFIG_CLASS_NAME_FLAG, VFXConstants.VFC_CONFIG_CLASS_NAME));
				}
				else if (iterLine.Contains(VFXConstants.VFX_CONFIG_TYPE_FLAG))
				{
					stringBuilder.AppendLine(iterLine);
					for (int iItem = 0; iItem < vfxConfigItems.Count; iItem++)
					{
						stringBuilder.AppendLine(string.Format(VFXConstants.VFX_CONFIG_TYPE_FORAMT, vfxConfigItems[iItem].Type));
					}
					stringBuilder.AppendLine(string.Format(VFXConstants.VFX_CONFIG_TYPE_FORAMT, "Count"));
				}
				else if (iterLine.Contains(VFXConstants.VFX_CONFIG_ITEM_CONFIGS_FLAG))
				{
					stringBuilder.AppendLine(iterLine);
					for (int iItem = 0; iItem < vfxConfigItems.Count; iItem++)
					{
						stringBuilder.AppendLine(string.Format(VFXConstants.VFX_CONFIG_ITEM_CONFIG_FORAMT, vfxConfigItems[iItem].ItemConfig.AssetKey));
					}
				}
				else
				{
					stringBuilder.AppendLine(iterLine);
				}
			}
			File.WriteAllText(generateFullPath, stringBuilder.ToString());
			AssetDatabase.ImportAsset(generateAssetPath);
			TextAsset generateAsset = AssetDatabase.LoadAssetAtPath(generateAssetPath, typeof(TextAsset)) as TextAsset;
			Selection.activeObject = generateAsset;
		}

		private void RemoveEmptyOrDuplicateEffects()
		{
			VFXConfigEditor config = target as VFXConfigEditor;
			HashSet<GameObject> effectHashSet = new HashSet<GameObject>();
			List<GameObject> effects = new List<GameObject>();
			for (int iEffect = 0; iEffect < config.Effects.Length; iEffect++)
			{
				GameObject iterEffect = config.Effects[iEffect];
				if (iterEffect != null
					&& effectHashSet.Add(iterEffect))
				{
					effects.Add(iterEffect);
				}
			}
			config.Effects = effects.ToArray();
		}

		private bool CheckPath(string generatePath)
		{
			try
			{
				File.WriteAllText(generatePath, "");
				File.Delete(generatePath);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private struct ForGenerateVFXConfigItem
		{
			public string Type;
			public VFXItemConfig ItemConfig;
		}
	}
}