using System.Collections;
using UnityEngine;

public class PlatformingLevelGroundMovementEnemy : AbstractPlatformingLevelEnemy
{
	public enum Direction
	{
		Right = 1,
		Left = -1
	}

	private enum RaycastAxis
	{
		X = 0,
		Y = 1
	}

	public class DirectionManager
	{
		public class Hit
		{
			public bool able = true;

			public Vector2 pos = Vector2.zero;

			public GameObject gameObject;

			public float distance = -1f;

			public Hit()
			{
			}

			public Hit(bool able, Vector2 pos, GameObject gameObject, float distance)
			{
				this.able = able;
				this.pos = pos;
				this.gameObject = gameObject;
				this.distance = distance;
			}
		}

		public Hit up;

		public Hit down;

		public Hit left;

		public Hit right;

		public void Reset()
		{
			up = new Hit();
			down = new Hit();
			left = new Hit();
			right = new Hit();
		}
	}

	public class JumpManager
	{
		public enum State
		{
			Ready = 0,
			Used = 1
		}

		public State state;

		public bool ableToLand;
	}

	public class PlatformManager
	{
		private PlatformingLevelGroundMovementEnemy motor;

		public bool OnPlatform
		{
			get
			{
				return motor.transform.parent != null;
			}
		}

		public PlatformManager(PlatformingLevelGroundMovementEnemy motor)
		{
			this.motor = motor;
		}
	}

	private const float SCREEN_PADDING = 100f;

	private const float DOWN_BOXCAST_Y = 30f;

	public float startPosition = 0.5f;

	[SerializeField]
	protected Direction _direction = Direction.Right;

	[SerializeField]
	private bool hasJumpAnimation;

	[SerializeField]
	private bool hasTurnAnimation;

	[SerializeField]
	private bool canSpawnOnPlatforms;

	[SerializeField]
	private float turnaroundDistance = 10f;

	[SerializeField]
	private Transform shadow;

	[SerializeField]
	private float maxShadowDistance;

	[SerializeField]
	private Effect jumpLandEffectPrefab;

	[SerializeField]
	protected bool noTurn;

	[SerializeField]
	protected bool lockDirectionWhenLanding;

	[SerializeField]
	protected bool gravityReversed;

	private Collider2D _collider;

	private bool _destroyEnemyAfterLeavingScreen;

	private bool _enteredScreen;

	private DirectionManager directionManager;

	private JumpManager jumpManager;

	private PlatformManager platformManager;

	protected bool turning;

	protected bool floating;

	protected bool manuallySetJumpX;

	protected float timeSinceTurn;

	private string turnTarget;

	private float moveSpeed;

	private bool jumping;

	protected bool landing;

	protected bool fallInPit;

	private Vector2 velocity = Vector2.zero;

	private RaycastHit2D[] hits;

	private const float RAY_DISTANCE = 2000f;

	private const float MAX_GROUNDED_FALL_DISTANCE = 30f;

	private readonly int wallMask = 262144;

	private readonly int ceilingMask = 524288;

	private readonly int groundMask = 1048576;

	public Direction direction
	{
		get
		{
			return _direction;
		}
	}

	public bool Grounded { get; private set; }

	protected virtual Collider2D collider
	{
		get
		{
			return _collider;
		}
	}

	public PlatformingLevelGroundMovementEnemy Spawn(Vector3 position, Direction startDirection, bool destroyEnemyAfterLeavingScreen)
	{
		PlatformingLevelGroundMovementEnemy platformingLevelGroundMovementEnemy = InstantiatePrefab<PlatformingLevelGroundMovementEnemy>();
		platformingLevelGroundMovementEnemy.transform.position = position;
		platformingLevelGroundMovementEnemy._destroyEnemyAfterLeavingScreen = destroyEnemyAfterLeavingScreen;
		platformingLevelGroundMovementEnemy._startCondition = StartCondition.Instant;
		platformingLevelGroundMovementEnemy._direction = startDirection;
		return platformingLevelGroundMovementEnemy;
	}

