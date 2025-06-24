using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[Serializable]
	public class ThemeSettings : ScriptableObject
	{
		[Serializable]
		private abstract class SelectableSettings_Base
		{
			[SerializeField]
			protected Selectable.Transition _transition;

			[SerializeField]
			protected CustomColorBlock _colors;

			[SerializeField]
			protected CustomSpriteState _spriteState;

			[SerializeField]
			protected CustomAnimationTriggers _animationTriggers;

			public Selectable.Transition transition
			{
				get
				{
					return _transition;
				}
			}

			public CustomColorBlock selectableColors
			{
				get
				{
					return _colors;
				}
			}

			public CustomSpriteState spriteState
			{
				get
				{
					return _spriteState;
				}
			}

			public CustomAnimationTriggers animationTriggers
			{
				get
				{
					return _animationTriggers;
				}
			}

			public virtual void Apply(Selectable item)
			{
				Selectable.Transition transition = _transition;
				bool flag = item.transition != transition;
				item.transition = transition;
				ICustomSelectable customSelectable = item as ICustomSelectable;
				switch (transition)
				{
				case Selectable.Transition.ColorTint:
				{
					CustomColorBlock colors = _colors;
					colors.fadeDuration = 0f;
					item.colors = colors;
					colors.fadeDuration = _colors.fadeDuration;
					item.colors = colors;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedColor = colors.disabledHighlightedColor;
					}
					break;
				}
				case Selectable.Transition.SpriteSwap:
					item.spriteState = _spriteState;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedSprite = _spriteState.disabledHighlightedSprite;
					}
					break;
				case Selectable.Transition.Animation:
					item.animationTriggers.disabledTrigger = _animationTriggers.disabledTrigger;
					item.animationTriggers.highlightedTrigger = _animationTriggers.highlightedTrigger;
					item.animationTriggers.normalTrigger = _animationTriggers.normalTrigger;
					item.animationTriggers.pressedTrigger = _animationTriggers.pressedTrigger;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedTrigger = _animationTriggers.disabledHighlightedTrigger;
					}
					break;
				}
				if (flag)
				{
					item.targetGraphic.CrossFadeColor(item.targetGraphic.color, 0f, true, true);
				}
			}
		}

		[Serializable]
		private class SelectableSettings : SelectableSettings_Base
		{
			[SerializeField]
			private ImageSettings _imageSettings;

			public ImageSettings imageSettings
			{
				get
				{
					return _imageSettings;
				}
			}

			public override void Apply(Selectable item)
			{
				if (!(item == null))
				{
					base.Apply(item);
					if (_imageSettings != null)
					{
						_imageSettings.CopyTo(item.targetGraphic as Image);
					}
				}
			}
		}

		[Serializable]
		private class SliderSettings : SelectableSettings_Base
		{
			[SerializeField]
			private ImageSettings _handleImageSettings;

			[SerializeField]
			private ImageSettings _fillImageSettings;

			[SerializeField]
			private ImageSettings _backgroundImageSettings;

			public ImageSettings handleImageSettings
			{
				get
				{
					return _handleImageSettings;
				}
			}

			public ImageSettings fillImageSettings
			{
				get
				{
					return _fillImageSettings;
				}
			}

			public ImageSettings backgroundImageSettings
			{
				get
				{
					return _backgroundImageSettings;
				}
			}

			private void Apply(Slider item)
			{
				if (item == null)
				{
					return;
				}
				if (_handleImageSettings != null)
				{
					_handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (_fillImageSettings != null)
				{
					RectTransform fillRect = item.fillRect;
					if (fillRect != null)
					{
						_fillImageSettings.CopyTo(fillRect.GetComponent<Image>());
					}
				}
				if (_backgroundImageSettings != null)
				{
					Transform transform = item.transform.Find("Background");
					if (transform != null)
					{
						_backgroundImageSettings.CopyTo(transform.GetComponent<Image>());
					}
				}
			}

			public override void Apply(Selectable item)
			{
				Apply(item as Slider);
			}
		}

		[Serializable]
		private class ScrollbarSettings : SelectableSettings_Base
		{
			[SerializeField]
			private ImageSettings _handleImageSettings;

			[SerializeField]
			private ImageSettings _backgroundImageSettings;

			public ImageSettings handle
			{
				get
				{
					return _handleImageSettings;
				}
			}

			public ImageSettings background
			{
				get
				{
					return _backgroundImageSettings;
				}
			}

			private void Apply(Scrollbar item)
			{
				if (!(item == null))
				{
					if (_handleImageSettings != null)
					{
						_handleImageSettings.CopyTo(item.targetGraphic as Image);
					}
					if (_backgroundImageSettings != null)
					{
						_backgroundImageSettings.CopyTo(item.GetComponent<Image>());
					}
				}
			}

			public override void Apply(Selectable item)
			{
				base.Apply(item);
				Apply(item as Scrollbar);
			}
		}

		[Serializable]
		private class ImageSettings
		{
			[SerializeField]
			private Color _color = Color.white;

			[SerializeField]
			private Sprite _sprite;

			[SerializeField]
			private Material _materal;

			[SerializeField]
			private Image.Type _type;

			[SerializeField]
			private bool _preserveAspect;

			[SerializeField]
			private bool _fillCenter;

			[SerializeField]
			private Image.FillMethod _fillMethod;

			[SerializeField]
			private float _fillAmout;

			[SerializeField]
			private bool _fillClockwise;

			[SerializeField]
			private int _fillOrigin;

			public Color color
			{
				get
				{
					return _color;
				}
			}

			public Sprite sprite
			{
				get
				{
					return _sprite;
				}
			}

			public Material materal
			{
				get
				{
					return _materal;
				}
			}

			public Image.Type type
			{
				get
				{
					return _type;
				}
			}

			public bool preserveAspect
			{
				get
				{
					return _preserveAspect;
				}
			}

			public bool fillCenter
			{
				get
				{
					return _fillCenter;
				}
			}

			public Image.FillMethod fillMethod
			{
				get
				{
					return _fillMethod;
				}
			}

			public float fillAmout
			{
				get
				{
					return _fillAmout;
				}
			}

			public bool fillClockwise
			{
				get
				{
					return _fillClockwise;
				}
			}

			public int fillOrigin
			{
				get
				{
					return _fillOrigin;
				}
			}

			public virtual void CopyTo(Image image)
			{
				if (!(image == null))
				{
					image.color = _color;
					image.sprite = _sprite;
					image.material = _materal;
					image.type = _type;
					image.preserveAspect = _preserveAspect;
					image.fillCenter = _fillCenter;
					image.fillMethod = _fillMethod;
					image.fillAmount = _fillAmout;
					image.fillClockwise = _fillClockwise;
					image.fillOrigin = _fillOrigin;
				}
			}
		}

		[Serializable]
		private struct CustomColorBlock
		{
			[SerializeField]
			private float m_ColorMultiplier;

			[SerializeField]
			private Color m_DisabledColor;

			[SerializeField]
			private float m_FadeDuration;

			[SerializeField]
			private Color m_HighlightedColor;

			[SerializeField]
			private Color m_NormalColor;

			[SerializeField]
			private Color m_PressedColor;

			[SerializeField]
			private Color m_DisabledHighlightedColor;

			public float colorMultiplier
			{
				get
				{
					return m_ColorMultiplier;
				}
				set
				{
					m_ColorMultiplier = value;
				}
			}

			public Color disabledColor
			{
				get
				{
					return m_DisabledColor;
				}
				set
				{
					m_DisabledColor = value;
				}
			}

			public float fadeDuration
			{
				get
				{
					return m_FadeDuration;
				}
				set
				{
					m_FadeDuration = value;
				}
			}

			public Color highlightedColor
			{
				get
				{
					return m_HighlightedColor;
				}
				set
				{
					m_HighlightedColor = value;
				}
			}

			public Color normalColor
			{
				get
				{
					return m_NormalColor;
				}
				set
				{
					m_NormalColor = value;
				}
			}

			public Color pressedColor
			{
				get
				{
					return m_PressedColor;
				}
				set
				{
					m_PressedColor = value;
				}
			}

			public Color disabledHighlightedColor
			{
				get
				{
					return m_DisabledHighlightedColor;
				}
				set
				{
					m_DisabledHighlightedColor = value;
				}
			}

			public static implicit operator ColorBlock(CustomColorBlock item)
			{
				return new ColorBlock
				{
					colorMultiplier = item.m_ColorMultiplier,
					disabledColor = item.m_DisabledColor,
					fadeDuration = item.m_FadeDuration,
					highlightedColor = item.m_HighlightedColor,
					normalColor = item.m_NormalColor,
					pressedColor = item.m_PressedColor
				};
			}
		}

		[Serializable]
		private struct CustomSpriteState
		{
			[SerializeField]
			private Sprite m_DisabledSprite;

			[SerializeField]
			private Sprite m_HighlightedSprite;

			[SerializeField]
			private Sprite m_PressedSprite;

			[SerializeField]
			private Sprite m_DisabledHighlightedSprite;

			public Sprite disabledSprite
			{
				get
				{
					return m_DisabledSprite;
				}
				set
				{
					m_DisabledSprite = value;
				}
			}

			public Sprite highlightedSprite
			{
				get
				{
					return m_HighlightedSprite;
				}
				set
				{
					m_HighlightedSprite = value;
				}
			}

			public Sprite pressedSprite
			{
				get
				{
					return m_PressedSprite;
				}
				set
				{
					m_PressedSprite = value;
				}
			}

			public Sprite disabledHighlightedSprite
			{
				get
				{
					return m_DisabledHighlightedSprite;
				}
				set
				{
					m_DisabledHighlightedSprite = value;
				}
			}

			public static implicit operator SpriteState(CustomSpriteState item)
			{
				return new SpriteState
				{
					disabledSprite = item.m_DisabledSprite,
					highlightedSprite = item.m_HighlightedSprite,
					pressedSprite = item.m_PressedSprite
				};
			}
		}

		[Serializable]
		private class CustomAnimationTriggers
		{
			[SerializeField]
			private string m_DisabledTrigger;

			[SerializeField]
			private string m_HighlightedTrigger;

			[SerializeField]
			private string m_NormalTrigger;

			[SerializeField]
			private string m_PressedTrigger;

			[SerializeField]
			private string m_DisabledHighlightedTrigger;

			public string disabledTrigger
			{
				get
				{
					return m_DisabledTrigger;
				}
				set
				{
					m_DisabledTrigger = value;
				}
			}

			public string highlightedTrigger
			{
				get
				{
					return m_HighlightedTrigger;
				}
				set
				{
					m_HighlightedTrigger = value;
				}
			}

			public string normalTrigger
			{
				get
				{
					return m_NormalTrigger;
				}
				set
				{
					m_NormalTrigger = value;
				}
			}

			public string pressedTrigger
			{
				get
				{
					return m_PressedTrigger;
				}
				set
				{
					m_PressedTrigger = value;
				}
			}

			public string disabledHighlightedTrigger
			{
				get
				{
					return m_DisabledHighlightedTrigger;
				}
				set
				{
					m_DisabledHighlightedTrigger = value;
				}
			}

			public CustomAnimationTriggers()
			{
				m_DisabledTrigger = string.Empty;
				m_HighlightedTrigger = string.Empty;
				m_NormalTrigger = string.Empty;
				m_PressedTrigger = string.Empty;
				m_DisabledHighlightedTrigger = string.Empty;
			}

			public static implicit operator AnimationTriggers(CustomAnimationTriggers item)
			{
				AnimationTriggers animationTriggers = new AnimationTriggers();
				animationTriggers.disabledTrigger = item.m_DisabledTrigger;
				animationTriggers.highlightedTrigger = item.m_HighlightedTrigger;
				animationTriggers.normalTrigger = item.m_NormalTrigger;
				animationTriggers.pressedTrigger = item.m_PressedTrigger;
				return animationTriggers;
			}
		}

		[Serializable]
		private class TextSettings
		{
			[SerializeField]
			private Color _color = Color.white;

			[SerializeField]
			private Font _font;

			[SerializeField]
			private FontStyleOverride _style;

			[SerializeField]
			private float _lineSpacing = 1f;

			[SerializeField]
			private float _sizeMultiplier = 1f;

			public Color color
			{
				get
				{
					return _color;
				}
			}

			public Font font
			{
				get
				{
					return _font;
				}
			}

			public FontStyleOverride style
			{
				get
				{
					return _style;
				}
			}

			public float lineSpacing
			{
				get
				{
					return _lineSpacing;
				}
			}

			public float sizeMultiplier
			{
				get
				{
					return _sizeMultiplier;
				}
			}
		}

		private enum FontStyleOverride
		{
			Default = 0,
			Normal = 1,
			Bold = 2,
			Italic = 3,
			BoldAndItalic = 4
		}

		[SerializeField]
		private ImageSettings _mainWindowBackground;

		[SerializeField]
		private ImageSettings _popupWindowBackground;

		[SerializeField]
		private ImageSettings _areaBackground;

		[SerializeField]
		private SelectableSettings _selectableSettings;

		[SerializeField]
		private SelectableSettings _buttonSettings;

		[SerializeField]
		private SelectableSettings _inputGridFieldSettings;

		[SerializeField]
		private ScrollbarSettings _scrollbarSettings;

		[SerializeField]
		private SliderSettings _sliderSettings;

		[SerializeField]
		private ImageSettings _invertToggle;

		[SerializeField]
		private Color _invertToggleDisabledColor;

		[SerializeField]
		private ImageSettings _calibrationValueMarker;

		[SerializeField]
		private ImageSettings _calibrationRawValueMarker;

		[SerializeField]
		private TextSettings _textSettings;

		[SerializeField]
		private TextSettings _buttonTextSettings;

		[SerializeField]
		private TextSettings _inputGridFieldTextSettings;

		public void Apply(ThemedElement.ElementInfo[] elementInfo)
		{
			if (elementInfo == null)
			{
				return;
			}
			for (int i = 0; i < elementInfo.Length; i++)
			{
				if (elementInfo[i] != null)
				{
					Apply(elementInfo[i].themeClass, elementInfo[i].component);
				}
			}
		}

		private void Apply(string themeClass, Component component)
		{
			if (component as Selectable != null)
			{
				Apply(themeClass, (Selectable)component);
			}
			else if (component as Image != null)
			{
				Apply(themeClass, (Image)component);
			}
			else if (component as Text != null)
			{
				Apply(themeClass, (Text)component);
			}
			else if (component as UIImageHelper != null)
			{
				Apply(themeClass, (UIImageHelper)component);
			}
		}

		private void Apply(string themeClass, Selectable item)
		{
			if (!(item == null))
			{
				SelectableSettings_Base selectableSettings_Base = ((item as Button != null) ? ((themeClass == null || !(themeClass == "inputGridField")) ? _buttonSettings : _inputGridFieldSettings) : ((item as Scrollbar != null) ? _scrollbarSettings : ((item as Slider != null) ? ((SelectableSettings_Base)_sliderSettings) : ((SelectableSettings_Base)((!(item as Toggle != null)) ? _selectableSettings : ((themeClass == null || !(themeClass == "button")) ? _selectableSettings : _buttonSettings))))));
				selectableSettings_Base.Apply(item);
			}
		}

		private void Apply(string themeClass, Image item)
		{
			if (!(item == null))
			{
				switch (themeClass)
				{
				case "area":
					_areaBackground.CopyTo(item);
					break;
				case "popupWindow":
					_popupWindowBackground.CopyTo(item);
					break;
				case "mainWindow":
					_mainWindowBackground.CopyTo(item);
					break;
				case "calibrationValueMarker":
					_calibrationValueMarker.CopyTo(item);
					break;
				case "calibrationRawValueMarker":
					_calibrationRawValueMarker.CopyTo(item);
					break;
				case "invertToggle":
					_invertToggle.CopyTo(item);
					break;
				case "invertToggleBackground":
					_inputGridFieldSettings.imageSettings.CopyTo(item);
					break;
				case "invertToggleButtonBackground":
					_buttonSettings.imageSettings.CopyTo(item);
					break;
				}
			}
		}

		private void Apply(string themeClass, Text item)
		{
			if (!(item == null))
			{
				TextSettings textSettings;
				switch (themeClass)
				{
				case "button":
					textSettings = _buttonTextSettings;
					break;
				case "inputGridField":
					textSettings = _inputGridFieldTextSettings;
					break;
				default:
					textSettings = _textSettings;
					break;
				}
				if (textSettings.font != null)
				{
					item.font = textSettings.font;
				}
				item.color = textSettings.color;
				item.lineSpacing = textSettings.lineSpacing;
				if (textSettings.sizeMultiplier != 1f)
				{
					item.fontSize = (int)((float)item.fontSize * textSettings.sizeMultiplier);
					item.resizeTextMaxSize = (int)((float)item.resizeTextMaxSize * textSettings.sizeMultiplier);
					item.resizeTextMinSize = (int)((float)item.resizeTextMinSize * textSettings.sizeMultiplier);
				}
				if (textSettings.style != FontStyleOverride.Default)
				{
					item.fontStyle = (FontStyle)(textSettings.style - 1);
				}
			}
		}

		private void Apply(string themeClass, UIImageHelper item)
		{
			if (!(item == null))
			{
				item.SetEnabledStateColor(_invertToggle.color);
				item.SetDisabledStateColor(_invertToggleDisabledColor);
				item.Refresh();
			}
		}
	}
}
