using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusPlatformingLevelHotdog : AbstractPlatformingLevelEnemy
{
	private const string DeathParameterName = "Death";

	private const string Right = "R";

	[SerializeField]
	private Transform[] projectilesSpawnPoints;

	[SerializeField]
	private string spawnPatternString;

	[SerializeField]
	private string condimentPatternString;

	[SerializeField]
	private string sidePatternString;

	[SerializeField]
	private string shotDelayPatternString;

	[SerializeField]
	private float projectileDistance;

	[SerializeField]
	private LevelBossDeathExploder exploder;

	private string[] spawnPattern;

	private string[] condimentPattern;

	private string[] sidePattern;

	private string[] shotDelayPattern;

	private int spawnIndex;

	private int condimentIndex;

	private int sideIndex;

	private int shotDelayIndex;

	private int currentDelay;

	private List<CircusPlatformingLevelHotdogProjectile> projectileList = new List<CircusPlatformingLevelHotdogProjectile>();

	private bool projectilesCanHit;

	public bool ProjectilesCanHit
	{
		get
		{
			return projectilesCanHit;
		}
		set
		{
			projectilesCanHit = value;
			for (int i = 0; i < projectileList.Count; i++)
			{
				projectileList[i].EnableCollider(projectilesCanHit);
			}
			base.animator.Play("Dance");
		}
	}

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		base.Start();
		spawnPattern = spawnPatternString.Split(',');
		condimentPattern = condimentPatternString.Split(',');
		sidePattern = sidePatternString.Split(',');
		shotDelayPattern = shotDelayPatternString.Split(',');
		spawnIndex = Random.Range(0, spawnPattern.Length);
		condimentIndex = Random.Range(0, condimentPattern.Length);
		sideIndex = Random.Range(0, sidePattern.Length);
		shotDelayIndex = Random.Range(0, shotDelayPattern.Length);
		currentDelay = int.Parse(shotDelayPattern[shotDelayIndex]);
	}

	public void ShootProjectile()
	{
		currentDelay--;
		if (currentDelay <= 0)
		{
			shotDelayIndex = (shotDelayIndex + 1) % shotDelayPattern.Length;
			currentDelay = int.Parse(shotDelayPattern[shotDelayIndex]);
			string text = sidePattern[sideIndex];
			bool flag = text == "R";
			int num = int.Parse(spawnPattern[spawnIndex]);
			if (flag)
			{
				num += projectilesSpawnPoints.Length / 2;
			}
			AudioManager.Play("circus_hotdog_projectile_shoot");
			emitAudioFromObject.Add("circus_hotdog_projectile_shoot");
			CircusPlatformingLevelHotdogProjectile circusPlatformingLevelHotdogProjectile = base.Properties.ProjectilePrefab.Create(projectilesSpawnPoints[num].position) as CircusPlatformingLevelHotdogProjectile;
			circusPlatformingLevelHotdogProjectile.Speed = 0f - base.Properties.ProjectileSpeed;
			circusPlatformingLevelHotdogProjectile.SetCondiment(condimentPattern[condimentIndex]);
			circusPlatformingLevelHotdogProjectile.Side(flag);
			circusPlatformingLevelHotdogProjectile.DestroyDistance = projectileDistance;
			projectileList.Add(circusPlatformingLevelHotdogProjectile);
			circusPlatformingLevelHotdogProjectile.OnDestroyCallback += HotDogProjectileDie;
			circusPlatformingLevelHotdogProjectile.EnableCollider(projectilesCanHit);
			spawnIndex = (spawnIndex + 1) % spawnPattern.Length;
			condimentIndex = (condimentIndex + 1) % condimentPattern.Length;
			sideIndex = (sideIndex + 1) % sidePattern.Length;
		}
	}

	private void HotDogProjectileDie(CircusPlatformingLevelHotdogProjectile obj)
	{
		obj.OnDestroyCallback -= HotDogProjectileDie;
		projectileList.Remove(obj);
	}

	protected override void Die()
	{
		base.animator.SetTrigger("Death");
		StartCoroutine(Explosion_cr());
		GetComponent<BoxCollider2D>().enabled = false;
	}

	private IEnumerator Explosion_cr()
	{
		exploder.StartExplosion();
		yield return new WaitForSeconds(2.5f);
		exploder.StopExplosions();
	}

	public void DeathAnimationEnd()
	{
		Object.Destroy(base.gameObject);
	}

	private void HotDogDanceSFX()
	{
		AudioManager.Play("circus_hotdog_dance");
		emitAudioFromObject.Add("circus_hotdog_dance");
	}

	private void HotDogDeathSFX()
	{
		AudioManager.Stop("circus_hotdog_dance");
		AudioManager.Play("circus_hotdog_death");
		emitAudioFromObject.Add("circus_hotdog_death");
	}
}
