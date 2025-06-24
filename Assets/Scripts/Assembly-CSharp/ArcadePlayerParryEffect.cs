public class ArcadePlayerParryEffect : AbstractParryEffect
{
	protected override bool IsHit
	{
		get
		{
			return (player as ArcadePlayerController).motor.IsHit;
		}
	}

	private ArcadePlayerController levelPlayer
	{
		get
		{
			return player as ArcadePlayerController;
		}
	}

	protected override void SetPlayer(AbstractPlayerController player)
	{
		base.SetPlayer(player);
		levelPlayer.motor.OnHitEvent += OnHitCancel;
		levelPlayer.motor.OnGroundedEvent += OnGroundedCancel;
		levelPlayer.motor.OnDashStartEvent += OnDashCancel;
		levelPlayer.weaponManager.OnExStart += OnWeaponCancel;
		levelPlayer.weaponManager.OnSuperStart += OnWeaponCancel;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		levelPlayer.motor.OnHitEvent -= OnHitCancel;
		levelPlayer.motor.OnGroundedEvent -= OnGroundedCancel;
		levelPlayer.motor.OnDashStartEvent -= OnDashCancel;
		levelPlayer.weaponManager.OnExStart -= OnWeaponCancel;
		levelPlayer.weaponManager.OnSuperStart -= OnWeaponCancel;
	}

	protected override void OnHitCancel()
	{
		base.OnHitCancel();
		levelPlayer.motor.OnParryHit();
	}

	private void OnDashCancel()
	{
		if (!didHitSomething && !(this == null))
		{
			Cancel();
		}
	}

	private void OnGroundedCancel()
	{
		if (!didHitSomething && !(this == null))
		{
			Cancel();
		}
	}

	private void OnWeaponCancel()
	{
		if (!didHitSomething && !(this == null))
		{
			Cancel();
		}
	}

	protected override void Cancel()
	{
		base.Cancel();
		levelPlayer.animationController.ResumeNormanAnim();
	}

	protected override void CancelSwitch()
	{
		base.CancelSwitch();
		levelPlayer.motor.OnParryCanceled();
	}

	protected override void OnPaused()
	{
		base.OnPaused();
		AudioManager.Play("Player_Parry");
		levelPlayer.animationController.OnParryPause();
		levelPlayer.weaponManager.ParrySuccess();
	}

	protected override void OnUnpaused()
	{
		base.OnUnpaused();
		levelPlayer.animationController.ResumeNormanAnim();
		levelPlayer.motor.OnParryComplete();
	}

	protected override void OnSuccess()
	{
		base.OnSuccess();
		levelPlayer.weaponManager.ParrySuccess();
		levelPlayer.animationController.OnParrySuccess();
	}

	protected override void OnEnd()
	{
		base.OnEnd();
		levelPlayer.animationController.OnParryAnimEnd();
	}
}
