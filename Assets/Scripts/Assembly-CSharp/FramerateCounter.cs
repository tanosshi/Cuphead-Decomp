using UnityEngine;

public class FramerateCounter : MonoBehaviour
{
	public static bool SHOW;

	public float updateInterval = 0.25f;

	private float accum;

	private int frames;

	private float timeleft;

	private GUIStyle style;

	private string text;

	private string color = "white";

	public static FramerateCounter Current { get; private set; }

	public static void Init()
	{
		if (Current == null)
		{
			GameObject gameObject = new GameObject("Framerate Counter");
			Current = gameObject.AddComponent<FramerateCounter>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	protected virtual void Start()
	{
		timeleft = updateInterval;
	}

	protected virtual void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F2} FPS", num);
			this.text = text;
			if (num < 10f)
			{
				color = "red";
			}
			else if (num < 30f)
			{
				color = "orange";
			}
			else
			{
				color = "lime";
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}

	protected virtual void OnGUI()
	{
		if (SHOW)
		{
			if (style == null)
			{
				style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.UpperRight;
				style.richText = true;
				style.padding = new RectOffset(20, 20, 20, 20);
			}
			GUI.Label(new Rect(0f, 0f, Screen.width + 1, Screen.height + 1), "<color=black>" + text + "</color>", style);
			GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "<color=" + color + ">" + text + "</color>", style);
		}
	}
}
