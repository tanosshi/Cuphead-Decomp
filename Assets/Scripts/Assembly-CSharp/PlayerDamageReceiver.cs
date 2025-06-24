using UnityEngine;

public class PlayerDamageReceiver : DamageReceiver
{
	public enum State
	{
		Vulnerable = 0,
		Invulnerable = 1,
		Other = 2
	}

	private const float TIME_HIT = 2f;

	private const float TIME_REVIVED = 3f;

	private AbstractPlayerController player;

	private float timer;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (type != Type.Player)
		{
			Debug.LogWarning("Set PlayerDamageReceiver:Type to Type.Player");
		}
		type = Type.Player;
		player = GetComponent<AbstractPlayerController>();
		player.OnReviveEvent += OnRevive;
	}

	protected override void Update()
	{
		base.Update();
		if (state == State.Invulnerable && timer > 0f)
		{
			timer -= CupheadTime.Delta;
			if (timer <= 0f)
			{
				Vulnerable();
			}
		}
	}

	public override void TakeDamage(DamageDealer.DamageInfo info)
	{
		if (player.stats.SuperInvincible)
		{
			return;
		}
		if (info.damage > 0f)
		{
			if (player.CanTakeDamage && base.enabled && !(timer > 0f))
			{
				float num = ((player.stats.Loadout.charm != Charm.charm_pit_saver) ? 1f : WeaponProperties.CharmPitSaver.invulnerabilityMultiplier);
				Invulnerable(2f * num);
				base.TakeDamage(info);
			}
		}
		else if (info.stoneTime > 0f)
		{
			base.TakeDamage(info);
		}
	}

	public void OnRevive(Vector3 pos)
	{
		Invulnerable(3f);
	}

	public void Invulnerable(float time)
	{
		state = State.Invulnerable;
		timer = time;
	}

	public void Vulnerable()
	{
		state = State.Vulnerable;
	}

	public void OnDeath()
	{
		state = State.Other;
	}

	public void OnWin()
	{
		state = State.Other;
	}
}
