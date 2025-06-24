using UnityEngine;

public class PlayerSuperGhostHeart : AbstractLevelEntity
{
	[SerializeField]
	private Effect spark;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		base.transform.AddPosition(0f, WeaponProperties.LevelSuperGhost.heartSpeed * CupheadTime.FixedDelta);
		if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(50f, 50f)))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public PlayerSuperGhostHeart Create(Vector2 pos)
	{
		PlayerSuperGhostHeart playerSuperGhostHeart = InstantiatePrefab<PlayerSuperGhostHeart>();
		playerSuperGhostHeart.transform.position = pos;
		return playerSuperGhostHeart;
	}

	public override void OnParry(AbstractPlayerController player)
	{
		base.OnParry(player);
		player.stats.AddEx();
		spark.Create(base.transform.position);
		Object.Destroy(base.gameObject);
	}
}
