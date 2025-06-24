using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMermaidLevelSkullBubble : BasicSineProjectile
{
	[SerializeField]
	private Effect smallBubblesPrefab;

	[SerializeField]
	private float smallBubblesSize;

	private const float GRAVITY = -25f;

	private const float bubblesOffsetX = 15f;

	private const float bubblesOffsetY = 15f;

	private List<Effect> smallBubbles;

	private LevelProperties.FlyingMermaid.Bubbles properties;

	private float accumulatedGravity;

	private bool isDead;

	public FlyingMermaidLevelSkullBubble Create(Vector2 pos, float velocity, float sinVelocity, float sinSize, float rotation, LevelProperties.FlyingMermaid.Bubbles properties)
	{
		FlyingMermaidLevelSkullBubble flyingMermaidLevelSkullBubble = Create(pos, rotation, velocity, sinVelocity, sinSize) as FlyingMermaidLevelSkullBubble;
		flyingMermaidLevelSkullBubble.properties = properties;
		return flyingMermaidLevelSkullBubble;
	}

	protected override void Awake()
	{
		base.Awake();
		smallBubbles = new List<Effect>();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_small_bubbles_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (!isDead)
		{
			base.damageDealer.DealDamage(hit);
			isDead = true;
			StopAllCoroutines();
			StartCoroutine(dying_cr());
		}
	}

	private IEnumerator spawn_small_bubbles_cr()
	{
		while (!isDead)
		{
			Vector3 offset = new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0f);
			smallBubbles.Add(smallBubblesPrefab.Create(base.transform.position + offset, new Vector3(smallBubblesSize, smallBubblesSize, smallBubblesSize)));
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
			yield return null;
		}
		yield return null;
	}

	protected override void Die()
	{
		base.Die();
	}

	private IEnumerator dying_cr()
	{
		while (base.transform.position.y > -660f)
		{
			base.transform.AddPosition(0f, (0f - velocity + accumulatedGravity) * (float)CupheadTime.Delta);
			accumulatedGravity += -25f;
			yield return null;
		}
		Die();
		yield return null;
	}
}
