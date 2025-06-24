using System;
using System.Collections;
using UnityEngine;

public class LevelPlayerParryController : AbstractLevelPlayerComponent, IParryAttack
{
	public enum ParryState
	{
		Init = 0,
		Ready = 1,
		Parrying = 2
	}

	public const float DURATION = 0.2f;

	private ParryState state;

	[SerializeField]
	private LevelPlayerParryEffect effect;

	public ParryState State
	{
		get
		{
			return state;
		}
	}

	public bool AttackParryUsed { get; set; }

	public bool HasHitEnemy { get; set; }

	public event Action OnParryStartEvent;

	public event Action OnParryEndEvent;

	protected override void Start()
	{
		base.Start();
		base.player.motor.OnParryEvent += StartParry;
		base.player.motor.OnGroundedEvent += OnGround;
	}

	private void OnGround()
	{
		AttackParryUsed = false;
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		state = ParryState.Ready;
	}

	private void StartParry()
	{
		state = ParryState.Parrying;
		if (this.OnParryStartEvent != null)
		{
			this.OnParryStartEvent();
		}
		StartCoroutine(parry_cr());
	}

	private IEnumerator parry_cr()
	{
		AbstractParryEffect p = effect.Create(base.player);
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		state = ParryState.Ready;
		if (this.OnParryEndEvent != null)
		{
			this.OnParryEndEvent();
		}
		HasHitEnemy = false;
	}
}