	protected override void Awake()
	{
		base.Awake();
		_collider = GetComponent<Collider2D>();
		directionManager = new DirectionManager();
		jumpManager = new JumpManager();
		platformManager = new PlatformManager(this);
		timeSinceTurn = 10000f;
		if (shadow != null)
		{
			shadow.parent = null;
		}
		SetTurnTarget("Turn");
	}

	protected override void OnStart()
	{
	}

	public void GoToGround(bool despawnOnPit = true)
	{
		base.animator.Play("Run");
		Bounds bounds = collider.bounds;
		Vector2 vector = bounds.center - base.transform.position;
		if (!gravityReversed)
		{
			hits = BoxCastAll(new Vector2(bounds.size.x, 1f), Vector2.down, groundMask, new Vector2(0f, (0f - bounds.size.y) / 4f));
		}
		else
		{
			hits = BoxCastAll(new Vector2(bounds.size.x, 1f), Vector2.up, ceilingMask, new Vector2(0f, bounds.size.y));
		}
		Vector2 vector2 = base.transform.position;
		bool flag = false;
		RaycastHit2D[] array = hits;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit2D raycastHit2D = array[i];
			LevelPlatform component = raycastHit2D.collider.gameObject.GetComponent<LevelPlatform>();
			if (raycastHit2D.collider != null && (canSpawnOnPlatforms || component == null || !component.canFallThrough))
			{
				vector2 = raycastHit2D.point;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Object.Destroy(base.gameObject);
		}
		base.transform.SetPosition(null, vector2.y);
		HandleRaycasts();
		jumpManager.ableToLand = true;
		OnGrounded();
		if (!despawnOnPit)
		{
			return;
		}
		Vector2 a = (Vector2)base.transform.position + vector + new Vector2((direction != Direction.Left) ? ((0f - bounds.size.x) / 2f) : (bounds.size.x / 2f), turnaroundDistance / 2f - bounds.size.y / 2f);
		Vector2 b = (Vector2)base.transform.position + vector + new Vector2((direction != Direction.Left) ? turnaroundDistance : (0f - turnaroundDistance), turnaroundDistance / 2f - bounds.size.y / 2f);
		for (int j = 0; j <= 10; j++)
		{
			float t = (float)j / 10f;
			if (!gravityReversed)
			{
				if (Physics2D.Raycast(Vector2.Lerp(a, b, t), Vector2.down, 30f + turnaroundDistance, groundMask).collider == null)
				{
					Object.Destroy(base.gameObject);
					break;
				}
			}
			else if (Physics2D.Raycast(Vector2.Lerp(a, b, t), Vector2.up, 30f + turnaroundDistance, ceilingMask).collider == null)
			{
				Object.Destroy(base.gameObject);
				break;
			}
		}
	}

	public void Float()
	{
		base.animator.Play("Float", 0, Random.Range(0f, 1f));
		floating = true;
	}

	protected override void Update()
	{
		base.Update();
		CalculateDirection();
		CalculateRender();
		if (shadow != null)
		{
			UpdateShadow();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.Dead && !GetComponent<DamageReceiver>().IsHitPaused)
		{
			HandleRaycasts();
			HandleFalling();
			Move();
		}
	}

	private void HandleFalling()
	{
		if (!Grounded)
		{
			if (floating)
			{
				velocity = new Vector2(0f, 0f - base.Properties.floatSpeed);
			}
			else if (!gravityReversed)
			{
				velocity.y -= base.Properties.gravity * CupheadTime.FixedDelta;
				jumpManager.ableToLand = velocity.y < 0f;
			}
			else
			{
				velocity.y += base.Properties.gravity * CupheadTime.FixedDelta;
				jumpManager.ableToLand = velocity.y < 0f;
			}
		}
	}

	protected virtual float GetMoveSpeed()
	{
		if (moveSpeed == 0f)
		{
			moveSpeed = base.Properties.MoveSpeed;
		}
		return moveSpeed;
	}

