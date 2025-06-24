using UnityEngine;
using UnityEngine.UI;

public class CupheadGlyph : MonoBehaviour
{
	protected const float PADDINGH = 25f;

	public int rewiredPlayerId;

	public CupheadButton button;

	[SerializeField]
	private Image glyphSymbolText;

	[SerializeField]
	private Text glyphText;

	[SerializeField]
	private Image glyphSymbolChar;

	[SerializeField]
	private Text glyphChar;

	[SerializeField]
	private RectTransform[] rectTransformTexts;

	[SerializeField]
	protected Vector2 startSize = new Vector2(37f, 37f);

	[SerializeField]
	protected float paddingText = 10.7f;

	public float preferredWidth
	{
		get
		{
			return glyphText.preferredWidth + paddingText;
		}
	}

	private void Start()
	{
		Init();
		PlayerManager.OnControlsChanged += OnControlsChanged;
	}

	private void OnControlsChanged()
	{
		Init();
	}

	public void Init()
	{
		string text = CupheadInput.InputDisplayForButton(button, rewiredPlayerId);
		bool flag = text.Length > 1;
		glyphSymbolText.gameObject.SetActive(flag);
		glyphText.gameObject.SetActive(flag);
		glyphChar.gameObject.SetActive(!flag);
		glyphSymbolChar.gameObject.SetActive(!flag);
		glyphText.text = text;
		glyphChar.text = text;
		for (int i = 0; i < rectTransformTexts.Length; i++)
		{
			rectTransformTexts[i].sizeDelta = new Vector2(preferredWidth, rectTransformTexts[i].sizeDelta.y);
		}
		LayoutElement component = GetComponent<LayoutElement>();
		if (component != null)
		{
			component.preferredWidth = preferredWidth;
		}
	}

	private void OnDestroy()
	{
		PlayerManager.OnControlsChanged -= OnControlsChanged;
	}
}
