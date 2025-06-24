using System.Collections;
using UnityEngine;

public class CupheadLevelCamera : AbstractCupheadGameCamera
{
	public enum Mode
	{
		Lerp = 0,
		TrapBox = 1,
		Relative = 2,
		Platforming = 3,
		Path = 4,
		Static = 10000
	}

	public const string EDITOR_PATH = "Assets/_CUPHEAD/Prefabs/Camera/LevelCamera.prefab";

	public const float AUTOSCROLL_SPEED = 200f;

	private bool leftOffset;

	private const float BOUND_COLLIDER_SIZE = 400f;

	private const float BORDER_THICKNESS = 1000f;

	private const float CENTER_SPEED = 10f;

	private const float AUTOSCROLL_CHECK = 500f;

	private const float OFFSET_AMOUNT = 500f;

	private bool moveX;

	private bool moveY;

	private bool stabilizeY;

	private float stabilizePaddingTop;

	private float stabilizePaddingBottom;

	private Vector3 targetPos;

	private Mode mode;

	private Level.Bounds bounds;

	private Transform collidersRoot;

	private VectorPath path;

	private bool pathMovesOnlyForward;

	public bool enablePathScrubbing;

	[Range(0f, 1f)]
	public float scrub;

	private Transform leftCollider;

	private Transform rightCollider;

	private Transform topCollider;

	private Transform bottomCollider;

	private const float LERP_SPEED = 2f;

	private const float RELATIVE_LERP_SPEED = 5f;

	private const float PLATFORMING_LERP_SPEED = 5f;

	private const float PATH_LERP_SPEED = 15f;

	private const float PATH_MAX_SPEED_BEFORE_ACCELERATION = 1000f;

	private const float PATH_ACCELERATION = 5000f;

	private float _minPathValue = float.MinValue;

	private float _speedLastFrame;

	public static CupheadLevelCamera Current { get; private set; }

	public bool cameraLocked { get; private set; }

	public bool cameraOffset { get; private set; }

	public bool autoScrolling { get; private set; }

	public float autoScrollSpeedMultiplier { get; private set; }

	public float Left { get; private set; }

	public float Right { get; private set; }

	public float Bottom { get; private set; }

	public new float Top { get; private set; }

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

