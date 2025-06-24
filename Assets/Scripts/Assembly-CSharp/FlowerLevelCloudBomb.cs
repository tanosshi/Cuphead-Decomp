using System.Collections;
using UnityEngine;

public class FlowerLevelCloudBomb : AbstractCollidableObject
{
	private bool hasDetonated;

	private float speed;

	private float detonationDelay;

	private Vector3 playerPos;

	private DamageDealer damageDealer;

	public void OnCloudBombStart(Vector3 target, float s, float delay)
	{
		playerPos = target;
		base.transform.LookAt2D(playerPos);
		speed = s;
		detonationDelay = delay;
		hasDetonated = false;
		damageDealer = (damageDealer = DamageDealer.NewEnemy());
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!hasDetonated)
		{
			if ((playerPos - base.transform.position).magnitude > (base.transform.right * (speed * CupheadTime.FixedDelta)).magnitude)
			{
				base.transform.position += base.transform.right * (speed * CupheadTime.FixedDelta);
				return;
			}
			hasDetonated = true;
			StartCoroutine(explode_cr());
			base.transform.position += playerPos - base.transform.position;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator explode_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, detonationDelay);
		base.animator.SetTrigger("Explode");
		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		collider.size = GetComponent<SpriteRenderer>().bounds.size;
	}

	protected void Die()
	{
		AudioManager.Play("flower_minion_simple_deathpop_high");
		emitAudioFromObject.Add("flower_minion_simple_deathpop_high");
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
