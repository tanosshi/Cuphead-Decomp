using UnityEngine;

public class SpeechInteractionPoint : AbstractLevelInteractiveEntity
{
	[SerializeField]
	protected string[] allDialogue;

	private int i;

	private bool activated;

	private bool isDisabledP1;

	private bool isDisabledP2;

	protected override void Awake()
	{
		base.Awake();
		dialogueProperties.text = "Talk";
		isDisabledP1 = false;
	}

	protected override void Check()
	{
		base.Check();
		if (PlayerWithinDistance(PlayerId.PlayerOne))
		{
			PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().DisableJump();
			isDisabledP1 = true;
		}
		else if (PlayerWithinDistance(PlayerId.PlayerTwo))
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerMotor>().DisableJump();
			isDisabledP2 = true;
		}
		else if (isDisabledP1)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().EnableJump();
			isDisabledP1 = false;
		}
		else if (isDisabledP2)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerMotor>().EnableJump();
			isDisabledP2 = false;
		}
	}

	protected override void Activate()
	{
		base.Activate();
		if (!activated)
		{
			if (PlayerWithinDistance(PlayerId.PlayerOne))
			{
				Show(PlayerId.PlayerOne);
			}
			if (PlayerWithinDistance(PlayerId.PlayerTwo) && PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
			{
				Show(PlayerId.PlayerTwo);
			}
			activated = true;
		}
	}
}
