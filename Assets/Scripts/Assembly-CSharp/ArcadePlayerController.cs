using UnityEngine;

public class ArcadePlayerController : AbstractPlayerController
{
	private bool initialized;

	private ArcadePlayerMotor _motor;

	private ArcadePlayerAnimationController _animationController;

	private ArcadePlayerWeaponManager _weaponManager;

	private ArcadePlayerParryController _parryController;

	private ArcadePlayerColliderManager _colliderManager;

	[SerializeField]
	private PlayerDeathEffect deathEffect;

	public ArcadePlayerMotor motor
	{
		get
		{
			if (_motor == null)
			{
				_motor = GetComponent<ArcadePlayerMotor>();
			}
			return _motor;
		}
	}

	public ArcadePlayerAnimationController animationController
	{
		get
		{
			if (_animationController == null)
			{
				_animationController = GetComponent<ArcadePlayerAnimationController>();
			}
			return _animationController;
		}
	}

	public ArcadePlayerWeaponManager weaponManager
	{
		get
		{
			if (_weaponManager == null)
			{
				_weaponManager = GetComponent<ArcadePlayerWeaponManager>();
			}
			return _weaponManager;
		}
	}

	public ArcadePlayerParryController parryController
	{
		get
		{
			if (_parryController == null)
			{
				_parryController = GetComponent<ArcadePlayerParryController>();
			}
			return _parryController;
		}
	}

	public ArcadePlayerColliderManager colliderManager
	{
		get
		{
			if (_colliderManager == null)
			{
				_colliderManager = GetComponent<ArcadePlayerColliderManager>();
			}
			return _colliderManager;
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

	protected override void LevelInit(PlayerId id)
	{
		base.LevelInit(id);
		animationController.LevelInit();
		weaponManager.LevelInit(id);
	}

	protected override void OnDeath(PlayerId playerId)
	{
		base.OnDeath(base.id);
		PlayerDeathEffect playerDeathEffect = deathEffect.Create(base.id, base.input, base.transform.position, base.stats.Deaths, PlayerMode.Level, true);
		playerDeathEffect.OnPreReviveEvent += OnPreRevive;
		playerDeathEffect.OnReviveEvent += OnRevive;
	}

	public void DisableInput()
	{
		motor.DisableInput();
		weaponManager.DisableInput();
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
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (Application.isPlaying)
		{
			Gizmos.DrawCube(CameraCenter, Vector3.one * 50f);
		}
	}

	public override void BufferInputs()
	{
		base.BufferInputs();
		motor.BufferInputs();
	}
}
