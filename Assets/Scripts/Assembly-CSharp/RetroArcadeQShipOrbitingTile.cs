using UnityEngine;

public class RetroArcadeQShipOrbitingTile : AbstractCollidableObject
{
	private float angle;

	private RetroArcadeQShip parent;

	private LevelProperties.RetroArcade.QShip properties;

	public RetroArcadeQShipOrbitingTile Create(RetroArcadeQShip parent, float angle, LevelProperties.RetroArcade.QShip properties)
	{
		RetroArcadeQShipOrbitingTile retroArcadeQShipOrbitingTile = InstantiatePrefab<RetroArcadeQShipOrbitingTile>();
		retroArcadeQShipOrbitingTile.transform.position = (Vector2)parent.transform.position + properties.tileRotationDistance * MathUtils.AngleToDirection(angle);
		retroArcadeQShipOrbitingTile.properties = properties;
		retroArcadeQShipOrbitingTile.transform.parent = parent.transform;
		retroArcadeQShipOrbitingTile.parent = parent;
		retroArcadeQShipOrbitingTile.angle = angle;
		DamageReceiver component = retroArcadeQShipOrbitingTile.GetComponent<DamageReceiver>();
		component.OnDamageTaken += retroArcadeQShipOrbitingTile.OnDamageTaken;
		return retroArcadeQShipOrbitingTile;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		angle += CupheadTime.FixedDelta * parent.TileRotationSpeed;
		base.transform.position = (Vector2)parent.transform.position + MathUtils.AngleToDirection(angle) * properties.tileRotationDistance;
		base.transform.SetEulerAngles(0f, 0f, angle);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		parent.ShootProjectile();
	}
}
