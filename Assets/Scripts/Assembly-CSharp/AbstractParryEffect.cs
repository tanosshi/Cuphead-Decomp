using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractParryEffect : Effect
{
	public const string TAG = "Parry";

	private const float PAUSE_TIME = 0.185f;

	private const float COLLIDER_LIFETIME = 0.2f;

	private const float SPRITE_LIFETIME = 1f;

	[SerializeField]
	private GameObject sprites;

	[SerializeField]
	private Effect spark;

	[SerializeField]
	private ParryAttackSpark parryAttack;

	protected AbstractPlayerController player;

	protected bool didHitSomething;

	protected bool cancel;

	private List<AbstractProjectile> projectiles;

	private List<Effect> sparks;

	private List<ParrySwitch> switches;

	private List<AbstractLevelEntity> entities;

	private RaycastHit2D[] contactsBuffer = new RaycastHit2D[10];

	protected abstract bool IsHit { get; }

	public AbstractParryEffect Create(AbstractPlayerController player)
	{
		AbstractParryEffect abstractParryEffect = base.Create(player.center, player.transform.localScale) as AbstractParryEffect;
		abstractParryEffect.SetPlayer(player);
		return abstractParryEffect;
	}

	protected override void Initialize(Vector3 position, Vector3 scale, bool randomR)
	{
		base.Initialize(position, scale, randomR);
		animator.enabled = false;
		sprites.SetActive(false);
		projectiles = new List<AbstractProjectile>();
		sparks = new List<Effect>();
		switches = new List<ParrySwitch>();
		entities = new List<AbstractLevelEntity>();
		base.tag = "Parry";
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		if (cancel)
		{
			return;
		}
		base.OnCollision(hit, phase);
		if (phase != CollisionPhase.Enter)
		{
			return;
		}
		AbstractProjectile component = hit.GetComponent<AbstractProjectile>();
		if (component != null && component.CanParry)
		{
			projectiles.Add(component);
			sparks.Add(spark.Create(component.transform.position));
			if (!didHitSomething)
			{
				StartCoroutine(hit_cr());
			}
		}
		ParrySwitch component2 = hit.GetComponent<ParrySwitch>();
		if (component2 != null && component2.enabled)
		{
			switches.Add(component2);
			if (!didHitSomething)
			{
				StartCoroutine(hit_cr());
			}
		}
		AbstractLevelEntity component3 = hit.GetComponent<AbstractLevelEntity>();
		if (component3 != null && component3.enabled && component3.canParry)
		{
			entities.Add(component3);
			if (!didHitSomething)
			{
				StartCoroutine(hit_cr());
			}
		}
		if (player.stats.Loadout.charm != Charm.charm_parry_attack || didHitSomething)
		{
			return;
		}
		IParryAttack component4 = player.GetComponent<IParryAttack>();
		if (component4 == null || component4.AttackParryUsed)
		{
			return;
		}
		DamageReceiver damageReceiver = hit.GetComponent<DamageReceiver>();
		if (damageReceiver == null)
		{
			DamageReceiverChild component5 = hit.GetComponent<DamageReceiverChild>();
			if (component5 != null)
			{
				damageReceiver = component5.Receiver;
			}
		}
		if (damageReceiver != null && damageReceiver.type == DamageReceiver.Type.Enemy)
		{
			component4.HasHitEnemy = true;
			DamageDealer damageDealer = new DamageDealer(WeaponProperties.CharmParryAttack.damage, 0f, false, true, false);
			float num = damageDealer.DealDamage(hit);
			ShowParryAttackEffect(hit);
			StartCoroutine(hit_cr(true));
		}
	}

	private void ShowParryAttackEffect(GameObject hit)
	{
		Collider2D component = hit.GetComponent<Collider2D>();
		int num = Physics2D.RaycastNonAlloc(hit.transform.position, base.transform.position - hit.transform.position, contactsBuffer, (base.transform.position - hit.transform.position).magnitude);
		if (num == 0)
		{
			return;
		}
		Vector3 position = contactsBuffer[0].point;
		for (int i = 1; i < num; i++)
		{
			if (contactsBuffer[i].collider.tag == "Parry")
			{
				position = contactsBuffer[i].point;
			}
		}
		ParryAttackSpark parryAttackSpark = parryAttack.Create(position) as ParryAttackSpark;
		parryAttackSpark.IsCuphead = player.id == PlayerId.PlayerOne;
		sparks.Add(parryAttackSpark);
		parryAttackSpark.Play();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected virtual void SetPlayer(AbstractPlayerController player)
	{
		this.player = player;
		base.transform.SetParent(player.transform);
		StartCoroutine(lifetime_cr());
	}

	protected virtual void OnHitCancel()
	{
		if (!(this == null))
		{
			Cancel();
			AudioManager.Stop("player_parry");
		}
	}

	protected virtual void Cancel()
	{
		foreach (Effect spark in sparks)
		{
			Object.Destroy(spark.gameObject);
		}
		cancel = true;
		CancelSwitch();
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	protected virtual void CancelSwitch()
	{
	}

	protected virtual void OnPaused()
	{
	}

	protected virtual void OnUnpaused()
	{
	}

	protected virtual void OnSuccess()
	{
	}

	protected virtual void OnEnd()
	{
	}

	private IEnumerator lifetime_cr()
	{
		if (player != null && player.stats.Loadout.charm != Charm.charm_parry_plus)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
			GetComponent<Collider2D>().enabled = false;
			CancelSwitch();
			yield return CupheadTime.WaitForSeconds(this, 1f);
		}
		yield return null;
	}

	private IEnumerator hit_cr(bool hitEnemy = false)
	{
		bool hit = false;
		didHitSomething = true;
		IParryAttack parryController = player.GetComponent<IParryAttack>();
		if (parryController != null)
		{
			parryController.AttackParryUsed = true;
		}
		animator.enabled = true;
		sprites.SetActive(true);
		if (!hitEnemy)
		{
			foreach (ParrySwitch @switch in switches)
			{
				@switch.OnParryPrePause(player);
			}
			foreach (AbstractLevelEntity entity in entities)
			{
				entity.OnParry(player);
			}
			foreach (AbstractProjectile projectile in projectiles)
			{
				projectile.OnParry(player);
				player.stats.OnParry(projectile.ParryMeterMultiplier);
			}
		}
		PauseManager.Pause();
		OnPaused();
		float pauseTime = ((!hitEnemy) ? 0.185f : 0.13875f);
		float t = 0f;
		while (t < pauseTime)
		{
			hit = IsHit;
			if (hit)
			{
				t = pauseTime;
			}
			t += Time.fixedDeltaTime;
			for (int i = 0; i < 2; i++)
			{
				PlayerId playerId = ((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
				if (player != null && player.id == playerId)
				{
					if (pauseTime - t < 0.134f)
					{
						player.BufferInputs();
					}
					continue;
				}
				AbstractPlayerController abstractPlayerController = PlayerManager.GetPlayer(playerId);
				if (abstractPlayerController != null)
				{
					abstractPlayerController.BufferInputs();
				}
			}
			yield return new WaitForFixedUpdate();
		}
		if (hit)
		{
			yield break;
		}
		OnSuccess();
		PauseManager.Unpause();
		OnUnpaused();
		OnEnd();
		base.transform.parent = null;
		GetComponent<Collider2D>().enabled = false;
		if (hitEnemy)
		{
			yield break;
		}
		foreach (ParrySwitch switch2 in switches)
		{
			switch2.OnParryPostPause(player);
		}
	}
}
