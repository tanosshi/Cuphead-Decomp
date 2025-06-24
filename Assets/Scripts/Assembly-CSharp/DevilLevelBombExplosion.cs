using System.Collections;
using UnityEngine;

public class DevilLevelBombExplosion : Effect
{
	[SerializeField]
	private float loopTime;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(timer_cr());
		AudioManager.Play("bat_bomb_explo");
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

	private IEnumerator timer_cr()
	{
		yield return animator.WaitForAnimationToEnd(this, "Intro");
		yield return CupheadTime.WaitForSeconds(this, loopTime);
		animator.SetTrigger("Continue");
	}
}
