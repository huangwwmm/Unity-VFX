using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
	public static class VFXConstants
	{
#if UNITY_EDITOR
		public const string VFX_CONFIG_EDITOR_FILENAME = "VFX_Config";
		public const string VFX_CONFIG_EDITOR_FOREDITOR_FILENAME = "VFX_Config_ForEditor";
		public const string VFX_CONFIG_CLASS_NAME_FLAG = "VFX_CONFIG_CLASS_NAME_FLAG";
		public const string VFX_CONFIG_TYPE_FLAG = "VFX_CONFIG_TYPE_FLAG";
		public const string VFX_CONFIG_ITEM_CONFIGS_FLAG = "VFX_CONFIG_ITEM_CONFIGS_FLAG";
		public const string VFX_EFFECT_ASSET_KEY_STARTWITH = "VFX_";
		public const string VFX_CONFIG_TYPE_FORAMT = "\t\t\t{0},";
		public const string VFX_CONFIG_ITEM_CONFIG_FORAMT = "\t\t\tnew VFXItemConfig(\"{0}\"),";
		public const string VFC_CONFIG_CLASS_NAME = "VFXConfig";
#endif

		public const bool PRELOAD_ALL_EFFECTS = true;
		/// <summary>
		/// 用于避免while死循环，单位毫秒
		/// </summary>
		public const int WHILE_SAFE_MILLISECONDS = 5 * 1000;
	}
}