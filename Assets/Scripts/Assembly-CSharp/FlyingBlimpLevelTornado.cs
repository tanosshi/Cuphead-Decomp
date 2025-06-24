using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelTornado : AbstractCollidableObject
{
	public enum State
	{
		Alive = 0,
		Dead = 1
	}

	private LevelProperties.FlyingBlimp.Tornado properties;

	private AbstractPlayerController player;

	private float movementSpeed;

	private DamageDealer damageDealer;

	public State state { get; private set; }

	public void Init(Vector2 pos, AbstractPlayerController player, LevelProperties.FlyingBlimp.Tornado properties)
	{
		base.transform.position = pos;
		this.player = player;
		this.properties = properties;
	}

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		state = State.Alive;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float t = 0f;
		while (base.transform.position.x >= -1280f)
		{
			t += CupheadTime.FixedDelta;
			Vector2 homingDirection = player.transform.position - base.transform.position;
			Vector2 homingVelocity = homingDirection * properties.homingSpeed;
			float velocity = homingVelocity.y;
			TransformExtensions.AddPosition(y: ((base.transform.position.x > player.transform.position.x) ? Mathf.Lerp(properties.moveSpeed, homingVelocity.y, 1f) : ((state == State.Dead) ? 0f : homingVelocity.y)) * CupheadTime.FixedDelta, transform: base.transform, x: (0f - properties.moveSpeed) * CupheadTime.FixedDelta);
			yield return wait;
		}
		Die();
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
