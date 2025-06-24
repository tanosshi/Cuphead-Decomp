using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	[ExecuteInEditMode]
	public class TMP_Settings : ScriptableObject
	{
		private static TMP_Settings s_Instance;

		[SerializeField]
		private bool m_enableWordWrapping;

		[SerializeField]
		private bool m_enableKerning;

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool m_enableTintAllSprites;

		[SerializeField]
		private bool m_warningsDisabled;

		[SerializeField]
		private TMP_FontAsset m_defaultFontAsset;

		[SerializeField]
		private List<TMP_FontAsset> m_fallbackFontAssets;

		[SerializeField]
		private TMP_SpriteAsset m_defaultSpriteAsset;

		[SerializeField]
		private TMP_StyleSheet m_defaultStyleSheet;

		public static bool enableWordWrapping
		{
			get
			{
				return instance.m_enableWordWrapping;
			}
		}

		public static bool enableKerning
		{
			get
			{
				return instance.m_enableKerning;
			}
		}

		public static bool enableExtraPadding
		{
			get
			{
				return instance.m_enableExtraPadding;
			}
		}

		public static bool enableTintAllSprites
		{
			get
			{
				return instance.m_enableTintAllSprites;
			}
		}

		public static bool warningsDisabled
		{
			get
			{
				return instance.m_warningsDisabled;
			}
		}

		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				return instance.m_defaultFontAsset;
			}
		}

		public static List<TMP_FontAsset> fallbackFontAssets
		{
			get
			{
				return instance.m_fallbackFontAssets;
			}
		}

		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				return instance.m_defaultSpriteAsset;
			}
		}

		public static TMP_StyleSheet defaultStyleSheet
		{
			get
			{
				return instance.m_defaultStyleSheet;
			}
		}

		public static TMP_Settings instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = Resources.Load("TMP Settings") as TMP_Settings;
				}
				return s_Instance;
			}
		}

		public static TMP_Settings LoadDefaultSettings()
		{
			if (s_Instance == null)
			{
				TMP_Settings tMP_Settings = Resources.Load("TMP Settings") as TMP_Settings;
				if (tMP_Settings != null)
				{
					s_Instance = tMP_Settings;
				}
			}
			return s_Instance;
		}

		public static TMP_Settings GetSettings()
		{
			if (instance == null)
			{
				return null;
			}
			return instance;
		}

		public static TMP_FontAsset GetFontAsset()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultFontAsset;
		}

		public static TMP_SpriteAsset GetSpriteAsset()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultSpriteAsset;
		}

		public static TMP_StyleSheet GetStyleSheet()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.m_defaultStyleSheet;
		}
	}
}
