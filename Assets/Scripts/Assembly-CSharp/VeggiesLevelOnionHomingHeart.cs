using System.Collections;
using UnityEngine;

public class VeggiesLevelOnionHomingHeart : AbstractProjectile
{
	public enum State
	{
		Alive = 0,
		Dead = 1
	}

	[SerializeField]
	private Effect deathPoof;

	[SerializeField]
	private SpriteDeathParts[] deathPieces;

	private AbstractPlayerController player;

	private bool isOnLeft;

	private SpriteRenderer sprite;

	private GroundHomingMovement homingMovement;

	private DamageReceiver damageReceiver;

	private new DamageDealer damageDealer;

	private float maxSpeed;

	private float acceletration;

	private float health;

	public State state { get; private set; }

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public VeggiesLevelOnionHomingHeart CreateRadish(Vector2 pos, float max, float acc, int hp, bool onLeft)
	{
		base.transform.position = pos;
		VeggiesLevelOnionHomingHeart veggiesLevelOnionHomingHeart = base.Create() as VeggiesLevelOnionHomingHeart;
		veggiesLevelOnionHomingHeart.maxSpeed = max;
		veggiesLevelOnionHomingHeart.acceletration = acc;
		veggiesLevelOnionHomingHeart.health = hp;
		veggiesLevelOnionHomingHeart.isOnLeft = onLeft;
		return veggiesLevelOnionHomingHeart;
	}

	protected override void Start()
	{
		if (!isOnLeft)
		{
			base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
		}
		base.Start();
		sprite = GetComponent<SpriteRenderer>();
		damageDealer = DamageDealer.NewEnemy();
		homingMovement = GetComponent<GroundHomingMovement>();
		homingMovement.maxSpeed = maxSpeed;
		homingMovement.acceleration = acceletration;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(start_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator start_cr()
	{
		state = State.Alive;
		sprite.sortingLayerName = SpriteLayer.Enemies.ToString();
		sprite.sortingOrder = 0;
		yield return animator.WaitForAnimationToEnd(this, "Radish_Intro");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		homingMovement.EnableHoming = true;
		StartCoroutine(loop_cr());
	}

	private void ChangeLayer()
	{
		sprite.sortingOrder = 3;
	}

	private IEnumerator loop_cr()
	{
		while (state != State.Dead)
		{
			homingMovement.TrackingPlayer = PlayerManager.GetNext();
			yield return CupheadTime.WaitForSeconds(this, 20f);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && state != State.Dead)
		{
			state = State.Dead;
			homingMovement.enabled = false;
			StopAllCoroutines();
			animator.SetTrigger("Dead");
		}
	}

	private void CreateEffect()
	{
		deathPoof.Create(base.transform.position);
	}

	private void CreatePieces()
	{
		SpriteDeathParts[] array = deathPieces;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
