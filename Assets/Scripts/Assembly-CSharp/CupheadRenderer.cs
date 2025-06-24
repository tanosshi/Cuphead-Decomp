using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CupheadRenderer : AbstractMonoBehaviour
{
	public enum RenderLayer
	{
		None = 0,
		Game = 1,
		UI = 2,
		Loader = 3
	}

	private static CupheadRenderer Instance;

	[SerializeField]
	private CupheadRendererCamera cameraPrefab;

	private CupheadRendererCamera camera;

	private Camera bgCamera;

	private Canvas canvas;

	private Dictionary<RenderLayer, RectTransform> rendererParents;

	private Image background;

	private Image fader;

	protected override void Awake()
	{
		base.Awake();
		if (Instance == null)
		{
			Instance = this;
			Setup();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void Setup()
	{
		camera = Object.Instantiate(cameraPrefab);
		camera.transform.SetParent(base.transform);
		camera.transform.ResetLocalTransforms();
	}
}
