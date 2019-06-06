using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

namespace VFX
{
	public class VFXConfigEditor : ScriptableObject
	{
		public string GeneratePath = "Assets/";
		public AddressableAssetGroup VFXAddressableAssetGroup;
		public TextAsset ConfigTemplate;

		public GameObject[] Effects;
	}
}