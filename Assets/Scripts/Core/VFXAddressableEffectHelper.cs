using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VFX
{
	public class VFXAddressableEffectHelper : IVFXEffectHelper
	{
		private EffectState[] m_EffectStates;

		public void Initialize()
		{
			m_EffectStates = new EffectState[(int)VFXConfig.VFXType.Count];
			for (int iEffect = 0; iEffect < m_EffectStates.Length; iEffect++)
			{
				EffectState iterEffectState = new EffectState();
				iterEffectState.VFXType = (VFXConfig.VFXType)iEffect;
				iterEffectState.IsLoading = false;
				iterEffectState.InstantiateCompletedActions = new Queue<Action<VFXAsyncResult>>(0);

				m_EffectStates[iEffect] = iterEffectState;
			}
		}

		public bool IsLoadedEffect(VFXConfig.VFXType vfxType)
		{
			return m_EffectStates[(int)vfxType].EffectAsset != null;
		}

		public void LoadEffectAsync(VFXConfig.VFXType vfxType, Action<VFXAsyncResult> completedAction)
		{
			int vfxTypeIndex = (int)vfxType;
			if (m_EffectStates[vfxTypeIndex].EffectAsset != null)
			{
				completedAction?.Invoke(new VFXAsyncResult(vfxType, m_EffectStates[vfxTypeIndex].EffectAsset, string.Empty));
			}
			else if (m_EffectStates[vfxTypeIndex].IsLoading)
			{
				m_EffectStates[vfxTypeIndex].CompletedAction += completedAction;
			}
			else
			{
				m_EffectStates[vfxTypeIndex].IsLoading = true;
				m_EffectStates[vfxTypeIndex].CompletedAction += completedAction;
				Addressables.LoadAssetAsync<GameObject>(VFXConfig.ITEM_CONFIGS[(int)vfxType].AssetKey).Completed += m_EffectStates[vfxTypeIndex]._OnLoadAsyncCompleted;
			}
		}

		public void InstantiateEffectAsync(VFXConfig.VFXType vfxType, Action<VFXAsyncResult> completedAction)
		{
			int vfxTypeIndex = (int)vfxType;
			m_EffectStates[vfxTypeIndex].InstantiateCompletedActions.Enqueue(completedAction);
			Addressables.InstantiateAsync(VFXConfig.ITEM_CONFIGS[(int)vfxType].AssetKey).Completed += m_EffectStates[vfxTypeIndex]._OnInstantiateAsyncCompleted;
		}

		public void ReleaseEffect(GameObject effect)
		{
			Addressables.ReleaseInstance(effect);
		}

		public GameObject GetEffect(VFXConfig.VFXType vfxType)
		{
			return m_EffectStates[(int)vfxType].EffectAsset;
		}

		private struct EffectState
		{
			public VFXConfig.VFXType VFXType;
			public GameObject EffectAsset;
			/// <summary>
			/// 在加载<see cref="EffectAsset"/>，用于避免重复加载
			/// </summary>
			public bool IsLoading;
			public Action<VFXAsyncResult> CompletedAction;
			public Queue<Action<VFXAsyncResult>> InstantiateCompletedActions;

			internal void _OnInstantiateAsyncCompleted(AsyncOperationHandle<GameObject> obj)
			{
				if (InstantiateCompletedActions.Count == 0)
				{
					throw new Exception("InstantiateCompletedActions.Count == 0");
				}

				string errorMessage = obj.OperationException != null
					? obj.OperationException.ToString()
					: string.Empty;
				InstantiateCompletedActions.Dequeue()?.Invoke(new VFXAsyncResult(VFXType, obj.Result, errorMessage));
			}

			internal void _OnLoadAsyncCompleted(AsyncOperationHandle<GameObject> obj)
			{
				IsLoading = false;

				// Addressable加载失败时会输出ErrorLog，所以这里不做ErrorHandle
				EffectAsset = obj.Result;
				string errorMessage = obj.OperationException != null
					? obj.OperationException.ToString()
					: string.Empty;
				CompletedAction?.Invoke(new VFXAsyncResult(VFXType, EffectAsset, errorMessage));
				CompletedAction = null;
			}
		}
	}
}