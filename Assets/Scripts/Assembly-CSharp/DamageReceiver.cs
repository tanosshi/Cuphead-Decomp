using System;
using System.Collections;
using UnityEngine;

public class DamageReceiver : AbstractPausableComponent
{
	public enum Type
	{
		Enemy = 0,
		Player = 1,
		Other = 2
	}

	public delegate void OnDamageTakenHandler(DamageDealer.DamageInfo info);

	public static bool DEBUG_DO_MEGA_DAMAGE;

	public const float ENEMY_HIT_PAUSE_TIME = 0.15f;

	public Type type;

	public AnimationHelper[] animatorsEffectedByPause;

	[NonSerialized]
	public Vector2 OffScreenPadding = new Vector2(50f, 50f);

	private AnimationHelper animHelper;

	public bool IsHitPaused { get; private set; }

	public event OnDamageTakenHandler OnDamageTaken;

	public static void Debug_ToggleMegaDamage()
	{
		DEBUG_DO_MEGA_DAMAGE = !DEBUG_DO_MEGA_DAMAGE;
		string text = ((!DEBUG_DO_MEGA_DAMAGE) ? "red" : "green");
		Debug.Log("DEBUG CONSOLE: player.megaDamage <color=\"" + text + "\">" + DEBUG_DO_MEGA_DAMAGE + "</color>");
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.animator != null)
		{
			animHelper = base.animator.GetComponent<AnimationHelper>();
		}
		if (type != Type.Other)
		{
			base.tag = type.ToString();
			IsHitPaused = false;
		}
	}

	public virtual void TakeDamage(DamageDealer.DamageInfo info)
	{
		if (base.enabled && this.OnDamageTaken != null)
		{
			if (DEBUG_DO_MEGA_DAMAGE && (type == Type.Enemy || type == Type.Other))
			{
				info.SetEditorPlayer();
			}
			this.OnDamageTaken(info);
			if ((type == Type.Enemy || type == Type.Other) && info.damageSource == DamageDealer.DamageSource.Super)
			{
				StartCoroutine(pauseAnim_cr());
			}
		}
	}

	private IEnumerator pauseAnim_cr()
	{
		if (animHelper != null)
		{
			animHelper.Speed = 0f;
		}
		if (base.animator != null)
		{
			base.animator.enabled = false;
		}
		for (int i = 0; i < animatorsEffectedByPause.Length; i++)
		{
			animatorsEffectedByPause[i].GetComponent<Animator>().enabled = false;
			animatorsEffectedByPause[i].Speed = 0f;
		}
		IsHitPaused = true;
		CupheadLevelCamera.Current.Shake(10f, 0.6f);
		yield return CupheadTime.WaitForSeconds(this, 0.15f);
		IsHitPaused = false;
		if (base.animator != null)
		{
			base.animator.enabled = true;
		}
		if (animHelper != null)
		{
			animHelper.Speed = 1f;
		}
		for (int j = 0; j < animatorsEffectedByPause.Length; j++)
		{
			animatorsEffectedByPause[j].GetComponent<Animator>().enabled = true;
			animatorsEffectedByPause[j].Speed = 1f;
		}
	}
}
