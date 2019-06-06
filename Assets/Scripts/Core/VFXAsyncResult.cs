using UnityEngine;

namespace VFX
{
	public struct VFXAsyncResult
	{
		public readonly VFXConfig.VFXType VFXType;
		public readonly bool Success;
		public readonly GameObject Result;
		public readonly string ErrorMessage;

		public VFXAsyncResult(VFXConfig.VFXType vfxType, GameObject result, string errorMessge)
		{
			VFXType = vfxType;
			Result = result;
			ErrorMessage = errorMessge;
			Success = Result && string.IsNullOrEmpty(errorMessge);
		}
	}
}