using UnityEngine;

namespace VFX
{
	public struct VFXEffectInstance
	{
		public readonly VFXConfig.VFXType VFXType;
		public readonly GameObject Instance;
		/// <summary>
		/// 为true的话，只能通过<see cref="IVFXEffectHelper.ReleaseEffect(GameObject)"/>释放
		/// </summary>
		public readonly bool IsInstantiatedAsync;

		public VFXEffectInstance(VFXConfig.VFXType vfxType, GameObject instance, bool isInstantiatedAsync)
		{
			VFXType = vfxType;
			Instance = instance;
			IsInstantiatedAsync = isInstantiatedAsync;
		}
	}
}