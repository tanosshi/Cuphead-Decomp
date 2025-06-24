using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public abstract class AbstractCupheadCamera : AbstractMonoBehaviour
{
	private Camera _camera;

	public Camera camera
	{
		get
		{
			if (_camera == null)
			{
				_camera = GetComponent<Camera>();
			}
			return _camera;
		}
	}

	public Rect Bounds
	{
		get
		{
			float width = camera.orthographicSize * 1.7777778f * 2f;
			float height = camera.orthographicSize * 2f;
			return RectUtils.NewFromCenter(base.transform.position.x, base.transform.position.y, width, height);
		}
	}

	public bool ContainsPoint(Vector2 point)
	{
		return ContainsPoint(point, Vector2.zero);
	}

	public bool ContainsPoint(Vector2 point, Vector2 padding)
	{
		float width = camera.orthographicSize * 1.7777778f * 2f + padding.x * 2f;
		float height = camera.orthographicSize * 2f + padding.y * 2f;
		return RectUtils.NewFromCenter(base.transform.position.x, base.transform.position.y, width, height).Contains(point);
	}

	protected override void Awake()
	{
		base.Awake();
		camera.clearFlags = CameraClearFlags.Nothing;
	}

	protected override void LateUpdate()
	{
		base.Update();
		UpdateRect();
	}

	public void UpdateRect()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1f - 0.1f * SettingsData.Data.overscan;
		Rect rect = ((!(num > 1.7777778f)) ? RectUtils.NewFromCenter(0.5f, 0.5f, num2 * 1f, num2 * num / 1.7777778f) : RectUtils.NewFromCenter(0.5f, 0.5f, num2 * 1.7777778f / num, num2 * 1f));
		if (camera.rect != rect)
		{
			camera.rect = rect;
			CanvasScaler[] array = Object.FindObjectsOfType<CanvasScaler>();
			CanvasScaler[] array2 = array;
			foreach (CanvasScaler canvasScaler in array2)
			{
				canvasScaler.referenceResolution = new Vector2(1280f / rect.height, 720f / rect.height);
			}
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.1f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Gizmos.color = Color.white * new Color(1f, 1f, 1f, a);
		Gizmos.DrawWireCube(camera.transform.position, new Vector3(camera.orthographicSize * camera.aspect * 2f, camera.orthographicSize * 2f, 0f));
		Gizmos.DrawWireSphere(camera.transform.position, 50f);
		Gizmos.DrawSphere(camera.transform.position, 10f);
	}
}
