using System;
using System.Collections;
using UnityEngine;

public class TrainLevelLollipopGhoulsManager : LevelProperties.Train.Entity
{
	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private TrainLevelLollipopGhoul ghoulLeft;

	[SerializeField]
	private TrainLevelLollipopGhoul ghoulRight;

	[Space(10f)]
	[SerializeField]
	private TrainLevelGhostCannons cannons;

	[Space(10f)]
	[SerializeField]
	private TrainLevelPassengerCar[] cars;

	private int deadCount;

	private int current;

	public event OnDamageTakenHandler OnDamageTakenEvent;

	public event Action OnDeathEvent;

	public void Setup()
	{
		cars[1].Explode(2);
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		ghoulLeft.LevelInit(properties);
		ghoulRight.LevelInit(properties);
		cannons.LevelInit(properties);
		ghoulLeft.OnDamageTakenEvent += OnDamageTaken;
		ghoulLeft.OnDeathEvent += OnDeath;
		ghoulRight.OnDamageTakenEvent += OnDamageTaken;
		ghoulRight.OnDeathEvent += OnDeath;
	}

	private void OnDeath()
	{
		deadCount++;
	}

	private void OnDamageTaken(float damage)
	{
		if (this.OnDamageTakenEvent != null)
		{
			this.OnDamageTakenEvent(damage);
		}
	}

	private IEnumerator start_cr()
	{
		AudioManager.Play(Sfx.Level_Train_Top_Explode);
		cars[0].Explode(0);
		cars[2].Explode(1);
		yield return null;
		ghoulLeft.AnimateIn();
		ghoulRight.AnimateIn();
		AudioManager.Play("train_lollipop_ghoul_intro");
		emitAudioFromObject.Add("train_lollipop_ghoul_intro");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.initDelay);
		StartCoroutine(ghouls_cr());
		StartCoroutine(cannons_cr());
	}

	public void StartGhouls()
	{
		StartCoroutine(start_cr());
	}

	private void EndGhouls()
	{
		Debug.Log("End Ghouls");
		StopAllCoroutines();
		cannons.End();
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
	}

	private TrainLevelLollipopGhoul NextGhoul()
	{
		if (deadCount > 1)
		{
			EndGhouls();
			return null;
		}
		if (ghoulRight.state == TrainLevelLollipopGhoul.State.Dead || ghoulRight.transform == null)
		{
			return ghoulLeft;
		}
		if (ghoulLeft.state == TrainLevelLollipopGhoul.State.Dead || ghoulLeft.transform == null)
		{
			return ghoulRight;
		}
		current = (int)Mathf.Repeat(current + 1, 2f);
		int num = current;
		if (num == 0 || num != 1)
		{
			return ghoulLeft;
		}
		return ghoulRight;
	}

	private IEnumerator ghouls_cr()
	{
		current = UnityEngine.Random.Range(0, 2);
		while (true)
		{
			TrainLevelLollipopGhoul ghoul = NextGhoul();
			yield return null;
			if (ghoul != null)
			{
				ghoul.Attack();
				while (ghoul.state == TrainLevelLollipopGhoul.State.Attacking)
				{
					yield return null;
				}
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.mainDelay);
			}
		}
	}

	private IEnumerator cannons_cr()
	{
		int cannon = UnityEngine.Random.Range(0, 3);
		int direction = 1;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.cannonDelay);
			cannons.Shoot(cannon);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			cannon += direction;
			if (cannon >= 3)
			{
				direction = -1;
				cannon = 1;
			}
			else if (cannon < 0)
			{
				cannon = 1;
				direction = 1;
			}
		}
	}
}
