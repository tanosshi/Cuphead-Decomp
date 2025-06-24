using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelYellProjectile : AbstractProjectile
{
	public enum State
	{
		Slowing = 0,
		Stopped = 1,
		Tracking = 2
	}

	public float launchSpeed;

	public float stopTime;

	public float waitTime;

	public float attackEaseTime;

	private State state;

	private float trackSpeed;

	private AbstractPlayerController target;

	private Vector2 direction;

	public FlyingMermaidLevelYellProjectile Create(Vector2 pos, float trackSpeed, float angle, AbstractPlayerController target)
	{
		FlyingMermaidLevelYellProjectile flyingMermaidLevelYellProjectile = base.Create() as FlyingMermaidLevelYellProjectile;
		flyingMermaidLevelYellProjectile.trackSpeed = trackSpeed;
		flyingMermaidLevelYellProjectile.target = target;
		flyingMermaidLevelYellProjectile.direction = MathUtils.AngleToDirection(angle);
		flyingMermaidLevelYellProjectile.transform.position = pos;
		return flyingMermaidLevelYellProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		float speed = launchSpeed;
		float t = 0f;
		while (true)
		{
			t += CupheadTime.FixedDelta;
			switch (state)
			{
			case State.Slowing:
				if (t < stopTime)
				{
					speed = EaseUtils.EaseOutSine(launchSpeed, 0f, t / stopTime);
					break;
				}
				speed = 0f;
				state = State.Stopped;
				t = 0f;
				break;
			case State.Stopped:
				if (t >= waitTime)
				{
					state = State.Tracking;
					t = 0f;
					if (target == null || target.IsDead)
					{
						target = PlayerManager.GetNext();
					}
					if (target != null)
					{
						direction = (target.center - base.transform.position).normalized;
						base.transform.SetEulerAngles(0f, 0f, MathUtils.DirectionToAngle(direction) + 180f);
						animator.SetTrigger("Continue");
					}
				}
				break;
			case State.Tracking:
				speed = ((!(t < attackEaseTime)) ? trackSpeed : EaseUtils.EaseInSine(0f, trackSpeed, t / attackEaseTime));
				break;
			}
			Vector2 pos = base.transform.localPosition;
			pos += speed * CupheadTime.FixedDelta * direction;
			base.transform.localPosition = pos;
			yield return new WaitForFixedUpdate();
		}
	}
}
