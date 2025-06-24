using System.Collections;
using UnityEngine;

public class DevilLevelDragonHead : AbstractCollidableObject
{
	public enum State
	{
		Idle = 0,
		Moving = 1,
		Stopped = 2
	}

	[SerializeField]
	private float speed;

	[SerializeField]
	private Transform leftRoot;

	[SerializeField]
	private Transform rightRoot;

	[SerializeField]
	private Transform children;

	public State state;

	private const float FRAME_RATE = 1f / 24f;

	private DamageDealer damageDealer;

	private Vector2 initialPos;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		CollisionChild[] componentsInChildren = children.GetComponentsInChildren<CollisionChild>();
		foreach (CollisionChild collisionChild in componentsInChildren)
		{
			collisionChild.OnPlayerCollision += OnCollisionPlayer;
		}
		initialPos = base.transform.position;
	}

	protected override void Update()
	{
		base.Update();
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

	public void Attack(DevilLevelSittingDevil parent, bool isLeft)
	{
		state = State.Moving;
		base.transform.SetScale(isLeft ? 1 : (-1));
		StartCoroutine(move_cr(parent, isLeft));
	}

	private IEnumerator move_cr(DevilLevelSittingDevil parent, bool isLeft)
	{
		float frameTime = 0f;
		YieldInstruction wait = new WaitForFixedUpdate();
		base.transform.position = ((!isLeft) ? rightRoot.position : leftRoot.position);
		Vector3 dir = ((!isLeft) ? Vector3.left : Vector3.right);
		yield return parent.animator.WaitForAnimationToEnd(this, "Morph_Start" + ((!isLeft) ? "_Right" : "_Left"));
		parent.animator.SetTrigger("OnDragonAttack");
		while (state == State.Moving)
		{
			base.transform.position += dir * (speed * CupheadTime.FixedDelta);
			yield return wait;
		}
		yield return parent.animator.WaitForAnimationToEnd(this, "Morph_Attack", 1);
		state = State.Idle;
	}
}
