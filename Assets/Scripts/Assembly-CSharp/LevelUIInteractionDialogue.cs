using UnityEngine;

public class LevelUIInteractionDialogue : AbstractUIInteractionDialogue
{
	public enum TailPosition
	{
		Right = 0,
		Left = 1,
		Bottom = 2
	}

	private const float OFFSET_GLYPH = 27f;

	private const float OFFSET_GLYPH_ONLY = 7.3f;

	private float glyphOffsetAddition;

	private TailPosition tailPosition;

	[SerializeField]
	private GameObject bottomTail;

	[SerializeField]
	private GameObject leftTail;

	[SerializeField]
	private GameObject rightTail;

	private static GameObject defaultTarget;

	protected override float PreferredWidth
	{
		get
		{
			if (tmpText.text.Length == 0)
			{
				return tmpText.preferredWidth + glyph.preferredWidth + 7.3f + glyphOffsetAddition;
			}
			return tmpText.preferredWidth + glyph.preferredWidth + 27f + glyphOffsetAddition;
		}
	}

	public static LevelUIInteractionDialogue Create(Properties properties, PlayerInput player, Vector2 offset, float glyphOffsetAddition = 0f, TailPosition tailPosition = TailPosition.Bottom, bool playerTarget = true)
	{
		LevelUIInteractionDialogue levelUIInteractionDialogue = Object.Instantiate(Level.Current.LevelResources.levelUIInteractionDialogue);
		levelUIInteractionDialogue.glyphOffsetAddition = glyphOffsetAddition;
		levelUIInteractionDialogue.tailPosition = tailPosition;
		levelUIInteractionDialogue.Init(properties, player, offset);
		if (!playerTarget && defaultTarget == null)
		{
			defaultTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
			defaultTarget.transform.position = Vector3.zero;
			defaultTarget.transform.localScale = Vector3.zero;
			levelUIInteractionDialogue.target = defaultTarget.transform;
		}
		else if (!playerTarget)
		{
			levelUIInteractionDialogue.target = defaultTarget.transform;
		}
		return levelUIInteractionDialogue;
	}

	protected override void Awake()
	{
		base.Awake();
		base.transform.SetParent(LevelHUD.Current.Canvas.transform, false);
	}

	protected override void Init(Properties properties, PlayerInput player, Vector2 offset)
	{
		base.Init(properties, player, offset);
		UpdatePos();
	}

	protected override void Update()
	{
		base.Update();
		UpdatePos();
		UpdateTailPosition();
	}

	private void UpdatePos()
	{
		if (target != null)
		{
			base.transform.position = (Vector2)target.position + dialogueOffset;
		}
	}

	private void UpdateTailPosition()
	{
		switch (tailPosition)
		{
		case TailPosition.Bottom:
			bottomTail.SetActive(true);
			break;
		case TailPosition.Right:
			rightTail.SetActive(true);
			break;
		case TailPosition.Left:
			leftTail.SetActive(true);
			break;
		}
	}
}
