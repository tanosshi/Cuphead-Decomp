using System;
using System.Collections;
using UnityEngine;

public class DicePalaceEightBallLevelPoolBall : AbstractProjectile
{
	private const float OffsetY = 55f;

	[SerializeField]
	private GameObject shadowPrefab;

	[SerializeField]
	private GameObject dustPrefab;

	[SerializeField]
	private GameObject[] colorVariations;

	private LevelProperties.DicePalaceEightBall.PoolBalls properties;

	private DicePalaceEightBallLevelEightBall parent;

	private float horSpeed;

	private float verSpeed;

	private float gravity;

	private float delay;

	private bool onLeft;

	private Transform shadowInstance;

	private Transform dustInstance;

	public DicePalaceEightBallLevelPoolBall Create(Vector2 pos, float horSpeed, float verSpeed, float gravity, float delay, bool onLeft, LevelProperties.DicePalaceEightBall.PoolBalls properties, DicePalaceEightBallLevelEightBall parent)
	{
		DicePalaceEightBallLevelPoolBall dicePalaceEightBallLevelPoolBall = base.Create() as DicePalaceEightBallLevelPoolBall;
		dicePalaceEightBallLevelPoolBall.properties = properties;
		dicePalaceEightBallLevelPoolBall.transform.position = pos;
		dicePalaceEightBallLevelPoolBall.horSpeed = horSpeed;
		dicePalaceEightBallLevelPoolBall.verSpeed = verSpeed;
		dicePalaceEightBallLevelPoolBall.gravity = gravity;
		dicePalaceEightBallLevelPoolBall.delay = delay;
		dicePalaceEightBallLevelPoolBall.onLeft = onLeft;
		dicePalaceEightBallLevelPoolBall.parent = parent;
		return dicePalaceEightBallLevelPoolBall;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(jump_cr());
		StartCoroutine(check_dying_cr());
		DicePalaceEightBallLevelEightBall dicePalaceEightBallLevelEightBall = parent;
		dicePalaceEightBallLevelEightBall.OnEightBallDeath = (Action)Delegate.Combine(dicePalaceEightBallLevelEightBall.OnEightBallDeath, new Action(EightBallDead));
		shadowInstance = UnityEngine.Object.Instantiate(shadowPrefab).transform;
		shadowInstance.gameObject.SetActive(false);
		dustInstance = UnityEngine.Object.Instantiate(dustPrefab).transform;
		shadowInstance.gameObject.SetActive(false);
	}

	public void SetVariation(int index)
	{
		for (int i = 0; i < colorVariations.Length; i++)
		{
			colorVariations[i].SetActive(false);
		}
		if (index >= 0 && index < colorVariations.Length)
		{
			colorVariations[index].SetActive(true);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator jump_cr()
	{
		bool jumping = false;
		bool goingUp = false;
		bool upsideDown = false;
		float velocityY = verSpeed;
		float velocityX = horSpeed;
		float ground = (float)Level.Current.Ground + 55f;
		while (base.transform.position.y > ground)
		{
			velocityY -= gravity * (float)CupheadTime.Delta;
			base.transform.AddPosition(0f, velocityY * (float)CupheadTime.Delta);
			yield return null;
		}
		Vector3 p = base.transform.position;
		p.y = ground;
		base.transform.position = p;
		animator.SetTrigger("Smash");
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
			jumping = true;
			goingUp = true;
			velocityY = verSpeed;
			velocityX = ((!onLeft) ? (0f - horSpeed) : horSpeed);
			animator.SetTrigger("Jump");
			shadowInstance.gameObject.SetActive(false);
			if (upsideDown)
			{
				yield return animator.WaitForAnimationToEnd(this, "UpsideDownJump", true);
			}
			else
			{
				yield return animator.WaitForAnimationToEnd(this, "Jump", true);
			}
			shadowInstance.gameObject.SetActive(true);
			dustInstance.gameObject.SetActive(false);
			while (jumping)
			{
				shadowInstance.position = new Vector3(base.transform.position.x, ground, 0f);
				velocityY -= gravity * (float)CupheadTime.Delta;
				base.transform.AddPosition(velocityX * (float)CupheadTime.Delta, velocityY * (float)CupheadTime.Delta);
				if (velocityY < 0f && goingUp)
				{
					animator.SetTrigger("Turn");
					goingUp = false;
					if (upsideDown)
					{
						yield return animator.WaitForAnimationToEnd(this, "RightSideUpSmash_start", true);
					}
					else
					{
						yield return animator.WaitForAnimationToEnd(this, "JumpTurn", true);
					}
				}
				if (velocityY < 0f && jumping && base.transform.position.y <= ground)
				{
					animator.SetTrigger("Smash");
					jumping = false;
					upsideDown = !upsideDown;
					Vector3 position = base.transform.position;
					position.y = ground;
					base.transform.position = position;
					dustInstance.position = base.transform.position;
					dustInstance.gameObject.SetActive(true);
				}
				yield return null;
			}
		}
	}

	private IEnumerator check_dying_cr()
	{
		while (true)
		{
			if (onLeft)
			{
				if (base.transform.position.x > 840f)
				{
					break;
				}
			}
			else if (base.transform.position.x < -840f)
			{
				break;
			}
			yield return null;
		}
		Die();
		yield return null;
	}

	private void EightBallDead()
	{
		StopAllCoroutines();
		StartCoroutine(eight_ball_death_cr());
	}

	private IEnumerator eight_ball_death_cr()
	{
		float speed = 2500f;
		float angle = UnityEngine.Random.Range(0, 360);
		Vector3 dir = MathUtils.AngleToDirection(angle);
		GetComponent<Collider2D>().enabled = false;
		while (true)
		{
			base.transform.position += dir * speed * CupheadTime.FixedDelta;
			yield return null;
		}
	}

	protected override void Die()
	{
		base.Die();
		DicePalaceEightBallLevelEightBall dicePalaceEightBallLevelEightBall = parent;
		dicePalaceEightBallLevelEightBall.OnEightBallDeath = (Action)Delegate.Remove(dicePalaceEightBallLevelEightBall.OnEightBallDeath, new Action(EightBallDead));
	}
}
