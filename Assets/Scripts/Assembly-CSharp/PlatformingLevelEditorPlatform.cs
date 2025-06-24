using UnityEngine;

public class PlatformingLevelEditorPlatform : AbstractMonoBehaviour
{
	public enum Type
	{
		Platform = 0,
		Solid = 1
	}

	private const int THICKNESS = 20;

	[SerializeField]
	private Type _type;

	[SerializeField]
	private bool _canFallThrough;

	[SerializeField]
	private Vector2 _size = new Vector2(100f, 10f);

	[SerializeField]
	private Vector2 _offset = new Vector2(0f, 0f);

	private LevelPlatform _platform;

	private BoxCollider2D _collider;

	private BoxCollider2D _topCollider;

	private BoxCollider2D _middleCollider;

	private BoxCollider2D _bottomCollider;

	protected override void Awake()
	{
		base.Awake();
		switch (_type)
		{
		case Type.Platform:
			_collider = base.gameObject.AddComponent<BoxCollider2D>();
			_collider.size = _size;
			_collider.offset = _offset;
			_collider.isTrigger = true;
			_platform = base.gameObject.AddComponent<LevelPlatform>();
			_platform.canFallThrough = _canFallThrough;
			break;
		case Type.Solid:
		{
			GameObject gameObject = new GameObject("ground");
			GameObject gameObject2 = new GameObject("walls");
			GameObject gameObject3 = new GameObject("ceiling");
			gameObject.layer = 20;
			gameObject.tag = Tags.Ground.ToString();
			gameObject2.layer = 18;
			gameObject2.tag = Tags.Wall.ToString();
			gameObject3.layer = 19;
			gameObject3.tag = Tags.Ceiling.ToString();
			gameObject.transform.SetParent(base.transform);
			gameObject2.transform.SetParent(base.transform);
			gameObject3.transform.SetParent(base.transform);
			gameObject.transform.ResetLocalTransforms();
			gameObject2.transform.ResetLocalTransforms();
			gameObject3.transform.ResetLocalTransforms();
			_topCollider = gameObject.AddComponent<BoxCollider2D>();
			_middleCollider = gameObject2.AddComponent<BoxCollider2D>();
			_bottomCollider = gameObject3.AddComponent<BoxCollider2D>();
			_topCollider.isTrigger = true;
			_middleCollider.isTrigger = true;
			_bottomCollider.isTrigger = true;
			_topCollider.size = new Vector2(_size.x, 20f);
			_middleCollider.size = _size - new Vector2(0f, 40f);
			_bottomCollider.size = new Vector2(_size.x, 20f);
			_topCollider.offset = new Vector2(0f, _size.y / 2f - _topCollider.size.y / 2f) + _offset;
			_middleCollider.offset = Vector2.zero + _offset;
			_bottomCollider.offset = new Vector2(0f, 0f - (_size.y / 2f - _bottomCollider.size.y / 2f)) + _offset;
			break;
		}
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.5f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector2 vector = Vector2.zero + _offset;
		Gizmos.color = new Color(0f, 0f, 0f, 0.4f * a);
		Gizmos.DrawCube(vector, _size);
		Gizmos.color = Color.cyan * new Color(1f, 1f, 1f, a);
		switch (_type)
		{
		case Type.Platform:
		{
			float num = vector.y + _size.y / 2f;
			float num2 = vector.y - _size.y / 2f;
			float y = num - 10f;
			float x = vector.x - _size.x / 2f;
			float x2 = vector.x + _size.x / 2f;
			Gizmos.DrawLine(new Vector2(x, num), new Vector2(x2, num));
			if (!_canFallThrough)
			{
				Gizmos.DrawLine(new Vector2(x, y), new Vector2(x2, y));
				break;
			}
			Gizmos.DrawLine(new Vector2(vector.x, num2 + 50f), new Vector2(vector.x, num2));
			Gizmos.DrawLine(new Vector2(vector.x - 20f, num2 + 20f), new Vector2(vector.x, num2));
			Gizmos.DrawLine(new Vector2(vector.x + 20f, num2 + 20f), new Vector2(vector.x, num2));
			break;
		}
		case Type.Solid:
			Gizmos.DrawWireCube(vector, _size);
			break;
		}
		Gizmos.matrix = matrix;
	}
}
