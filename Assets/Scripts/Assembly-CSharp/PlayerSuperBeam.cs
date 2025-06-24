using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuperBeam : AbstractPlayerSuper
{
	[Header("Effects")]
	[SerializeField]
	private Effect hitPrefab;

	private List<DamageReceiver> damageReceivers;

	protected override void Awake()
	{
		base.Awake();
		damageReceivers = new List<DamageReceiver>();
	}

	protected override void StartSuper()
	{
		base.StartSuper();
		AudioManager.Play("player_super_beam_start");
		StartCoroutine(super_cr());
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		DamageReceiver component = hit.GetComponent<DamageReceiver>();
		if (component != null && !damageReceivers.Contains(component))
		{
			damageReceivers.Add(component);
		}
	}

	private void OnDealDamage(float damage, DamageReceiver receiver, DamageDealer dealer)
	{
		Collider2D component = receiver.GetComponent<Collider2D>();
		Vector2 zero = Vector2.zero;
		Vector2 vector = Vector2.zero;
		if (component.GetType() == typeof(BoxCollider2D))
		{
			zero = (component as BoxCollider2D).size;
		}
		else
		{
			if (component.GetType() != typeof(CircleCollider2D))
			{
				return;
			}
			zero = Vector2.one * (component as CircleCollider2D).radius;
		}
		float x = receiver.transform.position.x + Random.Range((0f - zero.x) / 2f, zero.x / 2f);
		vector = new Vector2(x, base.transform.position.y + (float)Random.Range(-100, 100));
		hitPrefab.Create(vector);
	}

	private IEnumerator super_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Start");
		Fire();
		yield return CupheadTime.WaitForSeconds(this, WeaponProperties.LevelSuperBeam.time);
		base.animator.SetTrigger("OnEnd");
		AudioManager.Play("player_super_beam_end_ground");
		AudioManager.Stop("player_superbeam_firing_loop");
		EndSuper();
	}

	protected override void Fire()
	{
		base.Fire();
		AudioManager.Play("player_superbeam_firing_loop");
		AudioManager.Play("player_superbeam_milk_explosion");
		damageDealer = new DamageDealer(WeaponProperties.LevelSuperBeam.damage, WeaponProperties.LevelSuperBeam.damageRate, DamageDealer.DamageSource.Super, false, true, true);
		damageDealer.OnDealDamage += OnDealDamage;
		damageDealer.DamageMultiplier *= PlayerManager.DamageMultiplier;
		damageDealer.PlayerId = player.id;
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Super);
		meterScoreTracker.Add(damageDealer);
	}

	private void OnEndAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
