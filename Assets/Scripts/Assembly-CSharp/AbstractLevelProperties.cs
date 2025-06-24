using System;
using UnityEngine;

public abstract class AbstractLevelProperties<STATE, PATTERN, STATE_NAMES> where STATE : AbstractLevelState<PATTERN, STATE_NAMES>
{
	public delegate void OnBossDamagedHandler(float damage);

	public readonly float TotalHealth;

	public readonly Level.GoalTimes goalTimes;

	private readonly STATE[] states;

	private int stateIndex;

	public float CurrentHealth { get; private set; }

	public STATE CurrentState
	{
		get
		{
			stateIndex = Mathf.Clamp(stateIndex, 0, states.Length - 1);
			return states[stateIndex];
		}
	}

	public event OnBossDamagedHandler OnBossDamaged;

	public event Action OnBossDeath;

	public event Action OnStateChange;

	public AbstractLevelProperties(float hp, Level.GoalTimes goalTimes, STATE[] states)
	{
		TotalHealth = hp;
		CurrentHealth = TotalHealth;
		this.goalTimes = goalTimes;
		this.states = states;
		stateIndex = 0;
	}

	public void DealDamage(float damage)
	{
		CurrentHealth -= damage;
		if (this.OnBossDamaged != null)
		{
			this.OnBossDamaged(damage);
		}
		if (CurrentHealth <= 0f)
		{
			WinInstantly();
			return;
		}
		int num = 0;
		for (int i = 0; i < states.Length; i++)
		{
			float num2 = CurrentHealth / TotalHealth;
			if (num2 < states[i].healthTrigger)
			{
				num = i;
			}
		}
		if (stateIndex != num)
		{
			stateIndex = num;
			if (this.OnStateChange != null)
			{
				this.OnStateChange();
			}
		}
	}

	public void DealDamageToNextNamedState()
	{
		string text = CurrentState.stateName.ToString();
		for (int i = 0; (float)i < TotalHealth; i++)
		{
			DealDamage(1f);
			if (CurrentState.stateName.ToString() != "Generic" && text != CurrentState.stateName.ToString())
			{
				break;
			}
		}
	}

	public void WinInstantly()
	{
		if (this.OnBossDeath != null)
		{
			this.OnBossDeath();
		}
		this.OnBossDeath = null;
		if (!Level.IsDicePalace || Level.IsDicePalaceMain)
		{
			return;
		}
		if (DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS == null)
		{
			DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
		}
		PlayerStatsManager stats = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;
		DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP = stats.Health;
		DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.Super = stats.SuperMeter;
		if (PlayerManager.Multiplayer)
		{
			if (DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS == null)
			{
				DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
			}
			PlayerStatsManager stats2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats;
			DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP = stats2.Health;
			DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.Super = stats2.SuperMeter;
		}
	}
}
