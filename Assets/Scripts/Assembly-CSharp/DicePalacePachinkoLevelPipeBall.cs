using System.Collections;
using UnityEngine;

public class DicePalacePachinkoLevelPipeBall : AbstractProjectile
{
	private bool onGround;

	private float speed;

	private int directionIndex;

	private LevelProperties.DicePalacePachinko properties;

	private new DamageDealer damageDealer;

	private bool bouncing;

	private Collider2D lastCollider;

	private Collider2D currentCollider;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (base.transform.position.y < (float)(Level.Current.Ground - 20))
		{
			Object.Destroy(base.gameObject);
		}
		base.Update();
	}

	public void InitBall(LevelProperties.DicePalacePachinko properties)
	{
		this.properties = properties;
		directionIndex = Random.Range(0, properties.CurrentState.balls.directionString.Split(',').Length);
		speed = properties.CurrentState.balls.movementSpeed;
		onGround = false;
		StartCoroutine(pick_dir_cr());
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			if (bouncing)
			{
				yield return null;
				continue;
			}
			if (onGround)
			{
				base.transform.localPosition += Vector3.right * speed * CupheadTime.Delta;
			}
			else
			{
				base.transform.localPosition += Vector3.down * speed * CupheadTime.Delta;
			}
			yield return null;
		}
	}

	private IEnumerator pick_dir_cr()
	{
		while (!onGround)
		{
			yield return null;
		}
		directionIndex++;
		ChangeDirection();
	}

	private void ChangeDirection()
	{
		if (directionIndex >= properties.CurrentState.balls.directionString.Split(',').Length)
		{
			directionIndex = 0;
		}
		switch (properties.CurrentState.balls.directionString.Split(',')[directionIndex][0])
		{
		case 'L':
			speed = 0f - properties.CurrentState.balls.movementSpeed;
			break;
		case 'R':
			speed = properties.CurrentState.balls.movementSpeed;
			break;
		}
	}

	private IEnumerator changeState_cr(bool grounded, bool forceDirection)
	{
		yield return null;
		ChangeDirection();
		if (grounded)
		{
			onGround = true;
			if (currentCollider == lastCollider)
			{
				yield break;
			}
			animator.SetTrigger("Bounce");
			bouncing = true;
			yield return null;
			yield return animator.WaitForAnimationToEnd(this, "Bounce", 1);
			lastCollider = currentCollider;
			Animator platformAnimnator = currentCollider.GetComponent<Animator>();
			if (platformAnimnator == null)
			{
				bouncing = false;
				yield break;
			}
			if (base.transform.position.x - currentCollider.transform.position.x > 0f)
			{
				platformAnimnator.SetTrigger("Right");
				speed = properties.CurrentState.balls.movementSpeed;
			}
			else
			{
				platformAnimnator.SetTrigger("Left");
				speed = 0f - properties.CurrentState.balls.movementSpeed;
			}
			base.transform.SetParent(platformAnimnator.transform, true);
			yield return null;
			bouncing = false;
			float finalSpeed = speed;
			float acceleration = 0f;
			while (onGround)
			{
				acceleration += (float)CupheadTime.Delta;
				speed = Mathf.Min(Mathf.Lerp(0f, finalSpeed, acceleration * 2f), finalSpeed);
				yield return null;
			}
			platformAnimnator.SetTrigger("Back");
			base.transform.SetParent(null, true);
			base.transform.rotation = Quaternion.identity;
		}
		else
		{
			onGround = false;
			speed = properties.CurrentState.balls.movementSpeed;
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (phase == CollisionPhase.Enter && !onGround)
		{
			currentCollider = hit.GetComponent<Collider2D>();
			if (hit.GetComponent<LevelPlatform>() != null)
			{
				StartCoroutine(changeState_cr(true, true));
				base.OnCollisionOther(hit, phase);
			}
			else if (hit.GetComponent<DicePalacePachinkoLevelPeg>() != null)
			{
				StartCoroutine(changeState_cr(true, hit.GetComponent<DicePalacePachinkoLevelPeg>().forceDirection));
				base.OnCollisionOther(hit, phase);
			}
		}
		else if (phase == CollisionPhase.Exit)
		{
			StartCoroutine(changeState_cr(false, false));
			base.OnCollisionOther(hit, phase);
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionWalls(hit, phase);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
