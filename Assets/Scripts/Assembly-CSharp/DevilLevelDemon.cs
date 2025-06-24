using System;
using System.Collections;
using UnityEngine;

public class DevilLevelDemon : AbstractCollidableObject
{
	private const string PeekVariationParameterName = "PeekVariation";

	private const string JumpOutParameterName = "JumpOut";

	private const string RunOutParameterName = "RunOut";

	private const string JumpOutStateName = "JumpOut";

	private const string EnemyLayerName = "Enemies";

	private const int PeekVariations = 3;

	private const float StartScale = 0.9f;

	private const float PillarScale = 0.8f;

	[SerializeField]
	private Collider2D collider2d;

	[SerializeField]
	private float frontWaitTime;

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private Color backgroundTint;

	[SerializeField]
	private PlatformingLevelGenericExplosion explosion;

	private DamageDealer damageDealer;

	private bool enteredScreen;

	private float frontDirection;

	private float speed;

	private float hp;

	private bool moving;

	private bool hasJumped;

	private DamageReceiver damageReceiver;

	private DevilLevelSittingDevil parent;

	public Vector3 JumpRoot { get; set; }

	public Vector3 RunRoot { get; set; }

	public Vector3 PillarDestination { get; set; }

	public Vector3 FrontSpawn { get; set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
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
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public DevilLevelDemon Create(Vector2 position, float direction, float speed, float hp, DevilLevelSittingDevil parent)
	{
		DevilLevelDemon devilLevelDemon = InstantiatePrefab<DevilLevelDemon>();
		devilLevelDemon.transform.position = position;
		devilLevelDemon.frontDirection = direction;
		devilLevelDemon.speed = speed;
		devilLevelDemon.hp = hp;
		devilLevelDemon.parent = parent;
		devilLevelDemon.transform.localScale = new Vector3(direction * 0.9f, 0.9f, 0.9f);
		devilLevelDemon.sprite.color = backgroundTint;
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			devilLevelDemon.animator.SetFloat("PeekVariation", (float)UnityEngine.Random.Range(0, 3) / 2f);
			devilLevelDemon.animator.SetTrigger("JumpOut");
			break;
		case 1:
			devilLevelDemon.animator.SetFloat("PeekVariation", (float)UnityEngine.Random.Range(0, 3) / 2f);
			devilLevelDemon.animator.SetTrigger("RunOut");
			break;
		default:
			devilLevelDemon.animator.Play("JumpOut");
			break;
		}
		return devilLevelDemon;
	}

	protected override void Start()
	{
		base.Start();
		DevilLevelSittingDevil devilLevelSittingDevil = parent;
		devilLevelSittingDevil.OnPhase1Death = (Action)Delegate.Combine(devilLevelSittingDevil.OnPhase1Death, new Action(Die));
	}

	public void PlaceForJump()
	{
		base.transform.position = JumpRoot;
		hasJumped = true;
	}

	public void StartMoving()
	{
		if (!moving)
		{
			StartCoroutine(demonMovement_cr());
			AudioManager.Play("devil_imp_spawn");
			emitAudioFromObject.Add("devil_imp_spawn");
		}
	}

	protected IEnumerator demonMovement_cr()
	{
		moving = true;
		if (!hasJumped)
		{
			base.transform.position = RunRoot;
		}
		Vector3 backDirection = (PillarDestination - base.transform.position).normalized;
		while (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 150f)))
		{
			base.transform.position += backDirection * speed * CupheadTime.Delta;
			float scaleDelta = 0.099999964f * (float)CupheadTime.Delta;
			base.transform.localScale -= new Vector3(frontDirection * scaleDelta, scaleDelta, scaleDelta);
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, frontWaitTime);
		base.transform.localScale = new Vector3(0f - frontDirection, 1f, 1f);
		base.transform.position = FrontSpawn;
		collider2d.enabled = true;
		sprite.sortingLayerName = "Enemies";
		sprite.sortingOrder = 0;
		sprite.color = Color.black;
		while (true)
		{
			base.transform.AddPosition(frontDirection * speed * (float)CupheadTime.Delta);
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 150f)))
			{
				enteredScreen = true;
			}
			else if (enteredScreen)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			yield return null;
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f)
		{
			Die();
		}
	}

	private void RemoveEvent()
	{
		DevilLevelSittingDevil devilLevelSittingDevil = parent;
		devilLevelSittingDevil.OnPhase1Death = (Action)Delegate.Remove(devilLevelSittingDevil.OnPhase1Death, new Action(Die));
	}

	protected override void OnDestroy()
	{
		RemoveEvent();
		base.OnDestroy();
	}

	private void Die()
	{
		AudioManager.Play("devil_imp_death");
		emitAudioFromObject.Add("devil_imp_death");
		explosion.Create(collider2d.bounds.center);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void ImpStepSFX()
	{
		AudioManager.Play("devil_imp_step");
		emitAudioFromObject.Add("devil_imp_step");
	}
}
