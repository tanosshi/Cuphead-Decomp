using UnityEngine;

public class MapParallaxLayer : AbstractPausableComponent
{
	[Range(-3f, 3f)]
	public float percentage;

	[SerializeField]
	private Vector3 _cameraStartPosition;

	private CupheadMapCamera _camera;

	private Vector3 _startPosition;

	private Vector2 _offset
	{
		get
		{
			return _startPosition - _cameraStartPosition;
		}
	}

	protected override void Start()
	{
		base.Start();
		_camera = CupheadMapCamera.Current;
		_startPosition = base.transform.position;
	}

	protected override void LateUpdate()
	{
		base.Update();
		UpdateComparative();
	}

	private void UpdateComparative()
	{
		Vector3 position = base.transform.position;
		position.x = _offset.x + _camera.transform.position.x * percentage;
		position.y = _offset.y + _camera.transform.position.y * percentage;
		base.transform.position = position;
	}
}
