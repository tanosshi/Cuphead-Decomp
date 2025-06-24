using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePalaceBoozeLevelMartini : DicePalaceBoozeLevelBossBase
{
	[SerializeField]
	private DicePalaceBoozeLevelOlive olive;

	[SerializeField]
	private Transform spawnPoint;

	private bool allActive;

	private int oliveIndex;

	private int pinkShotIndex;

	private int activeOlives;

	private List<DicePalaceBoozeLevelOlive> olives;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Awake()
	{
		olives = new List<DicePalaceBoozeLevelOlive>();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && !base.isDead)
		{
			StartDying();
			MartiniDeathSFX();
		}
	}

	public override void LevelInit(LevelProperties.DicePalaceBooze properties)
	{
		activeOlives = 0;
		pinkShotIndex = Random.Range(0, properties.CurrentState.martini.pinkString.Split(',').Length);
		int num = properties.CurrentState.martini.olivePositionStringX.Length;
		int num2 = Random.Range(0, num);
		int num3 = Random.Range(0, num);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(olive.gameObject, spawnPoint.position, Quaternion.identity);
			gameObject.GetComponent<DicePalaceBoozeLevelOlive>().InitOlive(properties, int.Parse(properties.CurrentState.martini.pinkString.Split(',')[pinkShotIndex]), properties.CurrentState.martini.olivePositionStringY[num3], properties.CurrentState.martini.olivePositionStringX[num2]);
			pinkShotIndex++;
			if (pinkShotIndex >= properties.CurrentState.martini.pinkString.Split(',').Length)
			{
				pinkShotIndex = 0;
			}
			num3++;
			if (num3 >= num)
			{
				num3 = 0;
			}
			num2++;
			if (num2 >= num)
			{
				num2 = 0;
			}
			gameObject.SetActive(false);
			olives.Add(gameObject.GetComponent<DicePalaceBoozeLevelOlive>());
		}
		Level.Current.OnIntroEvent += OnIntroEnd;
		Level.Current.OnWinEvent += HandleDead;
		AudioManager.Play("booze_martini_intro");
		emitAudioFromObject.Add("booze_martini_intro");
		base.LevelInit(properties);
		health = properties.CurrentState.martini.martiniHP;
	}

	private void OnIntroEnd()
	{
		StartCoroutine(attack_cr());
	}

	private IEnumerator attack_cr()
	{
		oliveIndex = 0;
		int counter = 0;
		while (true)
		{
			counter = 0;
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.martini.oliveSpawnDelay - DicePalaceBoozeLevelBossBase.ATTACK_DELAY);
			yield return null;
			while (olives[oliveIndex].gameObject.activeSelf)
			{
				oliveIndex = (oliveIndex + 1) % olives.Count;
				counter++;
				if (counter >= olives.Count)
				{
					allActive = true;
					break;
				}
				yield return null;
			}
			if (counter < olives.Count)
			{
				allActive = false;
			}
			if (!allActive)
			{
				base.animator.SetTrigger("OnAttack");
				yield return base.animator.WaitForAnimationToStart(this, "Attack");
				AudioManager.Play("booze_martini_attack");
				emitAudioFromObject.Add("booze_martini_attack");
				yield return base.animator.WaitForAnimationToEnd(this, "Attack");
			}
			yield return null;
		}
	}

	private void ShootOlive()
	{
		olives[oliveIndex].transform.position = spawnPoint.position;
		olives[oliveIndex].gameObject.SetActive(true);
		olives[oliveIndex].ResetOlive(int.Parse(base.properties.CurrentState.martini.pinkString.Split(',')[pinkShotIndex]));
		pinkShotIndex++;
		if (pinkShotIndex >= base.properties.CurrentState.martini.pinkString.Split(',').Length)
		{
			pinkShotIndex = 0;
		}
		oliveIndex = (oliveIndex + 1) % olives.Count;
	}

	private void OnOliveDeath()
	{
		activeOlives--;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	private void MartiniDeathSFX()
	{
		AudioManager.Play("martini_death_vox");
		emitAudioFromObject.Add("martini_death_vox");
	}
}
