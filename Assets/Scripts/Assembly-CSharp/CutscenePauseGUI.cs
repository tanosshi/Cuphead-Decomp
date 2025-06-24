using System;

public class CutscenePauseGUI : AbstractPauseGUI
{
	public bool pauseAllowed = true;

	protected override bool CanPause
	{
		get
		{
			return PauseManager.state != PauseManager.State.Paused && pauseAllowed;
		}
	}

	public static event Action OnPauseEvent;

	public static event Action OnUnpauseEvent;

	protected override void OnPause()
	{
		base.OnPause();
		CupheadCutsceneCamera.Current.StartBlur();
		if (CutscenePauseGUI.OnPauseEvent != null)
		{
			CutscenePauseGUI.OnPauseEvent();
		}
	}

	protected override void OnUnpause()
	{
		base.OnUnpause();
		CupheadCutsceneCamera.Current.EndBlur();
		if (CutscenePauseGUI.OnUnpauseEvent != null)
		{
			CutscenePauseGUI.OnUnpauseEvent();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PauseManager.Unpause();
	}

	protected override void Update()
	{
		base.Update();
		if (base.state == State.Paused)
		{
			if (GetButtonDown(CupheadButton.Pause))
			{
				Unpause();
			}
			else if (GetButtonDown(CupheadButton.Cancel))
			{
				Unpause();
			}
			else if (GetButtonDown(CupheadButton.Accept))
			{
				Cutscene.Current.Skip();
			}
		}
	}

	private void Restart()
	{
		base.state = State.Animating;
		SceneLoader.ReloadLevel();
	}

	private void StartNewGame()
	{
		base.state = State.Animating;
		PlayerManager.ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	protected override void InAnimation(float i)
	{
	}

	protected override void OutAnimation(float i)
	{
	}
}
