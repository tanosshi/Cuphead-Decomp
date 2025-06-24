using UnityEngine;

public class MapZoomOut : AbstractCollidableObject
{
	[SerializeField]
	private CupheadMapCamera _camera;

	[SerializeField]
	private float _maxZoomOut;

	[SerializeField]
	private float ZoomSharpness = 1f;

	private float _startSize;

	private float _maxDistance;

	private float _zoomDistance;

	private float _currentZoomRatio;

	private BoxCollider2D _collider;

	private ContactFilter2D _filter;

	private Collider2D[] buffer = new Collider2D[10];

	protected override void Start()
	{
		base.Start();
		_filter = default(ContactFilter2D).NoFilter();
		_startSize = _camera.camera.orthographicSize;
		_collider = GetComponent<BoxCollider2D>();
		_maxDistance = _collider.bounds.extents.y;
		_zoomDistance = _maxZoomOut - _startSize;
	}

	protected override void Update()
	{
		base.Update();
		int num = _collider.OverlapCollider(_filter, buffer);
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < num; i++)
		{
			MapPlayerController component = buffer[i].GetComponent<MapPlayerController>();
			if (!(component == null))
			{
				num3 += 1f;
				float magnitude = (component.transform.position - base.transform.position).magnitude;
				num2 = ((!(num2 >= magnitude)) ? magnitude : num2);
			}
		}
		if ((PlayerManager.Multiplayer && num3 == 2f) || (!PlayerManager.Multiplayer && num3 == 1f))
		{
			_currentZoomRatio = 1f - Mathf.Clamp(num2 / _maxDistance, 0f, 1f);
		}
		else
		{
			_currentZoomRatio = 0f;
		}
		_camera.camera.orthographicSize = Mathf.Lerp(_camera.camera.orthographicSize, EaseInOutQuad(_startSize, _zoomDistance, _currentZoomRatio), Time.deltaTime * ZoomSharpness);
	}

	private float EaseInOutQuad(float startValue, float endValue, float time)
	{
		time *= 2f;
		if (time < 1f)
		{
			return endValue / 2f * time * time + startValue;
		}
		time -= 1f;
		return (0f - endValue) / 2f * (time * (time - 2f) - 1f) + startValue;
	}
}
