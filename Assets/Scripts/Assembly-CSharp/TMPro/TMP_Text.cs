using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro
{
	public class TMP_Text : MaskableGraphic
	{
		protected enum TextInputSources
		{
			Text = 0,
			SetText = 1,
			SetCharArray = 2
		}

		[SerializeField]
		protected string m_text;

		[SerializeField]
		protected TMP_FontAsset m_fontAsset;

		protected TMP_FontAsset m_currentFontAsset;

		protected bool m_isSDFShader;

		[SerializeField]
		protected Material m_sharedMaterial;

		protected Material m_currentMaterial;

		protected MaterialReference[] m_materialReferences = new MaterialReference[32];

		protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

		protected TMP_XmlTagStack<MaterialReference> m_materialReferenceStack = new TMP_XmlTagStack<MaterialReference>(new MaterialReference[16]);

		protected int m_currentMaterialIndex;

		protected int m_sharedMaterialHashCode;

		[SerializeField]
		protected Material[] m_fontSharedMaterials;

		[SerializeField]
		protected Material m_fontMaterial;

		[SerializeField]
		protected Material[] m_fontMaterials;

		protected bool m_isMaterialDirty;

		[FormerlySerializedAs("m_fontColor")]
		[SerializeField]
		protected Color32 m_fontColor32 = Color.white;

		[SerializeField]
		protected Color m_fontColor = Color.white;

		protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		[SerializeField]
		protected bool m_enableVertexGradient;

		[SerializeField]
		protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		protected TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		protected bool m_tintAllSprites;

		protected bool m_tintSprite;

		protected Color32 m_spriteColor;

		[SerializeField]
		protected bool m_overrideHtmlColors;

		[SerializeField]
		protected Color32 m_faceColor = Color.white;

		[SerializeField]
		protected Color32 m_outlineColor = Color.black;

		protected float m_outlineWidth;

		[SerializeField]
		protected float m_fontSize = 36f;

		protected float m_currentFontSize;

		[SerializeField]
		protected float m_fontSizeBase = 36f;

		protected TMP_XmlTagStack<float> m_sizeStack = new TMP_XmlTagStack<float>(new float[16]);

		[SerializeField]
		protected int m_fontWeight = 400;

		protected int m_fontWeightInternal;

		protected TMP_XmlTagStack<int> m_fontWeightStack = new TMP_XmlTagStack<int>(new int[16]);

		[SerializeField]
		protected bool m_enableAutoSizing;

		protected float m_maxFontSize;

		protected float m_minFontSize;

		[SerializeField]
		protected float m_fontSizeMin;

		[SerializeField]
		protected float m_fontSizeMax;

		[SerializeField]
		protected FontStyles m_fontStyle;

		protected FontStyles m_style;

		protected bool m_isUsingBold;

		[SerializeField]
		[FormerlySerializedAs("m_lineJustification")]
		protected TextAlignmentOptions m_textAlignment;

		protected TextAlignmentOptions m_lineJustification;

		protected Vector3[] m_textContainerLocalCorners = new Vector3[4];

		[SerializeField]
		protected float m_characterSpacing;

		protected float m_cSpacing;

		protected float m_monoSpacing;

		[SerializeField]
		protected float m_lineSpacing;

		protected float m_lineSpacingDelta;

		protected float m_lineHeight;

		[SerializeField]
		protected float m_lineSpacingMax;

		[SerializeField]
		protected float m_paragraphSpacing;

		[SerializeField]
		protected float m_charWidthMaxAdj;

		protected float m_charWidthAdjDelta;

		[SerializeField]
		protected bool m_enableWordWrapping;

		protected bool m_isCharacterWrappingEnabled;

		protected bool m_isNonBreakingSpace;

		protected bool m_isIgnoringAlignment;

		[SerializeField]
		protected float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		protected TextOverflowModes m_overflowMode;

		protected bool m_isTextTruncated;

		[SerializeField]
		protected bool m_enableKerning;

		[SerializeField]
		protected bool m_enableExtraPadding;

		[SerializeField]
		protected bool checkPaddingRequired;

		[SerializeField]
		protected bool m_isRichText = true;

		protected bool m_parseCtrlCharacters = true;

		protected bool m_isOverlay;

		[SerializeField]
		protected bool m_isOrthographic;

		[SerializeField]
		protected bool m_isCullingEnabled;

		[SerializeField]
		protected bool m_ignoreCulling = true;

		[SerializeField]
		protected TextureMappingOptions m_horizontalMapping;

		[SerializeField]
		protected TextureMappingOptions m_verticalMapping;

		protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

		protected int m_maxVisibleCharacters = 99999;

		protected int m_maxVisibleWords = 99999;

		protected int m_maxVisibleLines = 99999;

		[SerializeField]
		protected int m_pageToDisplay = 1;

		protected bool m_isNewPage;

		[SerializeField]
		protected Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		protected float m_marginLeft;

		protected float m_marginRight;

		protected float m_marginWidth;

		protected float m_marginHeight;

		protected float m_width = -1f;

		[SerializeField]
		protected TMP_TextInfo m_textInfo;

		[SerializeField]
		protected bool m_havePropertiesChanged;

		[SerializeField]
		protected bool m_isUsingLegacyAnimationComponent;

		protected Transform m_transform;

		protected RectTransform m_rectTransform;

		protected Mesh m_mesh;

		protected float m_flexibleHeight = -1f;

		protected float m_flexibleWidth = -1f;

		protected float m_minHeight;

		protected float m_minWidth;

		protected float m_preferredWidth = 9999f;

		protected float m_renderedWidth;

		protected float m_preferredHeight = 9999f;

		protected float m_renderedHeight;

		protected int m_layoutPriority;

		protected bool m_isCalculateSizeRequired;

		protected bool m_isLayoutDirty;

		protected bool m_verticesAlreadyDirty;

		protected bool m_layoutAlreadyDirty;

		[SerializeField]
		protected bool m_isInputParsingRequired;

		[SerializeField]
		protected bool m_isRightToLeft;

		[SerializeField]
		protected TextInputSources m_inputSource;

		protected string old_text;

		protected float old_arg0;

		protected float old_arg1;

		protected float old_arg2;

		protected float m_fontScale;

		protected float m_fontScaleMultiplier;

		protected char[] m_htmlTag = new char[64];

		protected XML_TagAttribute[] m_xmlAttribute = new XML_TagAttribute[8];

		protected float tag_LineIndent;

		protected float tag_Indent;

		protected TMP_XmlTagStack<float> m_indentStack = new TMP_XmlTagStack<float>(new float[16]);

		protected bool tag_NoParsing;

		protected bool m_isParsingText;

		protected int[] m_char_buffer;

		private TMP_CharacterInfo[] m_internalCharacterInfo;

		protected char[] m_input_CharArray = new char[256];

		private int m_charArray_Length;

		protected int m_totalCharacterCount;

		protected int m_characterCount;

		protected int m_visibleCharacterCount;

		protected int m_visibleSpriteCount;

		protected int m_firstCharacterOfLine;

		protected int m_firstVisibleCharacterOfLine;

		protected int m_lastCharacterOfLine;

		protected int m_lastVisibleCharacterOfLine;

		protected int m_lineNumber;

		protected int m_pageNumber;

		protected float m_maxAscender;

		protected float m_maxDescender;

		protected float m_maxLineAscender;

		protected float m_maxLineDescender;

		protected float m_startOfLineAscender;

		protected float m_lineOffset;

		protected Extents m_meshExtents;

		protected Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		protected TMP_XmlTagStack<Color32> m_colorStack = new TMP_XmlTagStack<Color32>(new Color32[16]);

		protected float m_tabSpacing;

		protected float m_spacing;

		protected bool IsRectTransformDriven;

		protected TMP_XmlTagStack<int> m_styleStack = new TMP_XmlTagStack<int>(new int[16]);

		protected TMP_XmlTagStack<int> m_actionStack = new TMP_XmlTagStack<int>(new int[16]);

		protected float m_padding;

		protected float m_baselineOffset;

		protected float m_xAdvance;

		protected TMP_TextElementType m_textElementType;

		protected TMP_TextElement m_cached_TextElement;

		protected TMP_Glyph m_cached_Underline_GlyphInfo;

		protected TMP_Glyph m_cached_Ellipsis_GlyphInfo;

		protected TMP_SpriteAsset m_defaultSpriteAsset;

		protected TMP_SpriteAsset m_currentSpriteAsset;

		protected int m_spriteCount;

		protected int m_spriteIndex;

		protected InlineGraphicManager m_inlineGraphics;

		private readonly float[] k_Power = new float[10] { 0.5f, 0.05f, 0.005f, 0.0005f, 5E-05f, 5E-06f, 5E-07f, 5E-08f, 5E-09f, 5E-10f };

		protected static Vector2 k_InfinityVectorPositive = new Vector2(1000000f, 1000000f);

		protected static Vector2 k_InfinityVectorNegative = new Vector2(-1000000f, -1000000f);

		public string text
		{
			get
			{
				return m_text;
			}
			set
			{
				if (!(m_text == value))
				{
					m_text = value;
					m_inputSource = TextInputSources.Text;
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					m_isInputParsingRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public TMP_FontAsset font
		{
			get
			{
				return m_fontAsset;
			}
			set
			{
				if (!(m_fontAsset == value))
				{
					m_fontAsset = value;
					LoadFontAsset();
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					m_isInputParsingRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public virtual Material fontSharedMaterial
		{
			get
			{
				return m_sharedMaterial;
			}
			set
			{
				if (!(m_sharedMaterial == value))
				{
					SetSharedMaterial(value);
					m_havePropertiesChanged = true;
					m_isInputParsingRequired = true;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public virtual Material[] fontSharedMaterials
		{
			get
			{
				return GetSharedMaterials();
			}
			set
			{
				SetSharedMaterials(value);
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}

		public Material fontMaterial
		{
			get
			{
				return GetMaterial(m_sharedMaterial);
			}
			set
			{
				if (!(m_sharedMaterial != null) || m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
				{
					m_sharedMaterial = value;
					m_padding = GetPaddingForMaterial();
					m_havePropertiesChanged = true;
					m_isInputParsingRequired = true;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public virtual Material[] fontMaterials
		{
			get
			{
				return GetMaterials(m_fontSharedMaterials);
			}
			set
			{
				SetSharedMaterials(value);
				m_havePropertiesChanged = true;
				m_isInputParsingRequired = true;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}

		public new Color color
		{
			get
			{
				return m_fontColor;
			}
			set
			{
				if (!(m_fontColor == value))
				{
					m_havePropertiesChanged = true;
					m_fontColor = value;
					SetVerticesDirty();
				}
			}
		}

		public float alpha
		{
			get
			{
				return m_fontColor.a;
			}
			set
			{
				if (m_fontColor.a != value)
				{
					m_fontColor.a = value;
					m_havePropertiesChanged = true;
					SetVerticesDirty();
				}
			}
		}

		public bool enableVertexGradient
		{
			get
			{
				return m_enableVertexGradient;
			}
			set
			{
				if (m_enableVertexGradient != value)
				{
					m_havePropertiesChanged = true;
					m_enableVertexGradient = value;
					SetVerticesDirty();
				}
			}
		}

		public VertexGradient colorGradient
		{
			get
			{
				return m_fontColorGradient;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_fontColorGradient = value;
				SetVerticesDirty();
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return m_spriteAsset;
			}
			set
			{
				m_spriteAsset = value;
			}
		}

		public bool tintAllSprites
		{
			get
			{
				return m_tintAllSprites;
			}
			set
			{
				if (m_tintAllSprites != value)
				{
					m_tintAllSprites = value;
					m_havePropertiesChanged = true;
					SetVerticesDirty();
				}
			}
		}

		public bool overrideColorTags
		{
			get
			{
				return m_overrideHtmlColors;
			}
			set
			{
				if (m_overrideHtmlColors != value)
				{
					m_havePropertiesChanged = true;
					m_overrideHtmlColors = value;
					SetVerticesDirty();
				}
			}
		}

		public Color32 faceColor
		{
			get
			{
				if (m_sharedMaterial == null)
				{
					return m_faceColor;
				}
				m_faceColor = m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
				return m_faceColor;
			}
			set
			{
				if (!m_faceColor.Compare(value))
				{
					SetFaceColor(value);
					m_havePropertiesChanged = true;
					m_faceColor = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Color32 outlineColor
		{
			get
			{
				if (m_sharedMaterial == null)
				{
					return m_outlineColor;
				}
				m_outlineColor = m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
				return m_outlineColor;
			}
			set
			{
				if (!m_outlineColor.Compare(value))
				{
					SetOutlineColor(value);
					m_havePropertiesChanged = true;
					m_outlineColor = value;
					SetVerticesDirty();
				}
			}
		}

		public float outlineWidth
		{
			get
			{
				if (m_sharedMaterial == null)
				{
					return m_outlineWidth;
				}
				m_outlineWidth = m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
				return m_outlineWidth;
			}
			set
			{
				if (m_outlineWidth != value)
				{
					SetOutlineThickness(value);
					m_havePropertiesChanged = true;
					m_outlineWidth = value;
					SetVerticesDirty();
				}
			}
		}

		public float fontSize
		{
			get
			{
				return m_fontSize;
			}
			set
			{
				if (m_fontSize != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_fontSize = value;
					if (!m_enableAutoSizing)
					{
						m_fontSizeBase = m_fontSize;
					}
				}
			}
		}

		public float fontScale
		{
			get
			{
				return m_fontScale;
			}
		}

		public int fontWeight
		{
			get
			{
				return m_fontWeight;
			}
			set
			{
				if (m_fontWeight != value)
				{
					m_fontWeight = value;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				if (!canvas)
				{
					return 1f;
				}
				if (!font)
				{
					return canvas.scaleFactor;
				}
				if (m_currentFontAsset == null || m_currentFontAsset.fontInfo.PointSize <= 0f || m_fontSize <= 0f)
				{
					return 1f;
				}
				return m_fontSize / m_currentFontAsset.fontInfo.PointSize;
			}
		}

		public bool enableAutoSizing
		{
			get
			{
				return m_enableAutoSizing;
			}
			set
			{
				if (m_enableAutoSizing != value)
				{
					m_enableAutoSizing = value;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public float fontSizeMin
		{
			get
			{
				return m_fontSizeMin;
			}
			set
			{
				if (m_fontSizeMin != value)
				{
					m_fontSizeMin = value;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public float fontSizeMax
		{
			get
			{
				return m_fontSizeMax;
			}
			set
			{
				if (m_fontSizeMax != value)
				{
					m_fontSizeMax = value;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public FontStyles fontStyle
		{
			get
			{
				return m_fontStyle;
			}
			set
			{
				if (m_fontStyle != value)
				{
					m_fontStyle = value;
					m_havePropertiesChanged = true;
					checkPaddingRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public bool isUsingBold
		{
			get
			{
				return m_isUsingBold;
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return m_textAlignment;
			}
			set
			{
				if (m_textAlignment != value)
				{
					m_havePropertiesChanged = true;
					m_textAlignment = value;
					SetVerticesDirty();
				}
			}
		}

		public float characterSpacing
		{
			get
			{
				return m_characterSpacing;
			}
			set
			{
				if (m_characterSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_characterSpacing = value;
				}
			}
		}

		public float lineSpacing
		{
			get
			{
				return m_lineSpacing;
			}
			set
			{
				if (m_lineSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_lineSpacing = value;
				}
			}
		}

		public float paragraphSpacing
		{
			get
			{
				return m_paragraphSpacing;
			}
			set
			{
				if (m_paragraphSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_paragraphSpacing = value;
				}
			}
		}

		public float characterWidthAdjustment
		{
			get
			{
				return m_charWidthMaxAdj;
			}
			set
			{
				if (m_charWidthMaxAdj != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_charWidthMaxAdj = value;
				}
			}
		}

		public bool enableWordWrapping
		{
			get
			{
				return m_enableWordWrapping;
			}
			set
			{
				if (m_enableWordWrapping != value)
				{
					m_havePropertiesChanged = true;
					m_isInputParsingRequired = true;
					m_isCalculateSizeRequired = true;
					m_enableWordWrapping = value;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public float wordWrappingRatios
		{
			get
			{
				return m_wordWrappingRatios;
			}
			set
			{
				if (m_wordWrappingRatios != value)
				{
					m_wordWrappingRatios = value;
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public TextOverflowModes OverflowMode
		{
			get
			{
				return m_overflowMode;
			}
			set
			{
				if (m_overflowMode != value)
				{
					m_overflowMode = value;
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public bool enableKerning
		{
			get
			{
				return m_enableKerning;
			}
			set
			{
				if (m_enableKerning != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_enableKerning = value;
				}
			}
		}

		public bool extraPadding
		{
			get
			{
				return m_enableExtraPadding;
			}
			set
			{
				if (m_enableExtraPadding != value)
				{
					m_havePropertiesChanged = true;
					m_enableExtraPadding = value;
					UpdateMeshPadding();
					SetVerticesDirty();
				}
			}
		}

		public bool richText
		{
			get
			{
				return m_isRichText;
			}
			set
			{
				if (m_isRichText != value)
				{
					m_isRichText = value;
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_isInputParsingRequired = true;
				}
			}
		}

		public bool parseCtrlCharacters
		{
			get
			{
				return m_parseCtrlCharacters;
			}
			set
			{
				if (m_parseCtrlCharacters != value)
				{
					m_parseCtrlCharacters = value;
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					SetVerticesDirty();
					SetLayoutDirty();
					m_isInputParsingRequired = true;
				}
			}
		}

		public bool isOverlay
		{
			get
			{
				return m_isOverlay;
			}
			set
			{
				if (m_isOverlay != value)
				{
					m_isOverlay = value;
					SetShaderDepth();
					m_havePropertiesChanged = true;
					SetVerticesDirty();
				}
			}
		}

		public bool isOrthographic
		{
			get
			{
				return m_isOrthographic;
			}
			set
			{
				if (m_isOrthographic != value)
				{
					m_havePropertiesChanged = true;
					m_isOrthographic = value;
					SetVerticesDirty();
				}
			}
		}

		public bool enableCulling
		{
			get
			{
				return m_isCullingEnabled;
			}
			set
			{
				if (m_isCullingEnabled != value)
				{
					m_isCullingEnabled = value;
					SetCulling();
					m_havePropertiesChanged = true;
				}
			}
		}

		public bool ignoreVisibility
		{
			get
			{
				return m_ignoreCulling;
			}
			set
			{
				if (m_ignoreCulling != value)
				{
					m_havePropertiesChanged = true;
					m_ignoreCulling = value;
				}
			}
		}

		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return m_horizontalMapping;
			}
			set
			{
				if (m_horizontalMapping != value)
				{
					m_havePropertiesChanged = true;
					m_horizontalMapping = value;
					SetVerticesDirty();
				}
			}
		}

		public TextureMappingOptions verticalMapping
		{
			get
			{
				return m_verticalMapping;
			}
			set
			{
				if (m_verticalMapping != value)
				{
					m_havePropertiesChanged = true;
					m_verticalMapping = value;
					SetVerticesDirty();
				}
			}
		}

		public TextRenderFlags renderMode
		{
			get
			{
				return m_renderMode;
			}
			set
			{
				if (m_renderMode != value)
				{
					m_renderMode = value;
					m_havePropertiesChanged = true;
				}
			}
		}

		public int maxVisibleCharacters
		{
			get
			{
				return m_maxVisibleCharacters;
			}
			set
			{
				if (m_maxVisibleCharacters != value)
				{
					m_havePropertiesChanged = true;
					m_maxVisibleCharacters = value;
					SetVerticesDirty();
				}
			}
		}

		public int maxVisibleWords
		{
			get
			{
				return m_maxVisibleWords;
			}
			set
			{
				if (m_maxVisibleWords != value)
				{
					m_havePropertiesChanged = true;
					m_maxVisibleWords = value;
					SetVerticesDirty();
				}
			}
		}

		public int maxVisibleLines
		{
			get
			{
				return m_maxVisibleLines;
			}
			set
			{
				if (m_maxVisibleLines != value)
				{
					m_havePropertiesChanged = true;
					m_isInputParsingRequired = true;
					m_maxVisibleLines = value;
					SetVerticesDirty();
				}
			}
		}

		public int pageToDisplay
		{
			get
			{
				return m_pageToDisplay;
			}
			set
			{
				if (m_pageToDisplay != value)
				{
					m_havePropertiesChanged = true;
					m_pageToDisplay = value;
					SetVerticesDirty();
				}
			}
		}

		public virtual Vector4 margin
		{
			get
			{
				return m_margin;
			}
			set
			{
				if (!(m_margin == value))
				{
					m_margin = value;
					ComputeMarginSize();
					m_havePropertiesChanged = true;
					SetVerticesDirty();
				}
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				return m_textInfo;
			}
		}

		public bool havePropertiesChanged
		{
			get
			{
				return m_havePropertiesChanged;
			}
			set
			{
				if (m_havePropertiesChanged != value)
				{
					m_havePropertiesChanged = value;
					SetVerticesDirty();
					SetLayoutDirty();
				}
			}
		}

		public bool isUsingLegacyAnimationComponent
		{
			get
			{
				return m_isUsingLegacyAnimationComponent;
			}
			set
			{
				m_isUsingLegacyAnimationComponent = value;
			}
		}

		public new Transform transform
		{
			get
			{
				if (m_transform == null)
				{
					m_transform = GetComponent<Transform>();
				}
				return m_transform;
			}
		}

		public new RectTransform rectTransform
		{
			get
			{
				if (m_rectTransform == null)
				{
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}

		public virtual bool autoSizeTextContainer { get; set; }

		public virtual Mesh mesh
		{
			get
			{
				return m_mesh;
			}
		}

		public virtual Bounds bounds { get; set; }

		public float flexibleHeight
		{
			get
			{
				return m_flexibleHeight;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return m_flexibleWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return m_minHeight;
			}
		}

		public float minWidth
		{
			get
			{
				return m_minWidth;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return (m_preferredWidth != 9999f) ? m_preferredWidth : GetPreferredWidth();
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return (m_preferredHeight != 9999f) ? m_preferredHeight : GetPreferredHeight();
			}
		}

		public int layoutPriority
		{
			get
			{
				return m_layoutPriority;
			}
		}

		protected virtual void LoadFontAsset()
		{
		}

		protected virtual void SetSharedMaterial(Material mat)
		{
		}

		protected virtual Material GetMaterial(Material mat)
		{
			return null;
		}

		protected virtual void SetFontBaseMaterial(Material mat)
		{
		}

		protected virtual Material[] GetSharedMaterials()
		{
			return null;
		}

		protected virtual void SetSharedMaterials(Material[] materials)
		{
		}

		protected virtual Material[] GetMaterials(Material[] mats)
		{
			return null;
		}

		protected virtual Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		protected virtual void SetFaceColor(Color32 color)
		{
		}

		protected virtual void SetOutlineColor(Color32 color)
		{
		}

		protected virtual void SetOutlineThickness(float thickness)
		{
		}

		protected virtual void SetShaderDepth()
		{
		}

		protected virtual void SetCulling()
		{
		}

		protected virtual float GetPaddingForMaterial()
		{
			return 0f;
		}

		protected virtual float GetPaddingForMaterial(Material mat)
		{
			return 0f;
		}

		protected virtual Vector3[] GetTextContainerLocalCorners()
		{
			return null;
		}

		public virtual void ForceMeshUpdate()
		{
		}

		public virtual void UpdateGeometry(Mesh mesh, int index)
		{
		}

		public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
		}

		public virtual void UpdateVertexData()
		{
		}

		public virtual void SetVertices(Vector3[] vertices)
		{
		}

		public virtual void UpdateMeshPadding()
		{
		}

		public void SetText(string text)
		{
			StringToCharArray(text, ref m_char_buffer);
			m_inputSource = TextInputSources.SetCharArray;
			m_isInputParsingRequired = true;
			m_havePropertiesChanged = true;
			m_isCalculateSizeRequired = true;
			SetVerticesDirty();
			SetLayoutDirty();
		}

		public void SetText(string text, float arg0)
		{
			SetText(text, arg0, 255f, 255f);
		}

		public void SetText(string text, float arg0, float arg1)
		{
			SetText(text, arg0, arg1, 255f);
		}

		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (text == old_text && arg0 == old_arg0 && arg1 == old_arg1 && arg2 == old_arg2)
			{
				return;
			}
			old_text = text;
			old_arg1 = 255f;
			old_arg2 = 255f;
			int precision = 0;
			int index = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '{')
				{
					if (text[i + 2] == ':')
					{
						precision = text[i + 3] - 48;
					}
					switch (text[i + 1] - 48)
					{
					case 0:
						old_arg0 = arg0;
						AddFloatToCharArray(arg0, ref index, precision);
						break;
					case 1:
						old_arg1 = arg1;
						AddFloatToCharArray(arg1, ref index, precision);
						break;
					case 2:
						old_arg2 = arg2;
						AddFloatToCharArray(arg2, ref index, precision);
						break;
					}
					i = ((text[i + 2] != ':') ? (i + 2) : (i + 4));
				}
				else
				{
					m_input_CharArray[index] = c;
					index++;
				}
			}
			m_input_CharArray[index] = '\0';
			m_charArray_Length = index;
			m_inputSource = TextInputSources.SetText;
			m_isInputParsingRequired = true;
			m_havePropertiesChanged = true;
			m_isCalculateSizeRequired = true;
			SetVerticesDirty();
			SetLayoutDirty();
		}

		public void SetText(StringBuilder text)
		{
			StringBuilderToIntArray(text, ref m_char_buffer);
			m_inputSource = TextInputSources.SetCharArray;
			m_isInputParsingRequired = true;
			m_havePropertiesChanged = true;
			m_isCalculateSizeRequired = true;
			SetVerticesDirty();
			SetLayoutDirty();
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray == null || charArray.Length == 0)
			{
				return;
			}
			if (m_char_buffer.Length <= charArray.Length)
			{
				int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
				m_char_buffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < charArray.Length; i++)
			{
				if (charArray[i] == '\\' && i < charArray.Length - 1)
				{
					switch (charArray[i + 1])
					{
					case 110:
						m_char_buffer[num2] = 10;
						i++;
						num2++;
						continue;
					case 114:
						m_char_buffer[num2] = 13;
						i++;
						num2++;
						continue;
					case 116:
						m_char_buffer[num2] = 9;
						i++;
						num2++;
						continue;
					}
				}
				m_char_buffer[num2] = charArray[i];
				num2++;
			}
			m_char_buffer[num2] = 0;
			m_inputSource = TextInputSources.SetCharArray;
			m_havePropertiesChanged = true;
			m_isInputParsingRequired = true;
		}

		protected void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray == null || m_charArray_Length == 0)
			{
				return;
			}
			if (charBuffer.Length <= m_charArray_Length)
			{
				int num = ((m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(m_charArray_Length + 1) : (m_charArray_Length + 256));
				charBuffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < m_charArray_Length; i++)
			{
				if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
				{
					charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
					i++;
					num2++;
				}
				else
				{
					charBuffer[num2] = charArray[i];
					num2++;
				}
			}
			charBuffer[num2] = 0;
		}

		protected void StringToCharArray(string text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
				chars = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (m_parseCtrlCharacters && text[i] == '\\' && text.Length > i + 1)
				{
					switch (text[i + 1])
					{
					case 85:
						if (text.Length > i + 9)
						{
							chars[num2] = GetUTF32(i + 2);
							i += 9;
							num2++;
							continue;
						}
						break;
					case 92:
						if (text.Length <= i + 2)
						{
							break;
						}
						chars[num2] = text[i + 1];
						chars[num2 + 1] = text[i + 2];
						i += 2;
						num2 += 2;
						continue;
					case 110:
						chars[num2] = 10;
						i++;
						num2++;
						continue;
					case 114:
						chars[num2] = 13;
						i++;
						num2++;
						continue;
					case 116:
						chars[num2] = 9;
						i++;
						num2++;
						continue;
					case 117:
						if (text.Length > i + 5)
						{
							chars[num2] = (ushort)GetUTF16(i + 2);
							i += 5;
							num2++;
							continue;
						}
						break;
					}
				}
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
				}
				else
				{
					chars[num2] = text[i];
					num2++;
				}
			}
			chars[num2] = 0;
		}

		protected void StringBuilderToIntArray(StringBuilder text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
				chars = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (m_parseCtrlCharacters && text[i] == '\\' && text.Length > i + 1)
				{
					switch (text[i + 1])
					{
					case 85:
						if (text.Length > i + 9)
						{
							chars[num2] = GetUTF32(i + 2);
							i += 9;
							num2++;
							continue;
						}
						break;
					case 92:
						if (text.Length <= i + 2)
						{
							break;
						}
						chars[num2] = text[i + 1];
						chars[num2 + 1] = text[i + 2];
						i += 2;
						num2 += 2;
						continue;
					case 110:
						chars[num2] = 10;
						i++;
						num2++;
						continue;
					case 114:
						chars[num2] = 13;
						i++;
						num2++;
						continue;
					case 116:
						chars[num2] = 9;
						i++;
						num2++;
						continue;
					case 117:
						if (text.Length > i + 5)
						{
							chars[num2] = (ushort)GetUTF16(i + 2);
							i += 5;
							num2++;
							continue;
						}
						break;
					}
				}
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
				}
				else
				{
					chars[num2] = text[i];
					num2++;
				}
			}
			chars[num2] = 0;
		}

		protected void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				m_input_CharArray[index++] = '-';
				number = 0f - number;
			}
			number += k_Power[Mathf.Min(9, precision)];
			int num = (int)number;
			AddIntToCharArray(num, ref index, precision);
			if (precision > 0)
			{
				m_input_CharArray[index++] = '.';
				number -= (float)num;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num2 = (int)number;
					m_input_CharArray[index++] = (char)(num2 + 48);
					number -= (float)num2;
				}
			}
		}

		protected void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				m_input_CharArray[index++] = '-';
				number = -number;
			}
			int num = index;
			do
			{
				m_input_CharArray[num++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num2 = num;
			while (index + 1 < num)
			{
				num--;
				char c = m_input_CharArray[index];
				m_input_CharArray[index] = m_input_CharArray[num];
				m_input_CharArray[num] = c;
				index++;
			}
			index = num2;
		}

		protected virtual int SetArraySizes(int[] chars)
		{
			return 0;
		}

		protected void ParseInputText()
		{
			m_isInputParsingRequired = false;
			switch (m_inputSource)
			{
			case TextInputSources.Text:
				StringToCharArray(m_text, ref m_char_buffer);
				break;
			case TextInputSources.SetText:
				SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
				break;
			}
			SetArraySizes(m_char_buffer);
		}

		protected virtual void GenerateTextMesh()
		{
		}

		public Vector2 GetPreferredValues()
		{
			if (m_isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			float x = GetPreferredWidth();
			float y = GetPreferredHeight();
			return new Vector2(x, y);
		}

		public Vector2 GetPreferredValues(float width, float height)
		{
			if (m_isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			Vector2 vector = new Vector2(width, height);
			float x = GetPreferredWidth(vector);
			float y = GetPreferredHeight(vector);
			return new Vector2(x, y);
		}

		public Vector2 GetPreferredValues(string text)
		{
			StringToCharArray(text, ref m_char_buffer);
			SetArraySizes(m_char_buffer);
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			float x = GetPreferredWidth(vector);
			float y = GetPreferredHeight(vector);
			return new Vector2(x, y);
		}

		public Vector2 GetPreferredValues(string text, float width, float height)
		{
			StringToCharArray(text, ref m_char_buffer);
			SetArraySizes(m_char_buffer);
			Vector2 vector = new Vector2(width, height);
			float x = GetPreferredWidth(vector);
			float y = GetPreferredHeight(vector);
			return new Vector2(x, y);
		}

		protected float GetPreferredWidth()
		{
			float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
			Vector2 marginSize = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			if (m_isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			return CalculatePreferredValues(defaultFontSize, marginSize).x;
		}

		protected float GetPreferredWidth(Vector2 margin)
		{
			float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
			return CalculatePreferredValues(defaultFontSize, margin).x;
		}

		protected float GetPreferredHeight()
		{
			float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
			Vector2 marginSize = new Vector2((m_marginWidth == 0f) ? float.PositiveInfinity : m_marginWidth, float.PositiveInfinity);
			if (m_isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			return CalculatePreferredValues(defaultFontSize, marginSize).y;
		}

		protected float GetPreferredHeight(Vector2 margin)
		{
			float defaultFontSize = ((!m_enableAutoSizing) ? m_fontSize : m_fontSizeMax);
			return CalculatePreferredValues(defaultFontSize, margin).y;
		}

		protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize)
		{
			if (m_fontAsset == null || m_fontAsset.characterDictionary == null)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + GetInstanceID());
				return Vector2.zero;
			}
			if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == 0)
			{
				return Vector2.zero;
			}
			m_currentFontAsset = m_fontAsset;
			m_currentMaterial = m_sharedMaterial;
			m_currentMaterialIndex = 0;
			m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
			int totalCharacterCount = m_totalCharacterCount;
			if (m_internalCharacterInfo == null || totalCharacterCount > m_internalCharacterInfo.Length)
			{
				m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount <= 1024) ? Mathf.NextPowerOfTwo(totalCharacterCount) : (totalCharacterCount + 256)];
			}
			m_fontScale = defaultFontSize / m_currentFontAsset.fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
			m_fontScaleMultiplier = 1f;
			float num = defaultFontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
			float num2 = m_fontScale;
			m_currentFontSize = defaultFontSize;
			m_sizeStack.SetDefault(m_currentFontSize);
			int num3 = 0;
			m_style = m_fontStyle;
			float num4 = 1f;
			m_baselineOffset = 0f;
			m_styleStack.Clear();
			m_lineOffset = 0f;
			m_lineHeight = 0f;
			float num5 = m_currentFontAsset.fontInfo.LineHeight - (m_currentFontAsset.fontInfo.Ascender - m_currentFontAsset.fontInfo.Descender);
			m_cSpacing = 0f;
			m_monoSpacing = 0f;
			float num6 = 0f;
			m_xAdvance = 0f;
			float a = 0f;
			tag_LineIndent = 0f;
			tag_Indent = 0f;
			m_indentStack.SetDefault(0f);
			tag_NoParsing = false;
			m_characterCount = 0;
			m_firstCharacterOfLine = 0;
			m_maxLineAscender = float.NegativeInfinity;
			m_maxLineDescender = float.PositiveInfinity;
			m_lineNumber = 0;
			float x = marginSize.x;
			m_marginLeft = 0f;
			m_marginRight = 0f;
			m_width = -1f;
			float num7 = 0f;
			float num8 = 0f;
			m_maxAscender = 0f;
			m_maxDescender = 0f;
			bool flag = true;
			bool flag2 = false;
			WordWrapState state = default(WordWrapState);
			SaveWordWrappingState(ref state, 0, 0);
			WordWrapState state2 = default(WordWrapState);
			int num9 = 0;
			int endIndex = 0;
			for (int i = 0; m_char_buffer[i] != 0; i++)
			{
				num3 = m_char_buffer[i];
				m_textElementType = TMP_TextElementType.Character;
				m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
				m_currentFontAsset = m_materialReferences[m_currentMaterialIndex].fontAsset;
				int currentMaterialIndex = m_currentMaterialIndex;
				if (m_isRichText && num3 == 60)
				{
					m_isParsingText = true;
					if (ValidateHtmlTag(m_char_buffer, i + 1, out endIndex))
					{
						i = endIndex;
						if (m_textElementType == TMP_TextElementType.Character)
						{
							continue;
						}
					}
				}
				m_isParsingText = false;
				float num10 = 1f;
				if (m_textElementType == TMP_TextElementType.Character)
				{
					if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num3))
						{
							num3 = char.ToUpper((char)num3);
						}
					}
					else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num3))
						{
							num3 = char.ToLower((char)num3);
						}
					}
					else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num3))
					{
						num10 = 0.8f;
						num3 = char.ToUpper((char)num3);
					}
				}
				if (m_textElementType == TMP_TextElementType.Sprite)
				{
					TMP_Sprite tMP_Sprite = m_currentSpriteAsset.spriteInfoList[m_spriteIndex];
					if (tMP_Sprite == null)
					{
						continue;
					}
					num3 = 57344 + m_spriteIndex;
					m_cached_TextElement = tMP_Sprite;
					num2 = m_fontAsset.fontInfo.Ascender / tMP_Sprite.height * tMP_Sprite.scale * num;
					m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Sprite;
					m_currentMaterialIndex = currentMaterialIndex;
				}
				else if (m_textElementType == TMP_TextElementType.Character)
				{
					m_cached_TextElement = m_textInfo.characterInfo[m_characterCount].textElement;
					m_currentFontAsset = m_textInfo.characterInfo[m_characterCount].fontAsset;
					m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
					m_fontScale = m_currentFontSize * num10 / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					num2 = m_fontScale * m_fontScaleMultiplier;
					m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Character;
				}
				m_internalCharacterInfo[m_characterCount].character = (char)num3;
				if (m_enableKerning && m_characterCount >= 1)
				{
					int character = m_internalCharacterInfo[m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num3);
					KerningPair value;
					m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out value);
					if (value != null)
					{
						m_xAdvance += value.XadvanceOffset * num2;
					}
				}
				float num11 = 0f;
				if (m_monoSpacing != 0f)
				{
					num11 = m_monoSpacing / 2f - (m_cached_TextElement.width / 2f + m_cached_TextElement.xOffset) * num2;
					m_xAdvance += num11;
				}
				num4 = (((m_style & FontStyles.Bold) != FontStyles.Bold && (m_fontStyle & FontStyles.Bold) != FontStyles.Bold) ? 1f : (1f + m_currentFontAsset.boldSpacing * 0.01f));
				m_internalCharacterInfo[m_characterCount].baseLine = 0f - m_lineOffset + m_baselineOffset;
				float num12 = m_currentFontAsset.fontInfo.Ascender * ((m_textElementType != TMP_TextElementType.Character) ? num : num2) + m_baselineOffset;
				m_internalCharacterInfo[m_characterCount].ascender = num12 - m_lineOffset;
				m_maxLineAscender = ((!(num12 > m_maxLineAscender)) ? m_maxLineAscender : num12);
				float num13 = m_currentFontAsset.fontInfo.Descender * ((m_textElementType != TMP_TextElementType.Character) ? num : num2) + m_baselineOffset;
				float num14 = (m_internalCharacterInfo[m_characterCount].descender = num13 - m_lineOffset);
				m_maxLineDescender = ((!(num13 < m_maxLineDescender)) ? m_maxLineDescender : num13);
				if ((m_style & FontStyles.Subscript) == FontStyles.Subscript || (m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					float num15 = (num12 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
					num12 = m_maxLineAscender;
					m_maxLineAscender = ((!(num15 > m_maxLineAscender)) ? m_maxLineAscender : num15);
					float num16 = (num13 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
					num13 = m_maxLineDescender;
					m_maxLineDescender = ((!(num16 < m_maxLineDescender)) ? m_maxLineDescender : num16);
				}
				if (m_lineNumber == 0)
				{
					m_maxAscender = ((!(m_maxAscender > num12)) ? num12 : m_maxAscender);
				}
				if (num3 == 9 || !char.IsWhiteSpace((char)num3) || m_textElementType == TMP_TextElementType.Sprite)
				{
					float num17 = ((m_width == -1f) ? (x + 0.0001f - m_marginLeft - m_marginRight) : Mathf.Min(x + 0.0001f - m_marginLeft - m_marginRight, m_width));
					if (m_xAdvance + m_cached_TextElement.xAdvance * num2 > num17 && enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
					{
						if (num9 == state2.previous_WordBreak || flag)
						{
							if (!m_isCharacterWrappingEnabled)
							{
								m_isCharacterWrappingEnabled = true;
							}
							else
							{
								flag2 = true;
							}
						}
						i = RestoreWordWrappingState(ref state2);
						num9 = i;
						if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f)
						{
							float num18 = m_maxLineAscender - m_startOfLineAscender;
							AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num18);
							m_lineOffset += num18;
							state2.lineOffset = m_lineOffset;
							state2.previousLineAscender = m_maxLineAscender;
						}
						float num19 = m_maxLineAscender - m_lineOffset;
						float num20 = m_maxLineDescender - m_lineOffset;
						m_maxDescender = ((!(m_maxDescender < num20)) ? num20 : m_maxDescender);
						m_firstCharacterOfLine = m_characterCount;
						num7 += m_xAdvance;
						num8 = ((!m_enableWordWrapping) ? Mathf.Max(num8, num19 - num20) : (m_maxAscender - m_maxDescender));
						SaveWordWrappingState(ref state, i, m_characterCount - 1);
						m_lineNumber++;
						if (m_lineHeight == 0f)
						{
							float num21 = m_internalCharacterInfo[m_characterCount].ascender - m_internalCharacterInfo[m_characterCount].baseLine;
							num6 = 0f - m_maxLineDescender + num21 + (num5 + m_lineSpacing + m_lineSpacingDelta) * num;
							m_lineOffset += num6;
							m_startOfLineAscender = num21;
						}
						else
						{
							m_lineOffset += m_lineHeight + m_lineSpacing * num;
						}
						m_maxLineAscender = float.NegativeInfinity;
						m_maxLineDescender = float.PositiveInfinity;
						m_xAdvance = tag_Indent;
						continue;
					}
				}
				if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
				{
					float num22 = m_maxLineAscender - m_startOfLineAscender;
					AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num22);
					num14 -= num22;
					m_lineOffset += num22;
					m_startOfLineAscender += num22;
					state2.lineOffset = m_lineOffset;
					state2.previousLineAscender = m_startOfLineAscender;
				}
				if (num3 == 9)
				{
					m_xAdvance += m_currentFontAsset.fontInfo.TabWidth * num2;
				}
				else if (m_monoSpacing != 0f)
				{
					m_xAdvance += m_monoSpacing - num11 + (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing;
				}
				else
				{
					m_xAdvance += (m_cached_TextElement.xAdvance * num4 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing;
				}
				if (num3 == 13)
				{
					a = Mathf.Max(a, num7 + m_xAdvance);
					num7 = 0f;
					m_xAdvance = tag_Indent;
				}
				if (num3 == 10 || m_characterCount == totalCharacterCount - 1)
				{
					if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f)
					{
						float num23 = m_maxLineAscender - m_startOfLineAscender;
						AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num23);
						num14 -= num23;
						m_lineOffset += num23;
					}
					float num24 = m_maxLineDescender - m_lineOffset;
					m_maxDescender = ((!(m_maxDescender < num24)) ? num24 : m_maxDescender);
					m_firstCharacterOfLine = m_characterCount + 1;
					if (num3 == 10 && m_characterCount != totalCharacterCount - 1)
					{
						a = Mathf.Max(a, num7 + m_xAdvance);
						num7 = 0f;
					}
					else
					{
						num7 = Mathf.Max(a, num7 + m_xAdvance);
					}
					num8 = m_maxAscender - m_maxDescender;
					if (num3 == 10)
					{
						SaveWordWrappingState(ref state, i, m_characterCount);
						SaveWordWrappingState(ref state2, i, m_characterCount);
						m_lineNumber++;
						if (m_lineHeight == 0f)
						{
							num6 = 0f - m_maxLineDescender + num12 + (num5 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * num;
							m_lineOffset += num6;
						}
						else
						{
							m_lineOffset += m_lineHeight + (m_lineSpacing + m_paragraphSpacing) * num;
						}
						m_maxLineAscender = float.NegativeInfinity;
						m_maxLineDescender = float.PositiveInfinity;
						m_startOfLineAscender = num12;
						m_xAdvance = tag_LineIndent + tag_Indent;
					}
				}
				if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((num3 == 9 || num3 == 32) && !m_isNonBreakingSpace)
					{
						SaveWordWrappingState(ref state2, i, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag = false;
					}
					else if (num3 > 11904 && num3 < 40959)
					{
						if (!m_currentFontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num3) && m_characterCount < totalCharacterCount - 1 && !m_currentFontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_internalCharacterInfo[m_characterCount + 1].character))
						{
							SaveWordWrappingState(ref state2, i, m_characterCount);
							m_isCharacterWrappingEnabled = false;
							flag = false;
						}
					}
					else if (flag || m_isCharacterWrappingEnabled || flag2)
					{
						SaveWordWrappingState(ref state2, i, m_characterCount);
					}
				}
				m_characterCount++;
			}
			m_isCharacterWrappingEnabled = false;
			num7 += ((!(m_margin.x > 0f)) ? 0f : m_margin.x);
			num7 += ((!(m_margin.z > 0f)) ? 0f : m_margin.z);
			num8 += ((!(m_margin.y > 0f)) ? 0f : m_margin.y);
			num8 += ((!(m_margin.w > 0f)) ? 0f : m_margin.w);
			return new Vector2(num7, num8);
		}

		protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
		}

		protected void ResizeLineExtents(int size)
		{
			size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < m_textInfo.lineInfo.Length)
				{
					array[i] = m_textInfo.lineInfo[i];
					continue;
				}
				array[i].lineExtents.min = k_InfinityVectorPositive;
				array[i].lineExtents.max = k_InfinityVectorNegative;
				array[i].ascender = k_InfinityVectorNegative.x;
				array[i].descender = k_InfinityVectorPositive.x;
			}
			m_textInfo.lineInfo = array;
		}

		public virtual TMP_TextInfo GetTextInfo(string text)
		{
			return null;
		}

		protected virtual void ComputeMarginSize()
		{
		}

		protected int GetArraySizes(int[] chars)
		{
			int endIndex = 0;
			m_totalCharacterCount = 0;
			m_isUsingBold = false;
			m_isParsingText = false;
			for (int i = 0; chars[i] != 0; i++)
			{
				int num = chars[i];
				if (m_isRichText && num == 60 && ValidateHtmlTag(chars, i + 1, out endIndex))
				{
					i = endIndex;
					if ((m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						m_isUsingBold = true;
					}
				}
				else
				{
					if (!char.IsWhiteSpace((char)num))
					{
					}
					m_totalCharacterCount++;
				}
			}
			return m_totalCharacterCount;
		}

		protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.currentFontAsset = m_currentFontAsset;
			state.currentSpriteAsset = m_currentSpriteAsset;
			state.currentMaterial = m_currentMaterial;
			state.currentMaterialIndex = m_currentMaterialIndex;
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = m_visibleCharacterCount;
			state.visible_SpriteCount = m_visibleSpriteCount;
			state.visible_LinkCount = m_textInfo.linkCount;
			state.firstCharacterIndex = m_firstCharacterOfLine;
			state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;
			state.fontStyle = m_style;
			state.fontScale = m_fontScale;
			state.fontScaleMultiplier = m_fontScaleMultiplier;
			state.currentFontSize = m_currentFontSize;
			state.xAdvance = m_xAdvance;
			state.maxAscender = m_maxAscender;
			state.maxDescender = m_maxDescender;
			state.maxLineAscender = m_maxLineAscender;
			state.maxLineDescender = m_maxLineDescender;
			state.previousLineAscender = m_startOfLineAscender;
			state.preferredWidth = m_preferredWidth;
			state.preferredHeight = m_preferredHeight;
			state.meshExtents = m_meshExtents;
			state.lineNumber = m_lineNumber;
			state.lineOffset = m_lineOffset;
			state.baselineOffset = m_baselineOffset;
			state.vertexColor = m_htmlColor;
			state.tagNoParsing = tag_NoParsing;
			state.colorStack = m_colorStack;
			state.sizeStack = m_sizeStack;
			state.fontWeightStack = m_fontWeightStack;
			state.styleStack = m_styleStack;
			state.actionStack = m_actionStack;
			state.materialReferenceStack = m_materialReferenceStack;
			if (m_lineNumber < m_textInfo.lineInfo.Length)
			{
				state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
			}
		}

		protected int RestoreWordWrappingState(ref WordWrapState state)
		{
			int previous_WordBreak = state.previous_WordBreak;
			m_currentFontAsset = state.currentFontAsset;
			m_currentSpriteAsset = state.currentSpriteAsset;
			m_currentMaterial = state.currentMaterial;
			m_currentMaterialIndex = state.currentMaterialIndex;
			m_characterCount = state.total_CharacterCount + 1;
			m_visibleCharacterCount = state.visible_CharacterCount;
			m_visibleSpriteCount = state.visible_SpriteCount;
			m_textInfo.linkCount = state.visible_LinkCount;
			m_firstCharacterOfLine = state.firstCharacterIndex;
			m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			m_style = state.fontStyle;
			m_fontScale = state.fontScale;
			m_fontScaleMultiplier = state.fontScaleMultiplier;
			m_currentFontSize = state.currentFontSize;
			m_xAdvance = state.xAdvance;
			m_maxAscender = state.maxAscender;
			m_maxDescender = state.maxDescender;
			m_maxLineAscender = state.maxLineAscender;
			m_maxLineDescender = state.maxLineDescender;
			m_startOfLineAscender = state.previousLineAscender;
			m_preferredWidth = state.preferredWidth;
			m_preferredHeight = state.preferredHeight;
			m_meshExtents = state.meshExtents;
			m_lineNumber = state.lineNumber;
			m_lineOffset = state.lineOffset;
			m_baselineOffset = state.baselineOffset;
			m_htmlColor = state.vertexColor;
			tag_NoParsing = state.tagNoParsing;
			m_colorStack = state.colorStack;
			m_sizeStack = state.sizeStack;
			m_fontWeightStack = state.fontWeightStack;
			m_styleStack = state.styleStack;
			m_actionStack = state.actionStack;
			m_materialReferenceStack = state.materialReferenceStack;
			if (m_lineNumber < m_textInfo.lineInfo.Length)
			{
				m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
			}
			return previous_WordBreak;
		}

		protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
			vertexColor.a = ((m_fontColor32.a >= vertexColor.a) ? vertexColor.a : m_fontColor32.a);
			if (!m_enableVertexGradient)
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
			}
			else
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradient.bottomLeft * vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradient.topLeft * vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradient.topRight * vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradient.bottomRight * vertexColor;
			}
			if (!m_isSDFShader)
			{
				style_padding = 0f;
			}
			FaceInfo fontInfo = m_currentFontAsset.fontInfo;
			Vector2 uv = default(Vector2);
			uv.x = (m_cached_TextElement.x - padding - style_padding) / fontInfo.AtlasWidth;
			uv.y = 1f - (m_cached_TextElement.y + padding + style_padding + m_cached_TextElement.height) / fontInfo.AtlasHeight;
			Vector2 uv2 = default(Vector2);
			uv2.x = uv.x;
			uv2.y = 1f - (m_cached_TextElement.y - padding - style_padding) / fontInfo.AtlasHeight;
			Vector2 uv3 = default(Vector2);
			uv3.x = (m_cached_TextElement.x + padding + style_padding + m_cached_TextElement.width) / fontInfo.AtlasWidth;
			uv3.y = uv2.y;
			Vector2 uv4 = default(Vector2);
			uv4.x = uv3.x;
			uv4.y = uv.y;
			m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv2;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv4;
		}

		protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
			if (m_tintAllSprites)
			{
				m_tintSprite = true;
			}
			Color32 color = ((!m_tintSprite) ? m_spriteColor : m_spriteColor.Multiply(vertexColor));
			color.a = ((color.a >= m_fontColor32.a) ? m_fontColor32.a : (color.a = ((color.a >= vertexColor.a) ? vertexColor.a : color.a)));
			if (!m_enableVertexGradient)
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = color;
			}
			else if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = color;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = color;
			}
			else
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = ((!m_tintSprite) ? color : color.Multiply(m_fontColorGradient.bottomLeft));
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = ((!m_tintSprite) ? color : color.Multiply(m_fontColorGradient.topLeft));
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = ((!m_tintSprite) ? color : color.Multiply(m_fontColorGradient.topRight));
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = ((!m_tintSprite) ? color : color.Multiply(m_fontColorGradient.bottomRight));
			}
			Vector2 uv = new Vector2(m_cached_TextElement.x / (float)m_currentSpriteAsset.spriteSheet.width, m_cached_TextElement.y / (float)m_currentSpriteAsset.spriteSheet.height);
			Vector2 uv2 = new Vector2(uv.x, (m_cached_TextElement.y + m_cached_TextElement.height) / (float)m_currentSpriteAsset.spriteSheet.height);
			Vector2 uv3 = new Vector2((m_cached_TextElement.x + m_cached_TextElement.width) / (float)m_currentSpriteAsset.spriteSheet.width, uv2.y);
			Vector2 uv4 = new Vector2(uv3.x, uv.y);
			m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv2;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv4;
		}

		protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			m_textInfo.characterInfo[i].vertexIndex = (short)index_X4;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			m_textInfo.characterInfo[i].vertexIndex = (short)index_X4;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, Color32 underlineColor)
		{
			if (m_cached_Underline_GlyphInfo == null)
			{
				if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
				}
				return;
			}
			int num = index + 12;
			if (num > m_textInfo.meshInfo[0].vertices.Length)
			{
				m_textInfo.meshInfo[0].ResizeMeshInfo(num / 4);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			float num2 = m_cached_Underline_GlyphInfo.width / 2f * maxScale;
			if (end.x - start.x < m_cached_Underline_GlyphInfo.width * maxScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float num3 = m_padding * startScale / maxScale;
			float num4 = m_padding * endScale / maxScale;
			float height = m_cached_Underline_GlyphInfo.height;
			Vector3[] vertices = m_textInfo.meshInfo[0].vertices;
			vertices[index] = start + new Vector3(0f, 0f - (height + m_padding) * maxScale, 0f);
			vertices[index + 1] = start + new Vector3(0f, m_padding * maxScale, 0f);
			vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
			vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
			vertices[index + 4] = vertices[index + 3];
			vertices[index + 5] = vertices[index + 2];
			vertices[index + 6] = end + new Vector3(0f - num2, m_padding * maxScale, 0f);
			vertices[index + 7] = end + new Vector3(0f - num2, (0f - (height + m_padding)) * maxScale, 0f);
			vertices[index + 8] = vertices[index + 7];
			vertices[index + 9] = vertices[index + 6];
			vertices[index + 10] = end + new Vector3(0f, m_padding * maxScale, 0f);
			vertices[index + 11] = end + new Vector3(0f, (0f - (height + m_padding)) * maxScale, 0f);
			Vector2[] uvs = m_textInfo.meshInfo[0].uvs0;
			Vector2 vector = new Vector2((m_cached_Underline_GlyphInfo.x - num3) / m_fontAsset.fontInfo.AtlasWidth, 1f - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector2 = new Vector2(vector.x, 1f - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector3 = new Vector2((m_cached_Underline_GlyphInfo.x - num3 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			Vector2 vector5 = new Vector2((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector6 = new Vector2(vector5.x, vector.y);
			Vector2 vector7 = new Vector2((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector8 = new Vector2(vector7.x, vector.y);
			uvs[index] = vector;
			uvs[1 + index] = vector2;
			uvs[2 + index] = vector3;
			uvs[3 + index] = vector4;
			uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
			uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
			uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
			uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
			uvs[8 + index] = vector6;
			uvs[9 + index] = vector5;
			uvs[10 + index] = vector7;
			uvs[11 + index] = vector8;
			float num5 = 0f;
			float x = (vertices[index + 2].x - start.x) / (end.x - start.x);
			float num6 = ((maxScale * m_rectTransform.lossyScale.y != 0f) ? m_rectTransform.lossyScale.y : 1f);
			float scale = num6;
			Vector2[] uvs2 = m_textInfo.meshInfo[0].uvs2;
			uvs2[index] = PackUV(0f, 0f, num6);
			uvs2[1 + index] = PackUV(0f, 1f, num6);
			uvs2[2 + index] = PackUV(x, 1f, num6);
			uvs2[3 + index] = PackUV(x, 0f, num6);
			num5 = (vertices[index + 4].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[4 + index] = PackUV(num5, 0f, scale);
			uvs2[5 + index] = PackUV(num5, 1f, scale);
			uvs2[6 + index] = PackUV(x, 1f, scale);
			uvs2[7 + index] = PackUV(x, 0f, scale);
			num5 = (vertices[index + 8].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[8 + index] = PackUV(num5, 0f, num6);
			uvs2[9 + index] = PackUV(num5, 1f, num6);
			uvs2[10 + index] = PackUV(1f, 1f, num6);
			uvs2[11 + index] = PackUV(1f, 0f, num6);
			Color32[] colors = m_textInfo.meshInfo[0].colors32;
			colors[index] = underlineColor;
			colors[1 + index] = underlineColor;
			colors[2 + index] = underlineColor;
			colors[3 + index] = underlineColor;
			colors[4 + index] = underlineColor;
			colors[5 + index] = underlineColor;
			colors[6 + index] = underlineColor;
			colors[7 + index] = underlineColor;
			colors[8 + index] = underlineColor;
			colors[9 + index] = underlineColor;
			colors[10 + index] = underlineColor;
			colors[11 + index] = underlineColor;
			index += 12;
		}

		protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
		{
			if (!fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo) && !TMP_Settings.warningsDisabled)
			{
				Debug.LogWarning("Underscore character wasn't found in the current Font Asset [" + fontAsset.name + "]. No characters assigned for Underline.\nNote: Warnings can be disabled in the TMP Settings file.", this);
			}
			if (!fontAsset.characterDictionary.TryGetValue(8230, out m_cached_Ellipsis_GlyphInfo) && !TMP_Settings.warningsDisabled)
			{
				Debug.LogWarning("Ellipsis character wasn't found in the current Font Asset [" + fontAsset.name + "]. No characters assigned for Ellipsis.\nNote: Warnings can be disabled in the TMP Settings file.", this);
			}
		}

		protected int GetMaterialReferenceForFontWeight()
		{
			m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentFontAsset.fontWeights[0].italicTypeface.material, m_currentFontAsset.fontWeights[0].italicTypeface, m_materialReferences, m_materialReferenceIndexLookup);
			return 0;
		}

		protected TMP_FontAsset GetAlternativeFontAsset()
		{
			bool flag = (m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
			TMP_FontAsset tMP_FontAsset = null;
			int num = m_fontWeightInternal / 100;
			tMP_FontAsset = ((!flag) ? m_currentFontAsset.fontWeights[num].regularTypeface : m_currentFontAsset.fontWeights[num].italicTypeface);
			if (tMP_FontAsset == null)
			{
				return m_currentFontAsset;
			}
			m_currentFontAsset = tMP_FontAsset;
			return m_currentFontAsset;
		}

		protected Vector2 PackUV(float x, float y, float scale)
		{
			Vector2 result = default(Vector2);
			result.x = Mathf.Floor(x * 511f);
			result.y = Mathf.Floor(y * 511f);
			result.x = result.x * 4096f + result.y;
			result.y = scale;
			return result;
		}

		protected float PackUV(float x, float y)
		{
			double num = Math.Floor(x * 511f);
			double num2 = Math.Floor(y * 511f);
			return (float)(num * 4096.0 + num2);
		}

		protected int HexToInt(char hex)
		{
			switch (hex)
			{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case 'A':
				return 10;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			case 'a':
				return 10;
			case 'b':
				return 11;
			case 'c':
				return 12;
			case 'd':
				return 13;
			case 'e':
				return 14;
			case 'f':
				return 15;
			default:
				return 15;
			}
		}

		protected int GetUTF16(int i)
		{
			int num = HexToInt(m_text[i]) * 4096;
			num += HexToInt(m_text[i + 1]) * 256;
			num += HexToInt(m_text[i + 2]) * 16;
			return num + HexToInt(m_text[i + 3]);
		}

		protected int GetUTF32(int i)
		{
			int num = 0;
			num += HexToInt(m_text[i]) * 268435456;
			num += HexToInt(m_text[i + 1]) * 16777216;
			num += HexToInt(m_text[i + 2]) * 1048576;
			num += HexToInt(m_text[i + 3]) * 65536;
			num += HexToInt(m_text[i + 4]) * 4096;
			num += HexToInt(m_text[i + 5]) * 256;
			num += HexToInt(m_text[i + 6]) * 16;
			return num + HexToInt(m_text[i + 7]);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			switch (tagCount)
			{
			case 7:
			{
				byte r4 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
				byte g4 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
				byte b4 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
				return new Color32(r4, g4, b4, byte.MaxValue);
			}
			case 9:
			{
				byte r3 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
				byte g3 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
				byte b3 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
				byte a2 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				return new Color32(r3, g3, b3, a2);
			}
			case 13:
			{
				byte r2 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				byte g2 = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
				byte b2 = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
				return new Color32(r2, g2, b2, byte.MaxValue);
			}
			case 15:
			{
				byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
				byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
				byte a = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));
				return new Color32(r, g, b, a);
			}
			default:
				return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}

		protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
		{
			switch (length)
			{
			case 7:
			{
				byte r2 = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
				byte g2 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
				byte b2 = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
				return new Color32(r2, g2, b2, byte.MaxValue);
			}
			case 9:
			{
				byte r = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
				byte g = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
				byte b = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
				byte a = (byte)(HexToInt(hexChars[startIndex + 7]) * 16 + HexToInt(hexChars[startIndex + 8]));
				return new Color32(r, g, b, a);
			}
			default:
				return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}

		protected float ConvertToFloat(char[] chars, int startIndex, int length, int decimalPointIndex)
		{
			if (startIndex == 0)
			{
				return -9999f;
			}
			int num = startIndex + length - 1;
			float num2 = 0f;
			float num3 = 1f;
			decimalPointIndex = ((decimalPointIndex <= 0) ? (num + 1) : decimalPointIndex);
			if (chars[startIndex] == '-')
			{
				startIndex++;
				num3 = -1f;
			}
			if (chars[startIndex] == '+' || chars[startIndex] == '%')
			{
				startIndex++;
			}
			for (int i = startIndex; i < num + 1; i++)
			{
				if (!char.IsDigit(chars[i]) && chars[i] != '.')
				{
					return -9999f;
				}
				switch (decimalPointIndex - i)
				{
				case 4:
					num2 += (float)((chars[i] - 48) * 1000);
					break;
				case 3:
					num2 += (float)((chars[i] - 48) * 100);
					break;
				case 2:
					num2 += (float)((chars[i] - 48) * 10);
					break;
				case 1:
					num2 += (float)(chars[i] - 48);
					break;
				case -1:
					num2 += (float)(chars[i] - 48) * 0.1f;
					break;
				case -2:
					num2 += (float)(chars[i] - 48) * 0.01f;
					break;
				case -3:
					num2 += (float)(chars[i] - 48) * 0.001f;
					break;
				}
			}
			return num2 * num3;
		}

		protected bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
		{
			int num = 0;
			byte b = 0;
			TagUnits tagUnits = TagUnits.Pixels;
			TagType tagType = TagType.None;
			int num2 = 0;
			m_xmlAttribute[num2].nameHashCode = 0;
			m_xmlAttribute[num2].valueType = TagType.None;
			m_xmlAttribute[num2].valueHashCode = 0;
			m_xmlAttribute[num2].valueStartIndex = 0;
			m_xmlAttribute[num2].valueLength = 0;
			m_xmlAttribute[num2].valueDecimalIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			for (int i = startIndex; i < chars.Length && chars[i] != 0; i++)
			{
				if (num >= m_htmlTag.Length)
				{
					break;
				}
				if (chars[i] == 60)
				{
					break;
				}
				if (chars[i] == 62)
				{
					flag2 = true;
					endIndex = i;
					m_htmlTag[num] = '\0';
					break;
				}
				m_htmlTag[num] = (char)chars[i];
				num++;
				if (b == 1)
				{
					if (m_xmlAttribute[num2].valueStartIndex == 0)
					{
						if (chars[i] == 43 || chars[i] == 45 || char.IsDigit((char)chars[i]))
						{
							tagType = TagType.NumericalValue;
							m_xmlAttribute[num2].valueType = TagType.NumericalValue;
							m_xmlAttribute[num2].valueStartIndex = num - 1;
							m_xmlAttribute[num2].valueLength++;
						}
						else if (chars[i] == 35)
						{
							tagType = TagType.ColorValue;
							m_xmlAttribute[num2].valueType = TagType.ColorValue;
							m_xmlAttribute[num2].valueStartIndex = num - 1;
							m_xmlAttribute[num2].valueLength++;
						}
						else if (chars[i] != 34)
						{
							tagType = TagType.StringValue;
							m_xmlAttribute[num2].valueType = TagType.StringValue;
							m_xmlAttribute[num2].valueStartIndex = num - 1;
							m_xmlAttribute[num2].valueHashCode = ((m_xmlAttribute[num2].valueHashCode << 5) + m_xmlAttribute[num2].valueHashCode) ^ chars[i];
							m_xmlAttribute[num2].valueLength++;
						}
					}
					else
					{
						switch (tagType)
						{
						case TagType.NumericalValue:
							if (chars[i] == 46)
							{
								m_xmlAttribute[num2].valueDecimalIndex = num - 1;
							}
							if (chars[i] == 112 || chars[i] == 101 || chars[i] == 37 || chars[i] == 32)
							{
								b = 2;
								tagType = TagType.None;
								num2++;
								m_xmlAttribute[num2].nameHashCode = 0;
								m_xmlAttribute[num2].valueType = TagType.None;
								m_xmlAttribute[num2].valueHashCode = 0;
								m_xmlAttribute[num2].valueStartIndex = 0;
								m_xmlAttribute[num2].valueLength = 0;
								m_xmlAttribute[num2].valueDecimalIndex = 0;
								if (chars[i] == 101)
								{
									tagUnits = TagUnits.FontUnits;
								}
								else if (chars[i] == 37)
								{
									tagUnits = TagUnits.Percentage;
								}
							}
							else if (b != 2)
							{
								m_xmlAttribute[num2].valueLength++;
							}
							break;
						case TagType.ColorValue:
							if (chars[i] != 32)
							{
								m_xmlAttribute[num2].valueLength++;
								break;
							}
							b = 2;
							tagType = TagType.None;
							num2++;
							m_xmlAttribute[num2].nameHashCode = 0;
							m_xmlAttribute[num2].valueType = TagType.None;
							m_xmlAttribute[num2].valueHashCode = 0;
							m_xmlAttribute[num2].valueStartIndex = 0;
							m_xmlAttribute[num2].valueLength = 0;
							m_xmlAttribute[num2].valueDecimalIndex = 0;
							break;
						case TagType.StringValue:
							if (chars[i] != 34)
							{
								m_xmlAttribute[num2].valueHashCode = ((m_xmlAttribute[num2].valueHashCode << 5) + m_xmlAttribute[num2].valueHashCode) ^ chars[i];
								m_xmlAttribute[num2].valueLength++;
								break;
							}
							b = 2;
							tagType = TagType.None;
							num2++;
							m_xmlAttribute[num2].nameHashCode = 0;
							m_xmlAttribute[num2].valueType = TagType.None;
							m_xmlAttribute[num2].valueHashCode = 0;
							m_xmlAttribute[num2].valueStartIndex = 0;
							m_xmlAttribute[num2].valueLength = 0;
							m_xmlAttribute[num2].valueDecimalIndex = 0;
							break;
						}
					}
				}
				if (chars[i] == 61)
				{
					b = 1;
				}
				if (b == 0 && chars[i] == 32)
				{
					if (flag)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagType = TagType.None;
					num2++;
					m_xmlAttribute[num2].nameHashCode = 0;
					m_xmlAttribute[num2].valueType = TagType.None;
					m_xmlAttribute[num2].valueHashCode = 0;
					m_xmlAttribute[num2].valueStartIndex = 0;
					m_xmlAttribute[num2].valueLength = 0;
					m_xmlAttribute[num2].valueDecimalIndex = 0;
				}
				if (b == 0)
				{
					m_xmlAttribute[num2].nameHashCode = (m_xmlAttribute[num2].nameHashCode << 3) - m_xmlAttribute[num2].nameHashCode + chars[i];
				}
				if (b == 2 && chars[i] == 32)
				{
					b = 0;
				}
			}
			if (!flag2)
			{
				return false;
			}
			if (tag_NoParsing && m_xmlAttribute[0].nameHashCode != 53822163)
			{
				return false;
			}
			if (m_xmlAttribute[0].nameHashCode == 53822163)
			{
				tag_NoParsing = false;
				return true;
			}
			if (m_htmlTag[0] == '#' && num == 7)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack.Add(m_htmlColor);
				return true;
			}
			if (m_htmlTag[0] == '#' && num == 9)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack.Add(m_htmlColor);
				return true;
			}
			float num3 = 0f;
			switch (m_xmlAttribute[0].nameHashCode)
			{
			case 98:
				m_style |= FontStyles.Bold;
				m_fontWeightInternal = 700;
				m_fontWeightStack.Add(700);
				return true;
			case 427:
				if ((m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
				{
					m_style &= (FontStyles)(-2);
					m_fontWeightInternal = m_fontWeightStack.Remove();
				}
				return true;
			case 105:
				m_style |= FontStyles.Italic;
				return true;
			case 434:
				m_style &= (FontStyles)(-3);
				return true;
			case 115:
				m_style |= FontStyles.Strikethrough;
				return true;
			case 444:
				if ((m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
				{
					m_style &= (FontStyles)(-65);
				}
				return true;
			case 117:
				m_style |= FontStyles.Underline;
				return true;
			case 446:
				if ((m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
				{
					m_style &= (FontStyles)(-5);
				}
				return true;
			case 6552:
				m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
				m_baselineOffset = m_currentFontAsset.fontInfo.SubscriptOffset * m_fontScale * m_fontScaleMultiplier;
				m_style |= FontStyles.Subscript;
				return true;
			case 22673:
				if ((m_style & FontStyles.Subscript) == FontStyles.Subscript)
				{
					if ((m_style & FontStyles.Superscript) == FontStyles.Superscript)
					{
						m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
						m_baselineOffset = m_currentFontAsset.fontInfo.SuperscriptOffset * m_fontScale * m_fontScaleMultiplier;
					}
					else
					{
						m_baselineOffset = 0f;
						m_fontScaleMultiplier = 1f;
					}
					m_style &= (FontStyles)(-257);
				}
				return true;
			case 6566:
				m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
				m_baselineOffset = m_currentFontAsset.fontInfo.SuperscriptOffset * m_fontScale * m_fontScaleMultiplier;
				m_style |= FontStyles.Superscript;
				return true;
			case 22687:
				if ((m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					if ((m_style & FontStyles.Subscript) == FontStyles.Subscript)
					{
						m_fontScaleMultiplier = ((!(m_currentFontAsset.fontInfo.SubSize > 0f)) ? 1f : m_currentFontAsset.fontInfo.SubSize);
						m_baselineOffset = m_currentFontAsset.fontInfo.SubscriptOffset * m_fontScale * m_fontScaleMultiplier;
					}
					else
					{
						m_baselineOffset = 0f;
						m_fontScaleMultiplier = 1f;
					}
					m_style &= (FontStyles)(-129);
				}
				return true;
			case -330774850:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				if ((m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
				{
					return true;
				}
				m_style &= (FontStyles)(-2);
				switch ((int)num3)
				{
				case 100:
					m_fontWeightInternal = 100;
					break;
				case 200:
					m_fontWeightInternal = 200;
					break;
				case 300:
					m_fontWeightInternal = 300;
					break;
				case 400:
					m_fontWeightInternal = 400;
					break;
				case 500:
					m_fontWeightInternal = 500;
					break;
				case 600:
					m_fontWeightInternal = 600;
					break;
				case 700:
					m_fontWeightInternal = 700;
					m_style |= FontStyles.Bold;
					break;
				case 800:
					m_fontWeightInternal = 800;
					break;
				case 900:
					m_fontWeightInternal = 900;
					break;
				}
				m_fontWeightStack.Add(m_fontWeightInternal);
				return true;
			case -1885698441:
				m_fontWeightInternal = m_fontWeightStack.Remove();
				return true;
			case 6380:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_xAdvance = num3;
					return true;
				case TagUnits.FontUnits:
					m_xAdvance = num3 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					return true;
				case TagUnits.Percentage:
					m_xAdvance = m_marginWidth * num3 / 100f;
					return true;
				default:
					return false;
				}
			case 22501:
				m_isIgnoringAlignment = false;
				return true;
			case 16034505:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_baselineOffset = num3;
					return true;
				case TagUnits.FontUnits:
					m_baselineOffset = num3 * m_fontScale * m_fontAsset.fontInfo.Ascender;
					return true;
				case TagUnits.Percentage:
					return false;
				default:
					return false;
				}
			case 54741026:
				m_baselineOffset = 0f;
				return true;
			case 43991:
				if (m_overflowMode == TextOverflowModes.Page)
				{
					m_xAdvance = tag_LineIndent + tag_Indent;
					m_lineOffset = 0f;
					m_pageNumber++;
					m_isNewPage = true;
				}
				return true;
			case 43969:
				m_isNonBreakingSpace = true;
				return true;
			case 156816:
				m_isNonBreakingSpace = false;
				return true;
			case 45545:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					if (m_htmlTag[5] == '+')
					{
						m_currentFontSize = m_fontSize + num3;
						m_sizeStack.Add(m_currentFontSize);
						m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
						return true;
					}
					if (m_htmlTag[5] == '-')
					{
						m_currentFontSize = m_fontSize + num3;
						m_sizeStack.Add(m_currentFontSize);
						m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
						return true;
					}
					m_currentFontSize = num3;
					m_sizeStack.Add(m_currentFontSize);
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					return true;
				case TagUnits.FontUnits:
					m_currentFontSize = m_fontSize * num3;
					m_sizeStack.Add(m_currentFontSize);
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					return true;
				case TagUnits.Percentage:
					m_currentFontSize = m_fontSize * num3 / 100f;
					m_sizeStack.Add(m_currentFontSize);
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					return true;
				default:
					return false;
				}
			case 158392:
				m_currentFontSize = m_sizeStack.Remove();
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			case 41311:
			{
				int valueHashCode2 = m_xmlAttribute[0].valueHashCode;
				int nameHashCode = m_xmlAttribute[1].nameHashCode;
				int valueHashCode3 = m_xmlAttribute[1].valueHashCode;
				if (valueHashCode2 == 764638571 || valueHashCode2 == 523367755)
				{
					m_currentFontAsset = m_materialReferences[0].fontAsset;
					m_currentMaterial = m_materialReferences[0].material;
					m_currentMaterialIndex = 0;
					m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
					m_materialReferenceStack.Add(m_materialReferences[0]);
					return true;
				}
				TMP_FontAsset fontAsset;
				if (!MaterialReferenceManager.TryGetFontAsset(valueHashCode2, out fontAsset))
				{
					fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/" + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
					if (fontAsset == null)
					{
						return false;
					}
					MaterialReferenceManager.AddFontAsset(fontAsset);
				}
				if (nameHashCode == 0 && valueHashCode3 == 0)
				{
					m_currentMaterial = fontAsset.material;
					m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
					m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
				}
				else
				{
					if (nameHashCode != 103415287)
					{
						return false;
					}
					Material currentMaterial;
					if (MaterialReferenceManager.TryGetMaterial(valueHashCode3, out currentMaterial))
					{
						m_currentMaterial = currentMaterial;
						m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
						m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
					}
					else
					{
						currentMaterial = Resources.Load<Material>("Fonts & Materials/" + new string(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength));
						if (currentMaterial == null)
						{
							return false;
						}
						MaterialReferenceManager.AddFontMaterial(valueHashCode3, currentMaterial);
						m_currentMaterial = currentMaterial;
						m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, fontAsset, m_materialReferences, m_materialReferenceIndexLookup);
						m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
					}
				}
				m_currentFontAsset = fontAsset;
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			}
			case 154158:
			{
				MaterialReference materialReference = m_materialReferenceStack.Remove();
				m_currentFontAsset = materialReference.fontAsset;
				m_currentMaterial = materialReference.material;
				m_currentMaterialIndex = materialReference.index;
				m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * ((!m_isOrthographic) ? 0.1f : 1f);
				return true;
			}
			case 320078:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_xAdvance += num3;
					return true;
				case TagUnits.FontUnits:
					m_xAdvance += num3 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					return true;
				case TagUnits.Percentage:
					return false;
				default:
					return false;
				}
			case 276254:
				if (m_xmlAttribute[0].valueLength != 3)
				{
					return false;
				}
				m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
				return true;
			case 1750458:
				return false;
			case 426:
				return true;
			case 43066:
				if (m_isParsingText)
				{
					int num6 = m_textInfo.linkInfo.Length;
					if (m_textInfo.linkCount + 1 > num6)
					{
						TMP_TextInfo.Resize(ref m_textInfo.linkInfo, num6 + 1);
					}
					int linkCount = m_textInfo.linkCount;
					m_textInfo.linkInfo[linkCount].textComponent = this;
					m_textInfo.linkInfo[linkCount].hashCode = m_xmlAttribute[0].valueHashCode;
					m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = m_characterCount;
					m_textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + m_xmlAttribute[0].valueStartIndex;
					m_textInfo.linkInfo[linkCount].linkIdLength = m_xmlAttribute[0].valueLength;
					m_textInfo.linkInfo[linkCount].SetLinkID(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
				}
				return true;
			case 155913:
				if (m_isParsingText)
				{
					m_textInfo.linkInfo[m_textInfo.linkCount].linkTextLength = m_characterCount - m_textInfo.linkInfo[m_textInfo.linkCount].linkTextfirstCharacterIndex;
					m_textInfo.linkCount++;
				}
				return true;
			case 275917:
				switch (m_xmlAttribute[0].valueHashCode)
				{
				case 3774683:
					m_lineJustification = TextAlignmentOptions.Left;
					return true;
				case 136703040:
					m_lineJustification = TextAlignmentOptions.Right;
					return true;
				case -458210101:
					m_lineJustification = TextAlignmentOptions.Center;
					return true;
				case -523808257:
					m_lineJustification = TextAlignmentOptions.Justified;
					return true;
				default:
					return false;
				}
			case 1065846:
				m_lineJustification = m_textAlignment;
				return true;
			case 327550:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_width = num3;
					break;
				case TagUnits.FontUnits:
					return false;
				case TagUnits.Percentage:
					m_width = m_marginWidth * num3 / 100f;
					break;
				}
				return true;
			case 1117479:
				m_width = -1f;
				return true;
			case 322689:
			{
				TMP_Style style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);
				if (style == null)
				{
					return false;
				}
				m_styleStack.Add(style.hashCode);
				for (int k = 0; k < style.styleOpeningTagArray.Length; k++)
				{
					if (style.styleOpeningTagArray[k] == 60)
					{
						ValidateHtmlTag(style.styleOpeningTagArray, k + 1, out k);
					}
				}
				return true;
			}
			case 1112618:
			{
				TMP_Style style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);
				if (style == null)
				{
					int hashCode = m_styleStack.Remove();
					style = TMP_StyleSheet.GetStyle(hashCode);
				}
				if (style == null)
				{
					return false;
				}
				for (int j = 0; j < style.styleClosingTagArray.Length; j++)
				{
					if (style.styleClosingTagArray[j] == 60)
					{
						ValidateHtmlTag(style.styleClosingTagArray, j + 1, out j);
					}
				}
				return true;
			}
			case 281955:
				if (m_htmlTag[6] == '#' && num == 13)
				{
					m_htmlColor = HexCharsToColor(m_htmlTag, num);
					m_colorStack.Add(m_htmlColor);
					return true;
				}
				if (m_htmlTag[6] == '#' && num == 15)
				{
					m_htmlColor = HexCharsToColor(m_htmlTag, num);
					m_colorStack.Add(m_htmlColor);
					return true;
				}
				switch (m_xmlAttribute[0].valueHashCode)
				{
				case 125395:
					m_htmlColor = Color.red;
					m_colorStack.Add(m_htmlColor);
					return true;
				case 3573310:
					m_htmlColor = Color.blue;
					m_colorStack.Add(m_htmlColor);
					return true;
				case 117905991:
					m_htmlColor = Color.black;
					m_colorStack.Add(m_htmlColor);
					return true;
				case 121463835:
					m_htmlColor = Color.green;
					m_colorStack.Add(m_htmlColor);
					return true;
				case 140357351:
					m_htmlColor = Color.white;
					m_colorStack.Add(m_htmlColor);
					return true;
				case 26556144:
					m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
					m_colorStack.Add(m_htmlColor);
					return true;
				case -36881330:
					m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
					m_colorStack.Add(m_htmlColor);
					return true;
				case 554054276:
					m_htmlColor = Color.yellow;
					m_colorStack.Add(m_htmlColor);
					return true;
				default:
					return false;
				}
			case 1983971:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_cSpacing = num3;
					break;
				case TagUnits.FontUnits:
					m_cSpacing = num3;
					m_cSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					return false;
				}
				return true;
			case 7513474:
				m_cSpacing = 0f;
				return true;
			case 2152041:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_monoSpacing = num3;
					break;
				case TagUnits.FontUnits:
					m_monoSpacing = num3;
					m_monoSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					return false;
				}
				return true;
			case 7681544:
				m_monoSpacing = 0f;
				return true;
			case 280416:
				return false;
			case 1071884:
				m_htmlColor = m_colorStack.Remove();
				return true;
			case 2068980:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					tag_Indent = num3;
					break;
				case TagUnits.FontUnits:
					tag_Indent = num3;
					tag_Indent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					tag_Indent = m_marginWidth * num3 / 100f;
					break;
				}
				m_indentStack.Add(tag_Indent);
				m_xAdvance = tag_Indent;
				return true;
			case 7598483:
				tag_Indent = m_indentStack.Remove();
				return true;
			case 1109386397:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					tag_LineIndent = num3;
					break;
				case TagUnits.FontUnits:
					tag_LineIndent = num3;
					tag_LineIndent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					tag_LineIndent = m_marginWidth * num3 / 100f;
					break;
				}
				m_xAdvance += tag_LineIndent;
				return true;
			case -445537194:
				tag_LineIndent = 0f;
				return true;
			case 2246877:
			{
				int valueHashCode4 = m_xmlAttribute[0].valueHashCode;
				TMP_SpriteAsset tMP_SpriteAsset;
				if (m_xmlAttribute[0].valueType == TagType.None || m_xmlAttribute[0].valueType == TagType.NumericalValue)
				{
					if (m_defaultSpriteAsset == null)
					{
						if (TMP_Settings.defaultSpriteAsset != null)
						{
							m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
						}
						else
						{
							m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
						}
					}
					m_currentSpriteAsset = m_defaultSpriteAsset;
					if (m_currentSpriteAsset == null)
					{
						return false;
					}
				}
				else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode4, out tMP_SpriteAsset))
				{
					m_currentSpriteAsset = tMP_SpriteAsset;
				}
				else
				{
					if (tMP_SpriteAsset == null)
					{
						tMP_SpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/" + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
					}
					if (tMP_SpriteAsset == null)
					{
						return false;
					}
					MaterialReferenceManager.AddSpriteAsset(valueHashCode4, tMP_SpriteAsset);
					m_currentSpriteAsset = tMP_SpriteAsset;
				}
				if (m_xmlAttribute[0].valueType == TagType.NumericalValue)
				{
					int num4 = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
					if (num4 == -9999)
					{
						return false;
					}
					if (num4 > m_currentSpriteAsset.spriteInfoList.Count - 1)
					{
						return false;
					}
					m_spriteIndex = num4;
				}
				else if (m_xmlAttribute[1].nameHashCode == 43347)
				{
					int spriteIndex = m_currentSpriteAsset.GetSpriteIndex(m_xmlAttribute[1].valueHashCode);
					if (spriteIndex == -1)
					{
						return false;
					}
					m_spriteIndex = spriteIndex;
				}
				else
				{
					if (m_xmlAttribute[1].nameHashCode != 295562)
					{
						return false;
					}
					int num5 = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength, m_xmlAttribute[1].valueDecimalIndex);
					if (num5 == -9999)
					{
						return false;
					}
					if (num5 > m_currentSpriteAsset.spriteInfoList.Count - 1)
					{
						return false;
					}
					m_spriteIndex = num5;
				}
				m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentSpriteAsset.material, m_currentSpriteAsset, m_materialReferences, m_materialReferenceIndexLookup);
				m_spriteColor = s_colorWhite;
				m_tintSprite = false;
				if (m_xmlAttribute[1].nameHashCode == 45819)
				{
					m_tintSprite = ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength, m_xmlAttribute[1].valueDecimalIndex) != 0f;
				}
				else if (m_xmlAttribute[2].nameHashCode == 45819)
				{
					m_tintSprite = ConvertToFloat(m_htmlTag, m_xmlAttribute[2].valueStartIndex, m_xmlAttribute[2].valueLength, m_xmlAttribute[2].valueDecimalIndex) != 0f;
				}
				if (m_xmlAttribute[1].nameHashCode == 281955)
				{
					m_spriteColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);
				}
				else if (m_xmlAttribute[2].nameHashCode == 281955)
				{
					m_spriteColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[2].valueStartIndex, m_xmlAttribute[2].valueLength);
				}
				m_xmlAttribute[1].nameHashCode = 0;
				m_xmlAttribute[2].nameHashCode = 0;
				m_textElementType = TMP_TextElementType.Sprite;
				return true;
			}
			case 730022849:
				m_style |= FontStyles.LowerCase;
				return true;
			case -1668324918:
				m_style &= (FontStyles)(-9);
				return true;
			case 13526026:
			case 781906058:
				m_style |= FontStyles.UpperCase;
				return true;
			case -1616441709:
			case 52232547:
				m_style &= (FontStyles)(-17);
				return true;
			case 766244328:
				m_style |= FontStyles.SmallCaps;
				return true;
			case -1632103439:
				m_style &= (FontStyles)(-33);
				return true;
			case 2109854:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				m_marginLeft = num3;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
					break;
				}
				m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
				m_marginRight = m_marginLeft;
				return true;
			case 7639357:
				m_marginLeft = 0f;
				m_marginRight = 0f;
				return true;
			case 1100728678:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				m_marginLeft = num3;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
					break;
				}
				m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
				return true;
			case -884817987:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				m_marginRight = num3;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginRight *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					m_marginRight = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginRight / 100f;
					break;
				}
				m_marginRight = ((!(m_marginRight >= 0f)) ? 0f : m_marginRight);
				return true;
			case 1109349752:
				num3 = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength, m_xmlAttribute[0].valueDecimalIndex);
				if (num3 == -9999f || num3 == 0f)
				{
					return false;
				}
				m_lineHeight = num3;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_lineHeight *= m_fontAsset.fontInfo.LineHeight * m_fontScale;
					break;
				case TagUnits.Percentage:
					m_lineHeight = m_fontAsset.fontInfo.LineHeight * m_lineHeight / 100f * m_fontScale;
					break;
				}
				return true;
			case -445573839:
				m_lineHeight = 0f;
				return true;
			case 15115642:
				tag_NoParsing = true;
				return true;
			case 1913798:
			{
				int valueHashCode = m_xmlAttribute[0].valueHashCode;
				if (m_isParsingText)
				{
					m_actionStack.Add(valueHashCode);
					Debug.Log("Action ID: [" + valueHashCode + "] First character index: " + m_characterCount);
				}
				return true;
			}
			case 7443301:
				if (m_isParsingText)
				{
					Debug.Log("Action ID: [" + m_actionStack.CurrentItem() + "] Last character index: " + (m_characterCount - 1));
				}
				m_actionStack.Remove();
				return true;
			default:
				return false;
			}
		}
	}
}
