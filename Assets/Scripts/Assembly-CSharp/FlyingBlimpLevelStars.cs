using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelStars : AbstractProjectile
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1
	}

	[SerializeField]
	private Transform starFXPrefab;

	private Transform starFx;

	private Vector3 spawnPoint;

	private LevelProperties.FlyingBlimp.Stars properties;

	private new DamageDealer damageDealer;

	public State state { get; private set; }

	public FlyingBlimpLevelStars Create(Vector2 pos, LevelProperties.FlyingBlimp.Stars properties)
	{
		FlyingBlimpLevelStars flyingBlimpLevelStars = base.Create() as FlyingBlimpLevelStars;
		flyingBlimpLevelStars.properties = properties;
		flyingBlimpLevelStars.transform.position = pos;
		return flyingBlimpLevelStars;
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
		float num = Random.Range(0, 2);
		starFx = Object.Instantiate(starFXPrefab);
		starFx.transform.parent = base.transform;
		Vector3 position = base.transform.position;
		if (num == 0f)
		{
			base.transform.SetScale(-1f, 1f, 1f);
			starFx.SetScale(-1f, 1f, 1f);
			position.x = base.transform.position.x + 70f;
		}
		else
		{
			position.x = base.transform.position.x - 10f;
			starFx.SetScale(1f, -1f, 1f);
		}
		starFx.transform.position = position;
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float speed = properties.speedX.RandomFloat();
		float angle = 0f;
		while (base.transform.position.x > -840f)
		{
			angle += properties.speedY * CupheadTime.FixedDelta;
			if ((float)CupheadTime.Delta != 0f)
			{
				Vector3 moveY = new Vector3(0f, Mathf.Sin(angle) * properties.sineSize);
				Vector3 moveX = base.transform.right * (0f - speed) * CupheadTime.FixedDelta;
				base.transform.position += moveX + moveY;
			}
			yield return wait;
		}
		Die();
		yield return wait;
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
