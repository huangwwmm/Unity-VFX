#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VFX
{
	public class VFXConfigForEditor : ScriptableObject
	{
		public GameObject[] EffectPrefabs;
	}

	public static class VFXConfigForEditorLoader
	{
		/// <summary>
		/// 原始的Asset
		/// </summary>
		private static VFXConfigForEditor ms_VFXConfigForEditorAsset;
		/// <summary>
		/// 实例化出来的副本
		/// </summary>
		private static VFXConfigForEditor ms_VFXConfigForEditorInstance;

		static VFXConfigForEditorLoader()
		{
			Load();
		}

		public static void Load()
		{
			string[] assets = AssetDatabase.FindAssets("t:VFXConfigForEditor");
			if( assets.Length > 0)
			{
				ms_VFXConfigForEditorAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(VFXConfigForEditor)) as VFXConfigForEditor;
				ms_VFXConfigForEditorInstance = UnityEngine.Object.Instantiate(ms_VFXConfigForEditorAsset);
			}
			else
			{
				Debug.LogError("not have VFXConfigForEditor");
			}
		}

		public static void Save()
		{
			string assetPath = AssetDatabase.GetAssetPath(ms_VFXConfigForEditorAsset);
			AssetDatabase.DeleteAsset(assetPath);
			AssetDatabase.CreateAsset(ms_VFXConfigForEditorInstance, assetPath);
			Load();
		}

		public static VFXConfigForEditor Get()
		{
			if (ms_VFXConfigForEditorInstance == null)
			{
				Load();
			}
			return ms_VFXConfigForEditorInstance;
		}
	}
}
#endif