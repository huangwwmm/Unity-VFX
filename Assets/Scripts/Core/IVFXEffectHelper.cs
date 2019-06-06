using System;
using UnityEngine;

namespace VFX
{
	public interface IVFXEffectHelper
	{
		/// <summary>
		/// 初始化
		/// </summary>
		void Initialize();
		/// <summary>
		/// Asset是否Loaded
		/// </summary>
		bool IsLoadedEffect(VFXConfig.VFXType vfxType);
		/// <summary>
		/// 异步把Asset从磁盘Load到内存
		/// </summary>
		void LoadEffectAsync(VFXConfig.VFXType vfxType, Action<VFXAsyncResult> completedAction);
		/// <summary>
		/// <see cref="IsLoadedEffect(VFXConfig.VFXType)"/>为true时才能调用此方法
		/// </summary>
		GameObject GetEffect(VFXConfig.VFXType vfxType);
		/// <summary>
		/// 用这个方法实例化的GameObject必须用<see cref="ReleaseEffect(GameObject)"/>释放
		/// 可以不用<see cref="LoadEffectAsync(VFXConfig.VFXType, Action{VFXAsyncResult})"/>
		/// </summary>
		void InstantiateEffectAsync(VFXConfig.VFXType vfxType, Action<VFXAsyncResult> completedAction);
		/// <summary>
		/// <see cref="InstantiateEffectAsync(VFXConfig.VFXType, Action{VFXAsyncResult})"/>
		/// </summary>
		void ReleaseEffect(GameObject effect);
	}
}