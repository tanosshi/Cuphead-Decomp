using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelFish : AbstractProjectile
{
	private LevelProperties.FlyingMermaid.Fish properties;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	public FlyingMermaidLevelFish Create(Vector2 pos, LevelProperties.FlyingMermaid.Fish properties)
	{
		FlyingMermaidLevelFish flyingMermaidLevelFish = base.Create() as FlyingMermaidLevelFish;
		flyingMermaidLevelFish.properties = properties;
		flyingMermaidLevelFish.transform.position = pos;
		flyingMermaidLevelFish.Init();
		return flyingMermaidLevelFish;
	}

	private void Init()
	{
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		float velocityY = properties.flyingUpSpeed;
		while (true)
		{
			base.transform.AddPosition((0f - properties.flyingSpeed) * (float)CupheadTime.Delta, velocityY * (float)CupheadTime.Delta);
			velocityY -= properties.flyingGravity * (float)CupheadTime.Delta;
			yield return null;
		}
	}
}
