using System.Collections;
using UnityEngine;

public class CupheadMapCamera : AbstractCupheadGameCamera
{
	public bool centerOnPlayer = true;

	private const float SPEED = 6f;

	private const float ORTHO_SIZE = 3.6f;

	private Map.Camera properties;

	private Vector2 offset;

	private EdgeCollider2D edgeCollider;

	public static CupheadMapCamera Current { get; private set; }

	public override float OrthographicSize
	{
		get
		{
			return 3.6f;
		}
	}

	private Vector2 playerCenter
	{
		get
		{
			if (PlayerManager.Multiplayer)
			{
				return (Map.Current.players[0].transform.position + Map.Current.players[1].transform.position) / 2f;
			}
			return Map.Current.players[0].transform.position;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		SetupColliders();
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
		if (Map.Current.CurrentState != Map.State.Event)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = playerCenter;
			if (properties.moveX)
			{
				position.x = vector.x;
			}
			if (properties.moveY)
			{
				position.y = vector.y;
			}
			position.x = Mathf.Clamp(position.x, properties.bounds.left + offset.x, properties.bounds.right - offset.x);
			position.y = Mathf.Clamp(position.y, properties.bounds.bottom + offset.y, properties.bounds.top - offset.y);
			if (centerOnPlayer)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, position, Time.deltaTime * 6f);
			}
			UpdateColliders();
		}
	}

	public void Init(Map.Camera properties)
	{
		base.camera.orthographicSize = 3.6f;
		this.properties = properties;
		offset = new Vector2(base.Bounds.width / 2f, base.Bounds.height / 2f);
		base.transform.position = playerCenter;
	}

	public bool IsCameraFarFromPlayer()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = playerCenter;
		vector.x = Mathf.Clamp(vector.x, properties.bounds.left + offset.x, properties.bounds.right - offset.x);
		vector.y = Mathf.Clamp(vector.y, properties.bounds.bottom + offset.y, properties.bounds.top - offset.y);
		return (double)(position - vector).sqrMagnitude > 0.1;
	}

	public Coroutine MoveToPosition(Vector2 position, float time, float zoom)
	{
		Zoom(zoom, time, EaseUtils.EaseType.easeInOutSine);
		return StartCoroutine(moveToPosition_cr(position, time));
	}

	private IEnumerator moveToPosition_cr(Vector2 position, float time)
	{
		Vector2 start = base.transform.position;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			float x = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start.x, position.x, val);
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start.y, position.y, val), transform: base.transform, x: x, z: 0f);
			t += base.LocalDeltaTime;
			yield return null;
		}
		base.transform.position = position;
		yield return null;
	}

	private void SetupColliders()
	{
		edgeCollider = base.gameObject.AddComponent<EdgeCollider2D>();
		edgeCollider.points = new Vector2[2];
		UpdateColliders();
	}

	private void UpdateColliders()
	{
		Vector2[] points = new Vector2[5]
		{
			new Vector3((0f - base.Bounds.width) / 2f, (0f - base.Bounds.height) / 2f, 0f),
			new Vector3((0f - base.Bounds.width) / 2f, base.Bounds.height / 2f, 0f),
			new Vector3(base.Bounds.width / 2f, base.Bounds.height / 2f, 0f),
			new Vector3(base.Bounds.width / 2f, (0f - base.Bounds.height) / 2f, 0f),
			new Vector3((0f - base.Bounds.width) / 2f, (0f - base.Bounds.height) / 2f, 0f)
		};
		edgeCollider.points = points;
	}

	public void SetActiveCollider(bool active)
	{
		edgeCollider.enabled = active;
	}
}
