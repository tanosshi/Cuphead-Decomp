using System;
using UnityEngine;

public class PlanePlayerParryController : AbstractPlanePlayerComponent, IParryAttack
{
	public enum ParryState
	{
		Init = 0,
		Ready = 1,
		Cooldown = 2,
		Parrying = 3,
		Disabled = 4
	}

	public const float DURATION = 0.16f;

	public const float COOLDOWN_DURATION = 0.3f;

	private ParryState state;

	[SerializeField]
	private PlanePlayerParryEffect effect;

	private PlanePlayerParryEffect effectInstance;

	private float timeSinceParry;

	public ParryState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public bool AttackParryUsed { get; set; }

	public bool HasHitEnemy { get; set; }

	public event Action OnParryStartEvent;

	public event Action OnParrySuccessEvent;

	protected override void Start()
	{
		base.Start();
		base.player.OnReviveEvent += OnRevive;
		base.player.stats.OnStoned += OnStoned;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		switch (state)
		{
		case ParryState.Ready:
			UpdateReady();
			break;
		case ParryState.Cooldown:
			UpdateCooldown();
			break;
		}
	}

	private void UpdateReady()
	{
		if (!base.player.Shrunk && !base.player.WeaponBusy && !(base.player.stats.StoneTime > 0f) && state == ParryState.Ready && (base.player.input.actions.GetButtonDown(2) || base.player.motor.HasBufferedInput(PlanePlayerMotor.BufferedInput.Jump)))
		{
			base.player.motor.ClearBufferedInput();
			StartParry();
		}
	}

	private void UpdateCooldown()
	{
		timeSinceParry += CupheadTime.FixedDelta;
		if (timeSinceParry > 0.3f)
		{
			state = ParryState.Ready;
			AttackParryUsed = false;
		}
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		state = ParryState.Ready;
	}

	private void StartParry()
	{
		if (state == ParryState.Ready)
		{
			state = ParryState.Parrying;
			if (this.OnParryStartEvent != null)
			{
				this.OnParryStartEvent();
			}
			effectInstance = effect.Create(base.player) as PlanePlayerParryEffect;
		}
	}

	public void OnParrySuccess()
	{
		if (this.OnParrySuccessEvent != null)
		{
			this.OnParrySuccessEvent();
		}
		state = ParryState.Cooldown;
		timeSinceParry = 0f;
		if (effectInstance != null)
		{
			UnityEngine.Object.Destroy(effectInstance.gameObject);
		}
	}

	private void OnParryEnd()
	{
		state = ParryState.Cooldown;
		timeSinceParry = 0f;
		HasHitEnemy = false;
		if (effectInstance != null)
		{
			UnityEngine.Object.Destroy(effectInstance.gameObject);
		}
	}

	private void OnRevive(Vector3 pos)
	{
		state = ParryState.Ready;
	}

	private void OnStoned()
	{
		state = ParryState.Ready;
	}

	private void SoundPlaneParry()
	{
		AudioManager.Play("player_plane_parry");
		emitAudioFromObject.Add("player_plane_parry");
	}
}
