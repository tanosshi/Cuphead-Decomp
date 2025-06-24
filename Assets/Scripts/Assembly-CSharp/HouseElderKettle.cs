using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HouseElderKettle : DialogueInteractionPoint
{
	private string lastDialogueSFXName;

	private int nbLoopsAnimator;

	private bool isTalking;

	private bool playFirstGroupMckellen = true;

	private bool playFirstGroupWarstory = true;

	private bool playFirstGroupExcited = true;

	private bool playFirstGroupLaugh = true;

	public void BeginDialogue()
	{
		Activate();
		speechBubble.waitForRealease = false;
	}

	protected override void Start()
	{
		base.Start();
		hasTarget = false;
		Dialoguer.events.onTextPhase += OnDialogueTextSound;
		Dialoguer.events.onStarted += StartTalkingCoroutine;
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Dialoguer.events.onTextPhase -= OnDialogueTextSound;
		Dialoguer.events.onStarted -= StartTalkingCoroutine;
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "ElderKettleBottle")
		{
			base.animator.SetTrigger("Bottle");
			StartCoroutine(bottle_sound_cr());
		}
		if (message == "ElderKettleFirstWeapon")
		{
			base.animator.SetTrigger("Continue");
		}
	}

	private void StartTalkingCoroutine()
	{
		StartCoroutine(talking_crs());
	}

	private void OnDialogueTextSound(DialoguerTextData data)
	{
		if (!string.IsNullOrEmpty(lastDialogueSFXName))
		{
			AudioManager.Stop(lastDialogueSFXName);
		}
		if (data.metadata == "excitedburst")
		{
			if (playFirstGroupExcited)
			{
				AudioManager.Play("ek_excitedburst");
				lastDialogueSFXName = "ek_excitedburst";
				playFirstGroupExcited = false;
			}
			else
			{
				AudioManager.Play("ek_excitedburst2");
				lastDialogueSFXName = "ek_excitedburst2";
				playFirstGroupExcited = true;
			}
		}
		else if (data.metadata == "laugh")
		{
			if (playFirstGroupLaugh)
			{
				AudioManager.Play("ek_laugh");
				lastDialogueSFXName = "ek_laugh";
				playFirstGroupLaugh = false;
			}
			else
			{
				AudioManager.Play("ek_laugh2");
				lastDialogueSFXName = "ek_laugh2";
				playFirstGroupLaugh = true;
			}
		}
		else if (data.metadata == "mckellen")
		{
			if (playFirstGroupMckellen)
			{
				AudioManager.Play("ek_mckellen");
				lastDialogueSFXName = "ek_mckellen";
				playFirstGroupMckellen = false;
			}
			else
			{
				AudioManager.Play("ek_mckellen2");
				lastDialogueSFXName = "ek_mckellen2";
				playFirstGroupMckellen = true;
			}
		}
		else if (data.metadata == "warstory")
		{
			if (playFirstGroupWarstory)
			{
				AudioManager.Play("ek_warstory");
				lastDialogueSFXName = "ek_warstory";
				playFirstGroupWarstory = false;
			}
			else
			{
				AudioManager.Play("ek_warstory2");
				lastDialogueSFXName = "ek_warstory2";
				playFirstGroupWarstory = true;
			}
		}
	}

	public void LoopAnimation()
	{
		nbLoopsAnimator--;
	}

	private IEnumerator bottle_sound_cr()
	{
		yield return new WaitForSeconds(0.1f);
		AudioManager.Play("sfx_potion_reveal");
	}

	private IEnumerator talking_crs()
	{
		base.animator.SetBool("IsTalking", true);
		nbLoopsAnimator = Random.Range(2, 7);
		while (conversationIsActive)
		{
			if (nbLoopsAnimator == 0)
			{
				base.animator.SetTrigger("Continue");
				nbLoopsAnimator = Random.Range(2, 7);
			}
			yield return null;
		}
		if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Talking_Loop_B"))
		{
			base.animator.SetTrigger("Continue");
		}
		base.animator.SetBool("IsTalking", false);
	}

	protected override IEnumerator ReactivateInputsCoroutine(LevelPlayerMotor playerOneMotor, LevelPlayerMotor playerTwoMotor, LevelPlayerWeaponManager playerOneWeaponManager, LevelPlayerWeaponManager playerTwoWeaponManager, Animator animator)
	{
		speechBubble.preventQuit = true;
		AbstractPlayerController playercontroller = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		if (playercontroller != null && playercontroller.animator != null && playercontroller.animator.GetCurrentAnimatorStateInfo(0).IsName("Power_Up"))
		{
			yield return playercontroller.animator.WaitForAnimationToEnd(this, "Power_Up");
		}
		speechBubble.preventQuit = false;
		yield return StartCoroutine(_003CReactivateInputsCoroutine_003E__BaseCallProxy0(playerOneMotor, playerTwoMotor, playerOneWeaponManager, playerTwoWeaponManager, animator));
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CReactivateInputsCoroutine_003E__BaseCallProxy0(LevelPlayerMotor playerOneMotor, LevelPlayerMotor playerTwoMotor, LevelPlayerWeaponManager playerOneWeaponManager, LevelPlayerWeaponManager playerTwoWeaponManager, Animator animator)
	{
		return base.ReactivateInputsCoroutine(playerOneMotor, playerTwoMotor, playerOneWeaponManager, playerTwoWeaponManager, animator);
	}
}
