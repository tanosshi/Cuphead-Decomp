using System.Collections;
using UnityEngine;

public class BaronessLevelGumballProjectile : AbstractProjectile
{
	[SerializeField]
	private Effect trail;

	private LevelProperties.Baroness.Gumball properties;

	private Vector2 velocity;

	private float gravity;

	private bool isDead;

	public BaronessLevelGumballProjectile Create(Vector2 pos, Vector2 velocity, LevelProperties.Baroness.Gumball properties, float gravity)
	{
		BaronessLevelGumballProjectile baronessLevelGumballProjectile = base.Create() as BaronessLevelGumballProjectile;
		baronessLevelGumballProjectile.properties = properties;
		baronessLevelGumballProjectile.velocity = velocity;
		baronessLevelGumballProjectile.transform.position = pos;
		baronessLevelGumballProjectile.gravity = gravity;
		return baronessLevelGumballProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_trail_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		if (base.transform.position.y <= -360f)
		{
			Die();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!isDead)
		{
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			velocity.y -= gravity * CupheadTime.FixedDelta;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator spawn_trail_cr()
	{
		while (true)
		{
			yield return null;
			trail.Create(base.transform.position);
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
		}
	}

	protected override void Die()
	{
		StopAllCoroutines();
		isDead = true;
		base.Die();
		animator.SetTrigger("Death");
	}

	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}
}
