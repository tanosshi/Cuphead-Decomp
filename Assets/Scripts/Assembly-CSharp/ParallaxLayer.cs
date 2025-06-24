using UnityEngine;

public class ParallaxLayer : AbstractPausableComponent
{
	public enum Type
	{
		MinMax = 0,
		Comparative = 1,
		Centered = 2
	}

	public Type type;

	[Range(-3f, 3f)]
	public float percentage;

	public Vector2 bottomLeft;

	public Vector2 topRight;

	private CupheadLevelCamera _camera;

	private bool _initialized;

	private Vector3 _startPosition;

	private Vector3 _cameraStartPosition;

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
		_camera = CupheadLevelCamera.Current;
		_startPosition = base.transform.position;
		_cameraStartPosition = _camera.transform.position;
	}

	protected override void LateUpdate()
	{
		base.Update();
		switch (type)
		{
		default:
			UpdateComparative();
			break;
		case Type.MinMax:
			UpdateMinMax();
			break;
		case Type.Centered:
			UpdateCentered();
			break;
		}
	}

	private void UpdateComparative()
	{
		Vector3 position = base.transform.position;
		position.x = _offset.x + _camera.transform.position.x * percentage;
		position.y = _offset.y + _camera.transform.position.y * percentage;
		base.transform.position = position;
	}

	private void UpdateMinMax()
	{
		Vector3 position = base.transform.position;
		Vector2 vector = _camera.transform.position;
		Vector2 zero = Vector2.zero;
		float num = vector.x + Mathf.Abs(_camera.Left);
		float num2 = _camera.Right + Mathf.Abs(_camera.Left);
		float num3 = vector.y + Mathf.Abs(_camera.Bottom);
		float num4 = _camera.Top + Mathf.Abs(_camera.Bottom);
		zero.x = num / num2;
		zero.y = num3 / num4;
		if (float.IsNaN(zero.x))
		{
			zero.x = 0.5f;
		}
		if (float.IsNaN(zero.y))
		{
			zero.y = 0.5f;
		}
		position.x = Mathf.Lerp(bottomLeft.x, topRight.x, zero.x) + _camera.transform.position.x;
		position.y = Mathf.Lerp(bottomLeft.y, topRight.y, zero.y) + _camera.transform.position.y;
		base.transform.position = position;
	}

	private void UpdateCentered()
	{
		Vector3 position = base.transform.position;
		position.x = _startPosition.x + (_camera.transform.position.x - _startPosition.x) * percentage;
		position.y = _startPosition.y + (_camera.transform.position.y - _startPosition.y) * percentage;
		base.transform.position = position;
	}
}