	protected virtual void SetMoveSpeed(float moveSpeed)
	{
		this.moveSpeed = moveSpeed;
	}

	private void Move()
	{
		if (turning || landing || (jumping && Grounded))
		{
			return;
		}
		timeSinceTurn += CupheadTime.FixedDelta;
		float num = ((_direction == Direction.Right) ? 1 : (-1));
		if (jumpManager.state == JumpManager.State.Ready && !floating)
		{
			velocity.x = GetMoveSpeed() * num;
		}
		base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		if (!gravityReversed)
		{
			if (Grounded && base.transform.position.y - directionManager.down.pos.y < 30f)
			{
				Vector2 vector = base.transform.position;
				vector.y = directionManager.down.pos.y;
				base.transform.position = vector;
			}
		}
		else if (Grounded && base.transform.position.y + directionManager.up.pos.y > 30f)
		{
			Vector2 vector2 = base.transform.position;
			vector2.y = directionManager.up.pos.y;
			base.transform.position = vector2;
		}
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
		if (PlatformingLevel.Current != null && (base.transform.position.x < (float)PlatformingLevel.Current.Left - 100f || base.transform.position.x > (float)PlatformingLevel.Current.Right + 100f || base.transform.position.y < (float)PlatformingLevel.Current.Ground - 100f))
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		CalculateDirection();
	}

	protected virtual void CalculateDirection()
	{
		if (_direction == Direction.Right)
		{
			base.transform.SetScale(-1f);
		}
		else
		{
			base.transform.SetScale(1f);
		}
	}

	protected override void Die()
	{
		if (shadow != null)
		{
			Object.Destroy(shadow.gameObject);
		}
		base.Die();
	}

	protected virtual Coroutine Turn()
	{
		turning = true;
		timeSinceTurn = 0f;
		return StartCoroutine(turn_cr());
	}

