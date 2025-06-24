using System.Collections;
using UnityEngine;

public class RetroArcadeCaterpillarBodyPart : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float TOP_Y = 230f;

	private const float BOTTOM_Y = -120f;

	private const float SPACING = 50f;

	private const float OFFSCREEN_Y = 300f;

	private const float MOVE_OFFSCREEN_SPEED = 500f;

	public const float TURNAROUND_X = 320f;

	[SerializeField]
	private BasicProjectile bulletPrefab;

	[SerializeField]
	private Transform bulletRoot;

	[SerializeField]
	private bool isHead;

	private LevelProperties.RetroArcade.Caterpillar properties;

	private RetroArcadeCaterpillarManager manager;

	private Direction direction;

	private Vector2 targetPos;

	private bool moveY;

	private int timesDropped;

	private bool atBottom;

	public RetroArcadeCaterpillarBodyPart Create(int index, Direction direction, RetroArcadeCaterpillarManager manager, LevelProperties.RetroArcade.Caterpillar properties)
	{
		RetroArcadeCaterpillarBodyPart retroArcadeCaterpillarBodyPart = InstantiatePrefab<RetroArcadeCaterpillarBodyPart>();
		retroArcadeCaterpillarBodyPart.transform.SetPosition((direction != Direction.Right) ? 320f : (-320f), 300f + (float)index * 50f);
		retroArcadeCaterpillarBodyPart.direction = direction;
		retroArcadeCaterpillarBodyPart.manager = manager;
		retroArcadeCaterpillarBodyPart.properties = properties;
		retroArcadeCaterpillarBodyPart.manager = manager;
		retroArcadeCaterpillarBodyPart.targetPos = new Vector2(retroArcadeCaterpillarBodyPart.transform.position.x, 230f);
		retroArcadeCaterpillarBodyPart.moveY = true;
		retroArcadeCaterpillarBodyPart.hp = properties.hp;
		return retroArcadeCaterpillarBodyPart;
	}

	protected override void Start()
	{
		base.Start();
		base.PointsWorth = properties.pointsGained;
		base.PointsBonus = properties.pointsBonus;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (movingY)
		{
			return;
		}
		float num = manager.moveSpeed * CupheadTime.FixedDelta;
		float magnitude = (targetPos - (Vector2)base.transform.position).magnitude;
		if (magnitude > num)
		{
			move(num);
			return;
		}
		base.transform.position = targetPos;
		if (moveY)
		{
			moveY = false;
			targetPos = new Vector2((direction != Direction.Left) ? 320f : (-320f), base.transform.position.y);
			if (atBottom && isHead)
			{
				manager.OnReachBottom();
			}
		}
		else
		{
			moveY = true;
			direction = ((direction == Direction.Left) ? Direction.Right : Direction.Left);
			if (atBottom)
			{
				targetPos = new Vector2(base.transform.position.x, 230f);
				atBottom = false;
			}
			else if (timesDropped >= properties.dropCount)
			{
				targetPos = new Vector2(base.transform.position.x, -120f);
				timesDropped = 0;
				atBottom = true;
			}
			else
			{
				targetPos = new Vector2(base.transform.position.x, base.transform.position.y - 50f);
				timesDropped++;
				if (bulletPrefab != null)
				{
					Shoot();
				}
			}
		}
		move(num - magnitude);
	}

	private void move(float distance)
	{
		base.transform.position = (Vector2)base.transform.position + (targetPos - (Vector2)base.transform.position).normalized * distance;
	}

	public override void Die()
	{
		base.Die();
		if (!isHead)
		{
			manager.OnBodyPartDie(this);
		}
	}

	public void Shoot()
	{
		float rotation = MathUtils.DirectionToAngle(PlayerManager.GetNext().transform.position - bulletRoot.position);
		bulletPrefab.Create(bulletRoot.position, rotation, properties.shotSpeed);
	}

	public void OnWaveEnd()
	{
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(420f, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
