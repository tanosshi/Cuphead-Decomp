using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_FontAsset : TMP_Asset
	{
		public enum FontAssetTypes
		{
			None = 0,
			SDF = 1,
			Bitmap = 2
		}

		private static TMP_FontAsset s_defaultFontAsset;

		public FontAssetTypes fontAssetType;

		[SerializeField]
		private FaceInfo m_fontInfo;

		[SerializeField]
		public Texture2D atlas;

		[SerializeField]
		private List<TMP_Glyph> m_glyphInfoList;

		private Dictionary<int, TMP_Glyph> m_characterDictionary;

		private Dictionary<int, KerningPair> m_kerningDictionary;

		[SerializeField]
		private KerningTable m_kerningInfo;

		[SerializeField]
		private KerningPair m_kerningPair;

		[SerializeField]
		private LineBreakingTable m_lineBreakingInfo;

		[SerializeField]
		public List<TMP_FontAsset> fallbackFontAssets;

		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		public TMP_FontWeights[] fontWeights = new TMP_FontWeights[10];

		private int[] m_characterSet;

		public float normalStyle;

		public float normalSpacingOffset;

		public float boldStyle = 0.75f;

		public float boldSpacing = 7f;

		public byte italicStyle = 35;

		public byte tabSize = 10;

		private byte m_oldTabSize;

		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				if (s_defaultFontAsset == null)
				{
					s_defaultFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
				}
				return s_defaultFontAsset;
			}
		}

		public FaceInfo fontInfo
		{
			get
			{
				return m_fontInfo;
			}
		}

		public Dictionary<int, TMP_Glyph> characterDictionary
		{
			get
			{
				return m_characterDictionary;
			}
		}

		public Dictionary<int, KerningPair> kerningDictionary
		{
			get
			{
				return m_kerningDictionary;
			}
		}

		public KerningTable kerningInfo
		{
			get
			{
				return m_kerningInfo;
			}
		}

		public LineBreakingTable lineBreakingInfo
		{
			get
			{
				return m_lineBreakingInfo;
			}
		}

		private void OnEnable()
		{
			if (m_characterDictionary == null)
			{
				ReadFontDefinition();
			}
		}

		private void OnDisable()
		{
		}

		public void AddFaceInfo(FaceInfo faceInfo)
		{
			m_fontInfo = faceInfo;
		}

		public void AddGlyphInfo(TMP_Glyph[] glyphInfo)
		{
			m_glyphInfoList = new List<TMP_Glyph>();
			int num = glyphInfo.Length;
			m_fontInfo.CharacterCount = num;
			m_characterSet = new int[num];
			for (int i = 0; i < num; i++)
			{
				TMP_Glyph tMP_Glyph = new TMP_Glyph();
				tMP_Glyph.id = glyphInfo[i].id;
				tMP_Glyph.x = glyphInfo[i].x;
				tMP_Glyph.y = glyphInfo[i].y;
				tMP_Glyph.width = glyphInfo[i].width;
				tMP_Glyph.height = glyphInfo[i].height;
				tMP_Glyph.xOffset = glyphInfo[i].xOffset;
				tMP_Glyph.yOffset = glyphInfo[i].yOffset + m_fontInfo.Padding;
				tMP_Glyph.xAdvance = glyphInfo[i].xAdvance;
				m_glyphInfoList.Add(tMP_Glyph);
				m_characterSet[i] = tMP_Glyph.id;
			}
			m_glyphInfoList = m_glyphInfoList.OrderBy((TMP_Glyph s) => s.id).ToList();
		}

		public void AddKerningInfo(KerningTable kerningTable)
		{
			m_kerningInfo = kerningTable;
		}

		public void ReadFontDefinition()
		{
			if (m_fontInfo == null)
			{
				return;
			}
			m_characterDictionary = new Dictionary<int, TMP_Glyph>();
			foreach (TMP_Glyph glyphInfo in m_glyphInfoList)
			{
				if (!m_characterDictionary.ContainsKey(glyphInfo.id))
				{
					m_characterDictionary.Add(glyphInfo.id, glyphInfo);
				}
			}
			TMP_Glyph tMP_Glyph = new TMP_Glyph();
			if (m_characterDictionary.ContainsKey(32))
			{
				m_characterDictionary[32].width = m_characterDictionary[32].xAdvance;
				m_characterDictionary[32].height = m_fontInfo.Ascender - m_fontInfo.Descender;
				m_characterDictionary[32].yOffset = m_fontInfo.Ascender;
			}
			else
			{
				tMP_Glyph = new TMP_Glyph();
				tMP_Glyph.id = 32;
				tMP_Glyph.x = 0f;
				tMP_Glyph.y = 0f;
				tMP_Glyph.width = m_fontInfo.Ascender / 5f;
				tMP_Glyph.height = m_fontInfo.Ascender - m_fontInfo.Descender;
				tMP_Glyph.xOffset = 0f;
				tMP_Glyph.yOffset = m_fontInfo.Ascender;
				tMP_Glyph.xAdvance = m_fontInfo.PointSize / 4f;
				m_characterDictionary.Add(32, tMP_Glyph);
			}
			if (!m_characterDictionary.ContainsKey(160))
			{
				tMP_Glyph = TMP_Glyph.Clone(m_characterDictionary[32]);
				m_characterDictionary.Add(160, tMP_Glyph);
			}
			if (!m_characterDictionary.ContainsKey(8203))
			{
				tMP_Glyph = TMP_Glyph.Clone(m_characterDictionary[32]);
				tMP_Glyph.width = 0f;
				tMP_Glyph.xAdvance = 0f;
				m_characterDictionary.Add(8203, tMP_Glyph);
			}
			if (!m_characterDictionary.ContainsKey(10))
			{
				tMP_Glyph = new TMP_Glyph();
				tMP_Glyph.id = 10;
				tMP_Glyph.x = 0f;
				tMP_Glyph.y = 0f;
				tMP_Glyph.width = 10f;
				tMP_Glyph.height = m_characterDictionary[32].height;
				tMP_Glyph.xOffset = 0f;
				tMP_Glyph.yOffset = m_characterDictionary[32].yOffset;
				tMP_Glyph.xAdvance = 0f;
				m_characterDictionary.Add(10, tMP_Glyph);
				if (!m_characterDictionary.ContainsKey(13))
				{
					m_characterDictionary.Add(13, tMP_Glyph);
				}
			}
			if (!m_characterDictionary.ContainsKey(9))
			{
				tMP_Glyph = new TMP_Glyph();
				tMP_Glyph.id = 9;
				tMP_Glyph.x = m_characterDictionary[32].x;
				tMP_Glyph.y = m_characterDictionary[32].y;
				tMP_Glyph.width = m_characterDictionary[32].width * (float)(int)tabSize + (m_characterDictionary[32].xAdvance - m_characterDictionary[32].width) * (float)(tabSize - 1);
				tMP_Glyph.height = m_characterDictionary[32].height;
				tMP_Glyph.xOffset = m_characterDictionary[32].xOffset;
				tMP_Glyph.yOffset = m_characterDictionary[32].yOffset;
				tMP_Glyph.xAdvance = m_characterDictionary[32].xAdvance * (float)(int)tabSize;
				m_characterDictionary.Add(9, tMP_Glyph);
			}
			m_fontInfo.TabWidth = m_characterDictionary[9].xAdvance;
			if (m_fontInfo.Scale == 0f)
			{
				m_fontInfo.Scale = 1f;
			}
			m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = m_kerningInfo.kerningPairs;
			for (int i = 0; i < kerningPairs.Count; i++)
			{
				KerningPair kerningPair = kerningPairs[i];
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
				else if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning("Kerning Key for [" + kerningPairKey.ascii_Left + "] and [" + kerningPairKey.ascii_Right + "] already exists.");
				}
			}
			m_lineBreakingInfo = new LineBreakingTable();
			TextAsset textAsset = Resources.Load("LineBreaking Leading Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				m_lineBreakingInfo.leadingCharacters = GetCharacters(textAsset);
			}
			TextAsset textAsset2 = Resources.Load("LineBreaking Following Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset2 != null)
			{
				m_lineBreakingInfo.followingCharacters = GetCharacters(textAsset2);
			}
			hashCode = TMP_TextUtilities.GetSimpleHashCode(base.name);
			materialHashCode = TMP_TextUtilities.GetSimpleHashCode(material.name);
		}

		private Dictionary<int, char> GetCharacters(TextAsset file)
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>();
			string text = file.text;
			foreach (char c in text)
			{
				if (!dictionary.ContainsKey(c))
				{
					dictionary.Add(c, c);
				}
			}
			return dictionary;
		}

		public bool HasCharacter(int character)
		{
			if (m_characterDictionary == null)
			{
				return false;
			}
			if (m_characterDictionary.ContainsKey(character))
			{
				return true;
			}
			return false;
		}

		public bool HasCharacter(char character)
		{
			if (m_characterDictionary == null)
			{
				return false;
			}
			if (m_characterDictionary.ContainsKey(character))
			{
				return true;
			}
			return false;
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			if (m_characterDictionary == null)
			{
				missingCharacters = null;
				return false;
			}
			missingCharacters = new List<char>();
			for (int i = 0; i < text.Length; i++)
			{
				if (!m_characterDictionary.ContainsKey(text[i]))
				{
					missingCharacters.Add(text[i]);
				}
			}
			if (missingCharacters.Count == 0)
			{
				return true;
			}
			return false;
		}
	}
}
