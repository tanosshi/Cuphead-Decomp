using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : AbstractMonoBehaviour
{
	public delegate void FadeHandler(float time);

	public enum Transition
	{
		Iris = 0,
		Fade = 1
	}

	public enum Icon
	{
		None = 0,
		Random = 1,
		Cuphead_Head = 2,
		Cuphead_Running = 3,
		Cuphead_Jumping = 4,
		Screen_OneMoment = 5,
		Hourglass = 6
	}

	public class Properties
	{
		public const float FADE_START_DEFAULT = 0.4f;

		public const float FADE_END_DEFAULT = 0.4f;

		public Icon icon;

		public Transition transitionStart;

		public Transition transitionEnd;

		public float transitionStartTime;

		public float transitionEndTime;

		public Properties()
		{
			Reset();
		}

		public void Reset()
		{
			icon = Icon.Hourglass;
			transitionStart = Transition.Fade;
			transitionEnd = Transition.Fade;
			transitionStartTime = 0.4f;
			transitionEndTime = 0.4f;
		}
	}

	private const string SCENE_LOADER_PATH = "UI/Scene_Loader";

	private const float ICON_IN_TIME = 0.4f;

	private const float ICON_OUT_TIME = 0.6f;

	private const float ICON_WAIT_TIME = 1f;

	private const float ICON_NONE_TIME = 1f;

	private const float FADER_DELAY = 0.5f;

	private const float IRIS_TIME = 0.6f;

	public static float EndTransitionDelay;

	public static bool IsInIrisTransition;

	private static SceneLoader _instance;

	private static bool currentlyLoading;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Image fader;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private SceneLoaderCamera camera;

	private bool doneLoadingSceneAsync;

	private float bgmVolume;

	private float bgmLevelVolume;

	private float bgmVolumeStart;

	private float bgmLevelVolumeStart;

	private float sfxVolumeStart;

	private Coroutine bgmCoroutine;

	public static bool Exists
	{
		get
		{
			return _instance != null;
		}
	}

	private static SceneLoader instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (UnityEngine.Object.Instantiate(Resources.Load("UI/Scene_Loader")) as GameObject).GetComponent<SceneLoader>();
			}
			return _instance;
		}
	}

	public static Levels CurrentLevel { get; private set; }

	public static string SceneName { get; private set; }

	public static Properties properties { get; private set; }

	public static bool CurrentlyLoading
	{
		get
		{
			return currentlyLoading;
		}
	}

	public static event FadeHandler OnFadeInStartEvent;

	public static event Action OnFadeInEndEvent;

	public static event FadeHandler OnFadeOutStartEvent;

	public static event Action OnFadeOutEndEvent;

	public static event FadeHandler OnFaderValue;

	public static event Action OnLoaderCompleteEvent;

	static SceneLoader()
	{
		SceneName = string.Empty;
		CurrentLevel = Levels.Veggies;
		properties = new Properties();
	}

	public static void LoadScene(string sceneName, Transition transitionStart, Transition transitionEnd, Icon icon = Icon.Hourglass)
	{
		Scenes result = Scenes.scene_start;
		if (!EnumUtils.TryParse<Scenes>(sceneName, out result))
		{
			Debug.LogWarning("Scene \"" + sceneName + "\" is not valid!");
		}
		else
		{
			LoadScene(result, transitionStart, transitionEnd, icon);
		}
	}

	public static void LoadScene(Scenes scene, Transition transitionStart, Transition transitionEnd, Icon icon = Icon.Hourglass)
	{
		if (currentlyLoading)
		{
			Debug.LogWarning("Already loading!");
			return;
		}
		InterruptingPrompt.SetCanInterrupt(false);
		properties.transitionStart = transitionStart;
		properties.transitionEnd = transitionEnd;
		properties.icon = icon;
		EndTransitionDelay = 0.6f;
		Debug.Log("Loading Scene: " + scene);
		SceneName = scene.ToString();
		instance.Load();
	}

	public static void LoadLevel(Levels level, Transition transitionStart, Icon icon = Icon.Hourglass)
	{
		CurrentLevel = level;
		LoadScene(LevelProperties.GetLevelScene(level), transitionStart, Transition.Iris, icon);
	}

	public static void LoadDicePalaceLevel(DicePalaceLevels dicePalaceLevel)
	{
		Levels level = (CurrentLevel = LevelProperties.GetDicePalaceLevel(dicePalaceLevel));
		LoadScene(LevelProperties.GetLevelScene(level), Transition.Fade, Transition.Fade, Icon.None);
	}

	public static void SetCurrentLevel(Levels level)
	{
		CurrentLevel = level;
	}

	public static void ReloadLevel()
	{
		if (Level.IsDicePalace)
		{
			LoadDicePalaceLevel(DicePalaceLevels.DicePalaceMain);
			return;
		}
		float transitionStartTime = properties.transitionStartTime;
		properties.transitionStartTime = 0.25f;
		LoadLevel(CurrentLevel, Transition.Fade, Icon.None);
		properties.transitionStartTime = transitionStartTime;
	}

	public static void LoadLastMap()
	{
		LoadScene(PlayerData.Data.CurrentMap, Transition.Iris, Transition.Iris);
	}

	public static void LoadSceneImmediate(Scenes scene)
	{
		SceneManager.LoadScene(scene.ToString());
	}

	public static void TransitionOut()
	{
		TransitionOut(properties.transitionStart);
	}

	public static void TransitionOut(Transition transition)
	{
		TransitionOut(transition, properties.transitionStartTime);
	}

	public static void TransitionOut(Transition transition, float time)
	{
		properties.transitionStart = transition;
		properties.transitionStartTime = time;
		instance.Out();
	}

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
		SetIconAlpha(0f);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Load()
	{
		if (SceneName != Scenes.scene_slot_select.ToString())
		{
			AudioManager.HandleSnapshot(AudioManager.Snapshots.Loadscreen.ToString(), 5f);
		}
		StartCoroutine(loop_cr());
	}

	private void In()
	{
		StartCoroutine(in_cr());
	}

	private void Out()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			if (SceneLoader.OnFadeOutEndEvent != null)
			{
				SceneLoader.OnFadeOutEndEvent();
			}
		}
		else
		{
			StartCoroutine(out_cr());
		}
	}

	private void UpdateProgress(float progress)
	{
	}

	private void SetIconAlpha(float a)
	{
		SetImageAlpha(icon, a);
	}

	private void SetFaderAlpha(float a)
	{
		SetImageAlpha(fader, a);
	}

	private void SetImageAlpha(Image i, float a)
	{
		Color color = i.color;
		color.a = a;
		i.color = color;
	}

	private IEnumerator loop_cr()
	{
		currentlyLoading = true;
		yield return StartCoroutine(in_cr());
		StartCoroutine(load_cr());
		yield return StartCoroutine(iconFadeIn_cr());
		while (!doneLoadingSceneAsync)
		{
			yield return null;
		}
		if (SceneName != Scenes.scene_slot_select.ToString())
		{
			AudioManager.SnapshotReset(SceneName, 0.15f);
		}
		yield return StartCoroutine(iconFadeOut_cr());
		yield return StartCoroutine(out_cr());
		properties.Reset();
		currentlyLoading = false;
	}

	private IEnumerator load_cr()
	{
		doneLoadingSceneAsync = false;
		GC.Collect();
		AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);
		while (!async.isDone)
		{
			UpdateProgress(async.progress);
			yield return null;
		}
		doneLoadingSceneAsync = true;
	}

	private IEnumerator in_cr()
	{
		Transition transitionStart = properties.transitionStart;
		if (transitionStart == Transition.Iris || transitionStart != Transition.Fade)
		{
			if (SceneName != Scenes.scene_slot_select.ToString())
			{
				FadeOutBGM(0.6f);
			}
			yield return StartCoroutine(irisIn_cr());
		}
		else
		{
			if (SceneName != Scenes.scene_slot_select.ToString())
			{
				FadeOutBGM(properties.transitionEndTime);
			}
			yield return StartCoroutine(faderFadeIn_cr());
		}
	}

	private IEnumerator out_cr()
	{
		yield return null;
		Transition transitionEnd = properties.transitionEnd;
		if (transitionEnd == Transition.Iris || transitionEnd != Transition.Fade)
		{
			yield return StartCoroutine(irisOut_cr());
		}
		else
		{
			yield return StartCoroutine(faderFadeOut_cr());
		}
		if (SceneName != Scenes.scene_slot_select.ToString())
		{
			ResetBgmVolume();
		}
		if (SceneLoader.OnLoaderCompleteEvent != null)
		{
			SceneLoader.OnLoaderCompleteEvent();
		}
		SceneLoader.OnLoaderCompleteEvent = null;
	}

	private IEnumerator irisIn_cr()
	{
		IsInIrisTransition = true;
		Animator animator = fader.GetComponent<Animator>();
		animator.SetTrigger("Iris_In");
		SetFaderAlpha(1f);
		if (SceneLoader.OnFadeInStartEvent != null)
		{
			SceneLoader.OnFadeInStartEvent(0.6f);
		}
		yield return new WaitForSeconds(0.6f);
		if (SceneLoader.OnFadeInEndEvent != null)
		{
			SceneLoader.OnFadeInEndEvent();
		}
	}

	private IEnumerator irisOut_cr()
	{
		Animator animator = fader.GetComponent<Animator>();
		animator.SetTrigger("Iris_Out");
		SetFaderAlpha(1f);
		if (SceneLoader.OnFadeOutStartEvent != null)
		{
			SceneLoader.OnFadeOutStartEvent(0.6f);
		}
		yield return new WaitForSeconds(0.6f);
		if (SceneLoader.OnFadeOutEndEvent != null)
		{
			SceneLoader.OnFadeOutEndEvent();
		}
		IsInIrisTransition = false;
	}

	private IEnumerator faderFadeIn_cr()
	{
		SetFaderAlpha(0f);
		Animator animator = fader.GetComponent<Animator>();
		animator.SetTrigger("Black");
		if (SceneLoader.OnFadeInStartEvent != null)
		{
			SceneLoader.OnFadeInStartEvent(properties.transitionStartTime);
		}
		yield return StartCoroutine(imageFade_cr(fader, properties.transitionStartTime, 0f, 1f));
		if (SceneLoader.OnFadeInEndEvent != null)
		{
			SceneLoader.OnFadeInEndEvent();
		}
	}

	private IEnumerator faderFadeOut_cr()
	{
		if (SceneLoader.OnFadeOutStartEvent != null)
		{
			SceneLoader.OnFadeOutStartEvent(properties.transitionEndTime);
		}
		yield return StartCoroutine(imageFade_cr(fader, properties.transitionEndTime, 1f, 0f));
		if (SceneLoader.OnFadeOutEndEvent != null)
		{
			SceneLoader.OnFadeOutEndEvent();
		}
	}

	private IEnumerator iconFadeIn_cr()
	{
		if (properties.icon == Icon.None)
		{
			SetIconAlpha(0f);
			yield break;
		}
		Animator animator = icon.GetComponent<Animator>();
		animator.SetTrigger(properties.icon.ToString());
		yield return StartCoroutine(imageFade_cr(icon, 0.4f, 0f, 1f, true));
	}

	private IEnumerator iconFadeOut_cr()
	{
		if (properties.icon == Icon.None)
		{
			SetIconAlpha(0f);
			yield return new WaitForSeconds(0.6f);
			yield break;
		}
		float startAlpha = icon.color.a;
		yield return StartCoroutine(imageFade_cr(icon, 0.6f * startAlpha, startAlpha, 0f));
		if (startAlpha < 1f)
		{
			yield return new WaitForSeconds(0.6f * (1f - startAlpha));
		}
	}

	private IEnumerator imageFade_cr(Image image, float time, float start, float end, bool interruptOnLoad = false)
	{
		float t = 0f;
		SetImageAlpha(image, start);
		while (t < time && (!interruptOnLoad || !doneLoadingSceneAsync))
		{
			float val = Mathf.Lerp(start, end, t / time);
			SetImageAlpha(image, val);
			t += Time.deltaTime;
			if (SceneLoader.OnFaderValue != null)
			{
				SceneLoader.OnFaderValue(t / time);
			}
			if (interruptOnLoad)
			{
				EndTransitionDelay = val * 0.6f;
			}
			yield return null;
		}
		SetImageAlpha(image, end);
		if (interruptOnLoad && !doneLoadingSceneAsync)
		{
			EndTransitionDelay = 0.6f;
		}
	}

	private IEnumerator fadeBGM_cr(float time)
	{
		if (AudioNoiseHandler.Instance != null)
		{
			AudioNoiseHandler.Instance.OpticalSound();
		}
		bgmVolumeStart = AudioManager.bgmOptionsVolume;
		bgmVolume = AudioManager.bgmOptionsVolume;
		sfxVolumeStart = AudioManager.sfxOptionsVolume;
		float t = 0f;
		while (t < time)
		{
			AudioManager.bgmOptionsVolume = Mathf.Lerp(t: t / time, a: bgmVolume, b: -80f);
			t += Time.deltaTime;
			yield return null;
		}
		AudioManager.bgmOptionsVolume = -80f;
		AudioManager.StopBGM();
	}

	private void FadeOutBGM(float time)
	{
		if (bgmCoroutine != null)
		{
			StopCoroutine(bgmCoroutine);
		}
		bgmCoroutine = StartCoroutine(fadeBGM_cr(time));
	}

	private void ResetBgmVolume()
	{
		if (bgmCoroutine != null)
		{
			StopCoroutine(bgmCoroutine);
		}
		AudioManager.bgmOptionsVolume = bgmVolumeStart;
		AudioManager.sfxOptionsVolume = sfxVolumeStart;
	}
}