	protected override void Start()
	{
		base.Start();
		autoScrolling = false;
		cameraLocked = false;
		cameraOffset = false;
		autoScrollSpeedMultiplier = 1f;
		_position = base.transform.position;
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
		if (PlayerManager.Count > 0)
		{
			UpdateBounds();
			Vector3 position = _position;
			switch (mode)
			{
			default:
				UpdateModeLerp();
				break;
			case Mode.TrapBox:
				UpdateModeTrapBox();
				break;
			case Mode.Relative:
				UpdateModeRelative();
				break;
			case Mode.Platforming:
				UpdatePlatforming();
				break;
			case Mode.Path:
				UpdatePath();
				break;
			case Mode.Static:
				break;
			}
			Vector3 position2 = _position;
			if (base.Width * 2f > (float)bounds.Width)
			{
				position2.x = Mathf.Lerp(position.x, 0f, (float)CupheadTime.Delta * 10f);
			}
			if (base.Height * 2f > (float)bounds.Height)
			{
				position2.y = Mathf.Lerp(position.y, 0f, (float)CupheadTime.Delta * 10f);
			}
			_position = position2;
			Move();
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		Move();
	}

	private void UpdateBounds()
	{
		Left = ((!bounds.leftEnabled) ? float.MinValue : ((float)(-bounds.left) + base.Width));
		Right = ((!bounds.rightEnabled) ? float.MaxValue : ((float)bounds.right - base.Width));
		Bottom = ((!bounds.bottomEnabled) ? float.MinValue : ((float)(-bounds.bottom) + base.Height));
		Top = ((!bounds.topEnabled) ? float.MaxValue : ((float)bounds.top - base.Height));
	}

	public void DisableRightCollider()
	{
		rightCollider.gameObject.SetActive(false);
	}

	public void MoveRightCollider()
	{
		rightCollider.transform.AddPosition(100f);
	}

	public void Init(Level.Camera properties)
	{
		base.enabled = true;
		mode = properties.mode;
		base.zoom = properties.zoom;
		moveX = properties.moveX;
		moveY = properties.moveY;
		stabilizeY = properties.stabilizeY;
		stabilizePaddingTop = properties.stabilizePaddingTop;
		stabilizePaddingBottom = properties.stabilizePaddingBottom;
		bounds = properties.bounds;
		path = properties.path;
		pathMovesOnlyForward = properties.pathMovesOnlyForward;
		if (properties.mode == Mode.Path)
		{
			base.transform.position = path.Lerp(0f);
		}
		UpdateBounds();
		if (properties.colliders)
		{
			collidersRoot = new GameObject("Colliders").transform;
			collidersRoot.parent = base.transform;
			collidersRoot.ResetLocalTransforms();
			leftCollider = SetupCollider(Level.Bounds.Side.Left);
			rightCollider = SetupCollider(Level.Bounds.Side.Right);
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
			vector = new Vector2(1000f, (float)bounds.Height + 2000f);
			vector2 = new Vector2(-bounds.left, bounds.Center.y);
			pivot = new Vector2(1f, 0.5f);
			break;
		case "right":
			vector = new Vector2(1000f, (float)bounds.Height + 2000f);
			vector2 = new Vector2(bounds.right, bounds.Center.y);
			pivot = new Vector2(0f, 0.5f);
			break;
		case "top":
			vector = new Vector2((float)bounds.Width + 2000f, 1000f);
			vector2 = new Vector2(bounds.Center.x, bounds.top);
			pivot = new Vector2(0.5f, 0f);
			break;
		case "bottom":
			vector = new Vector2((float)bounds.Width + 2000f, 1000f);
			vector2 = new Vector2(bounds.Center.x, -bounds.bottom);
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

	private Transform SetupCollider(Level.Bounds.Side side)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int layer = 0;
		int num = 0;
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = new Vector2(base.Bounds.xMin, base.Bounds.yMax);
		Vector2 vector3 = new Vector2(base.Bounds.xMax, base.Bounds.yMin);
		float x = vector2.x;
		float x2 = vector3.x;
		float y = vector3.y;
		float y2 = vector2.y;
		switch (side)
		{
		case Level.Bounds.Side.Left:
			text = "Level_Wall_Left";
			text2 = Tags.Wall.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = 90;
			vector = new Vector2(x - 200f, 0f);
			break;
		case Level.Bounds.Side.Right:
			text = "Level_Wall_Right";
			text2 = Tags.Wall.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = -90;
			vector = new Vector2(x2 + 200f, 0f);
			break;
		case Level.Bounds.Side.Top:
			text = "Level_Ceiling";
			text2 = Tags.Ceiling.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Ceiling.ToString());
			vector = new Vector2(0f, y + 200f);
			break;
		case Level.Bounds.Side.Bottom:
			text = "Level_Ground";
			text2 = Tags.Ground.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Ground.ToString());
			num = 180;
			vector = new Vector2(0f, y2 - 200f);
			break;
		}
		GameObject gameObject = new GameObject(text);
		gameObject.tag = text2;
		gameObject.layer = layer;
		gameObject.transform.ResetLocalTransforms();
		gameObject.transform.SetPosition(vector.x, vector.y);
		gameObject.transform.SetEulerAngles(null, null, num);
		gameObject.transform.parent = collidersRoot;
		BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = true;
		boxCollider2D.size = new Vector2(2000f, 400f);
		return gameObject.transform;
	}

	private void UpdateModeLerp()
	{
		Vector3 position = _position;
		Vector3 vector = PlayerManager.Center;
		if (moveX)
		{
			position.x = vector.x;
		}
		if (moveY)
		{
			position.y = vector.y;
		}
		position.x = Mathf.Clamp(position.x, Left, Right);
		position.y = Mathf.Clamp(position.y, Bottom, Top);
		_position = Vector3.Lerp(_position, position, (float)CupheadTime.Delta * 2f);
	}

	private void UpdateModeTrapBox()
	{
		Vector3 position = _position;
		Vector3 vector = PlayerManager.CameraCenter;
		if (moveX)
		{
			position.x = vector.x;
		}
		if (moveY)
		{
			position.y = vector.y;
		}
		position.x = Mathf.Clamp(position.x, Left, Right);
		position.y = Mathf.Clamp(position.y, Bottom, Top);
		_position = Vector3.Lerp(_position, position, Time.deltaTime * 2f);
	}

	private void UpdateModeRelative()
	{
		Vector2 vector = _position;
		Vector2 vector2 = new Vector2(0f, 0f);
		vector2.x = MathUtils.GetPercentage(Level.Current.Left, Level.Current.Right, PlayerManager.Center.x);
		vector2.y = MathUtils.GetPercentage(Level.Current.Ground, Level.Current.Ceiling, PlayerManager.Center.y);
		if (moveX)
		{
			vector.x = Mathf.Lerp(Left, Right, vector2.x);
		}
		if (moveY)
		{
			vector.y = Mathf.Lerp(Bottom, Top, vector2.y);
		}
		vector.x = Mathf.Clamp(vector.x, Left, Right);
		vector.y = Mathf.Clamp(vector.y, Bottom, Top);
		_position = Vector3.Lerp(_position, vector, (float)CupheadTime.Delta * 5f);
	}

