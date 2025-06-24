using System;
using UnityEngine;

public class RetroArcadeUFOTurret : AbstractCollidableObject
{
	private const float ANGLE_OFFSET = -90f;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	private LevelProperties.RetroArcade.UFO properties;

	private RetroArcadeUFO parent;

	private float t;

	public RetroArcadeUFOTurret Create(RetroArcadeUFO parent, LevelProperties.RetroArcade.UFO properties, float t)
	{
		RetroArcadeUFOTurret retroArcadeUFOTurret = InstantiatePrefab<RetroArcadeUFOTurret>();
		retroArcadeUFOTurret.properties = properties;
		retroArcadeUFOTurret.parent = parent;
		retroArcadeUFOTurret.t = t;
		retroArcadeUFOTurret.transform.parent = parent.transform;
		retroArcadeUFOTurret.transform.position = parent.transform.position;
		return retroArcadeUFOTurret;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		t += CupheadTime.FixedDelta * (properties.projectileSpeed / 600f);
		float f = t % 1f * (float)Math.PI;
		Vector2 vector = new Vector2(Mathf.Cos(f) * 600f / 2f, (0f - Mathf.Sin(f)) * 300f / 2f);
		base.transform.SetPosition(parent.transform.position.x + vector.x, parent.transform.position.y + vector.y);
		float value = MathUtils.DirectionToAngle(new Vector2(Mathf.Cos(f) * 300f / 2f, (0f - Mathf.Sin(f)) * 600f / 2f)) + -90f;
		base.transform.SetEulerAngles(0f, 0f, value);
	}

	public void Shoot()
	{
		projectilePrefab.Create(projectileRoot.position, base.transform.eulerAngles.z - -90f, properties.projectileSpeed);
	}
}
