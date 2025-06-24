using System.Collections;
using UnityEngine;

public class RetroArcadeQShipTentacle : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float SPAWN_X = 400f;

	private const float SPAWN_Y = -140f;

	private const float OFFSCREEN_Y = -250f;

	private const float MOVE_Y_SPEED = 500f;

	private LevelProperties.RetroArcade.QShip properties;

	private Direction direction;

	public RetroArcadeQShipTentacle Create(Direction direction, LevelProperties.RetroArcade.QShip properties)
	{
		RetroArcadeQShipTentacle retroArcadeQShipTentacle = InstantiatePrefab<RetroArcadeQShipTentacle>();
		retroArcadeQShipTentacle.transform.position = new Vector2((direction != Direction.Left) ? (-400f) : 400f, -140f);
		retroArcadeQShipTentacle.properties = properties;
		retroArcadeQShipTentacle.direction = direction;
		retroArcadeQShipTentacle.hp = 1f;
		retroArcadeQShipTentacle.transform.SetScale((direction == Direction.Right) ? 1 : (-1), 1f, 1f);
		return retroArcadeQShipTentacle;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		base.transform.AddPosition((float)((direction == Direction.Right) ? 1 : (-1)) * properties.tentacleSpeed * CupheadTime.FixedDelta);
		if ((direction != Direction.Left) ? (base.transform.position.x > 400f) : (base.transform.position.x < -400f))
		{
			Object.Destroy(base.gameObject);
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
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(-250f - base.transform.position.y, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
