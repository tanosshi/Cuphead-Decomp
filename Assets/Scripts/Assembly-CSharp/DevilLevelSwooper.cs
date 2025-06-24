using System.Collections;
using UnityEngine;

public class DevilLevelSwooper : AbstractCollidableObject
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Swooping = 2,
		Returning = 3,
		Dying = 4
	}

	[SerializeField]
	private Effect[] explosions;

	private const float SPAWN_X_RATIO = 0.5f;

	public State state;

	private DevilLevelGiantHead parent;

	private LevelProperties.Devil.Swoopers properties;

	private DamageDealer damageDealer;

	private float hp;

	public bool finalSwooping;

	private float yPos;

	public DevilLevelSwooper Create(DevilLevelGiantHead parent, LevelProperties.Devil.Swoopers properties, Vector3 spawnPos, float xPos)
	{
		DevilLevelSwooper devilLevelSwooper = InstantiatePrefab<DevilLevelSwooper>();
		devilLevelSwooper.parent = parent;
		devilLevelSwooper.properties = properties;
		devilLevelSwooper.state = State.Intro;
		devilLevelSwooper.transform.position = spawnPos;
		devilLevelSwooper.yPos = properties.yIdlePos.RandomFloat();
		devilLevelSwooper.StartCoroutine(devilLevelSwooper.spawn_cr(xPos));
		return devilLevelSwooper;
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f && state != State.Dying)
		{
			Die();
		}
	}

	private IEnumerator spawn_cr(float xPos)
	{
		hp = properties.hp;
		base.transform.SetEulerAngles(0f, 0f, 0f);
		yield return base.animator.WaitForAnimationToEnd(this, "Spawn");
		while (base.transform.position.y < CupheadLevelCamera.Current.Bounds.yMax + 50f)
		{
			base.transform.position += Vector3.up * 200f * CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger("Continue");
		float t = 0f;
		Vector2 start = base.transform.position;
		Vector2 end = new Vector3(xPos, yPos);
		while (t < 2f)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / 2f);
			base.transform.position = Vector3.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(xPos, yPos);
		state = State.Idle;
	}

	public void Swoop()
	{
		state = State.Swooping;
		StartCoroutine(swoop_cr());
		AudioManager.Play("mini_devil_attack");
	}

	private IEnumerator swoop_cr()
	{
		float bestDistance = float.MaxValue;
		Vector2 bestVelocity = Vector2.zero;
		Vector3 target = PlayerManager.GetNext().center;
		Vector2 relativeTargetPos = target - base.transform.position;
		relativeTargetPos.x = Mathf.Abs(relativeTargetPos.x);
		if (target.x > base.transform.position.x)
		{
			base.animator.SetTrigger("OnTurn");
			yield return base.animator.WaitForAnimationToEnd(this, "Turn");
		}
		base.animator.SetBool("Spinning", true);
		AttackSFX();
		for (float num = 0f; num < 1f; num += 0.01f)
		{
			float angle = 0f - properties.launchAngle.GetFloatAt(num);
			float floatAt = properties.launchSpeed.GetFloatAt(num);
			Vector2 vector = MathUtils.AngleToDirection(angle) * floatAt;
			float num2 = relativeTargetPos.x / vector.x;
			float num3 = vector.y * num2 + 0.5f * properties.gravity * num2 * num2;
			float num4 = Mathf.Abs(relativeTargetPos.y - num3);
			float num5 = vector.y + properties.gravity * num2;
			if (!(num5 < 0f) && num4 < bestDistance)
			{
				bestDistance = num4;
				bestVelocity = vector;
			}
		}
		if (target.x < base.transform.position.x)
		{
			bestVelocity.x *= -1f;
		}
		Vector2 velocity = bestVelocity;
		while (base.transform.position.y < (float)(Level.Current.Ceiling + 150))
		{
			velocity.y += properties.gravity * CupheadTime.FixedDelta;
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
		state = State.Returning;
		TransformExtensions.SetPosition(x: parent.PutSwooperInSlot(this), transform: base.transform);
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		float moveTime = 1.5f;
		float t = 0f;
		while (t < moveTime)
		{
			base.transform.SetPosition(null, EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, Level.Current.Ceiling + 150, yPos, t / moveTime));
			t += CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
		state = State.Idle;
		base.transform.SetPosition(null, yPos);
		base.animator.SetBool("Spinning", false);
		AttackSFXEnd();
	}

	public void Die()
	{
		if (finalSwooping)
		{
			AudioManager.Stop("swooper_spin");
		}
		if (state != State.Dying)
		{
			state = State.Dying;
			StopAllCoroutines();
			parent.OnSwooperDeath(this);
			StartCoroutine(death_cr());
			AudioManager.Play("mini_devil_die");
			emitAudioFromObject.Add("mini_devil_die");
			AudioManager.Stop("swooper_spin");
		}
	}

	private IEnumerator death_cr()
	{
		while (state == State.Intro)
		{
			yield return null;
		}
		Effect[] array = explosions;
		foreach (Effect effect in array)
		{
			effect.Create(base.transform.position);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnTurn()
	{
		base.transform.SetScale(0f - base.transform.localScale.x);
	}

	private void AttackSFX()
	{
		AudioManager.PlayLoop("swooper_spin");
		emitAudioFromObject.Add("swooper_spin_end");
	}

	private void AttackSFXEnd()
	{
		if (finalSwooping)
		{
			AudioManager.Stop("swooper_spin");
		}
		AudioManager.Play("swooper_spin_end");
		emitAudioFromObject.Add("swooper_spin_end");
		finalSwooping = false;
	}
}
