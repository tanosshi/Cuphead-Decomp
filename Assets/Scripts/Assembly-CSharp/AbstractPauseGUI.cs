using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractPauseGUI : AbstractMonoBehaviour
{
	public enum State
	{
		Unpaused = 0,
		Paused = 1,
		Animating = 2
	}

	public enum InputActionSet
	{
		LevelInput = 0,
		UIInput = 1
	}

	private delegate void AnimationDelegate(float i);

	[SerializeField]
	private bool isWorldMap;

	protected CanvasGroup canvasGroup;

	private CupheadInput.AnyPlayerInput input;

	private static bool plmPauseRequested;

	public State state { get; protected set; }

	protected virtual CupheadButton LevelInputButton
	{
		get
		{
			return CupheadButton.Pause;
		}
	}

	protected virtual CupheadButton UIInputButton
	{
		get
		{
			return CupheadButton.EquipMenu;
		}
	}

	protected virtual InputActionSet CheckedActionSet
	{
		get
		{
			return InputActionSet.LevelInput;
		}
	}

	protected abstract bool CanPause { get; }

	protected virtual bool CanUnpause
	{
		get
		{
			return false;
		}
	}

	protected virtual bool RespondToDeadPlayer
	{
		get
		{
			return false;
		}
	}

	protected virtual float InTime
	{
		get
		{
			return 0.15f;
		}
	}

	protected virtual float OutTime
	{
		get
		{
			return 0.15f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		canvasGroup = GetComponent<CanvasGroup>();
		HideImmediate();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		UpdateInput();
	}

	private void UpdateInput()
	{
		if (CanPause && ((CheckedActionSet != InputActionSet.LevelInput) ? GetButtonDown(UIInputButton) : GetButtonDown(LevelInputButton)))
		{
			StartCoroutine(ShowPauseMenu());
		}
	}

	public IEnumerator ShowPauseMenu()
	{
		if (MapEventNotification.Current != null)
		{
			while (MapEventNotification.Current.showing)
			{
				yield return null;
			}
		}
		if (state == State.Unpaused && PauseManager.state == PauseManager.State.Unpaused)
		{
			Pause();
		}
		else if (state == State.Paused && CanUnpause)
		{
			Unpause();
		}
	}

	public virtual void Init(bool checkIfDead, OptionsGUI options)
	{
		input = new CupheadInput.AnyPlayerInput(checkIfDead);
	}

	public virtual void Init(bool checkIfDead)
	{
		input = new CupheadInput.AnyPlayerInput(checkIfDead);
	}

	protected void Pause()
	{
		if (state == State.Unpaused && PauseManager.state == PauseManager.State.Unpaused)
		{
			StartCoroutine(pause_cr());
		}
	}

	protected void Unpause()
	{
		if (state == State.Paused)
		{
			StartCoroutine(unpause_cr());
		}
	}

	protected virtual void OnPause()
	{
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Paused.ToString(), 0.15f);
		AudioManager.PauseAllSFX();
	}

	protected virtual void OnPauseComplete()
	{
	}

	protected virtual void OnUnpause()
	{
		AudioManager.SnapshotReset((!isWorldMap) ? Level.Current.CurrentScene.ToString() : PlayerData.Data.CurrentMap.ToString(), 0.1f);
		OnUnpauseSound();
	}

	protected virtual void OnUnpauseComplete()
	{
	}

	protected virtual void OnUnpauseSound()
	{
		AudioManager.UnpauseAllSFX();
	}

	protected virtual void HideImmediate()
	{
		canvasGroup.alpha = 0f;
		SetInteractable(false);
	}

	protected virtual void ShowImmediate()
	{
		canvasGroup.alpha = 1f;
		SetInteractable(true);
	}

	private void SetInteractable(bool interactable)
	{
		canvasGroup.interactable = interactable;
		canvasGroup.blocksRaycasts = interactable;
	}

	private IEnumerator pause_cr()
	{
		OnPause();
		PauseManager.Pause();
		SetInteractable(true);
		yield return StartCoroutine(animate_cr(InTime, InAnimation, 0f, 1f));
		state = State.Paused;
		OnPauseComplete();
	}

	private IEnumerator unpause_cr()
	{
		OnUnpause();
		SetInteractable(true);
		yield return StartCoroutine(animate_cr(OutTime, OutAnimation, 1f, 0f));
		state = State.Unpaused;
		SetInteractable(false);
		PauseManager.Unpause();
		OnUnpauseComplete();
	}

	private IEnumerator animate_cr(float time, AnimationDelegate anim, float start, float end)
	{
		anim(0f);
		state = State.Animating;
		canvasGroup.alpha = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			canvasGroup.alpha = Mathf.Lerp(start, end, val);
			anim(val);
			t += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = end;
		anim(1f);
	}

	protected abstract void InAnimation(float i);

	protected abstract void OutAnimation(float i);

	protected bool GetButtonDown(CupheadButton button)
	{
		if (input.GetButtonDown(button))
		{
			MenuSelectSound();
			return true;
		}
		return false;
	}

	protected void MenuSelectSound()
	{
		AudioManager.Play("level_menu_select");
	}

	protected bool GetButton(CupheadButton button)
	{
		if (input.GetButton(button))
		{
			return true;
		}
		return false;
	}
}
