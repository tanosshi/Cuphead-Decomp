using System.Collections;
using UnityEngine;

public class BeeLevelQueenTriangle : AbstractProjectile
{
	public class Properties
	{
		public readonly AbstractPlayerController player;

		public readonly bool damageable;

		public readonly float introTime;

		public readonly float speedMax;

		public readonly float rotationSpeedMax;

		public readonly float healthMax;

		public readonly float childSpeed;

		public readonly float childDelay;

		public readonly float childHealth;

		public readonly int childCount;

		public readonly int direction;

		public float speed;

		public float rotationSpeed;

		public float health;

		public Properties(AbstractPlayerController player, float introTime, float speed, float rotationSpeed, float health, float childSpeed, float childDelay, float childHealth, int childCount, bool damageable)
		{
			this.player = player;
			this.damageable = damageable;
			this.introTime = introTime;
			speedMax = speed;
			rotationSpeedMax = rotationSpeed;
			healthMax = health;
			this.childSpeed = childSpeed;
			this.childDelay = childDelay;
			this.childHealth = childHealth;
			this.childCount = childCount;
			direction = MathUtils.PlusOrMinus();
			this.speed = 0f;
			this.rotationSpeed = 0f;
			this.health = health;
		}
	}

	[SerializeField]
	private bool isInvincible;

	[SerializeField]
	private Transform[] roots;

	[SerializeField]
	private BasicDamagableProjectile childPrefab;

	[SerializeField]
	private BasicProjectile childPrefabInvincible;

	private Properties properties;

	private Vector2 forward;

	private DamageReceiver damageReceiver;

	protected override float DestroyLifetime
	{
		get
		{
			return 100f;
		}
	}

	public BeeLevelQueenTriangle Create(Properties properties)
	{
		BeeLevelQueenTriangle beeLevelQueenTriangle = base.Create() as BeeLevelQueenTriangle;
		beeLevelQueenTriangle.transform.position = properties.player.center;
		beeLevelQueenTriangle.transform.SetEulerAngles(0f, 0f, Random.Range(0, 360));
		beeLevelQueenTriangle.properties = properties;
		return beeLevelQueenTriangle;
	}

	protected override void Awake()
	{
		base.Awake();
		if (!isInvincible)
		{
			damageReceiver = GetComponent<DamageReceiver>();
			damageReceiver.OnDamageTaken += OnDamageTaken;
		}
		AudioManager.Play("bee_queen_triangle_spawn");
		emitAudioFromObject.Add("bee_queen_triangle_spawn");
		AudioManager.PlayLoop("bee_queen_triangle_loop");
		emitAudioFromObject.Add("bee_queen_triangle_loop");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(go_cr());
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (!isInvincible)
		{
			damageReceiver.OnDamageTaken -= OnDamageTaken;
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		properties.health -= info.damage;
		if (properties.health <= 0f)
		{
			Die();
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Stop("bee_queen_triangle_loop");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		if (properties != null)
		{
			base.transform.AddPosition(forward.x * (float)CupheadTime.Delta * properties.speed, forward.y * (float)CupheadTime.Delta * properties.speed);
			base.transform.AddEulerAngles(0f, 0f, properties.rotationSpeed * (float)properties.direction * (float)CupheadTime.Delta);
		}
	}

	private IEnumerator go_cr()
	{
		base.transform.GetComponent<Collider2D>().enabled = false;
		Transform aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
		yield return StartCoroutine(tweenColor_cr(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), properties.introTime / 2f));
		yield return StartCoroutine(tweenColor_cr(new Color(0f, 0f, 0f, 1f), new Color(1f, 1f, 1f, 1f), properties.introTime / 2f));
		aim.LookAt2D(properties.player.center);
		forward = aim.transform.right;
		base.transform.GetComponent<Collider2D>().enabled = true;
		StartCoroutine(shoot_cr());
		float t = 0f;
		while (t < 1f)
		{
			float val = t / 1f;
			properties.speed = Mathf.Lerp(0f, properties.speedMax, val);
			properties.rotationSpeed = Mathf.Lerp(0f, properties.rotationSpeedMax, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		properties.speed = properties.speedMax;
		properties.rotationSpeed = properties.rotationSpeedMax;
	}

	private IEnumerator shoot_cr()
	{
		Debug.Log("Start Shooting");
		int count = 0;
		while (count < properties.childCount)
		{
			AudioManager.Play("bee_queen_triangle_shoot");
			emitAudioFromObject.Add("bee_queen_triangle_shoot");
			Transform[] array = roots;
			foreach (Transform transform in array)
			{
				if (properties.damageable)
				{
					childPrefab.Create(transform.position, transform.eulerAngles.z, properties.childSpeed, properties.childHealth).SetParryable(true);
				}
				else
				{
					childPrefabInvincible.Create(transform.position, transform.eulerAngles.z, properties.childSpeed).SetParryable(true);
				}
			}
			animator.Play("Attack");
			count++;
			yield return CupheadTime.WaitForSeconds(this, properties.childDelay);
		}
		animator.Play("Idle");
	}

	private IEnumerator tweenColor_cr(Color start, Color end, float time)
	{
		SpriteRenderer r = GetComponent<SpriteRenderer>();
		r.color = start;
		yield return null;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			r.color = Color.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		r.color = end;
		yield return null;
	}
}