	private IEnumerator turn_cr()
	{
		if (hasTurnAnimation && base.animator != null)
		{
			base.animator.Play("Turn");
			int target = Animator.StringToHash(base.animator.GetLayerName(0) + "." + turnTarget);
			while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != target)
			{
				yield return null;
			}
			float animLength = base.animator.GetCurrentAnimatorStateInfo(0).length;
			while (base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= (animLength - (float)CupheadTime.Delta) / animLength)
			{
				yield return null;
			}
		}
		if (_direction == Direction.Right)
		{
			_direction = Direction.Left;
		}
		else
		{
			_direction = Direction.Right;
		}
		CalculateDirection();
		turning = false;
	}

	protected virtual void SetTurnTarget(string turnTarget)
	{
		this.turnTarget = turnTarget;
	}

	private IEnumerator floatLand_cr()
	{
		floating = false;
		landing = true;
		if (!lockDirectionWhenLanding)
		{
			_direction = ((PlayerManager.GetNext().center.x > base.transform.position.x) ? Direction.Right : Direction.Left);
		}
		base.animator.Play("Land");
		base.transform.SetPosition(null, directionManager.down.pos.y);
		yield return base.animator.WaitForAnimationToEnd(this, "Land");
		velocity.y = 0f;
		landing = false;
	}

	public void Jump()
	{
		if (base.Properties.canJump && jumpManager.state == JumpManager.State.Ready && !turning)
		{
			jumpManager.state = JumpManager.State.Used;
			StartCoroutine(jump_cr());
			jumping = true;
		}
	}

	private IEnumerator jump_cr()
	{
		if (hasJumpAnimation && base.animator != null)
		{
			base.animator.Play("Jump");
			int target = Animator.StringToHash(base.animator.GetLayerName(0) + ".Jump");
			while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != target)
			{
				yield return null;
			}
			float animLength = base.animator.GetCurrentAnimatorStateInfo(0).length;
			while (base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= (animLength - (float)CupheadTime.Delta) / animLength)
			{
				yield return null;
			}
		}
		float directionSign = ((_direction == Direction.Right) ? 1 : (-1));
		float timeToApex = Mathf.Sqrt(2f * base.Properties.jumpHeight / base.Properties.gravity);
		float x = ((!manuallySetJumpX) ? base.Properties.jumpLength : GetMoveSpeed());
		LeaveGround();
		velocity.y = base.Properties.gravity * timeToApex;
		velocity.x = directionSign * x / (2f * timeToApex);
		if (hasJumpAnimation && base.animator != null)
		{
			yield return CupheadTime.WaitForSeconds(this, timeToApex);
			base.animator.SetTrigger("Apex");
		}
		while (jumping)
		{
			yield return null;
		}
		landing = true;
		if (directionManager.down != null)
		{
			base.transform.SetPosition(null, directionManager.down.pos.y);
		}
		if (jumpLandEffectPrefab != null)
		{
			jumpLandEffectPrefab.Create(base.transform.position);
		}
		if (hasJumpAnimation && base.animator != null)
		{
			base.animator.SetTrigger("Land");
			yield return base.animator.WaitForAnimationToEnd(this, "Jump_Land");
		}
		landing = false;
	}

	private RaycastHit2D BoxCast(Vector2 size, Vector2 direction, int layerMask)
	{
		return BoxCast(size, direction, layerMask, Vector2.zero);
	}

	private RaycastHit2D BoxCast(Vector2 size, Vector2 direction, int layerMask, Vector2 offset)
	{
		return Physics2D.BoxCast((Vector2)collider.bounds.center + offset, size, 0f, direction, 2000f, layerMask);
	}

	private RaycastHit2D[] BoxCastAll(Vector2 size, Vector2 direction, int layerMask, Vector2 offset)
	{
		return Physics2D.BoxCastAll((Vector2)collider.bounds.center + offset, size, 0f, direction, 2000f, layerMask);
	}

	private RaycastHit2D CircleCast(float radius, Vector2 direction, int layerMask)
	{
		return Physics2D.CircleCast(collider.bounds.center, radius, direction, 2000f, layerMask);
	}

	private bool DoesRaycastHitHaveCollider(RaycastHit2D hit)
	{
		return hit.collider != null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
		}
	}

	private void HandleRaycasts()
	{
		if (fallInPit)
		{
			return;
		}
		bool flag = true;
		if (directionManager != null && directionManager.up != null)
		{
			flag = directionManager.up.able;
		}
		Bounds bounds = collider.bounds;
		directionManager.Reset();
		RaycastHit2D raycastHit = BoxCast(new Vector2(bounds.size.x, 1f), Vector2.up, ceilingMask);
		RaycastHit2D raycastHit2 = BoxCast(new Vector2(bounds.size.x, 1f), Vector2.down, groundMask, new Vector2(base.transform.position.x - bounds.center.x, base.transform.position.y + 30f - bounds.center.y));
		RaycastHit2D raycastHit2D = Physics2D.Raycast((Vector2)base.transform.position + new Vector2((direction != Direction.Left) ? turnaroundDistance : (0f - turnaroundDistance), turnaroundDistance / 2f), Vector2.down, 30f + turnaroundDistance, groundMask);
		RaycastHit2D raycastHit2D2 = Physics2D.Raycast((Vector2)base.transform.position + new Vector2((direction != Direction.Left) ? turnaroundDistance : (0f - turnaroundDistance), turnaroundDistance / 2f), Vector2.up, 30f + turnaroundDistance, ceilingMask);
		RaycastObstacle(directionManager.up, raycastHit, bounds.size.y / 2f, RaycastAxis.Y, bounds.center);
		RaycastObstacle(directionManager.down, raycastHit2, 30f, RaycastAxis.Y, new Vector2(base.transform.position.x, base.transform.position.y + 30f));
		if (!Grounded)
		{
			if (!directionManager.down.able)
			{
				OnGrounded();
				directionManager.left.able = true;
				directionManager.right.able = true;
				if (floating)
				{
					StartCoroutine(floatLand_cr());
				}
			}
			if (!directionManager.up.able && directionManager.up.able != flag)
			{
				OnHitCeiling();
			}
		}
		RaycastHit2D raycastHit2D3 = (gravityReversed ? raycastHit2D2 : raycastHit2D);
		if (Grounded && raycastHit2D3.collider == null && timeSinceTurn > 0.1f && !jumping)
		{
			if (!noTurn)
			{
				Turn();
			}
			else
			{
				LeaveGround();
			}
		}
	}

	private float RaycastObstacle(DirectionManager.Hit directionProperties, RaycastHit2D raycastHit, float maxDistance, RaycastAxis axis, Vector2 origin)
	{
		if (!DoesRaycastHitHaveCollider(raycastHit))
		{
			return 1000f;
		}
		float num = ((axis != RaycastAxis.X) ? Mathf.Abs(origin.y - raycastHit.point.y) : Mathf.Abs(origin.x - raycastHit.point.x));
		directionProperties.pos = raycastHit.point;
		directionProperties.gameObject = raycastHit.collider.gameObject;
		directionProperties.distance = num;
		if (num < maxDistance)
		{
			directionProperties.able = false;
		}
		return num;
	}

	private void ValidateRaycast()
	{
	}

	private void OnGrounded()
	{
		if (!Grounded && jumpManager.ableToLand)
		{
			LevelPlatform levelPlatform = ((!(directionManager.down.gameObject == null)) ? directionManager.down.gameObject.GetComponent<LevelPlatform>() : null);
			LevelPlatform levelPlatform2 = ((!(directionManager.up.gameObject == null)) ? directionManager.up.gameObject.GetComponent<LevelPlatform>() : null);
			LevelPlatform levelPlatform3 = (gravityReversed ? levelPlatform2 : levelPlatform);
			if (levelPlatform3 != null)
			{
				levelPlatform3.AddChild(base.transform);
			}
			jumpManager.state = JumpManager.State.Ready;
			velocity.y = 0f;
			Grounded = true;
			if (jumping)
			{
				jumping = false;
			}
		}
	}

	private void LeaveGround()
	{
		Grounded = false;
		jumpManager.ableToLand = false;
		velocity.y = 0f;
		ClearParent();
		if (jumpManager.state == JumpManager.State.Ready)
		{
			jumpManager.state = JumpManager.State.Used;
		}
	}

	private void OnHitCeiling()
	{
		if (!gravityReversed)
		{
			if (jumpManager.ableToLand)
			{
				return;
			}
		}
		else
		{
			if (Grounded)
			{
				return;
			}
			jumpManager.state = JumpManager.State.Ready;
			LevelPlatform levelPlatform = ((!(directionManager.up.gameObject == null)) ? directionManager.up.gameObject.GetComponent<LevelPlatform>() : null);
			if (levelPlatform != null)
			{
				levelPlatform.AddChild(base.transform);
			}
			Grounded = true;
			if (jumping)
			{
				jumping = false;
			}
		}
		velocity.y = 0f;
		directionManager.left.able = true;
		directionManager.right.able = true;
	}

	private void ClearParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent.GetComponent<LevelPlatform>().OnPlayerExit(base.transform);
		}
		base.transform.parent = null;
	}

	private void UpdateShadow()
	{
		if (Grounded)
		{
			shadow.gameObject.SetActive(false);
			return;
		}
		RaycastHit2D raycastHit2D = Physics2D.BoxCast(base.transform.position, new Vector2(collider.bounds.size.x, 1f), 0f, Vector2.down, maxShadowDistance, groundMask);
		if (raycastHit2D.collider == null)
		{
			shadow.gameObject.SetActive(false);
			return;
		}
		shadow.gameObject.SetActive(true);
		shadow.SetPosition(base.transform.position.x, raycastHit2D.point.y);
		float num = base.transform.position.y - shadow.position.y;
		shadow.GetComponent<Animator>().Play("Idle", 0, num / maxShadowDistance);
		shadow.GetComponent<Animator>().speed = 0f;
	}
}
