using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusPlatformingLevelMagician : AbstractPlatformingLevelEnemy
{
	private const string EndAttackParameterName = "EndAttack";

	[SerializeField]
	private Transform startPos;

	[SerializeField]
	private Transform endPos;

	[SerializeField]
	private Transform spawnPointHolder;

	[SerializeField]
	private CircusPlatformingLevelMagicianBullet projectile;

	private List<Transform> spawnPoints;

	private bool attackTrigger;

	private bool disappearTrigger;

	private float t;

	private CircusPlatformingLevelMagicianBullet projectileInstance;

	protected override void Start()
	{
		base.Start();
		spawnPoints = new List<Transform>();
		spawnPoints.AddRange(spawnPointHolder.GetComponentsInChildren<Transform>());
		spawnPoints.RemoveAt(0);
		StartCoroutine(check_cr());
	}

	protected override void OnStart()
	{
	}

	private IEnumerator check_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AbstractPlayerController player = PlayerManager.GetNext();
		while (player.transform.position.x < startPos.transform.position.x)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			yield return null;
		}
		StartCoroutine(appear_cr());
		yield return null;
	}

	private IEnumerator appear_cr()
	{
		AbstractPlayerController player = PlayerManager.GetNext();
		yield return CupheadTime.WaitForSeconds(this, base.Properties.magicianAppearDelayRange.RandomFloat());
		while (true)
		{
			if (player.transform.position.x < startPos.transform.position.x || player.transform.position.x > endPos.transform.position.x)
			{
				yield return null;
				continue;
			}
			EnableMagician(true);
			while (!attackTrigger)
			{
				yield return null;
			}
			while (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(0f, 1000f)))
			{
				yield return null;
			}
			player = PlayerManager.GetFirst();
			Vector2 dir = player.transform.position - base.transform.position;
			projectileInstance = projectile.Create(base.transform.position, MathUtils.DirectionToAngle(dir), base.Properties.ProjectileSpeed) as CircusPlatformingLevelMagicianBullet;
			projectileInstance.OnProjectileDeath += OnProjectileDeath;
			while (!disappearTrigger)
			{
				yield return null;
			}
			disappearTrigger = false;
			attackTrigger = false;
			EnableMagician(false);
			while (t < base.Properties.magicianAppearDelayRange.RandomFloat())
			{
				t += CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
			yield return null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (projectileInstance != null)
		{
			projectileInstance.OnProjectileDeath -= OnProjectileDeath;
		}
	}

	private void OnProjectileDeath()
	{
		base.animator.SetTrigger("EndAttack");
	}

	public void Attack()
	{
		attackTrigger = true;
	}

	public void Disappear()
	{
		disappearTrigger = true;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.white;
		List<Transform> list = new List<Transform>();
		list.AddRange(spawnPointHolder.GetComponentsInChildren<Transform>());
		list.RemoveAt(0);
		for (int i = 0; i < list.Count; i++)
		{
			Gizmos.DrawWireSphere(list[i].transform.position, 50f);
		}
		Gizmos.DrawLine(new Vector2(startPos.transform.position.x, startPos.transform.position.y + 1000f), new Vector2(startPos.transform.position.x, startPos.transform.position.y - 1000f));
		Gizmos.DrawLine(new Vector2(endPos.transform.position.x, endPos.transform.position.y + 1000f), new Vector2(endPos.transform.position.x, endPos.transform.position.y - 1000f));
	}

	private void EnableMagician(bool enabled)
	{
		GetComponent<Animator>().enabled = enabled;
		GetComponent<Collider2D>().enabled = enabled;
		GetComponent<SpriteRenderer>().enabled = enabled;
		if (enabled)
		{
			base.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
		}
	}

	protected override void Die()
	{
		AudioManager.Play("circus_generic_death_big");
		emitAudioFromObject.Add("circus_generic_death_big");
		base.Die();
	}

	private void AttackAppearSFX()
	{
		AudioManager.Play("circus_magician_appears");
		emitAudioFromObject.Add("circus_magician_appears");
	}

	private void AttackIntroSFX()
	{
		AudioManager.Play("circus_magician_attack_intro");
		emitAudioFromObject.Add("circus_magician_attack_intro");
	}

	private void AttackOutroSFX()
	{
		AudioManager.Play("circus_magician_attack_outro");
		emitAudioFromObject.Add("circus_magician_attack_outro");
	}
}
