using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractUIInteractionDialogue : AbstractMonoBehaviour
{
	public enum AnimationType
	{
		Full = 0,
		Individual = 1
	}

	[Serializable]
	public class Properties
	{
		public const AnimationType DEFAULT_ANIM_TYPE = AnimationType.Full;

		public const CupheadButton DEFAULT_BUTTON = CupheadButton.Accept;

		public string text = string.Empty;

		public string subtext = string.Empty;

		public AnimationType animationType;

		public CupheadButton button = CupheadButton.Accept;

		public Properties()
		{
			text = string.Empty;
			subtext = string.Empty;
			button = CupheadButton.Accept;
			animationType = AnimationType.Full;
		}

		public Properties(string text)
		{
			this.text = text;
			subtext = string.Empty;
			button = CupheadButton.Accept;
			animationType = AnimationType.Full;
		}

		public Properties(string text, CupheadButton button)
		{
			this.text = text;
			subtext = string.Empty;
			this.button = button;
			animationType = AnimationType.Full;
		}

		public Properties(string text, CupheadButton button, AnimationType animationType)
		{
			this.text = text;
			subtext = string.Empty;
			this.button = button;
			this.animationType = animationType;
		}
	}

	protected const float PADDINGH = 10f;

	protected const float PADDINGV = 11f;

	[SerializeField]
	protected Text uiText;

	[SerializeField]
	protected TextMeshProUGUI tmpText;

	[SerializeField]
	protected CupheadGlyph glyph;

	[SerializeField]
	protected RectTransform back;

	protected Transform target;

	protected Vector2 dialogueOffset;

	private float closeScale;

	protected virtual float OpenTime
	{
		get
		{
			return 0.3f;
		}
	}

	protected virtual float CloseTime
	{
		get
		{
			return 0.3f;
		}
	}

	protected virtual float OpenScale
	{
		get
		{
			return 1f;
		}
	}

	protected string Text
	{
		get
		{
			return tmpText.text;
		}
		set
		{
			tmpText.text = value;
		}
	}

	protected virtual float PreferredWidth
	{
		get
		{
			return tmpText.preferredWidth + glyph.preferredWidth;
		}
	}

	protected override void Start()
	{
		base.Start();
		base.transform.localScale = Vector3.zero;
	}

	protected virtual void Init(Properties properties, PlayerInput player, Vector2 offset)
	{
		float num = 40f;
		target = player.transform;
		dialogueOffset = offset;
		int result;
		if (int.TryParse(properties.text, out result))
		{
			Text = Localization.Translate(result).text.ToUpper();
		}
		else
		{
			Text = properties.text.ToUpper();
		}
		glyph.rewiredPlayerId = (int)player.playerId;
		glyph.button = properties.button;
		glyph.Init();
		back.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PreferredWidth + 10f);
		back.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num + 11f);
		TweenValue(0f, 1f, OpenTime, EaseUtils.EaseType.linear, OpenTween);
	}

	public void Close()
	{
		closeScale = base.transform.localScale.x;
		StopAllCoroutines();
		TweenValue(0f, 1f, CloseTime, EaseUtils.EaseType.linear, CloseTween);
	}

	protected virtual void OpenTween(float value)
	{
		float num = 40f;
		back.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PreferredWidth + 10f);
		back.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num + 11f);
		base.transform.localScale = Vector3.one * EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, OpenScale, value);
	}

	protected virtual void CloseTween(float value)
	{
		base.transform.localScale = Vector3.one * EaseUtils.Ease(EaseUtils.EaseType.easeInBack, closeScale, 0f, value);
		if (base.transform.localScale.x < 0.001f)
		{
			StopAllCoroutines();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
