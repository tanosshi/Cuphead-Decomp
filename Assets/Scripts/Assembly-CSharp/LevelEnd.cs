using System;
using System.Collections;
using UnityEngine;

public class LevelEnd : AbstractMonoBehaviour
{
	private const string NAME = "LEVEL_END_CONTROLER";

	private const float WIN_FADE_TIME = 3f;

	private const float JOIN_WAIT = 1f;

	protected override void Awake()
	{
		base.Awake();
		base.transform.SetAsFirstSibling();
		base.gameObject.name = "LEVEL_END_CONTROLER";
	}

	private static LevelEnd Create()
	{
		GameObject gameObject = new GameObject();
		return gameObject.AddComponent<LevelEnd>();
	}

	public static void Win(Action onBossDeathCallback, Action explosionsCallback, Action explosionsFalloffCallback, Action explosionsEndCallback, AbstractPlayerController[] players, float bossDeathTime, bool goToWinScreen, bool isMausoleum, bool isDevil)
	{
		LevelEnd levelEnd = Create();
		levelEnd.StartCoroutine(levelEnd.win_cr(onBossDeathCallback, explosionsCallback, explosionsFalloffCallback, explosionsEndCallback, players, bossDeathTime, goToWinScreen, isMausoleum, isDevil));
	}

	private IEnumerator sting_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.4f);
		AudioManager.Play("level_boss_defeat_sting");
	}

	private IEnumerator win_cr(Action onBossDeathCallback, Action explosionsCallback, Action explosionsFalloffCallback, Action explosionsEndCallback, AbstractPlayerController[] players, float bossDeathTime, bool goToWinScreen, bool isMausoleum, bool isDevil)
	{
		PauseManager.Pause();
		LevelKOAnimation koAnim = LevelKOAnimation.Create(isMausoleum);
		if (!isMausoleum)
		{
			AudioManager.Play("level_announcer_knockout_bell");
			AudioManager.Play("level_announcer_knockout");
			StartCoroutine(sting_sound_cr());
		}
		else
		{
			AudioManager.Play("level_announcer_victory");
		}
		yield return koAnim.StartCoroutine(koAnim.anim_cr());
		PauseManager.Unpause();
		explosionsCallback();
		CupheadTime.SetAll(1f);
		if (!isMausoleum)
		{
			LevelPlayerController[] array = UnityEngine.Object.FindObjectsOfType<LevelPlayerController>();
			foreach (LevelPlayerController levelPlayerController in array)
			{
				levelPlayerController.OnLevelWin();
			}
		}
		if (onBossDeathCallback != null)
		{
			onBossDeathCallback();
		}
		yield return new WaitForSeconds(bossDeathTime + 0.3f);
		AbstractProjectile[] array2 = UnityEngine.Object.FindObjectsOfType<AbstractProjectile>();
		foreach (AbstractProjectile abstractProjectile in array2)
		{
			abstractProjectile.OnLevelEnd();
		}
		SceneLoader.properties.transitionStart = SceneLoader.Transition.Fade;
		SceneLoader.properties.transitionStartTime = 3f;
		if (goToWinScreen)
		{
			SceneLoader.LoadScene(Scenes.scene_win, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
		}
		else if (!isMausoleum)
		{
			SceneLoader.ReloadLevel();
		}
		yield return new WaitForSeconds(2.5f);
		explosionsEndCallback();
	}

	public static void Lose()
	{
		LevelEnd levelEnd = Create();
		levelEnd.StartCoroutine(levelEnd.lose_cr());
	}

	private IEnumerator lose_cr()
	{
		PauseManager.Unpause();
		AbstractPausableComponent[] array = UnityEngine.Object.FindObjectsOfType<AbstractPausableComponent>();
		foreach (AbstractPausableComponent abstractPausableComponent in array)
		{
			abstractPausableComponent.OnLevelEnd();
		}
		LevelGameOverGUI.Current.In();
		yield return null;
	}

	public static void PlayerJoined()
	{
		LevelEnd levelEnd = Create();
		levelEnd.StartCoroutine(levelEnd.playerJoined_cr());
	}

	private IEnumerator playerJoined_cr()
	{
		PauseManager.Unpause();
		AbstractPausableComponent[] array = UnityEngine.Object.FindObjectsOfType<AbstractPausableComponent>();
		foreach (AbstractPausableComponent abstractPausableComponent in array)
		{
			abstractPausableComponent.OnLevelEnd();
		}
		Debug.LogWarning("YEAH! INTEGRATE PLAYER TWO JOINED EVENT! YEAH! COOL!");
		yield return new WaitForSeconds(1f);
		Debug.LogWarning("COOL! YEAH! DO IT! INTEGRATE THE PLAYER TWO JOINED EVENT! YEAH!");
		yield return new WaitForSeconds(1f);
		SceneLoader.LoadLastMap();
	}
}
