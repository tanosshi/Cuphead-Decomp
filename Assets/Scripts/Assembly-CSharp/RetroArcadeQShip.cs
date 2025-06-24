using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroArcadeQShip : RetroArcadeEnemy
{
	private const float OFFSCREEN_Y = 350f;

	private const float MOVE_Y_SPEED = 500f;

	private LevelProperties.RetroArcade properties;

	private LevelProperties.RetroArcade.QShip p;

	[SerializeField]
	private RetroArcadeQShipOrbitingTile tilePrefab;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private RetroArcadeQShipTentacle tentaclePrefab;

	private List<RetroArcadeQShipOrbitingTile> tiles;

	public float TileRotationSpeed { get; private set; }

	public void LevelInit(LevelProperties.RetroArcade properties)
	{
		this.properties = properties;
	}

	public void StartQShip()
	{
		base.gameObject.SetActive(true);
		p = properties.CurrentState.qShip;
		hp = p.hp;
		TileRotationSpeed = p.tileRotationSpeed.min;
		base.PointsBonus = p.pointsGained;
		base.PointsWorth = p.pointsBonus;
		tiles = new List<RetroArcadeQShipOrbitingTile>();
		for (int i = 0; i < p.numSpinningTiles; i++)
		{
			RetroArcadeQShipOrbitingTile item = tilePrefab.Create(this, 360f * (float)i / (float)p.numSpinningTiles, p);
			tiles.Add(item);
		}
		StartCoroutine(move_cr());
		StartCoroutine(tentacle_cr());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		TileRotationSpeed = p.tileRotationSpeed.min * Mathf.Pow(p.tileRotationSpeed.max / p.tileRotationSpeed.min, 1f - hp / p.hp);
	}

	private IEnumerator move_cr()
	{
		base.transform.SetPosition(0f, 350f + p.tileRotationDistance);
		MoveY(p.yPos - (350f + p.tileRotationDistance), 500f);
		while (movingY)
		{
			yield return new WaitForFixedUpdate();
		}
		float t = 0f;
		float moveTime = p.maxXPos * 2f / p.moveSpeed;
		while (true)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetPosition(Mathf.Sin(t * (float)Math.PI / moveTime) * p.maxXPos);
			yield return new WaitForFixedUpdate();
		}
	}

	public override void Die()
	{
		StopAllCoroutines();
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			collider2D.enabled = false;
		}
		base.IsDead = true;
		SpriteRenderer[] componentsInChildren2 = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren2)
		{
			spriteRenderer.color = new Color(0f, 0f, 0f, 0.25f);
		}
		properties.DealDamageToNextNamedState();
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(350f + p.tileRotationDistance - base.transform.position.y, 500f);
		while (movingY)
		{
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ShootProjectile()
	{
		projectilePrefab.Create(projectileRoot.position, -90f - p.shotSpreadAngle, p.shotSpeed);
		projectilePrefab.Create(projectileRoot.position, -90f, p.shotSpeed);
		projectilePrefab.Create(projectileRoot.position, -90f + p.shotSpreadAngle, p.shotSpeed);
	}

	private IEnumerator tentacle_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.tentacleSpawnRange.RandomFloat());
			bool left = Rand.Bool();
			base.animator.SetBool((!left) ? "RightTentacle" : "LeftTentacle", true);
			yield return CupheadTime.WaitForSeconds(this, p.tentacleWarningDuration);
			base.animator.SetBool((!left) ? "RightTentacle" : "LeftTentacle", false);
			tentaclePrefab.Create(left ? RetroArcadeQShipTentacle.Direction.Right : RetroArcadeQShipTentacle.Direction.Left, p);
		}
	}
}
