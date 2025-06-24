using UnityEngine;
using UnityEngine.UI;

public class ControllerDisconnectedPrompt : InterruptingPrompt
{
	public static ControllerDisconnectedPrompt Instance;

	public PlayerId currentPlayer;

	public bool allowedToShow;

	[SerializeField]
	private Text playerText;

	[SerializeField]
	private LocalizationHelper localizationHelper;

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	public void Show(PlayerId player)
	{
		currentPlayer = player;
		localizationHelper.ApplyTranslation(Localization.Find((player != PlayerId.PlayerOne) ? "XboxPlayer2" : "XboxPlayer1"));
		PlayerManager.OnDisconnectPromptDisplayed(player);
		Show();
	}

	protected override void Update()
	{
		base.Update();
		if (base.Visible && !PlayerManager.IsControllerDisconnected(currentPlayer))
		{
			FrameDelayedCallback(base.Dismiss, 2);
		}
	}
}
