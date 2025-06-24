using System.Collections;
using UnityEngine;

public class PlayerStatsManager : AbstractPlayerComponent
{
	public enum PlayerState
	{
		Ready = 0,
		Super = 1
	}

	public delegate void OnPlayerHealthChangeHandler(int health, PlayerId playerId);

	public delegate void OnPlayerSuperChangedHandler(float super, PlayerId playerId, bool playEffect);

	public delegate void OnPlayerWeaponChangedHandler(Weapon weapon);

	public delegate void OnPlayerDeathHandler(PlayerId playerId);

	public delegate void OnStoneHandler();

	private const int HEALTH_MAX = 3;

	private const float TIME_HIT = 2f;

	private const float TIME_REVIVED = 3f;

	private const int SUPER_MAX = 50;

	private const float SUPER_ON_PARRY = 10f;

	private const float SUPER_ON_DEAL_DAMAGE = 0.0625f;

	private const float EX_COST = 10f;

	private float timeSinceStoned = 1000f;

	private bool hardInvincibility;

	private IEnumerator superBuilderRoutine;

	private const float STONE_REDUCTION = 0.1f;

	private Trilean2 lastMoveDir;

	private Trilean2 currentMoveDir;

	public static bool GlobalInvincibility { get; private set; }

	public int HealthMax { get; private set; }

	public int Health { get; private set; }

	public float SuperMeterMax { get; private set; }

	public float SuperMeter { get; private set; }

	public bool SuperInvincible { get; private set; }

	public float ExCost { get; private set; }

	public int Deaths { get; private set; }

	public int ParriesThisJump { get; private set; }

	public float StoneTime { get; private set; }

	public bool CanUseEx
	{
		get
		{
			return SuperMeter >= ExCost && !SuperInvincible;
		}
	}

	public PlayerData.PlayerLoadouts.PlayerLoadout Loadout { get; private set; }

	public PlayerState State { get; private set; }

	public bool DiceGameBonusHP { get; set; }

	public bool PartnerCanSteal
	{
		get
		{
			return Health > 1;
		}
	}

	public static bool DebugInvincible { get; private set; }

	public event OnPlayerHealthChangeHandler OnHealthChangedEvent;

	public event OnPlayerSuperChangedHandler OnSuperChangedEvent;

	public event OnPlayerWeaponChangedHandler OnWeaponChangedEvent;

	public event OnPlayerDeathHandler OnPlayerDeathEvent;

	public event OnPlayerDeathHandler OnPlayerReviveEvent;

	public event OnStoneHandler OnStoneShake;

	public event OnStoneHandler OnStoned;

	protected override void OnAwake()
	{
		base.OnAwake();
		GlobalInvincibility = false;
		DebugInvincible = false;
		SuperInvincible = false;
		base.basePlayer.damageReceiver.OnDamageTaken += OnDamageTaken;
		LevelPlayerWeaponManager component = GetComponent<LevelPlayerWeaponManager>();
		if (component != null)
		{
			component.OnWeaponChangeEvent += OnWeaponChange;
			component.OnSuperEnd += OnSuperEnd;
		}
		PlanePlayerWeaponManager component2 = GetComponent<PlanePlayerWeaponManager>();
		if (component2 != null)
		{
			component2.OnWeaponChangeEvent += OnWeaponChange;
		}
		Deaths = 0;
		hardInvincibility = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (superBuilderRoutine != null)
		{
			StopCoroutine(superBuilderRoutine);
		}
		superBuilderRoutine = charmSuperBuilder_cr();
		StartCoroutine(superBuilderRoutine);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateStone();
	}

