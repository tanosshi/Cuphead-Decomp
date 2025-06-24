using System;
using UnityEngine;

public class ParrySwitch : AbstractSwitch
{
	public const string TAG = "ParrySwitch";

	[SerializeField]
	private Effect parrySpark;

	public event Action OnPrePauseActivate;

	protected void FirePrePauseEvent()
	{
		if (this.OnPrePauseActivate != null)
		{
			this.OnPrePauseActivate();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.tag = "ParrySwitch";
		if (GetComponent<Collider2D>() == null)
		{
			Debug.LogWarning("ParrySwitch must have Collider2D attached!", base.gameObject);
		}
	}

	public virtual void OnParryPrePause(AbstractPlayerController player)
	{
		if ((bool)parrySpark)
		{
			parrySpark.Create(base.transform.position);
		}
		FirePrePauseEvent();
	}

	public virtual void OnParryPostPause(AbstractPlayerController player)
	{
		DispatchEvent();
	}

	public void ActivateFromOtherSource()
	{
		if ((bool)parrySpark)
		{
			parrySpark.Create(base.transform.position);
		}
		AudioManager.Play("Player_Parry");
		DispatchEvent();
	}
}
