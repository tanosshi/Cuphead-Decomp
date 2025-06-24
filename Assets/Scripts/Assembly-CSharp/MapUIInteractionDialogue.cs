using UnityEngine;

public class MapUIInteractionDialogue : AbstractUIInteractionDialogue
{
	private const float OFFSET_GLYPH = 10f;

	protected override float PreferredWidth
	{
		get
		{
			return tmpText.preferredWidth + glyph.preferredWidth + 10f;
		}
	}

	public static MapUIInteractionDialogue Create(Properties properties, PlayerInput player, Vector2 offset)
	{
		MapUIInteractionDialogue mapUIInteractionDialogue = Object.Instantiate(Map.Current.MapResources.mapUIInteractionDialogue);
		properties.text = string.Empty;
		mapUIInteractionDialogue.Init(properties, player, offset);
		return mapUIInteractionDialogue;
	}

	protected override void Awake()
	{
		base.Awake();
		base.transform.SetParent(MapUI.Current.sceneCanvas.transform);
		base.transform.ResetLocalTransforms();
	}

	protected override void Update()
	{
		base.Update();
		UpdatePos();
	}

	private void UpdatePos()
	{
		if (target == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Vector2 vector = (Vector2)target.position + dialogueOffset;
		base.transform.position = vector;
	}
}
