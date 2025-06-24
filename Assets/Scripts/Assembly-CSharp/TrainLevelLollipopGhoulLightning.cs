using System.Collections;
using UnityEngine;

public class TrainLevelLollipopGhoulLightning : AbstractCollidableObject
{
	[SerializeField]
	private Transform spark1;

	[SerializeField]
	private Transform spark2;

	private DamageDealer damageDealer;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(start_cr());
		damageDealer = DamageDealer.NewEnemy(0.2f);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void End()
	{
		StartCoroutine(end_cr());
	}

	private void GoAway()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private IEnumerator start_cr()
	{
		base.animator.SetTrigger("OnStart");
		yield return base.animator.WaitForAnimationToStart(this, "Loop");
		base.animator.SetBool("isFX", true);
		base.animator.SetTrigger("OnDustStart");
	}

	private IEnumerator end_cr()
	{
		base.animator.SetTrigger("OnEnd");
		base.animator.SetBool("isFX", false);
		yield return base.animator.WaitForAnimationToStart(this, "Init");
		base.animator.SetTrigger("OnDustEnd");
		yield return base.animator.WaitForAnimationToStart(this, "Init", 2);
		GoAway();
	}
}
