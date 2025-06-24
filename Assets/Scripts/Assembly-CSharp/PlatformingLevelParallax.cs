using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformingLevelParallax : AbstractPausableComponent
{
	public enum Sides
	{
		Background = 0,
		Foreground = 1
	}

	[SerializeField]
	private PlatformingLevel.Theme _theme;

	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private Sides _side;

	[SerializeField]
	[Range(0f, 19f)]
	private int _layer;

	[SerializeField]
	[Range(-2000f, 2000f)]
	private int _sortingOrderOffset;

	[HideInInspector]
	public Vector3 basePos;

	[HideInInspector]
	public Vector3 lastPos;

	public bool overrideLayerYSpeed;

	public float overrideYSpeed;

	private CupheadLevelCamera levelCamera;

	private ParallaxLayer _parallaxLayer;

	private SpriteRenderer[] _s;

	public PlatformingLevel.Theme Theme
	{
		get
		{
			return _theme;
		}
	}

	public Color Color
	{
		get
		{
			return _color;
		}
	}

	public Sides Side
	{
		get
		{
			return _side;
		}
	}

	public int Layer
	{
		get
		{
			return _layer;
		}
	}

	public int SortingOrderOffset
	{
		get
		{
			return _sortingOrderOffset;
		}
	}

	private SpriteRenderer[] _spriteRenderers
	{
		get
		{
			if (_s == null)
			{
				List<SpriteRenderer> list = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
				_s = list.ToArray();
			}
			return _s;
		}
	}

	private ParallaxPropertiesData.ThemeProperties.Layer LayerProperties
	{
		get
		{
			return ParallaxPropertiesData.Instance.GetProperty(_theme, _layer, _side);
		}
	}

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(DelayedStart, 1);
		UpdatePosition();
	}

	private void DelayedStart()
	{
		SetSpriteProperties();
		UpdatePosition();
	}

	public void UpdateBasePosition()
	{
		if (levelCamera == null)
		{
			levelCamera = Object.FindObjectOfType<CupheadLevelCamera>();
			if (levelCamera == null)
			{
				return;
			}
		}
		if (overrideLayerYSpeed)
		{
			basePos.x = base.transform.position.x - levelCamera.transform.position.x * LayerProperties.speed;
			basePos.y = base.transform.position.y - levelCamera.transform.position.y * overrideYSpeed;
		}
		else
		{
			basePos = base.transform.position - levelCamera.transform.position * LayerProperties.speed;
		}
	}

	public void SetSpriteProperties()
	{
		SpriteRenderer[] spriteRenderers = _spriteRenderers;
		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			spriteRenderer.sortingLayerName = ((_side != Sides.Background) ? SpriteLayer.Foreground.ToString() : SpriteLayer.Background.ToString());
			spriteRenderer.sortingOrder = LayerProperties.sortingOrder + _sortingOrderOffset;
			PlatformingLevelParallaxChild component = spriteRenderer.gameObject.GetComponent<PlatformingLevelParallaxChild>();
			if (component != null)
			{
				spriteRenderer.sortingOrder += component.SortingOrderOffset;
			}
			spriteRenderer.color = _color;
		}
	}

	protected override void LateUpdate()
	{
		base.Update();
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (levelCamera == null)
		{
			levelCamera = Object.FindObjectOfType<CupheadLevelCamera>();
			if (levelCamera == null)
			{
				return;
			}
		}
		if (overrideLayerYSpeed)
		{
			base.transform.SetPosition(basePos.x + levelCamera.transform.position.x * LayerProperties.speed, basePos.y + levelCamera.transform.position.y * overrideYSpeed);
		}
		else
		{
			base.transform.position = basePos + levelCamera.transform.position * LayerProperties.speed;
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		SpriteRenderer[] spriteRenderers = _spriteRenderers;
		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.hideFlags = HideFlags.None;
			}
		}
	}
}
