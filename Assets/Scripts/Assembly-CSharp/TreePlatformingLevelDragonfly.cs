using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TreePlatformingLevelDragonfly : PlatformingLevelBigEnemy
{
	[SerializeField]
	private LevelBossDeathExploder explosion;

	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private Transform projectileRoot;

	public GameObject platforms;

	private List<TreePlatformingLevelMosquito> mosquitos;

	private List<TreePlatformingLevelMosquito> currentMosquitos;

	private Vector3 startPos;

	private int delayIndex;

	private int aimIndex;

	private int cycleIndex;

	private bool isShooting;

	protected override void Start()
	{
		base.Start();
		LockDistance = 1550f;
		startPos = base.transform.position;
		aimIndex = Random.Range(0, base.Properties.dragonFlyAimString.Split(',').Length);
		delayIndex = Random.Range(0, base.Properties.dragonFlyAtkDelayString.Split(',').Length);
		LockDistance -= base.Properties.dragonFlyLockDistOffset;
		mosquitos = new List<TreePlatformingLevelMosquito>(platforms.GetComponentsInChildren<TreePlatformingLevelMosquito>());
		currentMosquitos = randomizeList(mosquitos);
		StartCoroutine(enter_cr());
	}

	protected override void Shoot()
	{
		if (!isShooting)
		{
			StartCoroutine(shoot_cr());
		}
	}

	private IEnumerator enter_cr()
	{
		base.transform.position = new Vector3(startPos.x + 800f, startPos.y);
		while (!bigEnemyCameraLock)
		{
			yield return null;
		}
		float t = 0f;
		float time = base.Properties.dragonFlyInitRiseTime;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(base.transform.position, startPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = startPos;
		GetComponent<Collider2D>().enabled = true;
		StartCoroutine(sine_cr());
	}

	private IEnumerator shoot_cr()
	{
		float t = 0f;
		float t2 = 0f;
		float angle = 0f;
		bool pickDir = false;
		Vector3 direction = Vector3.zero;
		isShooting = true;
		base.animator.SetTrigger("Shoot");
		yield return base.animator.WaitForAnimationToEnd(this, "Warning_Start");
		yield return CupheadTime.WaitForSeconds(this, base.Properties.dragonFlyWarningDuration);
		base.animator.SetTrigger("Continue");
		while (t < base.Properties.dragonFlyAttackDuration)
		{
			pickDir = false;
			while (t2 < base.Properties.dragonFlyProjectileDelay)
			{
				t2 += (float)CupheadTime.Delta;
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t2 = 0f;
			if (base.Properties.dragonFlyAimString.Split(',')[aimIndex][0] == 'R')
			{
				while (!pickDir)
				{
					if (currentMosquitos[cycleIndex].isActive)
					{
						direction = currentMosquitos[cycleIndex].transform.position - base.transform.position;
						currentMosquitos.RemoveAt(cycleIndex);
						if (currentMosquitos.Count > 0)
						{
							cycleIndex = (cycleIndex + 1) % currentMosquitos.Count;
						}
						else
						{
							currentMosquitos = randomizeList(mosquitos);
						}
						pickDir = true;
					}
					else
					{
						cycleIndex = (cycleIndex + 1) % currentMosquitos.Count;
						currentMosquitos = randomizeList(mosquitos);
					}
					yield return null;
				}
			}
			else if (base.Properties.dragonFlyAimString.Split(',')[aimIndex][0] == 'P' && _target.transform.position.x < base.transform.position.x)
			{
				direction = _target.transform.position - base.transform.position;
			}
			angle = MathUtils.DirectionToAngle(direction);
			projectile.Create(projectileRoot.transform.position, angle + 5f, base.Properties.dragonFlyProjectileSpeed);
			aimIndex = (aimIndex + 1) % base.Properties.dragonFlyAimString.Split(',').Length;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Attack_To_Idle");
		yield return CupheadTime.WaitForSeconds(this, float.Parse(base.Properties.dragonFlyAtkDelayString.Split(',')[delayIndex]));
		delayIndex = (delayIndex + 1) % base.Properties.dragonFlyAtkDelayString.Split(',').Length;
		isShooting = false;
		yield return null;
	}

	private List<TreePlatformingLevelMosquito> randomizeList(List<TreePlatformingLevelMosquito> platforms)
	{
		List<TreePlatformingLevelMosquito> list = new List<TreePlatformingLevelMosquito>();
		List<TreePlatformingLevelMosquito> list2 = new List<TreePlatformingLevelMosquito>();
		list2.AddRange(platforms);
		for (int i = 0; i < platforms.Count; i++)
		{
			int index = Random.Range(0, list2.Count);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		cycleIndex = 0;
		return list;
	}

	public IEnumerator sine_cr()
	{
		float time = 0.5f;
		float t = 0f;
		float val = 1f;
		while (true)
		{
			if (!isShooting && (float)CupheadTime.Delta != 0f)
			{
				t += (float)CupheadTime.Delta;
				float num = Mathf.Sin(t / time);
				base.transform.AddPosition(0f, num * val);
			}
			yield return null;
		}
	}

	protected override void Die()
	{
		if (!isDead)
		{
			StopAllCoroutines();
			isDead = true;
			GetComponent<Collider2D>().enabled = false;
			base.animator.Play("Death");
			AudioManager.Play("level_platform_dragonfly_death");
			emitAudioFromObject.Add("level_platform_dragonfly_death");
			explosion.StartExplosion();
			StartCoroutine(fall_cr());
		}
	}

	private IEnumerator fall_cr()
	{
		float velocity = 0f;
		float gravity = 1500f;
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		explosion.StopExplosions();
		while (base.transform.position.y > 0f - CupheadLevelCamera.Current.Height - 200f)
		{
			base.transform.AddPosition(0f, velocity * (float)CupheadTime.Delta);
			velocity -= gravity * (float)CupheadTime.Delta;
			yield return null;
		}
		_003CDie_003E__BaseCallProxy0();
		yield return null;
	}

	private void SoundDragonflyAttackWarning()
	{
		AudioManager.Play("level_platform_dragonfly_attack_warning");
		emitAudioFromObject.Add("level_platform_dragonfly_attack_warning");
	}

	private void SoundDragonflyAttackStart()
	{
		AudioManager.Play("level_platform_dragonfly_attack_start");
		emitAudioFromObject.Add("level_platform_dragonfly_attack_start");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
