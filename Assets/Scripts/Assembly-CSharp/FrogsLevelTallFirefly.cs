using System.Collections;
using UnityEngine;

public class FrogsLevelTallFirefly : AbstractProjectile
{
	[SerializeField]
	private Transform aim;

	[SerializeField]
	private SpriteRenderer sprite;

	private int Health;

	private float Speed;

	private float followDelay;

	private float followTime;

	private float followDistance;

	private Vector2 target;

	private float currentHp;

	private AbstractPlayerController player;

	private float invincibleDuration;

	protected override float DestroyLifetime
	{
		get
		{
			return 10000000f;
		}
	}

	public FrogsLevelTallFirefly Create(Vector2 pos, Vector2 target, float speed, int hp, float followDelay, float followTime, float followDistance, float invincibleDuration, AbstractPlayerController player, int layer)
	{
		FrogsLevelTallFirefly frogsLevelTallFirefly = Create(pos) as FrogsLevelTallFirefly;
		frogsLevelTallFirefly.Health = hp;
		frogsLevelTallFirefly.Speed = speed;
		frogsLevelTallFirefly.DamagesType.OnlyPlayer();
		frogsLevelTallFirefly.CollisionDeath.OnlyPlayer();
		frogsLevelTallFirefly.CollisionDeath.PlayerProjectiles = true;
		frogsLevelTallFirefly.Init(pos, target, followDelay, followTime, followDistance, player, layer, invincibleDuration);
		frogsLevelTallFirefly.DestroyDistance = 10000000f;
		return frogsLevelTallFirefly;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Level.Current == null || !Level.Current.Started)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Init(Vector2 pos, Vector2 target, float delay, float followTime, float followDistance, AbstractPlayerController player, int layer, float invincibleDuration)
	{
		base.transform.position = pos;
		this.target = target;
		followDelay = delay;
		this.followTime = followTime;
		this.followDistance = followDistance;
		currentHp = Health;
		this.player = player;
		sprite.sortingOrder = layer;
		this.invincibleDuration = invincibleDuration;
		GetComponent<CircleCollider2D>().enabled = false;
		StartCoroutine(firefly_cr());
	}

	protected override void Start()
	{
		base.Start();
		base.damageDealer.SetDamageFlags(true, false, false);
		base.damageDealer.SetDamage(1f);
		base.damageDealer.SetDamageSource(DamageDealer.DamageSource.Enemy);
		base.damageDealer.SetRate(0.3f);
		DamageReceiver component = GetComponent<DamageReceiver>();
		component.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		currentHp -= info.damage;
		Die();
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(target, 20f);
	}

	protected override void Die()
	{
		if (!(currentHp > 0f))
		{
			StopAllCoroutines();
			GetComponent<Collider2D>().enabled = false;
			animator.SetTrigger("OnDeath");
			AudioManager.Play("level_frogs_tall_firefly_death");
			emitAudioFromObject.Add("level_frogs_tall_firefly_death");
			base.Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		currentHp = 0f;
		base.damageDealer.DealDamage(hit);
		Die();
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		currentHp = 0f;
		Die();
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		currentHp = 0f;
		Die();
	}

	private void SetMovementPose()
	{
		Vector2 vector = target - (Vector2)aim.transform.position;
		if (vector.x > 0f)
		{
			base.transform.SetScale(-1f);
		}
		else
		{
			base.transform.SetScale(1f);
		}
		if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y))
		{
			if (vector.y < 0f)
			{
				animator.SetTrigger("OnMoveDown");
			}
			else
			{
				animator.SetTrigger("OnMoveForward");
			}
		}
		else
		{
			animator.SetTrigger("OnMoveForward");
		}
	}

	private IEnumerator firefly_cr()
	{
		yield return StartCoroutine(initialMove_cr());
		while (true)
		{
			yield return StartCoroutine(follow_cr());
		}
	}

	private IEnumerator initialMove_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		base.transform.SetScale(1f);
		float falloffDistance = 200f;
		int t = 0;
		int falloffFrames = (int)(falloffDistance * 2f / (Speed / 60f));
		Vector3 direction = (Vector3)target - aim.transform.position;
		direction.Normalize();
		float speed = Speed;
		SetMovementPose();
		while (Vector2.Distance(base.transform.position, target) > falloffDistance)
		{
			base.transform.position += direction * speed * CupheadTime.FixedDelta;
			yield return wait;
		}
		while (t < falloffFrames)
		{
			if (PauseManager.state != PauseManager.State.Paused)
			{
				float value = (float)t / (float)falloffFrames;
				speed = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, Speed, 0f, value);
				base.transform.position += direction * speed * CupheadTime.FixedDelta;
				t++;
			}
			yield return wait;
		}
		animator.SetTrigger("OnIdle");
	}

	private IEnumerator follow_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		animator.SetTrigger("OnIdle");
		yield return CupheadTime.WaitForSeconds(this, followDelay);
		Vector2 start = base.transform.position;
		aim.LookAt2D(player.center);
		target = base.transform.position + aim.right * followDistance;
		SetMovementPose();
		float t = 0f;
		while (t < followTime)
		{
			float val = t / followTime;
			float x = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, start.x, target.x, val);
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, start.y, target.y, val), transform: base.transform, x: x);
			t += CupheadTime.FixedDelta;
			yield return wait;
		}
		base.transform.SetPosition(target.x, target.y);
		animator.SetTrigger("OnIdle");
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (invincibleDuration > 0f)
		{
			invincibleDuration -= CupheadTime.FixedDelta;
			if (invincibleDuration <= 0f)
			{
				GetComponent<CircleCollider2D>().enabled = true;
			}
		}
	}
}
