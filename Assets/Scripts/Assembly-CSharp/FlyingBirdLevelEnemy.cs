using System.Collections;
using UnityEngine;

public class FlyingBirdLevelEnemy : AbstractProjectile
{
	public class Properties
	{
		public readonly int health;

		public readonly float speed;

		public readonly float floatRange;

		public readonly float floatTime;

		public readonly float projectileHeight;

		public readonly float projectileFallTime;

		public readonly float projectileDelay;

		public Properties(int health, float speed, float floatRange, float floatTime, float projectileHeight, float projectileFallTime, float projectileDelay)
		{
			this.health = health;
			this.speed = speed;
			this.floatRange = floatRange;
			this.floatTime = floatTime;
			this.projectileHeight = projectileHeight;
			this.projectileFallTime = projectileFallTime;
			this.projectileDelay = projectileDelay;
		}
	}

	[SerializeField]
	private FlyingBirdLevelEnemyProjectile projectilePrefab;

	private Properties properties;

	private Vector2 startPos;

	private float health;

	public FlyingBirdLevelEnemy Create(Vector2 pos, Properties properties)
	{
		GameObject gameObject = Object.Instantiate(base.gameObject);
		FlyingBirdLevelEnemy component = gameObject.GetComponent<FlyingBirdLevelEnemy>();
		component.transform.position = pos;
		component.properties = properties;
		component.Init();
		return component;
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private void Init()
	{
		startPos = base.transform.position;
		health = properties.health;
		StartCoroutine(x_cr());
		StartCoroutine(shoot_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void Shoot()
	{
	}

	public override void OnParryDie()
	{
		Die();
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
		AudioManager.Play("level_flying_bird_smallbird_death");
		emitAudioFromObject.Add("level_flying_bird_smallbird_death");
		GetComponent<Collider2D>().enabled = false;
	}

	private IEnumerator y_cr()
	{
		float start = startPos.y + properties.floatRange / 2f;
		float end = startPos.y - properties.floatRange / 2f;
		base.transform.SetPosition(null, start);
		float t = 0f;
		while (true)
		{
			t = 0f;
			while (t < properties.floatTime)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, t / properties.floatTime), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
			while (t < properties.floatTime)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, end, start, t / properties.floatTime), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator x_cr()
	{
		while (true)
		{
			base.transform.AddPosition((0f - properties.speed) * (float)CupheadTime.Delta);
			if (base.transform.position.x < -740f)
			{
				Die();
			}
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.projectileDelay);
			Shoot();
			yield return null;
		}
	}
}
