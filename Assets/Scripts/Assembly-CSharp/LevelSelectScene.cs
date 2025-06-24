using UnityEngine;

public class LevelSelectScene : AbstractMonoBehaviour
{
	[SerializeField]
	private CanvasGroup onePlayerButton;

	[SerializeField]
	private CanvasGroup twoPlayersButton;

	protected override void Awake()
	{
		base.Awake();
		Cuphead.Init();
		CupheadEventSystem.Init();
		DebugConsole.Init();
		UpdatePlayers();
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void OnOnePlayerButtonPressed()
	{
		PlayerManager.Multiplayer = false;
		UpdatePlayers();
	}

	public void OnTwoPlayersButtonPressed()
	{
		PlayerManager.Multiplayer = true;
		UpdatePlayers();
	}

	private void UpdatePlayers()
	{
		float alpha = 0.3f;
		onePlayerButton.alpha = alpha;
		twoPlayersButton.alpha = alpha;
		if (PlayerManager.Multiplayer)
		{
			twoPlayersButton.alpha = 1f;
		}
		else
		{
			onePlayerButton.alpha = 1f;
		}
	}
}
