using System.Collections;
using UnityEngine;

public class DicePalaceRabbitLevelMagic : AbstractProjectile
{
	private const float IdleSpeed = 10f;

	private new DamageDealer damageDealer;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private CircleCollider2D circleCollider;

	private IEnumerator idleRoutine;

	private Vector3 initialPosition;

	private bool StartMagicSFX;

	private bool StartMagicLaserSFX;

	public float AppearTime { get; set; }

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		initialPosition = base.transform.position;
		idleRoutine = wait_activation_cr();
		StartCoroutine(idleRoutine);
		StartCoroutine(FadeIn());
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		animator.SetBool("CanParry", parryable);
	}

	public void ActivateOrb()
	{
		circleCollider.enabled = true;
		Color color = spriteRenderer.color;
		color.a = 1f;
		spriteRenderer.color = color;
		StopCoroutine(idleRoutine);
		base.transform.position = initialPosition;
		animator.SetTrigger("Attack");
		if (!StartMagicLaserSFX)
		{
			AudioManager.Play("projectile_laser");
			emitAudioFromObject.Add("projectile_laser");
			StartMagicLaserSFX = true;
		}
	}

	public void Move(float startY, bool down, float speed)
	{
		StartCoroutine(move_cr(startY, down, speed));
	}

	private IEnumerator wait_activation_cr()
	{
		while (true)
		{
			Vector3 newDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0f).normalized * 10f;
			float progress = 0f;
			while (progress < 0.1f)
			{
				base.transform.position += newDirection * CupheadTime.Delta;
				progress += (float)CupheadTime.Delta;
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator move_cr(float startY, bool down, float speed)
	{
		StartMagicSFX = false;
		StartMagicLaserSFX = false;
		Vector3 velocity = speed * ((!down) ? Vector3.up : Vector3.down);
		float progress = 0f;
		while ((!down && startY + progress < 360f) || (down && startY + progress > -360f))
		{
			base.transform.position += velocity * CupheadTime.Delta;
			progress += velocity.y * (float)CupheadTime.Delta;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator FadeIn()
	{
		if (!StartMagicSFX)
		{
			AudioManager.Play("projectile_magic_start");
			emitAudioFromObject.Add("projectile_magic_start");
			StartMagicSFX = true;
		}
		while (spriteRenderer.color.a < 1f)
		{
			Color c = spriteRenderer.color;
			c.a += (float)CupheadTime.Delta / AppearTime;
			spriteRenderer.color = c;
			yield return null;
		}
	}

	public void SetSuit(int suit)
	{
		animator.SetInteger("Suit", suit);
	}

	public void IsOffset(bool offset)
	{
		animator.SetFloat("CycleOffset", (!offset) ? 0f : 0.5f);
	}
}
