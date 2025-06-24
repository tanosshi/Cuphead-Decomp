using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelArcade : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private Transform bulletSpawnA;

	[SerializeField]
	private Transform bulletSpawnB;

	[SerializeField]
	private Effect effect;

	[SerializeField]
	private Transform arcadeRoot;

	[SerializeField]
	private Transform introBullet;

	[SerializeField]
	private BasicProjectile bullet;

	[SerializeField]
	private LevelBossDeathExploder exploder;

	private float offset = 50f;

	private int attackCounter;

	private bool isAttacking;

	private bool goingRight;

	private Transform introBulletInstance;

	protected override void OnStart()
	{
		StartCoroutine(shoot_cr());
		goingRight = Rand.Bool();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(check_to_start_cr());
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
		yield return CupheadTime.WaitForSeconds(this, base.Properties.arcadeAttackDelayInit.RandomFloat());
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			base.animator.SetBool("IsAttacking", true);
			isAttacking = true;
			while (isAttacking)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, base.Properties.arcadeAttackDelay.RandomFloat());
			yield return null;
		}
	}

	private void Shoot()
	{
		goingRight = !goingRight;
		introBulletInstance = Object.Instantiate(introBullet);
		StartCoroutine(shoot_intro_cr());
		base.animator.SetBool("IsAttacking", false);
		attackCounter = 0;
		StartCoroutine(drop_cr());
	}

	private IEnumerator shoot_intro_cr()
	{
		while (introBulletInstance.position.y < (float)Level.Current.Ceiling + 100f)
		{
			introBulletInstance.position += Vector3.up * base.Properties.arcadeBulletSpeed * CupheadTime.Delta;
			yield return null;
		}
		Object.Destroy(introBulletInstance.gameObject);
	}

	private IEnumerator drop_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.Properties.arcadeBulletReturnDelay);
		AbstractPlayerController player = PlayerManager.GetNext();
		float sizeX = 100f;
		float posX = ((!goingRight) ? bulletSpawnB.transform.position.x : bulletSpawnA.transform.position.x);
		for (int i = 0; i < base.Properties.arcadeBulletCount; i++)
		{
			if (player == null)
			{
				player = PlayerManager.GetNext();
			}
			yield return null;
			bullet.Create(new Vector2((!goingRight) ? (posX - sizeX * (float)i) : (posX + sizeX * (float)i), CupheadLevelCamera.Current.Bounds.yMax + 50f), -90f, base.Properties.arcadeBulletSpeed);
			yield return CupheadTime.WaitForSeconds(this, base.Properties.arcadeBulletIndividualDelay);
		}
		isAttacking = false;
	}

	protected override void Die()
	{
		AudioManager.Play("circus_arcade_death");
		emitAudioFromObject.Add("circus_arcade_death");
		base.animator.Play("Death");
		effect.Create(base.transform.position);
		StopAllCoroutines();
		if (introBulletInstance != null)
		{
			Object.Destroy(introBulletInstance.gameObject);
		}
		StartCoroutine(Explosion_cr());
		GetComponent<Collider2D>().enabled = false;
	}

	private IEnumerator Explosion_cr()
	{
		exploder.StartExplosion();
		yield return new WaitForSeconds(2.5f);
		exploder.StopExplosions();
	}

	private void AttackSFX()
	{
		AudioManager.Play("circus_arcade_attack");
		emitAudioFromObject.Add("circus_arcade_attack");
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(bulletSpawnA.position, 50f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(bulletSpawnB.position, 50f);
	}
}
