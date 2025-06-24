using System.Collections;
using UnityEngine;

public class RetroArcadeWormRocket : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float SPAWN_X = 330f;

	private const float OFFSCREEN_Y = 330f;

	private const float BASE_Y = 270f;

	private const float MOVE_Y_SPEED = 500f;

	private Direction direction;

	private LevelProperties.RetroArcade.Worm properties;

	[SerializeField]
	private BasicProjectile brokenPiecePrefab;

	[SerializeField]
	private Transform brokenPieceRoot;

	public RetroArcadeWormRocket Create(Direction direction, LevelProperties.RetroArcade.Worm properties)
	{
		RetroArcadeWormRocket retroArcadeWormRocket = InstantiatePrefab<RetroArcadeWormRocket>();
		retroArcadeWormRocket.transform.position = new Vector2((direction != Direction.Left) ? (-330f) : 330f, 330f);
		retroArcadeWormRocket.properties = properties;
		retroArcadeWormRocket.direction = direction;
		retroArcadeWormRocket.hp = 1f;
		retroArcadeWormRocket.StartCoroutine(retroArcadeWormRocket.main_cr());
		return retroArcadeWormRocket;
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		BasicProjectile basicProjectile = brokenPiecePrefab.Create(brokenPieceRoot.position, -90f, properties.rocketBrokenPieceSpeed);
	}

	private IEnumerator main_cr()
	{
		MoveY(-60f, 500f);
		while (movingY)
		{
			yield return null;
		}
		while ((direction == Direction.Left && base.transform.position.x > -330f) || (direction == Direction.Right && base.transform.position.x < 330f))
		{
			base.transform.AddPosition((float)((direction == Direction.Right) ? 1 : (-1)) * properties.rocketSpeed * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
		MoveY(60f, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
