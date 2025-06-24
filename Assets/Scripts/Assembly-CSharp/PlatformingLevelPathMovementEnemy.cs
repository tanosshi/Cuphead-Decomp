using System.Collections;
using UnityEngine;

public class PlatformingLevelPathMovementEnemy : AbstractPlatformingLevelEnemy
{
	public enum Direction
	{
		Forward = 1,
		Back = -1
	}

	protected int pathIndex;

	private const float SCREEN_PADDING = 100f;

	public float loopRepeatDelay;

	public float startPosition = 0.5f;

	public VectorPath path;

	[SerializeField]
	protected Direction _direction = Direction.Forward;

	[SerializeField]
	private bool _hasTurnAnimation;

	[SerializeField]
	private bool _hasFacingDirection;

	[SerializeField]
	private EaseUtils.EaseType _easeType = EaseUtils.EaseType.linear;

	protected Vector3 _offset;

	protected bool hasStarted;

	private SpriteRenderer _spriteRenderer;

	private Collider2D _collider;

	private bool _destroyEnemyAfterLeavingScreen;

	private bool _enteredScreen;

	protected float[] allValues { get; private set; }

	protected virtual SpriteRenderer spriteRenderer
	{
		get
		{
			return _spriteRenderer;
		}
	}

	protected virtual Collider2D collider
	{
		get
		{
			return _collider;
		}
	}

	public PlatformingLevelPathMovementEnemy Spawn(Vector3 position, VectorPath path, float startPosition, bool destroyEnemyAfterLeavingScreen)
	{
		PlatformingLevelPathMovementEnemy platformingLevelPathMovementEnemy = InstantiatePrefab<PlatformingLevelPathMovementEnemy>();
		platformingLevelPathMovementEnemy.transform.position = position;
		platformingLevelPathMovementEnemy.startPosition = startPosition;
		platformingLevelPathMovementEnemy.path = path;
		platformingLevelPathMovementEnemy._destroyEnemyAfterLeavingScreen = destroyEnemyAfterLeavingScreen;
		platformingLevelPathMovementEnemy._startCondition = StartCondition.Instant;
		return platformingLevelPathMovementEnemy;
	}

	protected override void Start()
	{
		base.Start();
		_offset = base.transform.position;
		MoveCallback(startPosition);
		_collider = GetComponent<Collider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (base.Properties.MoveLoopMode == EnemyProperties.LoopMode.DelayAtPoint)
		{
			SetUp();
		}
	}

	protected override void OnStart()
	{
		hasStarted = true;
		switch (base.Properties.MoveLoopMode)
		{
		case EnemyProperties.LoopMode.PingPong:
			StartCoroutine(pingpong_cr());
			break;
		case EnemyProperties.LoopMode.Repeat:
			StartCoroutine(repeat_cr());
			break;
		case EnemyProperties.LoopMode.Once:
			StartCoroutine(once_cr());
			break;
		case EnemyProperties.LoopMode.DelayAtPoint:
			StartCoroutine(delay_at_point_cr());
			break;
		default:
			Debug.LogWarning("Mode '" + base.Properties.MoveLoopMode.ToString() + "' not configured");
			break;
		}
	}

	protected override void Update()
	{
		base.Update();
		CalculateCollider();
		CalculateDirection();
		CalculateRender();
	}

