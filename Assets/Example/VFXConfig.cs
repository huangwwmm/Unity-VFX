namespace VFX
{
	public static class VFXConfig
	{
		public enum VFXType : uint
		{
			#region VFX_CONFIG_TYPE_FLAG
			Effect_1,
			Effect_2,
			Count,
			#endregion
		}

		public static readonly VFXItemConfig[] ITEM_CONFIGS = new VFXItemConfig[]
		{
			#region VFX_CONFIG_ITEM_CONFIGS_FLAG			
			new VFXItemConfig("VFX_Effect_1"),
			new VFXItemConfig("VFX_Effect_2"),
			#endregion
		};
	}
}
