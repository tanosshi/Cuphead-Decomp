using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraAnimationController : MonoBehaviour
{
	public bool CenterOnPlayer = true;

	public float OrthoSize;

	public float Blur;

	private Camera Camera;

	private CupheadMapCamera MapCamera;

	private BlurOptimized BlurOptimized;

	private void Start()
	{
		GameObject gameObject = GameObject.FindWithTag("MainCamera");
		Camera = gameObject.GetComponent<Camera>();
		MapCamera = gameObject.GetComponent<CupheadMapCamera>();
		BlurOptimized = gameObject.GetComponent<BlurOptimized>();
	}

	private void Update()
	{
		ApplyProperties();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		ApplyProperties();
	}

	private void ApplyProperties()
	{
		if (Blur > 0f && !BlurOptimized.enabled)
		{
			BlurOptimized.enabled = true;
		}
		if (Blur <= 0f && BlurOptimized.enabled)
		{
			BlurOptimized.enabled = false;
		}
		MapCamera.centerOnPlayer = CenterOnPlayer;
		Camera.orthographicSize = OrthoSize;
		BlurOptimized.blurSize = Blur;
	}
}
