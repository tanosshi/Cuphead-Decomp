using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePalaceEightBallLevelEightBall : LevelProperties.DicePalaceEightBall.Entity
{
	[SerializeField]
	private Effect attackEffect;

	[SerializeField]
	private Effect projectileEffect;

	[SerializeField]
	private Transform root;

	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private BasicProjectile pinkProjectile;

	[SerializeField]
	private List<DicePalaceEightBallLevelPoolBall> balls;

	private List<int> newList;

	private DamageReceiver damageReceiver;

	private int currentLoops;

	private int ballIndex;

	public Action OnEightBallDeath;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		List<int> list = new List<int>(balls.Count);
		newList = new List<int>();
		for (int i = 0; i < balls.Count; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < balls.Count; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			newList.Add(list[index]);
			list.RemoveAt(index);
		}
		ballIndex = 0;
	}

	public override void LevelInit(LevelProperties.DicePalaceEightBall properties)
	{
		base.LevelInit(properties);
		Level.Current.OnWinEvent += OnDeath;
		StartCoroutine(intro_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		AudioManager.Play("dice_palace_eight_ball_intro");
		emitAudioFromObject.Add("dice_palace_eight_ball_intro");
		yield return base.animator.WaitForAnimationToStart(this, "Right_Idle");
		StartCoroutine(shoot_bullet_cr());
		StartCoroutine(spawn_balls_cr());
		yield return null;
	}

	private void LoopCounter()
	{
		if (currentLoops < base.properties.CurrentState.general.idleLoopAmount)
		{
			currentLoops++;
			return;
		}
		base.animator.SetTrigger("Continue");
		currentLoops = 0;
	}

	private void HitLeftIdle()
	{
		base.animator.SetBool("MovingLeft", false);
	}

	private void HitRightIdle()
	{
		base.animator.SetBool("MovingLeft", true);
	}

	private IEnumerator shoot_bullet_cr()
	{
		LevelProperties.DicePalaceEightBall.General p = base.properties.CurrentState.general;
		string[] projectileType = p.shootString.GetRandom().Split(',');
		int projectileIndex = UnityEngine.Random.Range(0, projectileType.Length);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.shootDelay);
			base.animator.SetTrigger("OnAttack");
			yield return base.animator.WaitForAnimationToStart(this, "Attack_Start");
			AudioManager.Play("dice_palace_eight_ball_attack_start");
			emitAudioFromObject.Add("dice_palace_eight_ball_attack_start");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_Start");
			Effect effect = UnityEngine.Object.Instantiate(projectileEffect);
			effect.transform.position = root.transform.position;
			yield return effect.GetComponent<Animator>().WaitForAnimationToEnd(this, "Projectile");
			AbstractPlayerController player = PlayerManager.GetNext();
			Vector3 dir = player.transform.position - base.transform.position;
			AudioManager.Play("dice_palace_eight_ball_eight_attack_fire");
			emitAudioFromObject.Add("dice_palace_eight_ball_eight_attack_fire");
			if (projectileType[projectileIndex][0] == 'R')
			{
				attackEffect.Create(root.transform.position);
				projectile.Create(root.transform.position, MathUtils.DirectionToAngle(dir), base.properties.CurrentState.general.shootSpeed);
			}
			else if (projectileType[projectileIndex][0] == 'P')
			{
				attackEffect.Create(root.transform.position);
				pinkProjectile.Create(root.transform.position, MathUtils.DirectionToAngle(dir), base.properties.CurrentState.general.shootSpeed);
			}
			projectileIndex = (projectileIndex + 1) % projectileType.Length;
			yield return CupheadTime.WaitForSeconds(this, p.attackDuration);
			base.animator.SetTrigger("OnEnd");
			AudioManager.Play("dice_palace_eight_ball_attack_end");
			emitAudioFromObject.Add("dice_palace_eight_ball_attack_end");
			yield return null;
		}
	}

	private void IntroSFX()
	{
		AudioManager.Play("dice_palace_eight_ball_eight_intro");
		emitAudioFromObject.Add("dice_palace_eight_ball_eight_intro");
	}

	private IEnumerator spawn_balls_cr()
	{
		LevelProperties.DicePalaceEightBall.PoolBalls p = base.properties.CurrentState.poolBalls;
		string[] side = p.sideString.GetRandom().Split(',');
		float offset = GetComponent<Renderer>().bounds.size.x / 2f;
		int sideIndex = UnityEngine.Random.Range(0, side.Length);
		bool onLeft = false;
		Vector3 leftPos = new Vector3(-640f, 360f + offset, 0f);
		Vector3 rightPos = new Vector3(640f, 360f + offset, 0f);
		Vector3 pos = Vector3.zero;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.spawnDelay);
			DicePalaceEightBallLevelPoolBall ballInstance = null;
			if (side[sideIndex][0] == 'L')
			{
				onLeft = true;
				pos = leftPos;
			}
			else if (side[sideIndex][0] == 'R')
			{
				onLeft = false;
				pos = rightPos;
			}
			else
			{
				Debug.LogError("sideString pattern is wrong");
			}
			int index = newList[ballIndex];
			while (index < 0 || index > balls.Count)
			{
				ballIndex = (ballIndex + 1) % balls.Count;
				index = newList[ballIndex];
				yield return null;
			}
			switch (index)
			{
			case 0:
				ballInstance = balls[index].Create(pos, p.oneJumpHorizontalSpeed, p.oneJumpVerticalSpeed, p.oneJumpGravity, p.oneGroundDelay, onLeft, p, this);
				break;
			case 1:
				ballInstance = balls[index].Create(pos, p.twoJumpHorizontalSpeed, p.twoJumpVerticalSpeed, p.twoJumpGravity, p.twoGroundDelay, onLeft, p, this);
				break;
			case 2:
				ballInstance = balls[index].Create(pos, p.threeJumpHorizontalSpeed, p.threeJumpVerticalSpeed, p.threeJumpGravity, p.threeGroundDelay, onLeft, p, this);
				break;
			case 3:
				ballInstance = balls[index].Create(pos, p.fourJumpHorizontalSpeed, p.fourJumpVerticalSpeed, p.fourJumpGravity, p.fourGroundDelay, onLeft, p, this);
				break;
			case 4:
				ballInstance = balls[index].Create(pos, p.fiveJumpHorizontalSpeed, p.fiveJumpVerticalSpeed, p.fiveJumpGravity, p.fiveGroundDelay, onLeft, p, this);
				break;
			default:
				Debug.LogError("Invalid index");
				break;
			}
			if (ballInstance != null)
			{
				ballInstance.SetVariation(newList[ballIndex]);
			}
			ballIndex = (ballIndex + 1) % balls.Count;
			sideIndex = (sideIndex + 1) % side.Length;
		}
	}

	private void OnDeath()
	{
		if (OnEightBallDeath != null)
		{
			OnEightBallDeath();
		}
		StopAllCoroutines();
		AudioManager.PlayLoop("dice_palace_eight_ball_attack_death_loop");
		emitAudioFromObject.Add("dice_palace_eight_ball_attack_death_loop");
		base.animator.SetTrigger("OnDeath");
		GetComponent<Collider2D>().enabled = false;
	}
}