	private void CalculateRender()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position) && !_enteredScreen)
		{
			_enteredScreen = true;
		}
		if (_enteredScreen && _destroyEnemyAfterLeavingScreen && !CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 100f)))
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		CalculateCollider();
		CalculateDirection();
	}

	protected virtual void CalculateCollider()
	{
		if (!(collider == null) && !(spriteRenderer == null) && !base.Dead)
		{
			if (spriteRenderer.isVisible)
			{
				collider.enabled = true;
			}
			else
			{
				collider.enabled = false;
			}
		}
	}

	private void CalculateDirection()
	{
		if (_direction == Direction.Forward && _hasFacingDirection)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}
	}

	private void MoveCallback(float value)
	{
		base.transform.position = _offset + path.Lerp(value);
	}

	private float CalculateRemainingTime(float t, Direction d)
	{
		float num = CalculateTime();
		return (d != Direction.Forward) ? (t * num) : ((1f - t) * num);
	}

	private float CalculateTime()
	{
		return path.Distance / base.Properties.MoveSpeed;
	}

	private float CalculatePartTime(int current, int next)
	{
		return Vector3.Distance(path.Points[current], path.Points[next]) / base.Properties.MoveSpeed;
	}

	private Coroutine Turn()
	{
		return StartCoroutine(turn_cr());
	}

	private IEnumerator turn_cr()
	{
		if (_hasTurnAnimation && base.animator != null)
		{
			base.animator.Play("Turn");
			yield return base.animator.WaitForAnimationToEnd(this, "Turn");
		}
		if (_direction == Direction.Forward)
		{
			_direction = Direction.Back;
		}
		else
		{
			_direction = Direction.Forward;
		}
	}

	private IEnumerator pingpong_cr()
	{
		if (_direction == Direction.Back)
		{
			yield return TweenValue(startPosition, 0f, CalculateRemainingTime(startPosition, Direction.Back), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
		}
		else
		{
			yield return TweenValue(startPosition, 1f, CalculateRemainingTime(startPosition, Direction.Forward), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
			yield return TweenValue(1f, 0f, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
		}
		while (true)
		{
			yield return TweenValue(0f, 1f, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
			yield return TweenValue(1f, 0f, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
		}
	}

	private IEnumerator repeat_cr()
	{
		float start = 0f;
		float end = 1f;
		if (_direction == Direction.Back)
		{
			start = 1f;
			end = 0f;
		}
		yield return TweenValue(startPosition, end, CalculateRemainingTime(startPosition, _direction), _easeType, MoveCallback);
		while (true)
		{
			yield return TweenValue(start, end, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
		}
	}

	private void SetUp()
	{
		_easeType = EaseUtils.EaseType.linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		allValues = new float[path.Points.Count];
		for (int i = 0; i < path.Points.Count; i++)
		{
			int index = ((i != 0) ? (i - 1) : 0);
			float f = path.Points[i].y - path.Points[index].y;
			float f2 = path.Points[i].x - path.Points[index].x;
			float num4 = Mathf.Pow(f, 2f);
			float num5 = Mathf.Pow(f2, 2f);
			float f3 = num4 + num5;
			num3 += Mathf.Sqrt(f3);
		}
		for (int j = 0; j < path.Points.Count; j++)
		{
			num2 += num;
			int index2 = ((j != 0) ? (j - 1) : 0);
			float f4 = path.Points[j].y - path.Points[index2].y;
			float f5 = path.Points[j].x - path.Points[index2].x;
			float num6 = Mathf.Pow(f4, 2f);
			float num7 = Mathf.Pow(f5, 2f);
			float f6 = num6 + num7;
			num = Mathf.Sqrt(f6);
			allValues[j] = (num2 + num) / num3;
		}
	}

	private IEnumerator delay_at_point_cr()
	{
		float prevVal = startPosition;
		while (hasStarted)
		{
			yield return TweenValue(prevVal, allValues[pathIndex], CalculatePartTime(pathIndex - 1, pathIndex), _easeType, MoveCallback);
			yield return null;
			if (_hasTurnAnimation)
			{
				base.animator.SetTrigger("Turn");
				yield return base.animator.WaitForAnimationToEnd(this, "Turn");
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			}
			yield return null;
			if (pathIndex == path.Points.Count - 1)
			{
				break;
			}
			prevVal = allValues[pathIndex];
			pathIndex++;
			yield return null;
		}
		EndPath();
		yield return null;
	}

	protected virtual void EndPath()
	{
	}

	private IEnumerator once_cr()
	{
		if (_direction == Direction.Back)
		{
			yield return TweenValue(startPosition, 0f, CalculateRemainingTime(startPosition, Direction.Back), _easeType, MoveCallback);
			Die();
		}
		else
		{
			yield return TweenValue(startPosition, 1f, CalculateRemainingTime(startPosition, Direction.Forward), _easeType, MoveCallback);
			Die();
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		if (Application.isPlaying)
		{
			path.DrawGizmos(a, _offset);
			return;
		}
		path.DrawGizmos(a, base.baseTransform.position);
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Gizmos.DrawSphere(path.Lerp(startPosition) + base.baseTransform.position, 10f);
		Gizmos.DrawWireSphere(path.Lerp(startPosition) + base.baseTransform.position, 11f);
	}
}
