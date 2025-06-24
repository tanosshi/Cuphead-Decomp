using UnityEngine;

public class CupheadCutsceneCamera : AbstractCupheadGameCamera
{
	public enum Mode
	{
		Lerp = 0,
		TrapBox = 1,
		Relative = 2,
		Platforming = 3,
		Static = 10000
	}

	public const string EDITOR_PATH = "Assets/_CUPHEAD/Prefabs/Camera/CutsceneCamera.prefab";

	private const float BOUND_COLLIDER_SIZE = 400f;

	private const float BORDER_THICKNESS = 1000f;

	public const float LEFT = -640f;

	public const float RIGHT = 640f;

	public const float BOTTOM = -360f;

	public const float TOP = 360f;

	public bool noShake;

	public bool minimalShake;

	public bool noBars;

	public static CupheadCutsceneCamera Current { get; private set; }

	public override float OrthographicSize
	{
		get
		{
			return 360f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		Move();
	}

	public void Init()
	{
		base.enabled = true;
		Texture2D texture = CreateBorderTexture();
		if (!noBars)
		{
			Transform parent = new GameObject("Border").transform;
			CreateBorderRenderer(texture, parent, "Left");
			CreateBorderRenderer(texture, parent, "Right");
			CreateBorderRenderer(texture, parent, "Top");
			CreateBorderRenderer(texture, parent, "Bottom");
		}
		if (!noShake)
		{
			if (minimalShake)
			{
				StartSmoothShake(3f, 1.5f, 6);
			}
			else
			{
				StartSmoothShake(4f, 0.75f, 8);
			}
		}
	}

	private SpriteRenderer CreateBorderRenderer(Texture2D texture, Transform parent, string name)
	{
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		Vector2 pivot = Vector2.zero;
		switch (name.ToLower())
		{
		default:
			Debug.Log("Border '" + name + "' not configured!");
			break;
		case "left":
			vector = new Vector2(1000f, 2720f);
			vector2 = new Vector2(-640f, 0f);
			pivot = new Vector2(1f, 0.5f);
			break;
		case "right":
			vector = new Vector2(1000f, 2720f);
			vector2 = new Vector2(640f, 0f);
			pivot = new Vector2(0f, 0.5f);
			break;
		case "top":
			vector = new Vector2(3280f, 1000f);
			vector2 = new Vector2(0f, 360f);
			pivot = new Vector2(0.5f, 0f);
			break;
		case "bottom":
			vector = new Vector2(3280f, 1000f);
			vector2 = new Vector2(0f, -360f);
			pivot = new Vector2(0.5f, 1f);
			break;
		}
		SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
		spriteRenderer.transform.SetParent(parent);
		Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), pivot, 1f);
		spriteRenderer.sprite = sprite;
		spriteRenderer.transform.localScale = vector;
		spriteRenderer.transform.position = vector2;
		spriteRenderer.sortingLayerName = SpriteLayer.Foreground.ToString();
		spriteRenderer.sortingOrder = 10000;
		return spriteRenderer;
	}

	private Texture2D CreateBorderTexture()
	{
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.filterMode = FilterMode.Point;
		texture2D.SetPixel(0, 0, Color.black);
		texture2D.Apply();
		return texture2D;
	}
}
