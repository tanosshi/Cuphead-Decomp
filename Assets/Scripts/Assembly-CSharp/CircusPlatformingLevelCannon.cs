using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelCannon : AbstractPausableComponent
{
	private const string ShootParameterName = "Cannon";

	[SerializeField]
	private float health;

	[SerializeField]
	private DamageReceiver[] cannons;

	[SerializeField]
	private Transform[] shootRoots;

	[SerializeField]
	private CircusPlatformingLevelCannonProjectile projectile;

	[SerializeField]
	private float projectileSpeed;

	[SerializeField]
	private float projectileDelay;

	[SerializeField]
	private Transform startTrigger;

	[SerializeField]
	private string pinkString;

	private int shootIndex;

	private bool goingBackwards;

	private bool isDead;

	private string[] pinkSplits;

	private int pinkIndex;

	protected override void Start()
	{
		base.Start();
		goingBackwards = Rand.Bool();
		shootIndex = Random.Range(0, shootRoots.Length);
		pinkSplits = pinkString.Split(',');
		pinkIndex = Random.Range(0, pinkSplits.Length);
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		DamageReceiver[] array = cannons;
		foreach (DamageReceiver damageReceiver in array)
		{
			damageReceiver.OnDamageTaken += OnDamageTaken;
		}
		StartCoroutine(shoot_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && !isDead)
		{
			isDead = true;
			StopAllCoroutines();
			StartCoroutine(slide_off_cr());
		}
	}

	private IEnumerator shoot_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		while (true)
		{
			if (PlayerManager.GetNext().transform.position.x < startTrigger.transform.position.x)
			{
				yield return null;
				continue;
			}
			base.animator.SetInteger("Cannon", shootIndex + 1);
			yield return CupheadTime.WaitForSeconds(this, projectileDelay);
			yield return null;
		}
	}

	private void Shoot()
	{
		CircusPlatformingLevelCannonProjectile circusPlatformingLevelCannonProjectile = projectile.Create(shootRoots[shootIndex].transform.position, 0f, 0f - projectileSpeed) as CircusPlatformingLevelCannonProjectile;
		circusPlatformingLevelCannonProjectile.SetColor(pinkSplits[pinkIndex]);
		circusPlatformingLevelCannonProjectile.DestroyDistance = 0f;
		pinkIndex = (pinkIndex + 1) % pinkSplits.Length;
		if (goingBackwards)
		{
			if (shootIndex > 0)
			{
				shootIndex--;
			}
			else
			{
				shootIndex = shootRoots.Length - 1;
			}
		}
		else
		{
			shootIndex = (shootIndex + 1) % shootRoots.Length;
		}
		base.animator.SetInteger("Cannon", 0);
	}

	private IEnumerator slide_off_cr()
	{
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		base.animator.SetTrigger("Droop");
		float slideOffSpeed = 500f;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.y < 1220f)
		{
			base.transform.AddPosition(0f, slideOffSpeed * CupheadTime.FixedDelta);
			yield return wait;
		}
		GetComponent<LevelBossDeathExploder>().StopExplosions();
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawLine(new Vector2(startTrigger.transform.position.x, startTrigger.transform.position.y - 1000f), new Vector2(startTrigger.transform.position.x, startTrigger.transform.position.y + 1000f));
	}

	private void ShootSFX()
	{
		AudioManager.Play("circus_cannon_shoot");
		emitAudioFromObject.Add("circus_cannon_shoot");
	}

	private void DroopSFX()
	{
		AudioManager.Play("circus_cannon_droop");
		emitAudioFromObject.Add("circus_cannon_droop");
	}
}
