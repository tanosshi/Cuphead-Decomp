using UnityEngine;

public class DicePalaceDominoLevelBaseTile : AbstractCollidableObject
{
	[SerializeField]
	protected Sprite[] colours;

	protected LevelProperties.DicePalaceDomino properties;

	public int currentColourIndex { get; protected set; }

	public bool isActivated { get; protected set; }

	public virtual void InitTile()
	{
		isActivated = true;
	}

	public virtual void InitTile(DicePalaceDominoLevelFloor parent, LevelProperties.DicePalaceDomino properties)
	{
		this.properties = properties;
		isActivated = true;
	}

	public virtual void DeactivateTile()
	{
		isActivated = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
