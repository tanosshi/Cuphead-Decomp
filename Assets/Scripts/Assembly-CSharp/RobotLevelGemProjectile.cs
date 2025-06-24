using System.Collections;
using UnityEngine;

public class RobotLevelGemProjectile : AbstractProjectile
{
	private const string GemParameterName = "Gem";

	private const float SpeedVariation = 0.08f;

	private const float FadeTime = 0.3f;

	private const float FadeRate = 0.3f;

	[SerializeField]
	private Effect effectPrefab;

	[SerializeField]
	private Transform effectRoot;

	private Vector3 originalPosition;

	private Vector3 originalScale;

	private float minSpeed;

	private float maxSpeed;

	private float acceleration;

	private float waveLength;

	private float waveSpeedMultiplier;

	private float time;

	private float lifeTime;

	private bool isBlue;

	public float Speed { get; private set; }

	protected override float DestroyLifetime
	{
		get
		{
			return lifeTime;
		}
	}

	public virtual AbstractProjectile Init(MinMax speed, float acceleration, float waveLength, float waveSpeedMultiplier, float lifeTime, bool isBlue)
	{
		ResetLifetime();
		ResetDistance();
		float num = Random.Range(0.92f, 1.08f);
		minSpeed = speed.min * num;
		maxSpeed = speed.max * num;
		Speed = minSpeed;
		this.acceleration = acceleration;
		this.waveLength = waveLength;
		this.waveSpeedMultiplier = waveSpeedMultiplier;
		this.lifeTime = lifeTime;
		this.isBlue = isBlue;
		animator.SetFloat("Gem", isBlue ? 1 : 0);
		time = 0f;
		originalPosition = base.transform.position;
		animator.Play("Gem", 0, Random.value);
		StartCoroutine(speed_cr());
		StartCoroutine(fadeIn_cr());
		return this;
	}

	protected override void Update()
	{
		base.Update();
		originalPosition += -base.transform.right * Speed * CupheadTime.Delta;
		base.transform.position = originalPosition + Mathf.Sin(time * waveSpeedMultiplier) * waveLength * base.transform.up;
		time += CupheadTime.Delta;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private void SetCollider(bool c)
	{
		GetComponent<CircleCollider2D>().enabled = c;
	}

	private IEnumerator effect_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, 0.3f));
		while (true)
		{
			effectPrefab.Create(effectRoot.position);
			yield return CupheadTime.WaitForSeconds(this, 0.3f);
		}
	}

	private IEnumerator speed_cr()
	{
		Speed = minSpeed;
		while (Speed < maxSpeed)
		{
			Speed += acceleration;
			yield return null;
		}
	}

	private IEnumerator fadeIn_cr()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		while (sprite.color.a < 1f)
		{
			Color c = sprite.color;
			c.a += 1f * (float)CupheadTime.Delta;
			sprite.color = c;
			yield return null;
		}
		Color color = sprite.color;
		color.a = 1f;
		sprite.color = color;
	}

	public override void OnParryDie()
	{
		this.Recycle();
	}

	protected override void OnDieDistance()
	{
		this.Recycle();
	}

	protected override void OnDieLifetime()
	{
		this.Recycle();
	}

	protected override void OnDieAnimationComplete()
	{
		this.Recycle();
	}
}
