using System.Collections;
using UnityEngine;

public class StartScreen : AbstractMonoBehaviour
{
	public enum State
	{
		Animating = 0,
		MDHR_Splash = 1,
		Title = 2
	}

	public AudioClip[] SelectSound;

	[SerializeField]
	private Animator mdhrSplash;

	[SerializeField]
	private SpriteRenderer fader;

	private CupheadInput.AnyPlayerInput input;

	private bool shouldLoadSlotSelect;

	private const string PATH = "Audio/TitleScreenAudio";

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Cuphead.Init();
		CupheadTime.Reset();
		PauseManager.Reset();
		shouldLoadSlotSelect = false;
		PlayerData.inGame = false;
		PlayerManager.ResetPlayers();
	}

	protected override void Start()
	{
		base.Start();
		if (AudioNoiseHandler.Instance != null)
		{
			AudioNoiseHandler.Instance.OpticalSound();
		}
		if (StartScreenAudio.Instance == null)
		{
			StartScreenAudio startScreenAudio = Object.Instantiate(Resources.Load("Audio/TitleScreenAudio")) as StartScreenAudio;
			startScreenAudio.name = "StartScreenAudio";
		}
		SettingsData.ApplySettingsOnStartup();
		FrameDelayedCallback(StartFrontendSnapshot, 1);
		StartCoroutine(loop_cr());
	}

	protected override void Update()
	{
		base.Update();
		switch (state)
		{
		case State.MDHR_Splash:
			UpdateSplashMDHR();
			break;
		case State.Title:
			UpdateTitleScreen();
			break;
		}
	}

	private void UpdateSplashMDHR()
	{
	}

	private void UpdateTitleScreen()
	{
		if (shouldLoadSlotSelect)
		{
			AudioManager.Play("ui_playerconfirm");
			AudioManager.Play("level_select");
			SceneLoader.LoadScene(Scenes.scene_slot_select, SceneLoader.Transition.Iris, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
			base.enabled = false;
		}
	}

	private void onPlayerJoined(PlayerId playerId)
	{
		shouldLoadSlotSelect = true;
	}

	private IEnumerator loop_cr()
	{
		yield return new WaitForSeconds(1f);
		AudioManager.Play("mdhr_logo_sting");
		yield return StartCoroutine(tweenRenderer_cr(fader, 1f));
		mdhrSplash.Play("Logo");
		yield return mdhrSplash.WaitForAnimationToEnd(this, "Logo");
		AudioManager.SnapshotReset(Scenes.scene_title.ToString(), 0.3f);
		AudioManager.PlayBGMPlaylistManually(true);
		SettingsData.Data.hasBootedUpGame = true;
		yield return StartCoroutine(tweenRenderer_cr(mdhrSplash.GetComponent<SpriteRenderer>(), 0.4f));
		state = State.Title;
		PlayerManager.OnPlayerJoinedEvent += onPlayerJoined;
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerOne, true, false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.OnPlayerJoinedEvent -= onPlayerJoined;
	}

	private IEnumerator tweenRenderer_cr(SpriteRenderer renderer, float time)
	{
		float t = 0f;
		Color c = renderer.color;
		c.a = 1f;
		yield return null;
		while (t < time)
		{
			c.a = 1f - t / time;
			renderer.color = c;
			t += Time.deltaTime;
			yield return null;
		}
		c.a = 0f;
		renderer.color = c;
		yield return null;
	}

	protected virtual void StartFrontendSnapshot()
	{
		AudioManager.HandleSnapshot(AudioManager.Snapshots.FrontEnd.ToString(), 0.15f);
	}
}