	public void LevelInit()
	{
		Level.Current.OnWinEvent += OnWin;
		Level.Current.OnLoseEvent += OnLose;
		Loadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.basePlayer.id);
		ExCost = 10f;
		SuperMeterMax = 50f;
		CalculateHealthMax();
		DicePalaceMainLevelGameInfo.PlayerStats playerStats = ((base.basePlayer.id != PlayerId.PlayerOne) ? DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS : DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS);
		if ((!Level.IsDicePalace && !Level.IsDicePalaceMain) || playerStats == null)
		{
			Health = HealthMax;
			SuperMeter = 0f;
		}
		else
		{
			Health = playerStats.HP;
			SuperMeter = playerStats.Super;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Level.Current != null)
		{
			Level.Current.OnWinEvent -= OnWin;
			Level.Current.OnLoseEvent -= OnLose;
		}
	}

	private void CalculateHealthMax()
	{
		HealthMax = 3;
		if (Loadout.charm == Charm.charm_health_up_1)
		{
			HealthMax += WeaponProperties.CharmHealthUpOne.healthIncrease;
		}
		if (Loadout.charm == Charm.charm_health_up_2)
		{
			HealthMax += WeaponProperties.CharmHealthUpTwo.healthIncrease;
		}
		DicePalaceMainLevelGameInfo.PlayerStats playerStats = ((base.basePlayer.id != PlayerId.PlayerOne) ? DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS : DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS);
		if ((Level.IsDicePalace || Level.IsDicePalaceMain) && playerStats != null)
		{
			HealthMax += playerStats.BonusHP;
		}
	}

	private void OnWin()
	{
		Debug.Log("STATS: PLAYER WON");
		GlobalInvincibility = true;
	}

	private void OnLose()
	{
		GlobalInvincibility = true;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (SuperInvincible)
		{
			return;
		}
		if (Loadout.charm == Charm.charm_pit_saver)
		{
			if (info.damageSource == DamageDealer.DamageSource.Pit)
			{
				return;
			}
			SuperMeter += WeaponProperties.CharmPitSaver.meterAmount;
			OnSuperChanged();
		}
		if (info.stoneTime > 0f)
		{
			GetStoned(info.stoneTime);
		}
		if (info.damage > 0f)
		{
			TakeDamage();
		}
	}

	public void GetStoned(float time)
	{
		if (time > 0f && StoneTime <= 0f && timeSinceStoned > 1f)
		{
			StoneTime = time;
			timeSinceStoned = 0f;
			this.OnStoned();
		}
	}

	private void TakeDamage()
	{
		if (SuperInvincible || hardInvincibility || Level.Current.Ending || State != PlayerState.Ready)
		{
			return;
		}
		if (StoneTime > 0f)
		{
			StoneTime = 0f;
		}
		if (!GlobalInvincibility && !DebugInvincible)
		{
			Health--;
			DicePalaceMainLevelGameInfo.PlayerStats playerStats = ((base.basePlayer.id != PlayerId.PlayerOne) ? DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS : DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS);
			if ((Level.IsDicePalace || Level.IsDicePalaceMain) && playerStats != null)
			{
				playerStats.LoseBonusHP();
				CalculateHealthMax();
			}
			OnHealthChanged();
			Level.ScoringData.numTimesHit++;
			Vibrator.Vibrate(1f, 0.2f, base.basePlayer.id);
			if (Health <= 0)
			{
				OnStatsDeath();
			}
			else
			{
				StartCoroutine(hit_cr());
			}
		}
	}

	public void OnPitKnockUp()
	{
		Debug.LogWarning("<color='red'>DAMAGED BY PIT</color>");
		base.basePlayer.damageReceiver.TakeDamage(new DamageDealer.DamageInfo(1f, DamageDealer.Direction.Neutral, base.transform.position, DamageDealer.DamageSource.Pit));
	}

	public void OnDealDamage(float damage, DamageDealer dealer)
	{
		if (!SuperInvincible)
		{
			SuperMeter += 0.0625f * damage / dealer.DamageMultiplier;
			OnSuperChanged(false);
		}
	}

	public void OnParry(float multiplier = 1f)
	{
		if (!SuperInvincible)
		{
			SuperMeter += 10f * multiplier;
			OnSuperChanged();
		}
		Level.ScoringData.numParries++;
		OnlineManager.Instance.Interface.IncrementStat(base.basePlayer.id, "Parries", 1);
		if (Level.Current.CurrentLevel != Levels.Tutorial && Level.Current.CurrentLevel != Levels.ShmupTutorial && (Level.Current.playerMode == PlayerMode.Level || Level.Current.playerMode == PlayerMode.Arcade))
		{
			ParriesThisJump++;
			if (ParriesThisJump > PlayerData.Data.GetNumParriesInRow(base.basePlayer.id))
			{
				PlayerData.Data.SetNumParriesInRow(base.basePlayer.id, ParriesThisJump);
			}
			if (ParriesThisJump == 5)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(base.basePlayer.id, "ParryChain");
			}
		}
		if (SuperMeter == SuperMeterMax)
		{
			AudioManager.Play("player_parry_power_up_full");
		}
		else
		{
			AudioManager.Play("player_parry_power_up");
		}
	}

	public void ParryOneQuarter()
	{
		OnParry(0.25f);
	}

	public void ResetJumpParries()
	{
		ParriesThisJump = 0;
	}

	public void OnPartnerStealHealth()
	{
		if (PartnerCanSteal)
		{
			Health--;
			DicePalaceMainLevelGameInfo.PlayerStats playerStats = ((base.basePlayer.id != PlayerId.PlayerOne) ? DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS : DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS);
			if ((Level.IsDicePalace || Level.IsDicePalaceMain) && playerStats != null)
			{
				playerStats.LoseBonusHP();
				CalculateHealthMax();
			}
			OnHealthChanged();
		}
	}

	public void OnSuper()
	{
		if (Loadout.super != Super.level_super_invincible || Level.Current.playerMode != PlayerMode.Level)
		{
			SuperMeter = 0f;
			OnSuperChanged();
		}
		State = PlayerState.Super;
	}

	public void OnSuperEnd()
	{
		if (Loadout.super == Super.level_super_invincible && Level.Current.playerMode == PlayerMode.Level)
		{
			StartCoroutine(emptySuper_cr());
		}
		State = PlayerState.Ready;
	}

	public void OnEx()
	{
		SuperMeter -= 10f;
		OnSuperChanged();
	}

	private void OnWeaponChange(Weapon weapon)
	{
		if (this.OnWeaponChangedEvent != null)
		{
			this.OnWeaponChangedEvent(weapon);
		}
	}

	private void OnHealthChanged()
	{
		Health = Mathf.Clamp(Health, 0, HealthMax);
		if (this.OnHealthChangedEvent != null)
		{
			this.OnHealthChangedEvent(Health, base.basePlayer.id);
		}
	}

	private void OnSuperChanged(bool playEffect = true)
	{
		SuperMeter = Mathf.Clamp(SuperMeter, 0f, SuperMeterMax);
		if (this.OnSuperChangedEvent != null)
		{
			this.OnSuperChangedEvent(SuperMeter, base.basePlayer.id, playEffect);
		}
	}

	private void OnStatsDeath()
	{
		AudioManager.Play("player_die");
		StartCoroutine(death_sound_cr());
		if (this.OnPlayerDeathEvent != null)
		{
			this.OnPlayerDeathEvent(base.basePlayer.id);
		}
		Deaths++;
		PlayerData.Data.Die(base.basePlayer.id);
	}

	public void OnPreRevive()
	{
		Health = 1;
	}

	public void OnRevive()
	{
		OnHealthChanged();
		if (this.OnPlayerReviveEvent != null)
		{
			this.OnPlayerReviveEvent(base.basePlayer.id);
		}
	}

	public void SetHealth(int health)
	{
		Health = health;
		CalculateHealthMax();
		OnHealthChanged();
	}

	public void SetInvincible(bool superInvincible)
	{
		SuperInvincible = superInvincible;
	}

	public void AddEx()
	{
		if (!SuperInvincible)
		{
			SuperMeter += 10f;
			OnSuperChanged();
		}
	}

	private void UpdateStone()
	{
		PlanePlayerController planePlayerController = base.basePlayer as PlanePlayerController;
		if (planePlayerController != null)
		{
			currentMoveDir = planePlayerController.motor.MoveDirection;
		}
		lastMoveDir = currentMoveDir;
		timeSinceStoned += CupheadTime.FixedDelta;
		if (!(StoneTime <= 0f) && ((lastMoveDir != currentMoveDir && ((int)currentMoveDir.x != 0 || (int)currentMoveDir.y != 0)) || base.basePlayer.input.actions.GetAnyButtonDown()))
		{
			StoneTime -= CupheadTime.Delta;
			StoneTime -= 0.1f;
			this.OnStoneShake();
		}
	}

	public override void StopAllCoroutines()
	{
	}

	private IEnumerator death_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		AudioManager.Play("player_die_vinylscratch");
	}

	private IEnumerator hit_cr()
	{
		hardInvincibility = true;
		for (int i = 0; i < 10; i++)
		{
			yield return null;
		}
		hardInvincibility = false;
	}

	private IEnumerator charmSuperBuilder_cr()
	{
		while (Loadout == null)
		{
			yield return null;
		}
		if (Loadout.charm != Charm.charm_super_builder)
		{
			yield break;
		}
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, WeaponProperties.CharmSuperBuilder.delay);
			if (!SuperInvincible)
			{
				SuperMeter += WeaponProperties.CharmSuperBuilder.amount;
				OnSuperChanged(false);
			}
		}
	}

	private IEnumerator emptySuper_cr()
	{
		while (SuperMeter > 0f)
		{
			SuperMeter -= SuperMeterMax * (float)CupheadTime.Delta / WeaponProperties.LevelSuperInvincibility.durationFX;
			OnSuperChanged();
			yield return null;
		}
		SuperMeter = 0f;
		OnSuperChanged();
	}

	public void DebugAddSuper()
	{
		AddEx();
	}

	public void DebugFillSuper()
	{
		SuperMeter = 50f;
		OnSuperChanged();
	}

	public static void DebugToggleInvincible()
	{
		DebugInvincible = !DebugInvincible;
		string text = ((!DebugInvincible) ? "red" : "green");
		Debug.Log("DEBUG CONSOLE: player.invincible <color=\"" + text + "\">" + DebugInvincible + "</color>");
	}
}
