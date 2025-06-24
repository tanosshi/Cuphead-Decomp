using UnityEngine;

public class LevelPlayerController : AbstractPlayerController
{
	private const float PLATFORMING_CAMERA_DISTANCE_RUNNING = 250f;

	private const float PLATFORMING_CAMERA_DISTANCE_STATIC = 50f;

	private const float PLATFORMING_CAMERA_TIME_RUNNING = 1.2f;

	private const float PLATFORMING_CAMERA_TIME_STATIC = 6f;

	private bool initialized;

	private LevelPlayerMotor _motor;

	private LevelPlayerAnimationController _animationController;

	private LevelPlayerWeaponManager _weaponManager;

	private LevelPlayerParryController _parryController;

	private LevelPlayerColliderManager _colliderManager;

	[SerializeField]
	private PlayerDeathEffect deathEffect;

	private float cameraCenterPosition;

	public bool Ducking
	{
		get
		{
			return motor.Ducking;
		}
	}

	public LevelPlayerMotor motor
	{
		get
		{
			if (_motor == null)
			{
				_motor = GetComponent<LevelPlayerMotor>();
			}
			return _motor;
		}
	}

	public LevelPlayerAnimationController animationController
	{
		get
		{
			if (_animationController == null)
			{
				_animationController = GetComponent<LevelPlayerAnimationController>();
			}
			return _animationController;
		}
	}

	public LevelPlayerWeaponManager weaponManager
	{
		get
		{
			if (_weaponManager == null)
			{
				_weaponManager = GetComponent<LevelPlayerWeaponManager>();
			}
			return _weaponManager;
		}
	}

	public LevelPlayerParryController parryController
	{
		get
		{
			if (_parryController == null)
			{
				_parryController = GetComponent<LevelPlayerParryController>();
			}
			return _parryController;
		}
	}

	public LevelPlayerColliderManager colliderManager
	{
		get
		{
			if (_colliderManager == null)
			{
				_colliderManager = GetComponent<LevelPlayerColliderManager>();
			}
			return _colliderManager;
		}
	}

	public override Vector3 center
	{
		get
		{
			if (base.transform == null)
			{
				return Vector3.zero;
			}
			return base.transform.position + new Vector3(base.collider.offset.x, base.collider.offset.y * motor.GravityReversalMultiplier, 0f);
		}
	}

	public override bool CanTakeDamage
	{
		get
		{
			if (base.damageReceiver.state != PlayerDamageReceiver.State.Vulnerable)
			{
				return false;
			}
			if (base.stats.Loadout.charm == Charm.charm_smoke_dash && motor.Dashing)
			{
				return false;
			}
			return true;
		}
	}

	public override Vector3 CameraCenter
	{
		get
		{
			if (Level.Current.LevelType == Level.Type.Platforming)
			{
				cameraCenterPosition = Mathf.Lerp(cameraCenterPosition, 250f * (float)_motor.TrueLookDirection.x.Value, 1.2f * (float)CupheadTime.Delta);
				return center + new Vector3(cameraCenterPosition, 0f);
			}
			return base.CameraCenter;
		}
	}

	public void PauseAll()
	{
		AbstractPausableComponent[] components = GetComponents<AbstractPausableComponent>();
		foreach (AbstractPausableComponent abstractPausableComponent in components)
		{
			abstractPausableComponent.enabled = false;
		}
	}

	public void UnpauseAll(bool forced = false)
	{
		AbstractPausableComponent[] components = GetComponents<AbstractPausableComponent>();
		foreach (AbstractPausableComponent abstractPausableComponent in components)
		{
			if (forced)
			{
				abstractPausableComponent.preEnabled = true;
			}
			abstractPausableComponent.enabled = true;
		}
	}

	public void OnPitKnockUp(float y, float velocityScale = 1f)
	{
		if (base.damageReceiver.state == PlayerDamageReceiver.State.Vulnerable)
		{
			base.stats.OnPitKnockUp();
		}
		Debug.Log("PlayerController: OnPitKnockUp");
		motor.OnPitKnockUp(y, velocityScale);
	}

	protected override void LevelInit(PlayerId id)
	{
		base.LevelInit(id);
		animationController.LevelInit();
		weaponManager.LevelInit(id);
		if (base.stats.Health == 0)
		{
			StartDead();
		}
	}

	protected override void OnDeath(PlayerId playerId)
	{
		base.OnDeath(base.id);
		Vector3 position = base.transform.position;
		if (motor.GravityReversed)
		{
			position.y += (center.y - base.transform.position.y) * 2f;
		}
		PlayerDeathEffect playerDeathEffect = deathEffect.Create(base.id, base.input, position, base.stats.Deaths, PlayerMode.Level, true);
		playerDeathEffect.OnPreReviveEvent += OnPreRevive;
		playerDeathEffect.OnReviveEvent += OnRevive;
	}

	public override void OnLeave(PlayerId playerId)
	{
		if (!base.IsDead)
		{
			Vector3 position = base.transform.position;
			if (motor.GravityReversed)
			{
				position.y += (center.y - base.transform.position.y) * 2f;
			}
			deathEffect.CreateExplosionOnly(playerId, position, PlayerMode.Level);
		}
		base.OnLeave(playerId);
	}

	private void StartDead()
	{
		base.gameObject.SetActive(false);
		Vector3 position = base.transform.position;
		position.y += 1000f;
		PlayerDeathEffect playerDeathEffect = deathEffect.Create(base.id, base.input, position, base.stats.Deaths, PlayerMode.Level, true);
		playerDeathEffect.OnPreReviveEvent += OnPreRevive;
		playerDeathEffect.OnReviveEvent += OnRevive;
	}

	public void DisableInput()
	{
		motor.DisableInput();
		weaponManager.DisableInput();
		AudioManager.Stop("player_default_fire_loop");
	}

	public override void BufferInputs()
	{
		base.BufferInputs();
		motor.BufferInputs();
	}

	public void OnLevelWinPause()
	{
		PauseAll();
		base.collider.enabled = false;
	}

	public void OnLevelWin()
	{
		UnpauseAll();
		weaponManager.DisableInput();
		base.collider.enabled = false;
		AudioManager.Stop("player_default_fire_loop");
		if (Level.Current.LevelType == Level.Type.Platforming)
		{
			motor.OnPlatformingLevelExit();
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (Application.isPlaying)
		{
			Gizmos.DrawCube(CameraCenter, Vector3.one * 50f);
		}
	}
}
