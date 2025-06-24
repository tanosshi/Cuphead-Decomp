using System.Collections;
using UnityEngine;

public class BeeLevelQueenFollower : AbstractProjectile
{
	public class Properties
	{
		public readonly AbstractPlayerController player;

		public readonly float introTime;

		public readonly float speedMax;

		public readonly float rotationSpeed;

		public readonly float homingTime;

		public readonly float healthMax;

		public readonly float childDelay;

		public readonly float childHealth;

		public readonly bool parryable;

		public float speed;

		public float health;

		public Properties(AbstractPlayerController player, float introTime, float speed, float rotationSpeed, float homingTime, float health, float childDelay, float childHealth, bool parryable)
		{
			this.player = player;
			this.introTime = introTime;
			speedMax = speed;
			this.rotationSpeed = rotationSpeed;
			this.homingTime = homingTime;
			healthMax = health;
			this.childDelay = childDelay;
			this.childHealth = childHealth;
			this.speed = 0f;
			this.health = health;
			this.parryable = parryable;
		}
	}

	[SerializeField]
	private BasicDamagableProjectile childPrefab;

	private Properties properties;

	private Transform aim;

	private CircleCollider2D collider;

	private DamageReceiver damageReceiver;

	private bool attacking;

	private bool rotate = true;

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0.25f;
		}
	}

	protected override float DestroyLifetime
	{
		get
		{
			return 300f;
		}
	}

	public BeeLevelQueenFollower Create(Vector2 pos, Properties properties)
	{
		BeeLevelQueenFollower beeLevelQueenFollower = base.Create() as BeeLevelQueenFollower;
		beeLevelQueenFollower.transform.position = pos;
		beeLevelQueenFollower.properties = properties;
		return beeLevelQueenFollower;
	}

	protected override void Awake()
	{
		base.Awake();
		collider = GetComponent<CircleCollider2D>();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		AudioManager.PlayLoop("bee_queen_follower_loop");
		emitAudioFromObject.Add("bee_queen_follower_loop");
		StartCoroutine(check_pos_cr());
	}

	private IEnumerator check_pos_cr()
	{
		float offset = 175f;
		while (base.transform.position.y < (float)Level.Current.Ceiling + offset && base.transform.position.y > (float)Level.Current.Ground - offset && base.transform.position.x > (float)Level.Current.Left - offset && base.transform.position.x < (float)Level.Current.Right + offset)
		{
			yield return null;
		}
		Die();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		damageReceiver.OnDamageTaken -= OnDamageTaken;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (attacking && base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
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

	protected override void Start()
	{
		base.Start();
		if (properties.parryable)
		{
			SetParryable(true);
		}
		StartCoroutine(go_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (!(aim == null) && !(properties.player == null) && !base.dead)
		{
			base.transform.position += base.transform.right * (properties.speed * (float)CupheadTime.Delta);
			aim.LookAt2D(properties.player.center);
			if (rotate)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, aim.rotation, properties.rotationSpeed * (float)CupheadTime.Delta);
			}
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Stop("bee_queen_follower_loop");
		collider.enabled = false;
		StopAllCoroutines();
	}

	private IEnumerator go_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.introTime);
		animator.SetTrigger("Continue");
		attacking = true;
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
		float t = 0f;
		while (t < 2f)
		{
			float val = t / 2f;
			properties.speed = Mathf.Lerp(0f, properties.speedMax, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		properties.speed = properties.speedMax;
		yield return CupheadTime.WaitForSeconds(this, properties.homingTime);
		rotate = false;
	}

	private IEnumerator children_cr()
	{
		while (true)
		{
			childPrefab.Create(base.transform.position, Random.Range(0, 360), 0f, properties.childHealth);
			yield return CupheadTime.WaitForSeconds(this, properties.childDelay);
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
	}

	public override void OnParryDie()
	{
	}
}