	private void UpdatePlatforming()
	{
		Vector3 position = _position;
		Vector3 vector = PlayerManager.Center;
		if (moveX && position.x < vector.x)
		{
			position.x = vector.x;
		}
		if (moveY)
		{
			position.y = vector.y;
		}
		position.x = Mathf.Clamp(position.x, Left, Right);
		position.y = Mathf.Clamp(position.y, Bottom, Top);
		_position = Vector3.Lerp(_position, position, (float)CupheadTime.Delta * 5f);
	}

	public void LockCamera(bool lockCamera)
	{
		cameraLocked = lockCamera;
	}

	public void SetAutoScroll(bool isScrolling)
	{
		autoScrolling = isScrolling;
	}

	public void OffsetCamera(bool cameraOffset, bool leftOffset)
	{
		this.cameraOffset = cameraOffset;
		this.leftOffset = leftOffset;
	}

	public void SetAutoscrollSpeedMultiplier(float multiplier)
	{
		autoScrollSpeedMultiplier = multiplier;
	}

	private void UpdatePath()
	{
		Vector3 position = _position;
		Vector2 vector = PlayerManager.Center;
		if (stabilizeY)
		{
			AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
			AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			Vector2 vector2 = ((!(player == null)) ? ((Vector2)player.center) : Vector2.zero);
			Vector2 vector3 = ((!(player2 == null)) ? ((Vector2)player2.center) : Vector2.zero);
			if (vector2.y > position.y + stabilizePaddingTop)
			{
				vector2.y -= stabilizePaddingTop;
			}
			else if (vector2.y < position.y - stabilizePaddingBottom)
			{
				vector2.y += stabilizePaddingBottom;
			}
			else
			{
				vector2.y = position.y;
			}
			if (vector3.y > position.y + stabilizePaddingTop)
			{
				vector3.y -= stabilizePaddingTop;
			}
			else if (vector3.y < position.y - stabilizePaddingBottom)
			{
				vector3.y += stabilizePaddingBottom;
			}
			else
			{
				vector3.y = position.y;
			}
			if (player != null && !player.IsDead && player2 != null && !player2.IsDead)
			{
				vector = (vector2 + vector3) / 2f;
			}
			else if (player != null && !player.IsDead)
			{
				vector = vector2;
			}
			else if (player2 != null && !player2.IsDead)
			{
				vector = vector3;
			}
		}
		if (cameraOffset)
		{
			float num = ((!leftOffset) ? (-500f) : 500f);
			targetPos = new Vector3(vector.x + num, vector.y);
		}
		else
		{
			targetPos = vector;
		}
		Vector3 vector4 = path.GetClosestPoint(_position, targetPos, moveX, moveY);
		float num2 = (vector4 - position).magnitude / (float)CupheadTime.Delta;
		float num3 = Mathf.Max(_speedLastFrame + 5000f * (float)CupheadTime.Delta, 1000f);
		if (num2 > num3)
		{
			vector4 = position + (vector4 - position).normalized * num3 * CupheadTime.Delta;
		}
		_speedLastFrame = Mathf.Min(num2, num3);
		if (pathMovesOnlyForward)
		{
			float closestNormalizedPoint = path.GetClosestNormalizedPoint(_position, vector4, moveX, moveY);
			if (closestNormalizedPoint < _minPathValue)
			{
				return;
			}
		}
		position.x = vector4.x;
		position.y = vector4.y;
		if (!cameraLocked)
		{
			if (!autoScrolling)
			{
				_position = Vector3.Lerp(_position, position, (float)CupheadTime.Delta * 15f);
			}
			else
			{
				Vector3 vector5 = new Vector3(base.transform.position.x + 500f, base.transform.position.y);
				Vector3 target = path.GetClosestPoint(_position, vector5, moveX, moveY);
				float num4 = 200f * autoScrollSpeedMultiplier;
				_position = Vector3.MoveTowards(_position, target, (float)CupheadTime.Delta * num4);
			}
		}
		if (pathMovesOnlyForward)
		{
			_minPathValue = path.GetClosestNormalizedPoint(_position, _position, moveX, moveY);
		}
	}

	public IEnumerator rotate_camera()
	{
		float time = 2f;
		float t = 0f;
		while (true)
		{
			t += (float)CupheadTime.Delta;
			float phase = Mathf.Sin(t / time);
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, phase * 1f));
			yield return null;
		}
	}

	public IEnumerator change_zoom_cr(float newSize, float time)
	{
		float t = 0f;
		while (t < time)
		{
			base.zoom = Mathf.Lerp(t: t / time, a: base.zoom, b: newSize);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.zoom = newSize;
		yield return null;
	}

	public IEnumerator slide_camera_cr(Vector3 slideAmount, float time)
	{
		float t = 0f;
		Vector3 start = _position;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			_position = Vector3.Lerp(start, slideAmount, val);
			yield return null;
		}
	}
}
