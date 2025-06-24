using System.Collections;
using UnityEngine;

public class RetroArcadeWorm : RetroArcadeEnemy
{
	private enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float OFFSCREEN_Y = -650f;

	private const float ONSCREEN_Y = -220f;

	private const float MOVE_Y_SPEED = 100f;

	private const float TURNAROUND_X = 160f;

	private LevelProperties.RetroArcade properties;

	private LevelProperties.RetroArcade.Worm p;

	[SerializeField]
	private RetroArcadeWormPlatform platform;

	[SerializeField]
	private RetroArcadeWormTongue tongue;

	[SerializeField]
	private RetroArcadeWormRocket rocketPrefab;

	private Direction direction;

	public void LevelInit(LevelProperties.RetroArcade properties)
	{
		this.properties = properties;
	}

	public void StartWorm()
	{
		base.gameObject.SetActive(true);
		p = properties.CurrentState.worm;
		hp = p.hp;
		base.PointsWorth = p.pointsGained;
		platform.Rise();
		base.transform.SetPosition(0f, -650f);
		direction = ((!Rand.Bool()) ? Direction.Right : Direction.Left);
		tongue.transform.parent = null;
		tongue.Init(p);
		tongue.Extend();
		StartCoroutine(move_cr());
		StartCoroutine(rocket_cr());
	}

	private IEnumerator move_cr()
	{
		MoveY(430f, 100f);
		while (movingY)
		{
			yield return new WaitForFixedUpdate();
		}
		while (true)
		{
			float normalizedHpRemaining = hp / p.hp;
			float speed = p.moveSpeed.min * Mathf.Pow(p.moveSpeed.max / p.moveSpeed.min, 1f - normalizedHpRemaining);
			base.transform.AddPosition((float)((direction != Direction.Left) ? 1 : (-1)) * speed * CupheadTime.FixedDelta);
			if ((direction == Direction.Left && base.transform.position.x < -160f) || (direction == Direction.Right && base.transform.position.x > 160f))
			{
				direction = ((direction == Direction.Left) ? Direction.Right : Direction.Left);
			}
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
		properties.DealDamageToNextNamedState();
		tongue.Retract();
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(-430f, 100f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator rocket_cr()
	{
		RetroArcadeWormRocket.Direction rocketDirection = ((!Rand.Bool()) ? RetroArcadeWormRocket.Direction.Right : RetroArcadeWormRocket.Direction.Left);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.rocketSpawnDelay);
			rocketPrefab.Create(rocketDirection, p);
		}
	}
}
