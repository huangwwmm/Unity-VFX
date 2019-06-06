using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
	public class Example : MonoBehaviour
	{
		private bool m_IsInitialized;
		private VFXEffectInstance m_Effect1;
		private VFXEffectInstance m_Effect2;

		void Start()
		{
			VFXManager.GetInstance().Initialize(OnVFXManagerInitialized);
		}

		void LateUpdate()
		{
			if (Input.GetKeyDown(KeyCode.F5))
			{
				VFXManager.GetInstance().PushEffect(m_Effect1);
				VFXManager.GetInstance().PushEffect(m_Effect2);
			}
			if (Input.GetKeyDown(KeyCode.F6))
			{
				m_Effect1 = VFXManager.GetInstance().PopEffect(VFXConfig.VFXType.Effect_1);
				m_Effect2 = VFXManager.GetInstance().PopEffect(VFXConfig.VFXType.Effect_1);
			}
		}

		private void OnVFXManagerInitialized()
		{
			m_IsInitialized = true;

			VFXManager.GetInstance().PopEffectAsync(VFXConfig.VFXType.Effect_1, OnPopEffect1);
			VFXManager.GetInstance().PopEffectAsync(VFXConfig.VFXType.Effect_1, OnPopEffect2);
		}

		private void OnPopEffect1(VFXEffectInstance obj)
		{
			m_Effect1 = obj;
		}

		private void OnPopEffect2(VFXEffectInstance obj)
		{
			m_Effect2 = obj;
		}
	}
}