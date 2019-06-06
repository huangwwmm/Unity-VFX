#if UNITY_EDITOR
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
		private static VFXConfigForEditor ms_VFXConfigForEditor;

		static VFXConfigForEditorLoader()
		{
			string[] assets = AssetDatabase.FindAssets("t:VFXConfigForEditor");
			if (assets.Length > 0)
			{
				ms_VFXConfigForEditor = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(VFXConfigForEditor)) as VFXConfigForEditor;
			}
		}

		public static VFXConfigForEditor Get()
		{
			if (ms_VFXConfigForEditor == null)
			{
				Debug.LogError("not have VFXConfigForEditor");
			}
			return ms_VFXConfigForEditor;
		}
	}
}
#endif