using System.Collections;
using UnityEngine;

public class HomingProjectile : AbstractProjectile
{
	private AbstractPlayerController player;

	private Vector2 launchVelocity;

	private float homingMoveSpeed;

	private float rotationSpeed;

	private float timeBeforeDeath;

	private float timeBeforeHoming;

	private float easeTime;

	private Vector2 homingDirection;

	[SerializeField]
	private bool trackGround;

	[SerializeField]
	private bool faceMoveDirection;

	[SerializeField]
	private float spriteRotation;

	protected Vector2 velocity;

	public bool HomingEnabled { get; set; }

	public HomingProjectile Create(Vector2 pos, float launchRotation, float launchSpeed, float homingMoveSpeed, float rotationSpeed, float timeBeforeDeath, float homingEaseTime, AbstractPlayerController player)
	{
		return Create(pos, launchRotation, launchSpeed, homingMoveSpeed, rotationSpeed, timeBeforeDeath, 0f, homingEaseTime, player);
	}

	public HomingProjectile Create(Vector2 pos, float launchRotation, float launchSpeed, float homingMoveSpeed, float rotationSpeed, float timeBeforeDeath, float timeBeforeHoming, float homingEaseTime, AbstractPlayerController player)
	{
		HomingProjectile homingProjectile = base.Create() as HomingProjectile;
		homingProjectile.homingDirection = MathUtils.AngleToDirection(launchRotation);
		homingProjectile.launchVelocity = MathUtils.AngleToDirection(launchRotation) * launchSpeed;
		homingProjectile.transform.position = pos;
		homingProjectile.player = player;
		homingProjectile.rotationSpeed = rotationSpeed;
		homingProjectile.homingMoveSpeed = homingMoveSpeed;
		homingProjectile.timeBeforeDeath = timeBeforeDeath;
		homingProjectile.timeBeforeHoming = timeBeforeHoming;
		homingProjectile.easeTime = homingEaseTime;
		homingProjectile.HomingEnabled = true;
		return homingProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator move_cr()
	{
		float t = 0f;
		while (t < timeBeforeDeath + easeTime + timeBeforeHoming)
		{
			while (!HomingEnabled)
			{
				yield return null;
			}
			t += CupheadTime.FixedDelta;
			if (player != null && !player.IsDead)
			{
				Vector3 center = player.center;
				if (trackGround)
				{
					center.y = Level.Current.Ground;
				}
				Vector2 direction = (center - base.transform.position).normalized;
				Quaternion b = Quaternion.Euler(0f, 0f, MathUtils.DirectionToAngle(direction));
				Quaternion a = Quaternion.Euler(0f, 0f, MathUtils.DirectionToAngle(homingDirection));
				homingDirection = MathUtils.AngleToDirection(Quaternion.Slerp(a, b, Mathf.Min(1f, CupheadTime.FixedDelta * rotationSpeed)).eulerAngles.z);
			}
			Vector2 homingVelocity = (velocity = homingDirection * homingMoveSpeed);
			if (t < timeBeforeHoming)
			{
				velocity = launchVelocity;
			}
			else if (t < timeBeforeHoming + easeTime)
			{
				float t2 = EaseUtils.EaseOutSine(0f, 1f, (t - timeBeforeHoming) / easeTime);
				velocity = Vector2.Lerp(launchVelocity, homingVelocity, t2);
			}
			if (faceMoveDirection)
			{
				base.transform.SetEulerAngles(0f, 0f, MathUtils.DirectionToAngle(velocity) + spriteRotation);
			}
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
		Die();
	}
}
