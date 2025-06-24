using System.Collections;
using UnityEngine;

public class FlyingGenieLevelHelixProjectile : AbstractProjectile
{
	private LevelProperties.FlyingGenie.Coffin properties;

	private bool topOne;

	public FlyingGenieLevelHelixProjectile Create(Vector3 pos, LevelProperties.FlyingGenie.Coffin properties, bool topOne)
	{
		FlyingGenieLevelHelixProjectile flyingGenieLevelHelixProjectile = base.Create() as FlyingGenieLevelHelixProjectile;
		flyingGenieLevelHelixProjectile.properties = properties;
		flyingGenieLevelHelixProjectile.transform.position = pos;
		flyingGenieLevelHelixProjectile.topOne = topOne;
		return flyingGenieLevelHelixProjectile;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void Start()
	{
		base.Start();
		animator.SetBool("OnTop", topOne);
		StartCoroutine(moveY_cr());
	}

	private IEnumerator moveY_cr()
	{
		float angle = 0f;
		float xSpeed = properties.heartShotXSpeed;
		float ySpeed = properties.heartShotYSpeed;
		Vector3 moveX = base.transform.position;
		while (base.transform.position.x != -640f)
		{
			float loopSize;
			if (topOne)
			{
				loopSize = properties.heartLoopYSize;
				ySpeed = properties.heartShotYSpeed;
			}
			else
			{
				loopSize = 0f - properties.heartLoopYSize;
				ySpeed = 0f - properties.heartShotYSpeed;
			}
			angle += ySpeed * (float)CupheadTime.Delta;
			Vector3 moveY = new Vector3(0f, Mathf.Sin(angle + properties.heartLoopYSize) * (float)CupheadTime.Delta * 60f * loopSize / 2f);
			moveX = -base.transform.right * xSpeed * CupheadTime.Delta;
			base.transform.position += moveX + moveY;
			yield return null;
		}
		Die();
		yield return null;
	}
}
