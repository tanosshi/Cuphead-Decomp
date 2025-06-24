using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelStarCannon : PlatformingLevelPathMovementEnemy
{
	[SerializeField]
	private Effect diagFX;

	[SerializeField]
	private Effect straightFX;

	[SerializeField]
	private bool killable;

	[SerializeField]
	private Transform[] diagRootPositions;

	[SerializeField]
	private Transform[] straightRootPositions;

	[SerializeField]
	private FunhousePlatformingLevelCannonProjectile projectile;

	private float offset = 50f;

	private bool justShot;

	private Vector3 direction = Vector3.zero;

	protected override void Start()
	{
		base.Start();
		if (killable)
		{
			base._damageReceiver.enabled = false;
		}
		base.animator.SetBool("IsA", Rand.Bool());
		StartCoroutine(check_to_start_cr());
	}

	protected override void OnStart()
	{
		base.OnStart();
		StartCoroutine(shoot_cr());
	}

	private IEnumerator check_to_start_cr()
	{
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset)
		{
			yield return null;
		}
		OnStart();
		yield return null;
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, base.Properties.cannonShotDelay);
			base.animator.SetBool("isShooting", true);
			while (!justShot)
			{
				yield return null;
			}
			justShot = false;
			yield return CupheadTime.WaitForSeconds(this, 0.7f);
			base.animator.SetBool("isShooting", false);
			yield return null;
		}
	}

	private void ShootStraight()
	{
		justShot = true;
		AudioManager.Play("funhouse_starcannon_shoot");
		emitAudioFromObject.Add("funhouse_starcannon_shoot");
		for (int i = 0; i < straightRootPositions.Length; i++)
		{
			FunhousePlatformingLevelCannonProjectile funhousePlatformingLevelCannonProjectile = projectile.Create(straightRootPositions[i].transform.position, 0f, base.Properties.cannonSpeed) as FunhousePlatformingLevelCannonProjectile;
			funhousePlatformingLevelCannonProjectile.direction = straightRootPositions[i].transform.rotation * Vector3.right;
			funhousePlatformingLevelCannonProjectile.Properties = base.Properties;
			funhousePlatformingLevelCannonProjectile.Init();
		}
	}

	private void ShootDiag()
	{
		justShot = true;
		AudioManager.Play("funhouse_starcannon_shoot");
		emitAudioFromObject.Add("funhouse_starcannon_shoot");
		for (int i = 0; i < diagRootPositions.Length; i++)
		{
			FunhousePlatformingLevelCannonProjectile funhousePlatformingLevelCannonProjectile = projectile.Create(diagRootPositions[i].transform.position, 0f, base.Properties.cannonSpeed) as FunhousePlatformingLevelCannonProjectile;
			funhousePlatformingLevelCannonProjectile.direction = diagRootPositions[i].transform.rotation * Vector3.right;
			funhousePlatformingLevelCannonProjectile.Properties = base.Properties;
			funhousePlatformingLevelCannonProjectile.Init();
		}
	}

	private void DiagFX()
	{
		diagFX.Create(base.transform.position);
	}

	private void StraightFX()
	{
		straightFX.Create(base.transform.position);
	}

	private void SoundCannonRotate()
	{
		AudioManager.Play("funhouse_starcannon_rotation");
		emitAudioFromObject.Add("funhouse_starcannon_rotation");
	}
}
