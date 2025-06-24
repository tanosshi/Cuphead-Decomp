using UnityEngine;

public class CircusPlatformingLevelBallRunnerBall : AbstractCollidableObject
{
	public bool isMoving;

	[SerializeField]
	private float Speed = 500f;

	[SerializeField]
	private CircusPlatformingLevelBallRunner runner;

	private DamageDealer damageDealer;

	public Vector3 direction = Vector3.right;

	protected override void Start()
	{
		base.animator.SetBool("isBlue", Rand.Bool());
		damageDealer = DamageDealer.NewEnemy();
		base.Start();
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionWalls(hit, phase);
		if ((bool)hit.GetComponentInParent<PlatformingLevelEditorPlatform>() && phase == CollisionPhase.Enter && isMoving)
		{
			direction *= -1f;
		}
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
		damageDealer.DealDamage(hit);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (isMoving)
		{
			Move();
		}
	}

	protected virtual void Move()
	{
		base.transform.position += direction * Speed * CupheadTime.FixedDelta;
	}
}
