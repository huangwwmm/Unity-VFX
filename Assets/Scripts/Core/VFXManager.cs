using System;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
	public class VFXManager : MonoBehaviour
	{
		private static VFXManager ms_Instance;

		private EffectState[] m_EffectStates;

		private IVFXEffectHelper m_AssetHelper;
		/// <summary>
		/// <see cref="Initialize(Action)"/>成功后的回调
		/// </summary>
		private Action m_InitializeCompletedAction;
		/// <summary>
		/// <see cref="Initialize(Action)"/>时Load的Effect数量
		/// </summary>
		private int m_InitializingEffectCount;

		public static VFXManager GetInstance()
		{
			if (ms_Instance == null)
			{
				ms_Instance = new GameObject("VFXManager").AddComponent<VFXManager>();
				DontDestroyOnLoad(ms_Instance.gameObject);
			}
			return ms_Instance;
		}

		public void Initialize(Action completedAction)
		{
			ms_Instance = this;

			m_InitializeCompletedAction = completedAction;

			m_EffectStates = new EffectState[(int)VFXConfig.VFXType.Count];
			for (int iEffect = 0; iEffect < m_EffectStates.Length; iEffect++)
			{
				EffectState iterEffectState = new EffectState();
				iterEffectState.InstantiateCount = 0;
				iterEffectState.InstantiatePool = new Stack<VFXEffectInstance>();
				iterEffectState.PopCompletedActions = new Queue<Action<VFXEffectInstance>>();
				m_EffectStates[iEffect] = iterEffectState;
			}

			m_AssetHelper = new VFXAddressableEffectHelper();

			m_AssetHelper.Initialize();

			if (VFXConstants.PRELOAD_ALL_EFFECTS)
			{
				m_InitializingEffectCount = m_EffectStates.Length;
				for (int iEffect = 0; iEffect < m_EffectStates.Length; iEffect++)
				{
					LoadEffectAsync((VFXConfig.VFXType)iEffect, OnInitializedEffect);
				}
			}
			else
			{
#pragma warning disable 0162
				m_InitializeCompletedAction?.Invoke();
#pragma warning restore 0162
			}
		}

		private void OnInitializedEffect(VFXAsyncResult obj)
		{
			m_InitializingEffectCount--;
			if (m_InitializingEffectCount == 0)
			{
				m_InitializeCompletedAction?.Invoke();
				m_InitializeCompletedAction = null;
			}
		}

		/// <summary>
		/// Effect是否加载
		/// </summary>
		public bool IsLoadedEffect(VFXConfig.VFXType vfxType)
		{
			return m_AssetHelper.IsLoadedEffect(vfxType);
		}

		/// <summary>
		/// Effect在池中的剩余数量
		/// </summary>
		public int GetEffectCountInPool(VFXConfig.VFXType vfxType)
		{
			return m_EffectStates[(int)vfxType].InstantiatePool.Count;
		}

		/// <summary>
		/// 实例化中的Effect数量
		/// </summary>
		public int GetInstantiatingEffectCount(VFXConfig.VFXType vfxType)
		{
			return m_EffectStates[(int)vfxType].PopCompletedActions.Count;
		}

		/// <summary>
		/// 异步加载Effect
		/// </summary>
		public void LoadEffectAsync(VFXConfig.VFXType vfxType, Action<VFXAsyncResult> completedAction)
		{
			m_AssetHelper.LoadEffectAsync(vfxType, completedAction);
		}

		/// <summary>
		/// 异步创建Effect
		///	    <see cref="GetEffectCountInPool(VFXConfig.VFXType)"/>大于0的话从池中拿
		///		异步实例化
		/// </summary>
		public void PopEffectAsync(VFXConfig.VFXType vfxType, Action<VFXEffectInstance> completedAction)
		{
			int vfxTypeIndex = (int)vfxType;

			if (m_EffectStates[vfxTypeIndex].InstantiatePool.Count > 0)
			{
				VFXEffectInstance effectInstance = m_EffectStates[vfxTypeIndex].InstantiatePool.Pop();
				effectInstance.Instance.SetActive(true);
				completedAction?.Invoke(effectInstance);
			}
			else
			{
				m_EffectStates[vfxTypeIndex].PopCompletedActions.Enqueue(completedAction);
				m_AssetHelper.InstantiateEffectAsync(vfxType, OnInstantiateEffectCompleted);
			}
		}

		/// <summary>
		/// 同步创建Effect
		///	    <see cref="GetEffectCountInPool(VFXConfig.VFXType)"/>大于0的话从池中拿
		///		<see cref="IsLoadedEffect(VFXConfig.VFXType)"/>的话，通过Loaded出来的Effect实例化
		/// </summary>
		public VFXEffectInstance PopEffect(VFXConfig.VFXType vfxType)
		{
			if (GetEffectCountInPool(vfxType) > 0)
			{
				VFXEffectInstance effectInstance = m_EffectStates[(int)vfxType].InstantiatePool.Pop();
				effectInstance.Instance.SetActive(true);
				return effectInstance;
			}
			else if(IsLoadedEffect(vfxType))
			{
				m_EffectStates[(int)vfxType].InstantiateCount++;
				return new VFXEffectInstance(vfxType, Instantiate(m_AssetHelper.GetEffect(vfxType)), false);
			}
			else
			{
				throw new Exception(string.Format("effect ({0}) not loaded", vfxType));
			}
		}

		/// <summary>
		/// 放回池里，并不销毁
		/// </summary>
		public void PushEffect(VFXEffectInstance effect)
		{
			if (effect.Instance)
			{
				int vfxTypeIndex = (int)effect.VFXType;
				effect.Instance.SetActive(false);
				effect.Instance.transform.SetParent(transform, false);
				m_EffectStates[vfxTypeIndex].InstantiatePool.Push(effect);
			}
		}

		private void OnInstantiateEffectCompleted(VFXAsyncResult result)
		{
			int vfxTypeIndex = (int)result.VFXType;
			if (m_EffectStates[vfxTypeIndex].PopCompletedActions.Count == 0)
			{
				throw new Exception("InstantiateCompletedActions.Count == 0");
			}

			Action<VFXEffectInstance> completedAction = m_EffectStates[vfxTypeIndex].PopCompletedActions.Dequeue();
			if (result.Success)
			{
				m_EffectStates[vfxTypeIndex].InstantiateCount++;
				completedAction?.Invoke(new VFXEffectInstance(result.VFXType, result.Result, true));
			}
			else
			{
				Debug.LogError(string.Format("Instantiate effect ({0}) failed, Error:\n", result.VFXType, result.ErrorMessage));
			}
		}

		private struct EffectState
		{
			/// <summary>
			/// 实例化出来的Effect数量
			/// </summary>
			public int InstantiateCount;
			/// <summary>
			/// Effect的实例池
			/// </summary>
			public Stack<VFXEffectInstance> InstantiatePool;
			public Queue<Action<VFXEffectInstance>> PopCompletedActions;
		}
	}
}