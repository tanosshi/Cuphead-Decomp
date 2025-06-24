using UnityEngine;

public class FrogsLevelOniBullet : AbstractFrogsLevelSlotBullet
{
	[SerializeField]
	private Transform parryBox;

	[SerializeField]
	private Transform hurtBox;

	private LevelProperties.Frogs.Demon properties;

	public FrogsLevelOniBullet Create(Vector2 pos, float speed, LevelProperties.Frogs.Demon properties)
	{
		FrogsLevelOniBullet frogsLevelOniBullet = Create(pos, speed) as FrogsLevelOniBullet;
		frogsLevelOniBullet.properties = properties;
		return frogsLevelOniBullet;
	}

	protected override void Start()
	{
		base.Start();
		SetSize();
	}

	private void SetSize()
	{
		parryBox.SetScale(null, properties.demonParryHeight);
		hurtBox.SetScale(null, properties.demonFlameHeight);
	}
}
