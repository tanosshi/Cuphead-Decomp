using System;
using System.Collections;
using UnityEngine;

public class PlayerSuperGhost : AbstractPlayerSuper
{
	public enum State
	{
		Intro = 0,
		Spinning = 1,
		Dying = 2
	}

	[SerializeField]
	private PlayerSuperGhostHeart heartPrefab;

	[SerializeField]
	private Transform heartRoot;

	[SerializeField]
	private SpriteRenderer cupheadBottom;

	[SerializeField]
	private SpriteRenderer mugmanBottom;

	private State state;

	private Vector2 velocity = Vector2.zero;

	private float t;

	private Trilean2 lookDir;

	private bool createHeart;

	protected override void StartSuper()
	{
		base.StartSuper();
		AudioManager.Play("player_super_ghost");
		if (!player.motor.Grounded)
		{
			cupheadBottom.enabled = false;
			mugmanBottom.enabled = false;
		}
		createHeart = true;
		StartCoroutine(super_cr());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (state != State.Spinning)
		{
			return;
		}
		t += CupheadTime.FixedDelta;
		Quaternion localRotation = base.transform.localRotation;
		float num = 0f;
		if (t < WeaponProperties.LevelSuperGhost.initialSpeedTime)
		{
			num = Mathf.Clamp01(t / WeaponProperties.LevelSuperGhost.accelerationTime) * WeaponProperties.LevelSuperGhost.initialSpeed;
		}
		else
		{
			float num2 = Mathf.Clamp01((t - WeaponProperties.LevelSuperGhost.initialSpeedTime) / WeaponProperties.LevelSuperGhost.accelerationTime);
			num = Mathf.Lerp(WeaponProperties.LevelSuperGhost.initialSpeed, WeaponProperties.LevelSuperGhost.maxSpeed, num2);
		}
		Trilean2 lookDirection = player.motor.LookDirection;
		if (player.motor.GravityReversed)
		{
			lookDirection.y = (int)lookDirection.y * -1;
		}
		if (player.IsDead || ((int)lookDirection.x == 0 && (int)lookDirection.y == 0))
		{
			lookDirection = lookDir;
		}
		lookDir = lookDirection;
		Vector2 normalized = new Vector2(lookDir.x, lookDir.y).normalized;
		velocity = Vector2.Lerp(b: new Vector2(normalized.x * num, normalized.y * num), a: velocity, t: CupheadTime.FixedDelta * WeaponProperties.LevelSuperGhost.turnaroundEaseMultiplier);
		base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		if ((int)lookDirection.x > 0)
		{
			if (localRotation.z < (float)Math.PI / 90f)
			{
				localRotation.z += 0.01f;
			}
		}
		else if (localRotation.z > -(float)Math.PI / 90f)
		{
			localRotation.z -= 0.01f;
		}
		base.transform.localRotation = localRotation;
		if (!CupheadLevelCamera.Current.ContainsPoint((Vector2)base.transform.position + new Vector2(0f, 150f * base.transform.localScale.y), new Vector2(200f, 200f)))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void EndPlayerAnimation()
	{
		Fire();
		EndSuper();
	}

	private IEnumerator super_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Start");
		AudioManager.Play(Sfx.Player_Super_Beam);
		state = State.Spinning;
		damageDealer = new DamageDealer(WeaponProperties.LevelSuperGhost.damage, WeaponProperties.LevelSuperGhost.damageRate, DamageDealer.DamageSource.Super, false, true, true);
		damageDealer.DamageMultiplier *= PlayerManager.DamageMultiplier;
		damageDealer.PlayerId = player.id;
		MeterScoreTracker tracker = new MeterScoreTracker(MeterScoreTracker.Type.Super);
		tracker.Add(damageDealer);
		lookDir = player.motor.TrueLookDirection;
		yield return CupheadTime.WaitForSeconds(this, WeaponProperties.LevelSuperGhost.initialSpeedTime);
		base.animator.SetTrigger("Continue");
		yield return CupheadTime.WaitForSeconds(this, (!createHeart) ? WeaponProperties.LevelSuperGhost.noHeartMaxSpeedTime : WeaponProperties.LevelSuperGhost.maxSpeedTime);
		state = State.Dying;
		base.animator.SetTrigger("Death");
	}

	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SpawnHeart()
	{
		if (createHeart)
		{
			heartPrefab.Create(heartRoot.position);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private void SoundSuperGhostVoice()
	{
		AudioManager.Play("player_super_ghost_voice");
		emitAudioFromObject.Add("player_super_ghost_voice");
	}
}
