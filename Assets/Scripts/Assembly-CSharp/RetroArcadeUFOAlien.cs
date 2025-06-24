using UnityEngine;

public class RetroArcadeUFOAlien : RetroArcadeEnemy
{
	private LevelProperties.RetroArcade.UFO properties;

	private RetroArcadeUFO parent;

	private int cyclePositionIndex;

	public float NormalizedHpRemaining
	{
		get
		{
			return hp / properties.hp;
		}
	}

	public RetroArcadeUFOAlien Create(RetroArcadeUFO parent, LevelProperties.RetroArcade.UFO properties)
	{
		RetroArcadeUFOAlien retroArcadeUFOAlien = InstantiatePrefab<RetroArcadeUFOAlien>();
		retroArcadeUFOAlien.properties = properties;
		retroArcadeUFOAlien.parent = parent;
		retroArcadeUFOAlien.hp = properties.hp;
		retroArcadeUFOAlien.transform.parent = parent.transform;
		retroArcadeUFOAlien.transform.position = parent.transform.position;
		cyclePositionIndex = Random.Range(0, retroArcadeUFOAlien.properties.cyclePositionX.Length);
		return retroArcadeUFOAlien;
	}

	protected override void Start()
	{
		base.Start();
		base.transform.position = base.transform.position + new Vector3(properties.initialPositionX, properties.alienYPosition, 0f) * parent.transform.localScale.y;
		base.PointsWorth = properties.pointsGained;
		base.PointsWorth = properties.pointsBonus;
	}

	public override void Die()
	{
		base.Die();
		parent.OnAlienDie();
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		base.transform.position = new Vector3(properties.cyclePositionX[cyclePositionIndex], base.transform.position.y, 0f);
		cyclePositionIndex = (cyclePositionIndex + 1) % properties.cyclePositionX.Length;
	}
}
