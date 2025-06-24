using System;
using System.Collections;
using UnityEngine;

public class FlyingGenieLevelSpawner : AbstractProjectile
{
	private const string AttackParameterName = "Attack";

	private const string EndAttackParameterName = "End";

	[SerializeField]
	private FlyingGenieLevelSpawnerPoint pointPrefab;

	private FlyingGenieLevelSpawnerPoint[] points;

	private LevelProperties.FlyingGenie.Bullets properties;

	private AbstractPlayerController player;

	private FlyingGenieLevelGenieTransform parent;

	private float speed;

	private int attackCount;

	public bool isDead;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public FlyingGenieLevelSpawner Create(Vector2 pos, AbstractPlayerController player, LevelProperties.FlyingGenie.Bullets properties, FlyingGenieLevelGenieTransform parent)
	{
		FlyingGenieLevelSpawner flyingGenieLevelSpawner = base.Create(pos) as FlyingGenieLevelSpawner;
		flyingGenieLevelSpawner.properties = properties;
		flyingGenieLevelSpawner.player = player;
		flyingGenieLevelSpawner.parent = parent;
		return flyingGenieLevelSpawner;
	}

	protected override void Start()
	{
		base.Start();
		SetUpSpawnPoints();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		for (int i = 0; i < points.Length; i++)
		{
			points[i].transform.Rotate(0f, 0f, properties.spawnerRotateSpeed * (float)CupheadTime.Delta);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
	}

	private void SetUpSpawnPoints()
	{
		float num = GetComponent<CircleCollider2D>().radius / 2f;
		float num2 = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		float rotation = 0f;
		float x = base.transform.position.x;
		float y = base.transform.position.y;
		points = new FlyingGenieLevelSpawnerPoint[properties.spawnerCount];
		for (int i = 0; i < properties.spawnerCount; i++)
		{
			switch (i)
			{
			case 0:
				rotation = num2 * 57.29578f + 90f;
				break;
			case 1:
				rotation = num2 * 57.29578f - 90f;
				break;
			case 2:
				rotation = num2 * 57.29578f + 360f;
				break;
			case 3:
				rotation = num2 * 57.29578f - 180f;
				break;
			}
			points[i] = pointPrefab.Create(new Vector3(x, y), rotation, properties.childSpeed, properties);
			points[i].transform.parent = base.transform;
		}
	}

	private IEnumerator move_cr()
	{
		float offset = 200f;
		float size = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
		int count = 0;
		int maxCount = properties.spawnerMoveCountRange.RandomInt();
		YieldInstruction wait = new WaitForFixedUpdate();
		Vector3 startDir = Vector3.zero - base.transform.position;
		while (base.transform.position.y > 0f)
		{
			base.transform.position += startDir.normalized * properties.spawnerSpeed * CupheadTime.FixedDelta;
			yield return wait;
		}
		while (true)
		{
			Vector3 start = base.transform.position;
			Vector3 dir = player.transform.position - base.transform.position;
			Vector3 endDist = start + dir.normalized * properties.spawnerDistance;
			if (isDead)
			{
				while (true)
				{
					base.transform.position += dir.normalized * properties.spawnerSpeed * CupheadTime.FixedDelta;
					if (base.transform.position.x < -640f - offset || base.transform.position.x > 640f + offset || base.transform.position.y > 360f + offset || base.transform.position.y < -360f - offset)
					{
						break;
					}
					yield return wait;
				}
				Kill();
				StopAllCoroutines();
			}
			while (base.transform.position != endDist)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, endDist, properties.spawnerSpeed * CupheadTime.FixedDelta);
				if (base.transform.position.x < -640f + size || base.transform.position.x > 640f - size || base.transform.position.y > 360f - size || base.transform.position.y < -360f + size)
				{
					break;
				}
				yield return wait;
			}
			yield return CupheadTime.WaitForSeconds(this, properties.spawnerMoveDelay);
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			count++;
			if (count >= maxCount)
			{
				while (attackCount < properties.spawnerShotCount)
				{
					animator.SetTrigger("Attack");
					yield return CupheadTime.WaitForSeconds(this, properties.spawnerShotDelay);
				}
				yield return CupheadTime.WaitForSeconds(this, properties.spawnerHesitate);
				count = 0;
				attackCount = 0;
				maxCount = properties.spawnerMoveCountRange.RandomInt();
			}
			yield return wait;
		}
	}

	private void Kill()
	{
		FlyingGenieLevelSpawnerPoint[] array = points;
		foreach (FlyingGenieLevelSpawnerPoint flyingGenieLevelSpawnerPoint in array)
		{
			flyingGenieLevelSpawnerPoint.Dead();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Attack()
	{
		FlyingGenieLevelSpawnerPoint[] array = points;
		foreach (FlyingGenieLevelSpawnerPoint flyingGenieLevelSpawnerPoint in array)
		{
			flyingGenieLevelSpawnerPoint.Shoot();
		}
		attackCount++;
		if (attackCount >= properties.spawnerShotCount)
		{
			animator.SetTrigger("End");
		}
	}
}
