using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayerMotor : AbstractLevelPlayerComponent
{
	private enum RaycastAxis
	{
		X = 0,
		Y = 1
	}

	public enum BufferedInput
	{
		Jump = 0,
		Dash = 1,
		Super = 2
	}

	public class Properties
	{
		public float moveSpeed = 490f;

		public float maxSpeedY = 1620f;

		public float timeToMaxY = 7.3f;

		public EaseUtils.EaseType yEase = EaseUtils.EaseType.linear;

		public float jumpHoldMin = 0.01f;

		public float jumpHoldMax = 0.16f;

		[Range(0f, -1f)]
		public float jumpPower = -0.755f;

		private float trampolineJumpPower = -3f;

		public float dashSpeed = 1100f;

		public float dashTime = 0.3f;

		public float dashEndTime = 0.21f;

		public EaseUtils.EaseType dashEase = EaseUtils.EaseType.easeOutSine;

		public float platformIgnoreTime = 1f;

		public float hitStunTime = 0.3f;

		public float hitFalloff = 0.25f;

		[Range(0f, -1f)]
		public float hitJumpPower = -0.6f;

		public float hitKnockbackPower = 300f;

		public EaseUtils.EaseType hitKnockbackEase = EaseUtils.EaseType.linear;

		public float knockUpStunTime = 0.2f;

		[Range(0f, -3f)]
		public float pitKnockUpPower = -1.5f;

		[Range(0f, -3f)]
		public float platformingPitKnockUpPower = -1.5f;

		public float parryPower = -1f;

		public float parryAttackBounce = -1f;

		public float deathSpeed = 5f;

		public float reviveKnockUpPower = -1f;

		public float exKnockback = 230f;

		public float superKnockUp = -0.6f;

		public float superInvincibleKnockUp = -1.2f;
	}

	public class VelocityManager
	{
		public class Force
		{
			public enum Type
			{
				All = 0,
				Ground = 1,
				Air = 2
			}

			public bool yAxisForce;

			public bool enabled = true;

			public readonly Type type;

			public float value;

			public Force()
			{
				type = Type.All;
				value = 0f;
			}

			public Force(Type type)
			{
				this.type = type;
				value = 0f;
			}

			public Force(Type type, float force)
			{
				this.type = type;
				value = force;
			}

			public Force(Type type, float force, bool yAxis)
			{
				this.type = type;
				value = force;
				yAxisForce = yAxis;
			}
		}

		public float move;

		public float dash;

		public float hit;

		private List<Force> forces;

		private EaseUtils.EaseType yEase;

		private float maxY;

		private float _y;

		public bool yAxisForce { get; private set; }

		public float GroundForce { get; private set; }

		public float AirForce { get; private set; }

		public float y
		{
			get
			{
				_y = Mathf.Clamp(_y, -10f, 1f);
				return _y;
			}
			set
			{
				_y = Mathf.Clamp(value, -10f, 1f);
			}
		}

		public Vector2 Total
		{
			get
			{
				float value = y / 2f + 0.5f;
				Vector2 result = new Vector2
				{
					y = EaseUtils.Ease(yEase, maxY, 0f - maxY, value)
				};
				result.x += move + dash + hit;
				return result;
			}
		}

		public VelocityManager(LevelPlayerMotor motor, float maxY, EaseUtils.EaseType yEase)
		{
			this.maxY = maxY;
			this.yEase = yEase;
			forces = new List<Force>();
		}

		public void Calculate()
		{
			GroundForce = 0f;
			AirForce = 0f;
			foreach (Force force in forces)
			{
				if (force.enabled)
				{
					switch (force.type)
					{
					case Force.Type.All:
						AirForce += force.value;
						GroundForce += force.value;
						break;
					case Force.Type.Air:
						AirForce += force.value;
						break;
					case Force.Type.Ground:
						GroundForce += force.value;
						break;
					}
					if (force.yAxisForce)
					{
						yAxisForce = true;
					}
				}
			}
		}

		public void Clear()
		{
			move = 0f;
			dash = 0f;
			hit = 0f;
			y = 0f;
		}

		public void AddForce(Force force)
		{
			if (!forces.Contains(force))
			{
				forces.Add(force);
			}
		}

		public void RemoveForce(Force force)
		{
			yAxisForce = false;
			if (forces.Contains(force))
			{
				forces.Remove(force);
			}
		}
	}

	public class JumpManager
	{
		public enum State
		{
			Ready = 0,
			Hold = 1,
			Used = 2
		}

		public State state;

		public float timer;

		public float timeSinceDownJump = 1000f;

		public float timeInAir;

		public float longestTimeInAir;

		public bool ableToLand;
	}

	public class DashManager
	{
		public enum State
		{
			Ready = 0,
			Start = 1,
			Dashing = 2,
			Ending = 3,
			End = 4
		}

		public State state;

		public int direction;

		public float timer;

		public const float DASH_COOLDOWN_DURATION = 0.1f;

		public float timeSinceGroundDash = 0.1f;

		public bool groundDash;

		public bool IsDashing
		{
			get
			{
				State state = this.state;
				if (state == State.Start || state == State.Dashing || state == State.Ending)
				{
					return true;
				}
				return false;
			}
		}
	}

	public class ParryManager
	{
		public enum State
		{
			Ready = 0,
			NotReady = 1
		}

		public State state;
	}

	public class PlatformManager
	{
		private List<Transform> ignoredPlatforms;

		private LevelPlayerMotor motor;

		private IEnumerator ignoreCoroutine;

		public bool OnPlatform
		{
			get
			{
				return motor.transform.parent != null;
			}
		}

		public PlatformManager(LevelPlayerMotor motor)
		{
			ignoredPlatforms = new List<Transform>();
			this.motor = motor;
		}

		public void Ignore(Transform platform)
		{
			StopCoroutine();
			ignoreCoroutine = ignorePlatform_cr(platform);
			motor.StartCoroutine(ignoreCoroutine);
		}

		public void StopCoroutine()
		{
			if (ignoreCoroutine != null)
			{
				motor.StopCoroutine(ignoreCoroutine);
			}
			ignoreCoroutine = null;
		}

		public void Add(Transform platform)
		{
			ignoredPlatforms.Add(platform);
		}

		public void Remove(Transform platform)
		{
			ignoredPlatforms.Remove(platform);
		}

		public bool IsPlatformIgnored(Transform platform)
		{
			return ignoredPlatforms.Contains(platform);
		}

		public void ResetAll()
		{
			StopCoroutine();
			ignoredPlatforms = new List<Transform>();
		}

		private IEnumerator ignorePlatform_cr(Transform platform)
		{
			Add(platform);
			yield return CupheadTime.WaitForSeconds(motor, motor.properties.platformIgnoreTime);
			Remove(platform);
		}
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

		public DirectionManager()
		{
			Reset();
		}

		public void Reset()
		{
			up = new Hit();
			down = new Hit();
			left = new Hit();
			right = new Hit();
		}
	}

	public class HitManager
	{
		public enum State
		{
			Inactive = 0,
			Hit = 1,
			KnockedUp = 2
		}

		public State state;

		public float timer;

		public int direction;

		public void Reset()
		{
			state = State.Inactive;
			timer = 0f;
			direction = 0;
		}
	}

	public class SuperManager
	{
		public enum State
		{
			Ready = 0,
			Ex = 1,
			Super = 2
		}

		public State state;
	}

	public class BoundsManager
	{
		private readonly Transform transform;

		private BoxCollider2D boxCollider;

		public LevelPlayerMotor Motor { get; set; }

		public Vector3 Top
		{
			get
			{
				return new Vector3(Center.x, Center.y + boxCollider.size.y / 2f, 0f);
			}
		}

		public Vector3 TopLeft
		{
			get
			{
				return new Vector3(Center.x - boxCollider.size.x / 2f, Center.y + boxCollider.size.y / 2f, 0f);
			}
		}

		public Vector3 TopRight
		{
			get
			{
				return new Vector3(Center.x + boxCollider.size.x / 2f, Center.y + boxCollider.size.y / 2f, 0f);
			}
		}

		public Vector3 CenterLeft
		{
			get
			{
				return new Vector3(Center.x - boxCollider.size.x / 2f, Center.y, 0f);
			}
		}

		public Vector3 CenterRight
		{
			get
			{
				return new Vector3(Center.x + boxCollider.size.x / 2f, Center.y, 0f);
			}
		}

		public Vector2 Center
		{
			get
			{
				return (Vector2)transform.position + new Vector2(boxCollider.offset.x, Motor.GravityReversalMultiplier * boxCollider.offset.y);
			}
		}

		public Vector3 Bottom
		{
			get
			{
				return new Vector3(Center.x, Center.y - boxCollider.size.y / 2f, 0f);
			}
		}

		public Vector3 BottomLeft
		{
			get
			{
				return new Vector3(Center.x - boxCollider.size.x / 2f, Center.y - boxCollider.size.y / 2f, 0f);
			}
		}

		public Vector3 BottomRight
		{
			get
			{
				return new Vector3(Center.x + boxCollider.size.x / 2f, Center.y - boxCollider.size.y / 2f, 0f);
			}
		}

		public float TopY
		{
			get
			{
				return Top.y - transform.position.y;
			}
		}

		public float BottomY
		{
			get
			{
				return Bottom.y - transform.position.y;
			}
		}

		public BoundsManager(LevelPlayerMotor motor)
		{
			Motor = motor;
			transform = motor.transform;
			boxCollider = transform.GetComponent<Collider2D>() as BoxCollider2D;
		}
	}

	[SerializeField]
	private Properties properties;

	private Vector2 lastPositionFixed;

	private Vector2 lastPosition;

	private VelocityManager velocityManager;

	private JumpManager jumpManager;

	private DashManager dashManager;

	private ParryManager parryManager;

	private DirectionManager directionManager;

	private PlatformManager platformManager;

	private HitManager hitManager;

	private SuperManager superManager;

	private BoundsManager boundsManager;

	private bool allowInput;

	private bool allowJumping;

	private bool allowFalling;

	private bool forceLaunchUp;

	private bool hardExitParry;

	private LevelPlayerParryController parryController;

	private const float RAY_DISTANCE = 2000f;

	private const float MAX_GROUNDED_FALL_DISTANCE = 30f;

	private readonly int wallMask = 262144;

	private readonly int ceilingMask = 524288;

	private readonly int groundMask = 1048576;

	private LevelPlayerWeaponManager.Pose exFirePose;

	private const float JUMP_BUFFER_TIME = 0.0834f;

	public const float INPUT_BUFFER_TIME = 0.134f;

	private BufferedInput bufferedInput;

	private float timeSinceInputBuffered = 0.134f;

	public Trilean2 LookDirection { get; private set; }

	public Trilean2 TrueLookDirection { get; private set; }

	public Trilean2 MoveDirection { get; private set; }

	public JumpManager.State JumpState
	{
		get
		{
			return jumpManager.state;
		}
	}

	public bool Dashing
	{
		get
		{
			return dashManager.IsDashing;
		}
	}

	public int DashDirection
	{
		get
		{
			return dashManager.direction;
		}
	}

	public bool Locked { get; private set; }

	public bool Grounded { get; private set; }

	public bool Parrying { get; private set; }

	public bool Ducking
	{
		get
		{
			return (int)LookDirection.y < 0 && !Locked && Grounded;
		}
	}

	public bool IsHit
	{
		get
		{
			return hitManager.state == HitManager.State.Hit;
		}
	}

	public bool IsUsingSuperOrEx
	{
		get
		{
			return superManager.state == SuperManager.State.Super || superManager.state == SuperManager.State.Ex;
		}
	}

	public bool GravityReversed { get; private set; }

	public float GravityReversalMultiplier
	{
		get
		{
			return (!GravityReversed) ? 1 : (-1);
		}
	}

	public event Action OnGroundedEvent;

	public event Action OnJumpEvent;

	public event Action OnParryEvent;

	public event Action OnParrySuccess;

	public event Action OnHitEvent;

	public event Action OnDashStartEvent;

	public event Action OnDashEndEvent;

	protected override void OnAwake()
	{
		base.OnAwake();
		properties = new Properties();
		MoveDirection = new Trilean2(0, 0);
		LookDirection = new Trilean2(1, 0);
		TrueLookDirection = new Trilean2(1, 0);
		velocityManager = new VelocityManager(this, properties.maxSpeedY, properties.yEase);
		jumpManager = new JumpManager();
		dashManager = new DashManager();
		parryManager = new ParryManager();
		directionManager = new DirectionManager();
		platformManager = new PlatformManager(this);
		hitManager = new HitManager();
		superManager = new SuperManager();
		boundsManager = new BoundsManager(this);
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		allowInput = true;
		allowFalling = true;
		allowJumping = true;
		forceLaunchUp = false;
	}

	protected override void Start()
	{
		base.Start();
		base.player.weaponManager.OnExStart += StartEx;
		base.player.weaponManager.OnSuperStart += StartSuper;
		base.player.weaponManager.OnExFire += OnExFired;
		base.player.weaponManager.OnSuperEnd += OnSuperEnd;
		base.player.weaponManager.OnExEnd += ResetSuperAndEx;
		base.player.weaponManager.OnSuperEnd += ResetSuperAndEx;
		base.player.OnReviveEvent += OnRevive;
		parryController = base.player.GetComponent<LevelPlayerParryController>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.player.IsDead)
		{
			return;
		}
		HandleLooking();
		if (base.player.weaponManager.FreezePosition)
		{
			return;
		}
		HandleInput();
		if (allowFalling)
		{
			HandleFalling();
		}
		if (!Grounded)
		{
			jumpManager.timeInAir += CupheadTime.FixedDelta;
			if (jumpManager.state == JumpManager.State.Ready && jumpManager.timeInAir > 0.0834f)
			{
				jumpManager.state = JumpManager.State.Used;
			}
		}
		Move();
		HandleRaycasts();
		Vector2 vector = base.transform.localPosition;
		Vector2 vector2 = vector - lastPositionFixed;
		vector2.x = (int)vector2.x;
		vector2.y = (int)vector2.y;
		MoveDirection = vector2;
		lastPositionFixed = new Vector2(vector.x, vector.y);
		lastPosition = base.transform.position;
		ClampToBounds();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		ClampToBounds();
	}

	public void DisableInput()
	{
		allowInput = false;
		Locked = false;
		MoveDirection = new Trilean2(0, 0);
		velocityManager.move = 0f;
		velocityManager.dash = 0f;
	}

	public void EnableInput()
	{
		allowInput = true;
	}

	public void DisableJump()
	{
		allowJumping = false;
	}

	public void EnableJump()
	{
		allowJumping = true;
	}

	public void DisableGravity()
	{
		allowFalling = false;
		MoveDirection = new Trilean2(MoveDirection.x, 0);
		velocityManager.y = 0f;
	}

	public void EnableGravity()
	{
		allowFalling = true;
		velocityManager.y = 0f;
	}

	public void SetGravityReversed(bool reversed)
	{
		if (reversed != GravityReversed)
		{
			GravityReversed = reversed;
			base.player.animationController.OnGravityReversed();
			base.transform.AddPosition(0f, (0f - (base.player.center.y - base.transform.position.y)) * 2f);
		}
	}

	private RaycastHit2D BoxCast(Vector2 size, Vector2 direction, int layerMask)
	{
		return BoxCast(size, direction, layerMask, Vector2.zero);
	}

	private RaycastHit2D BoxCast(Vector2 size, Vector2 direction, int layerMask, Vector2 offset)
	{
		return Physics2D.BoxCast(base.player.colliderManager.DefaultCenter + offset, size, 0f, direction, 2000f, layerMask);
	}

	private RaycastHit2D CircleCast(float radius, Vector2 direction, int layerMask)
	{
		return Physics2D.CircleCast(base.player.colliderManager.DefaultCenter, radius, direction, 2000f, layerMask);
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
			Gizmos.DrawSphere(base.player.center, 5f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
		}
	}

	private void HandleRaycasts()
	{
		bool flag = true;
		if (directionManager != null && directionManager.up != null)
		{
			flag = directionManager.up.able;
		}
		LevelPlayerColliderManager colliderManager = base.player.colliderManager;
		directionManager.Reset();
		RaycastHit2D raycastHit = BoxCast(new Vector2(1f, colliderManager.DefaultHeight), Vector2.left, wallMask);
		RaycastHit2D raycastHit2 = BoxCast(new Vector2(1f, colliderManager.DefaultHeight), Vector2.right, wallMask);
		RaycastHit2D raycastHit3 = BoxCast(new Vector2(colliderManager.DefaultWidth, 1f), (!GravityReversed) ? Vector2.up : Vector2.down, (!GravityReversed) ? ceilingMask : groundMask);
		RaycastObstacle(directionManager.left, raycastHit, colliderManager.DefaultWidth / 2f, RaycastAxis.X);
		RaycastObstacle(directionManager.right, raycastHit2, colliderManager.DefaultWidth / 2f, RaycastAxis.X);
		RaycastObstacle(directionManager.up, raycastHit3, colliderManager.DefaultHeight / 2f, RaycastAxis.Y);
		Vector2 vector = colliderManager.DefaultCenter + new Vector2(0f, colliderManager.DefaultHeight * GravityReversalMultiplier);
		RaycastHit2D[] array = Physics2D.BoxCastAll(vector, new Vector2(colliderManager.DefaultWidth, 1f), 0f, (!GravityReversed) ? Vector2.down : Vector2.up, 1000f, (!GravityReversed) ? groundMask : ceilingMask);
		directionManager.down.pos = new Vector2(colliderManager.DefaultCenter.x, -10000f * GravityReversalMultiplier);
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array2[i];
			if (((!GravityReversed) ? (raycastHit2D.point.y > directionManager.down.pos.y) : (raycastHit2D.point.y < directionManager.down.pos.y)) && !((!GravityReversed) ? (raycastHit2D.point.y > 20f + base.transform.position.y) : (raycastHit2D.point.y < -20f + base.transform.position.y)))
			{
				float num = Math.Abs(base.transform.position.y - raycastHit2D.point.y);
				directionManager.down.pos = new Vector2(vector.x, raycastHit2D.point.y);
				directionManager.down.gameObject = raycastHit2D.collider.gameObject;
				directionManager.down.distance = num;
				if (num < 20f)
				{
					directionManager.down.able = false;
				}
				Debug.DrawLine(vector, directionManager.down.pos, Color.red);
			}
		}
		if (!Grounded)
		{
			if (!directionManager.down.able)
			{
				OnGrounded();
				directionManager.left.able = true;
				directionManager.right.able = true;
			}
			if (!directionManager.up.able && directionManager.up.able != flag)
			{
				GameObject gameObject = directionManager.up.gameObject;
				LevelPlatform levelPlatform = ((!(gameObject == null)) ? gameObject.GetComponent<LevelPlatform>() : null);
				if (!GravityReversed || levelPlatform == null || !levelPlatform.canFallThrough)
				{
					OnHitCeiling();
				}
			}
		}
		float num2 = Mathf.Abs(base.transform.position.y - directionManager.down.pos.y);
		if (Grounded && num2 > 30f)
		{
			LeaveGround(true);
		}
	}

	private float RaycastObstacle(DirectionManager.Hit directionProperties, RaycastHit2D raycastHit, float maxDistance, RaycastAxis axis)
	{
		if (!DoesRaycastHitHaveCollider(raycastHit))
		{
			return 1000f;
		}
		float num = ((axis != RaycastAxis.X) ? Math.Abs(base.player.colliderManager.DefaultCenter.y - raycastHit.point.y) : Math.Abs(base.player.colliderManager.DefaultCenter.x - raycastHit.point.x));
		directionProperties.pos = raycastHit.point;
		directionProperties.gameObject = raycastHit.collider.gameObject;
		directionProperties.distance = num;
		if (num < maxDistance)
		{
			directionProperties.able = false;
		}
		return num;
	}

	private void OnGrounded()
	{
		if (GravityReversed)
		{
			int num = 0;
		}
		if (Grounded || !jumpManager.ableToLand || platformManager.IsPlatformIgnored(directionManager.down.gameObject.transform))
		{
			return;
		}
		LevelPlatform component = directionManager.down.gameObject.GetComponent<LevelPlatform>();
		if (component != null)
		{
			if (component.canFallThrough && jumpManager.timeSinceDownJump < 0.1f)
			{
				return;
			}
			component.AddChild(base.transform);
		}
		jumpManager.state = JumpManager.State.Ready;
		parryManager.state = ParryManager.State.Ready;
		velocityManager.y = 0f;
		platformManager.ResetAll();
		Grounded = true;
		Parrying = false;
		dashManager.timeSinceGroundDash = 1000f;
		if (jumpManager.timeInAir > jumpManager.longestTimeInAir)
		{
			jumpManager.longestTimeInAir = jumpManager.timeInAir;
			OnlineManager.Instance.Interface.SetStat(base.player.id, "HangTime", jumpManager.timeInAir);
		}
		if (this.OnGroundedEvent != null)
		{
			this.OnGroundedEvent();
		}
	}

	private void LeaveGround(bool allowLateJump = false)
	{
		if (!Dashing && base.player.stats.Loadout.charm == Charm.charm_parry_plus)
		{
			ForceParry();
		}
		if (Grounded)
		{
			Grounded = false;
			jumpManager.ableToLand = false;
			jumpManager.timeInAir = 0f;
			base.player.stats.ResetJumpParries();
		}
		velocityManager.y = 0f;
		ClearParent();
		if (jumpManager.state == JumpManager.State.Ready && !allowLateJump)
		{
			jumpManager.state = JumpManager.State.Used;
		}
	}

	private void OnHitCeiling()
	{
		if (!jumpManager.ableToLand)
		{
			velocityManager.y = 0f;
			directionManager.left.able = true;
			directionManager.right.able = true;
		}
	}

	public IEnumerator MoveToX_cr(float x, int endingLookDirection = 1)
	{
		float walk = 0f;
		MoveDirection = new Trilean2(0, 0);
		LookDirection = new Trilean2(1, 0);
		bool left = base.transform.position.x < x;
		if (!left)
		{
			LookDirection = new Trilean2(-1, 0);
		}
		while ((!left) ? (base.transform.position.x > x) : (base.transform.position.x < x))
		{
			if (((int)LookDirection.y >= 0 || !Grounded) && !Locked)
			{
				walk = (float)(left ? 1 : (-1)) * properties.moveSpeed;
			}
			velocityManager.move = walk;
			yield return null;
		}
		walk = 0f;
		velocityManager.move = walk;
		MoveDirection = new Trilean2(0, 0);
		LookDirection = new Trilean2(endingLookDirection, 0);
		yield return null;
		LookDirection = new Trilean2(0, 0);
	}

	private void Move()
	{
		velocityManager.Calculate();
		Vector3 vector = velocityManager.Total;
		if (hitManager.state != HitManager.State.Hit && superManager.state == SuperManager.State.Ready)
		{
			if (!velocityManager.yAxisForce)
			{
				forceLaunchUp = false;
				if (Grounded)
				{
					vector.x += velocityManager.GroundForce;
				}
				else
				{
					vector.x += velocityManager.AirForce;
				}
			}
			else if (Grounded)
			{
				if (!forceLaunchUp)
				{
					LeaveGround();
					velocityManager.y = properties.jumpPower * 2f;
					DisableGravity();
					forceLaunchUp = true;
				}
			}
			else
			{
				vector.y += velocityManager.AirForce;
				FrameDelayedCallback(EnableGravity, 1);
			}
		}
		if (vector.x > 0f && !directionManager.right.able)
		{
			vector.x = 0f;
		}
		if (vector.x < 0f && !directionManager.left.able)
		{
			vector.x = 0f;
		}
		if (platformManager.OnPlatform)
		{
			if (!directionManager.right.able && (int)MoveDirection.x > 0)
			{
				vector.x = 0f;
				base.transform.SetPosition(lastPosition.x);
			}
			if (!directionManager.left.able && (int)MoveDirection.x < 0)
			{
				vector.x = 0f;
				base.transform.SetPosition(lastPosition.x);
			}
		}
		if (GravityReversed)
		{
			vector.y *= -1f;
		}
		base.transform.localPosition += vector * CupheadTime.FixedDelta;
		if (Grounded)
		{
			Vector2 vector2 = base.transform.position;
			vector2.y = directionManager.down.pos.y;
			base.transform.position = vector2;
			LevelPlatform component = directionManager.down.gameObject.GetComponent<LevelPlatform>();
			if (component == null && base.transform.parent != null)
			{
				ClearParent();
			}
			else if (component != null && (base.transform.parent == null || component.gameObject != base.transform.parent.gameObject))
			{
				ClearParent();
				component.AddChild(base.transform);
			}
		}
	}

	private void ClampToBounds()
	{
		CupheadBounds cupheadBounds = new CupheadBounds();
		cupheadBounds.left = directionManager.left.pos.x + base.player.colliderManager.Width / 2f;
		cupheadBounds.right = directionManager.right.pos.x - base.player.colliderManager.Width / 2f;
		cupheadBounds.top = directionManager.up.pos.y - ((!GravityReversed) ? boundsManager.TopY : boundsManager.BottomY);
		cupheadBounds.bottom = directionManager.down.pos.y - ((!GravityReversed) ? boundsManager.BottomY : boundsManager.TopY);
		GameObject gameObject = directionManager.up.gameObject;
		LevelPlatform levelPlatform = ((!(gameObject == null)) ? gameObject.GetComponent<LevelPlatform>() : null);
		bool flag = !GravityReversed || levelPlatform == null || !levelPlatform.canFallThrough;
		Vector3 position = base.transform.position;
		if (!directionManager.left.able && base.transform.position.x < cupheadBounds.left)
		{
			position.x = cupheadBounds.left;
		}
		if (!directionManager.right.able && base.transform.position.x > cupheadBounds.right)
		{
			position.x = cupheadBounds.right;
		}
		if (!directionManager.up.able && flag && ((!GravityReversed) ? (base.transform.position.y > cupheadBounds.top) : (base.transform.position.y < cupheadBounds.top)))
		{
			position.y = cupheadBounds.top;
		}
		position.x = Mathf.Clamp(position.x, (float)Level.Current.Left + base.player.colliderManager.Width / 2f, (float)Level.Current.Right - base.player.colliderManager.Width / 2f);
		base.transform.position = position;
	}

	private void ResetSuperAndEx()
	{
		if (superManager.state != SuperManager.State.Ready)
		{
			if (jumpManager.state != JumpManager.State.Ready)
			{
				jumpManager.state = JumpManager.State.Used;
			}
			StopCoroutine(exMove_cr());
			superManager.state = SuperManager.State.Ready;
			EnableInput();
			EnableGravity();
		}
	}

	private void StartSuper()
	{
		LeaveGround();
		jumpManager.state = JumpManager.State.Used;
		jumpManager.timer = 0f;
		velocityManager.y = 0f;
	}

	public void OnSuperEnd()
	{
		if (Grounded)
		{
			jumpManager.state = JumpManager.State.Ready;
			return;
		}
		LeaveGround();
		velocityManager.y = ((base.player.stats.Loadout.super != Super.level_super_invincible) ? properties.superKnockUp : properties.superInvincibleKnockUp);
	}

	private void StartEx()
	{
		exFirePose = base.player.weaponManager.GetDirectionPose();
		DisableInput();
		DisableGravity();
		superManager.state = SuperManager.State.Ex;
	}

	private void OnExFired()
	{
		if (exFirePose == LevelPlayerWeaponManager.Pose.Up || exFirePose == LevelPlayerWeaponManager.Pose.Down)
		{
			StartCoroutine(exDelay_cr());
		}
		else
		{
			StartCoroutine(exMove_cr());
		}
	}

	private IEnumerator exDelay_cr()
	{
		while (superManager.state != SuperManager.State.Ready)
		{
			yield return null;
		}
		EnableInput();
		EnableGravity();
		superManager.state = SuperManager.State.Ready;
	}

	private IEnumerator exMove_cr()
	{
		while (superManager.state != SuperManager.State.Ready)
		{
			velocityManager.move = (float)((int)TrueLookDirection.x * -1) * properties.exKnockback;
			yield return null;
		}
		EnableInput();
		EnableGravity();
		superManager.state = SuperManager.State.Ready;
	}

	private void HandleInput()
	{
		if (!base.player.levelStarted)
		{
			return;
		}
		timeSinceInputBuffered += CupheadTime.FixedDelta;
		dashManager.timeSinceGroundDash += CupheadTime.FixedDelta;
		if ((!allowInput || dashManager.IsDashing) && hitManager.state == HitManager.State.Inactive)
		{
			BufferInputs();
		}
		if (!allowInput || HandleDash())
		{
			return;
		}
		if (hitManager.state == HitManager.State.Hit)
		{
			HandleHit();
			return;
		}
		if (hitManager.state != HitManager.State.KnockedUp)
		{
			HandleParry();
			HandleJumping();
			HandleLocked();
		}
		else
		{
			HandlePitKnockUp();
		}
		HandleWalking();
	}

	private void BufferInput(BufferedInput input)
	{
		bufferedInput = input;
		timeSinceInputBuffered = 0f;
	}

	public void BufferInputs()
	{
		if (base.player.input.actions.GetButtonDown(2))
		{
			BufferInput(BufferedInput.Jump);
		}
		else if (base.player.input.actions.GetButtonDown(7) && !dashManager.IsDashing)
		{
			BufferInput(BufferedInput.Dash);
		}
		else if (base.player.input.actions.GetButtonDown(4))
		{
			BufferInput(BufferedInput.Super);
		}
	}

	public void ClearBufferedInput()
	{
		timeSinceInputBuffered = 0.134f;
	}

	public bool HasBufferedInput(BufferedInput input)
	{
		return bufferedInput == input && timeSinceInputBuffered < 0.134f;
	}

	private void HandleJumping()
	{
		if (!allowJumping)
		{
			return;
		}
		if (jumpManager.state == JumpManager.State.Ready && (base.player.input.actions.GetButtonDown(2) || HasBufferedInput(BufferedInput.Jump)))
		{
			hardExitParry = false;
			ClearBufferedInput();
			if ((int)LookDirection.y < 0 && Grounded && base.transform.parent != null)
			{
				LevelPlatform component = base.transform.parent.GetComponent<LevelPlatform>();
				if (component.canFallThrough)
				{
					platformManager.Ignore(base.transform.parent);
					jumpManager.state = JumpManager.State.Used;
					LeaveGround();
					jumpManager.timeSinceDownJump = 0f;
					return;
				}
			}
			AudioManager.Play(Sfx.Player_Jump);
			jumpManager.state = JumpManager.State.Hold;
			LeaveGround();
			velocityManager.y = properties.jumpPower;
			jumpManager.timer = CupheadTime.FixedDelta;
			if (this.OnJumpEvent != null)
			{
				this.OnJumpEvent();
			}
		}
		if (jumpManager.state == JumpManager.State.Hold)
		{
			if (!directionManager.up.able || (jumpManager.timer >= properties.jumpHoldMin && (base.player.input.actions.GetButtonUp(2) || !base.player.input.actions.GetButton(2))) || jumpManager.timer >= properties.jumpHoldMax)
			{
				jumpManager.state = JumpManager.State.Used;
				jumpManager.timer = 0f;
			}
			velocityManager.y = properties.jumpPower;
			jumpManager.timer += CupheadTime.FixedDelta;
		}
		jumpManager.timeSinceDownJump += CupheadTime.FixedDelta;
	}

	private void HandleParry()
	{
		if (!IsHit && parryManager.state == ParryManager.State.Ready && (base.player.input.actions.GetButtonDown(2) || HasBufferedInput(BufferedInput.Jump)) && jumpManager.state != JumpManager.State.Ready && !IsHit)
		{
			ClearBufferedInput();
			hitManager.state = HitManager.State.Inactive;
			parryManager.state = ParryManager.State.NotReady;
			if (dashManager.IsDashing)
			{
				dashManager.state = DashManager.State.End;
			}
			Parrying = true;
			if (this.OnParryEvent != null)
			{
				this.OnParryEvent();
			}
		}
	}

	public void OnParryComplete()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_parry_plus)
		{
			hardExitParry = true;
		}
		LeaveGround();
		parryManager.state = ParryManager.State.Ready;
		velocityManager.y = ((!parryController.HasHitEnemy) ? properties.parryPower : properties.parryAttackBounce);
		if (this.OnParrySuccess != null)
		{
			this.OnParrySuccess();
		}
	}

	public void OnParryHit()
	{
		StartCoroutine(parryHit_cr());
	}

	public void OnParryCanceled()
	{
		Parrying = false;
	}

	public void OnParryAnimEnd()
	{
		Parrying = false;
	}

	private bool HandleDash()
	{
		if (dashManager.state == DashManager.State.Ready && (!Grounded || dashManager.timeSinceGroundDash > 0.1f) && (base.player.input.actions.GetButtonDown(7) || HasBufferedInput(BufferedInput.Dash)))
		{
			ClearBufferedInput();
			AudioManager.Play("Player_Dash");
			dashManager.state = DashManager.State.Start;
			dashManager.direction = TrueLookDirection.x;
			dashManager.groundDash = Grounded;
			if (jumpManager.state == JumpManager.State.Hold)
			{
				jumpManager.state = JumpManager.State.Used;
			}
			if (this.OnDashStartEvent != null)
			{
				this.OnDashStartEvent();
			}
			velocityManager.move = 0f;
			return true;
		}
		if (dashManager.state == DashManager.State.Start)
		{
			dashManager.state = DashManager.State.Dashing;
		}
		if (dashManager.state == DashManager.State.Dashing)
		{
			velocityManager.dash = properties.dashSpeed * (float)dashManager.direction;
			dashManager.timer += CupheadTime.FixedDelta;
			velocityManager.y = 0f;
			if (dashManager.timer >= properties.dashTime)
			{
				DashComplete();
			}
			if (!Grounded)
			{
				jumpManager.ableToLand = true;
			}
			return true;
		}
		if (dashManager.state == DashManager.State.End)
		{
			if (Grounded)
			{
				dashManager.state = DashManager.State.Ready;
				if (dashManager.groundDash)
				{
					dashManager.timeSinceGroundDash = 0f;
				}
			}
			else
			{
				dashManager.groundDash = false;
			}
		}
		return false;
	}

	private void DashComplete()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_parry_plus)
		{
			ForceParry();
		}
		dashManager.state = DashManager.State.End;
		dashManager.timer = 0f;
		velocityManager.dash = 0f;
		if (this.OnDashEndEvent != null)
		{
			this.OnDashEndEvent();
		}
	}

	private void ForceParry()
	{
		if (hitManager.state != HitManager.State.Hit && !hardExitParry)
		{
			Debug.Log("In parry frames");
			hitManager.state = HitManager.State.Inactive;
			parryManager.state = ParryManager.State.NotReady;
			if (dashManager.IsDashing)
			{
				dashManager.state = DashManager.State.End;
			}
			Parrying = true;
			if (this.OnParryEvent != null)
			{
				this.OnParryEvent();
			}
		}
	}

	private void HandleLocked()
	{
		if (base.player.input.actions.GetButton(6) && Grounded)
		{
			Locked = true;
		}
		else
		{
			Locked = false;
		}
	}

	private void HandleWalking()
	{
		float move = 0f;
		if (((int)LookDirection.y >= 0 || !Grounded) && !Locked)
		{
			move = (float)base.player.input.GetAxisInt(PlayerInput.Axis.X) * properties.moveSpeed;
		}
		velocityManager.move = move;
	}

	private void HandleLooking()
	{
		if (base.player.levelStarted && allowInput)
		{
			int axisInt = base.player.input.GetAxisInt(PlayerInput.Axis.X);
			int num = base.player.input.GetAxisInt(PlayerInput.Axis.Y, true, Grounded && !Locked && !IsUsingSuperOrEx);
			if (GravityReversed)
			{
				num *= -1;
			}
			LookDirection = new Trilean2(axisInt, num);
		}
		int x = TrueLookDirection.x;
		int num2 = TrueLookDirection.y;
		if ((int)LookDirection.x != 0)
		{
			x = LookDirection.x;
		}
		num2 = LookDirection.y;
		TrueLookDirection = new Trilean2(x, num2);
	}

	private void HandleFalling()
	{
		if (!Grounded && !dashManager.IsDashing && !(Level.Current.LevelTime < 0.2f))
		{
			float num = properties.timeToMaxY * 60f;
			float num2 = properties.maxSpeedY / num * CupheadTime.FixedDelta;
			velocityManager.y += num2;
			jumpManager.ableToLand = velocityManager.y > 0f;
		}
	}

	public void HandlePitKnockUp()
	{
		if (hitManager.state == HitManager.State.KnockedUp)
		{
			if (hitManager.timer > properties.knockUpStunTime)
			{
				hitManager.state = HitManager.State.Inactive;
				velocityManager.hit = 0f;
			}
			else
			{
				hitManager.timer += CupheadTime.FixedDelta;
			}
		}
	}

	private void HandleHit()
	{
		if (hitManager.state == HitManager.State.Hit)
		{
			if (hitManager.timer > properties.hitStunTime)
			{
				hitManager.state = HitManager.State.Inactive;
				velocityManager.hit = 0f;
			}
			else
			{
				float value = hitManager.timer / properties.hitStunTime;
				velocityManager.hit = EaseUtils.Ease(properties.hitKnockbackEase, properties.hitKnockbackPower, 0f, value) * (float)hitManager.direction;
				hitManager.timer += CupheadTime.FixedDelta;
			}
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!base.player.stats.SuperInvincible)
		{
			hitManager.state = HitManager.State.Hit;
			if (this.OnHitEvent != null)
			{
				this.OnHitEvent();
			}
			DashComplete();
			velocityManager.Clear();
			ResetSuperAndEx();
			int direction = (int)TrueLookDirection.x * -1;
			hitManager.direction = direction;
			LeaveGround();
			velocityManager.y = properties.hitJumpPower;
			hitManager.timer = 0f;
		}
	}

	public void OnPitKnockUp(float y, float velocityScale = 1f)
	{
		if (base.player.IsDead)
		{
			base.transform.SetPosition(null, y + 200f * GravityReversalMultiplier);
			return;
		}
		base.transform.SetPosition(null, y);
		hitManager.state = HitManager.State.KnockedUp;
		DashComplete();
		velocityManager.Clear();
		ResetSuperAndEx();
		hitManager.direction = 0;
		LeaveGround();
		if (Level.Current.LevelType == Level.Type.Platforming)
		{
			velocityManager.y = properties.platformingPitKnockUpPower * velocityScale;
		}
		else
		{
			velocityManager.y = properties.pitKnockUpPower * velocityScale;
		}
		hitManager.timer = 0f;
		dashManager.state = DashManager.State.Ready;
		parryManager.state = ParryManager.State.Ready;
	}

	public void OnTrampolineKnockUp(float y)
	{
		if (base.player.IsDead)
		{
			base.transform.SetPosition(null, y * GravityReversalMultiplier);
			return;
		}
		LeaveGround();
		StartCoroutine(launch_player_cr(y));
		hitManager.state = HitManager.State.KnockedUp;
		DashComplete();
		velocityManager.Clear();
		ResetSuperAndEx();
		hitManager.direction = 0;
		if (Level.Current.LevelType == Level.Type.Platforming)
		{
			velocityManager.y = properties.platformingPitKnockUpPower;
		}
		else
		{
			velocityManager.y = properties.pitKnockUpPower;
		}
		hitManager.timer = 0f;
		dashManager.state = DashManager.State.Ready;
		parryManager.state = ParryManager.State.Ready;
	}

	private IEnumerator launch_player_cr(float end)
	{
		float time = 0.1f;
		float t = 0f;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			TransformExtensions.SetPosition(y: Mathf.Lerp(base.transform.position.y, end, t / time), transform: base.transform);
			yield return wait;
		}
	}

	public void OnRevive(Vector3 pos)
	{
		if (GravityReversed)
		{
			pos.y -= (base.player.center.y - base.transform.position.y) * 2f;
		}
		base.transform.position = pos;
		hitManager.state = HitManager.State.KnockedUp;
		DashComplete();
		velocityManager.Clear();
		ResetSuperAndEx();
		hitManager.direction = 0;
		LeaveGround();
		velocityManager.y = properties.reviveKnockUpPower;
		hitManager.timer = 0f;
		base.player.animationController.UpdateAnimator();
	}

	public void AddForce(VelocityManager.Force force)
	{
		velocityManager.AddForce(force);
	}

	public void RemoveForce(VelocityManager.Force force)
	{
		velocityManager.RemoveForce(force);
		force.yAxisForce = false;
	}

	private void ClearParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent.GetComponent<LevelPlatform>().OnPlayerExit(base.transform);
		}
		base.transform.parent = null;
	}

	public void OnPlatformingLevelExit()
	{
		StartCoroutine(platformingExit_cr());
	}

	private IEnumerator platformingExit_cr()
	{
		while (true)
		{
			if (Dashing)
			{
				DashComplete();
			}
			allowInput = false;
			Locked = false;
			LookDirection = new Trilean2(1, 0);
			velocityManager.move = properties.moveSpeed;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator parryHit_cr()
	{
		CupheadTime.GlobalSpeed = 1f;
		velocityManager.Clear();
		yield return null;
		PauseManager.Unpause();
		velocityManager.Clear();
		CupheadTime.GlobalSpeed = 1f;
	}
}
