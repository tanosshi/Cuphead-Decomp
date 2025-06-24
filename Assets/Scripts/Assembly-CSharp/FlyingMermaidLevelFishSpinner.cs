using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelFishSpinner : AbstractProjectile
{
	private LevelProperties.FlyingMermaid.SpinnerFish properties;

	private Vector2 direction;

	public FlyingMermaidLevelFishSpinner Create(Vector2 pos, Vector2 direction, LevelProperties.FlyingMermaid.SpinnerFish properties)
	{
		FlyingMermaidLevelFishSpinner flyingMermaidLevelFishSpinner = base.Create() as FlyingMermaidLevelFishSpinner;
		flyingMermaidLevelFishSpinner.properties = properties;
		flyingMermaidLevelFishSpinner.direction = direction;
		flyingMermaidLevelFishSpinner.transform.position = pos;
		return flyingMermaidLevelFishSpinner;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
		StartCoroutine(tails_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator tails_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.timeBeforeTails);
		animator.SetTrigger("StartTails");
	}

	private IEnumerator move_cr()
	{
		base.transform.SetEulerAngles(null, null, Random.Range(0, 360));
		Vector2 velocity = direction * properties.bulletSpeed;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			base.transform.Rotate(0f, 0f, properties.rotationSpeed * CupheadTime.FixedDelta);
			yield return wait;
		}
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
