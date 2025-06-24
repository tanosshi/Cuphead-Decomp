using System.Collections;
using UnityEngine;

public class FlyingBirdLevelBirdFeather : AbstractProjectile
{
	[SerializeField]
	private Effect effectPrefab;

	[SerializeField]
	private Transform effectRoot;

	public float Speed { get; private set; }

	public virtual AbstractProjectile Init(float speed)
	{
		Speed = speed;
		ResetLifetime();
		ResetDistance();
		return this;
	}

	protected override void Update()
	{
		base.Update();
		base.transform.position += -base.transform.right * Speed * CupheadTime.Delta;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		DamagesType.OnlyPlayer();
		CollisionDeath.OnlyPlayer();
		SetCollider(true);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SetCollider(false);
		StopAllCoroutines();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private void SetCollider(bool c)
	{
		GetComponent<BoxCollider2D>().enabled = c;
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
