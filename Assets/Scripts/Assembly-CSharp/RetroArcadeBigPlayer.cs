using UnityEngine;

public class RetroArcadeBigPlayer : AbstractPausableComponent
{
	private const int BOIL_LAYER = 0;

	private const int BUTTON_LAYER = 1;

	private const int STICK_LAYER = 2;

	private const int MAIN_LAYER = 3;

	private ArcadePlayerController player;

	[SerializeField]
	private bool isCuphead;

	private bool trackingInputs;

	public void Init(ArcadePlayerController player)
	{
		this.player = player;
		if (player == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		player.motor.OnHitEvent += OnHit;
		base.animator.SetBool("IsCuphead", isCuphead);
	}

	protected override void FixedUpdate()
	{
		base.Update();
		if (player == null || !trackingInputs)
		{
			return;
		}
		base.animator.SetBool("Dead", player.IsDead);
		base.animator.Update(Time.deltaTime);
		base.animator.Update(0f);
		if (base.animator.GetCurrentAnimatorStateInfo(3).IsName("Idle"))
		{
			if (player.input.actions.GetButtonDown(3))
			{
				base.animator.SetTrigger("A");
			}
			if (player.input.actions.GetButtonDown(2))
			{
				base.animator.SetTrigger("B");
			}
			if (player.input.actions.GetButtonDown(7))
			{
				base.animator.SetTrigger("C");
			}
			base.animator.SetInteger("MoveX", player.input.GetAxisInt(PlayerInput.Axis.X));
		}
		else
		{
			base.animator.Play("Idle", 2);
			base.animator.Play("Idle", 1);
			base.animator.SetInteger("MoveX", 0);
		}
	}

	private void OnHit()
	{
		base.animator.SetTrigger("Hit");
	}

	public void LevelStart()
	{
		trackingInputs = true;
	}

	public void OnVictory()
	{
		if (player != null && !player.IsDead)
		{
			base.animator.SetTrigger("Victory");
		}
	}

	private void PlayButtonASound()
	{
		AudioManager.Play("level_button_a");
		emitAudioFromObject.Add("level_button_a");
	}

	private void PlayButtonBSound()
	{
		AudioManager.Play("level_button_b");
		emitAudioFromObject.Add("level_button_b");
	}

	private void PlayButtonCSound()
	{
		AudioManager.Play("level_button_c");
		emitAudioFromObject.Add("level_button_c");
	}
}
