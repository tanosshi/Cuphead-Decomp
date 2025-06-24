using System.Collections;
using UnityEngine;

public class ClownLevelDogBalloon : AbstractProjectile
{
	public enum State
	{
		Spawned = 0,
		Unspawned = 1
	}

	public LevelProperties.Clown.HeliumClown properties;

	private AbstractPlayerController player;

	private Vector3 pointAtPlayer;

	private Vector3 normalized;

	private float health;

	private float pointAt;

	private float velocity;

	private float angle;

	private float waveSize = 5f;

	private DamageReceiver damageReceiver;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		AudioManager.Play("clown_dog_balloon_regular_intro");
		emitAudioFromObject.Add("clown_dog_balloon_regular_intro");
	}

	public void Init(float HP, Vector2 pos, float velocity, AbstractPlayerController player, LevelProperties.Clown.HeliumClown properties, bool flipped)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.player = player;
		this.velocity = velocity;
		health = HP;
		if (flipped)
		{
			base.transform.SetScale(1f, 0f - base.transform.localScale.y, 1f);
		}
		CalculateDirection();
		CalculateSin();
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		if (properties.dogDieOnGround && phase == CollisionPhase.Enter && state != State.Unspawned)
		{
			state = State.Unspawned;
			StopAllCoroutines();
			animator.SetTrigger("Death");
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && state != State.Unspawned)
		{
			state = State.Unspawned;
			StopAllCoroutines();
			animator.SetTrigger("Death");
		}
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void CalculateSin()
	{
		Vector2 zero = Vector2.zero;
		zero.x = (player.transform.position.x + base.transform.position.x) / 2f;
		zero.y = (player.transform.position.y + base.transform.position.y) / 2f;
		float num = 0f - (player.transform.position.x - base.transform.position.x) / (player.transform.position.y - base.transform.position.y);
		float num2 = zero.y - num * zero.x;
		Vector2 zero2 = Vector2.zero;
		zero2.x = zero.x + 1f;
		zero2.y = num * zero2.x + num2;
		normalized = Vector3.zero;
		normalized = zero2 - zero;
		normalized.Normalize();
	}

	private void CalculateDirection()
	{
		float x = player.transform.position.x - base.transform.position.x;
		float y = player.transform.position.y - base.transform.position.y;
		float value = Mathf.Atan2(y, x) * 57.29578f;
		pointAtPlayer = MathUtils.AngleToDirection(value);
		base.transform.SetEulerAngles(null, null, value);
	}

	private IEnumerator move_cr()
	{
		Vector3 pos = base.transform.position;
		yield return animator.WaitForAnimationToEnd(this, "Intro");
		while (base.transform.position.y > -560f)
		{
			angle += 10f * (float)CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				pos += normalized * Mathf.Sin(angle) * 2f;
			}
			pos += pointAtPlayer * velocity * CupheadTime.Delta;
			base.transform.position = pos;
			yield return null;
		}
		yield return null;
	}

	private void ChompSound()
	{
		AudioManager.Play("clown_dog_balloon_regular_chomp");
		emitAudioFromObject.Add("clown_dog_balloon_regular_chomp");
	}

	protected override void Die()
	{
		AudioManager.Play("clown_dog_balloon_regular_death");
		emitAudioFromObject.Add("clown_dog_balloon_regular_death");
		base.Die();
		StopAllCoroutines();
		GetComponent<SpriteRenderer>().enabled = false;
	}
}
