using System;
using UnityEngine;

[Serializable]
public class EnemyProperties
{
	public enum AimMode
	{
		AimedAtPlayer = 0,
		ArcAimedAtPlayer = 1,
		Straight = 2,
		Spread = 3,
		Arc = 4
	}

	public enum LoopMode
	{
		PingPong = 0,
		Repeat = 1,
		Once = 2,
		DelayAtPoint = 3
	}

	[SerializeField]
	private string _displayName = string.Empty;

	[SerializeField]
	private string _enumName = string.Empty;

	[SerializeField]
	private int _id;

	[SerializeField]
	private float _health = 1f;

	[SerializeField]
	private bool _parryable;

	[Header("Movement")]
	[SerializeField]
	private LoopMode _moveLoopMode;

	[SerializeField]
	private float _moveSpeed = 500f;

	[SerializeField]
	private bool _canJump;

	[SerializeField]
	private float _gravity = 1200f;

	[SerializeField]
	private float _floatSpeed = 400f;

	[SerializeField]
	private float _jumpHeight = 200f;

	[SerializeField]
	private float _jumpLength = 500f;

	[Header("Projectiles")]
	[SerializeField]
	private AimMode _projectileAimMode = AimMode.Straight;

	[SerializeField]
	private BasicProjectile _projectilePrefab;

	[SerializeField]
	private bool _projectileParryable;

	[SerializeField]
	private float _projectileSpeed = 500f;

	[SerializeField]
	private float _arcProjectileMinSpeed = 250f;

	[SerializeField]
	private float _projectileAngle;

	[SerializeField]
	private float _arcProjectileMinAngle;

	[SerializeField]
	private float _projectileGravity = 15f;

	[SerializeField]
	private float _projectileStoneTime;

	[SerializeField]
	private MinMax _projectileDelay = new MinMax(1f, 1f);

	[SerializeField]
	private MinMax _mushroomPinkNumber = new MinMax(3f, 5f);

	[SerializeField]
	private float _acornFlySpeed = 500f;

	[SerializeField]
	private float _acornDropSpeed = 500f;

	[SerializeField]
	private float _acornPropellerSpeed = 300f;

	[SerializeField]
	private MinMax _blobRunnerMeltDelay = new MinMax(2f, 3f);

	[SerializeField]
	private float _blobRunnerUnnmeltLoopTime = 0.5f;

	public float ClamTimeSpeedUp = 0.8f;

	public float ClamTimeSpeedDown = 1f;

	public MinMax ClamMaxPointRange = new MinMax(600f, 700f);

	public int ClamShotCount = 4;

	public MinMax ClamDespawnDelayRange = new MinMax(3.5f, 5f);

	public float fastMovement = 400f;

	public float slowMovement = 200f;

	public string dragonFlyAimString;

	public string dragonFlyAtkDelayString;

	public float dragonFlyWarningDuration;

	public float dragonFlyAttackDuration;

	public float dragonFlyProjectileSpeed;

	public float dragonFlyProjectileDelay;

	public float dragonFlyLockDistOffset;

	public float dragonFlyInitRiseTime;

	public float WoodpeckerWarningDuration;

	public float WoodpeckerAttackDuration;

	public float WoodpeckermoveDownTime;

	public float WoodpeckermoveUpTime;

	public float flyingFishVelocity;

	public float flyingFishSinVelocity;

	public float flyingFishSinSize;

	public float lobsterTuckTime;

	public float lobsterOffscreenTime;

	public float lobsterSpeed;

	public float lobsterWarningTime;

	public float lobsterY;

	public MinMax krillVelocityX;

	public MinMax krillVelocityY;

	public float krillLaunchDelay;

	public float krillGravity;

	public float dragonTimeIn;

	public float dragonTimeOut;

	public float dragonLeaveDelay;

	public float minerShootSpeed;

	public float minerDescendTime;

	public float minerRopeAscendTime;

	public MinMax minerShotDelay;

	public float minerDistance;

	public float wallFaceTravelTime;

	public MinMax wallAttackDelay;

	public float wallProjectileXSpeed;

	public float wallProjectileYSpeed;

	public float wallProjectileGravity;

	public float flamerCirSpeed;

	public MinMax flamerXSpeed;

	public float flamerLoopSize;

	public float fanVelocity;

	public MinMax fanWaitTime;

	public MinMax funWallTopDelayRange;

	public MinMax funWallBottomDelayRange;

	public float funWallProjectileSpeed;

	public float funWallMouthOpenTime;

	public MinMax funWallCarDelayRange;

	public float funWallCarSpeed;

	public MinMax funWallTongueDelayRange;

	public float funWallTongueLoopTime;

	public float jackLaunchVelocity;

	public float jackHomingMoveSpeed;

	public float jackRotationSpeed;

	public float jacktimeBeforeDeath;

	public float jacktimeBeforeHoming;

	public float jackEaseTime;

	public string jackinDirectionString;

	public MinMax jackinAppearDelay;

	public MinMax jackinDeathAppearDelay;

	public float jackinWarningDuration;

	public float jackinShootDelay;

	public int tubaACount;

	public float tubaInitialDelay;

	public MinMax tubaMainDelayRange;

	public float cannonSpeed;

	public float cannonShotDelay;

	public float bulletDeathTime;

	public MinMax pretzelXSpeedRange;

	public float pretzelYSpeed;

	public float pretzelGroundDelay;

	public MinMax arcadeAttackDelayInit;

	public MinMax arcadeAttackDelay;

	public float arcadeBulletSpeed;

	public MinMax arcadeBulletReturnDelay;

	public int arcadeBulletCount;

	public float arcadeBulletIndividualDelay;

	public MinMax magicianAppearDelayRange;

	public MinMax magicianDeathDelayRange;

	public float magicianDurationAppear;

	public float poleSpeedMovement;

	public string DisplayName
	{
		get
		{
			return _displayName;
		}
	}

	public string EnumName
	{
		get
		{
			return _enumName;
		}
	}

	public int ID
	{
		get
		{
			return _id;
		}
	}

	public float Health
	{
		get
		{
			return _health;
		}
	}

	public bool CanParry
	{
		get
		{
			return _parryable;
		}
	}

	public LoopMode MoveLoopMode
	{
		get
		{
			return _moveLoopMode;
		}
	}

	public float MoveSpeed
	{
		get
		{
			return _moveSpeed;
		}
	}

	public bool canJump
	{
		get
		{
			return _canJump;
		}
	}

	public float gravity
	{
		get
		{
			return _gravity;
		}
	}

	public float floatSpeed
	{
		get
		{
			return _floatSpeed;
		}
	}

	public float jumpHeight
	{
		get
		{
			return _jumpHeight;
		}
	}

	public float jumpLength
	{
		get
		{
			return _jumpLength;
		}
	}

	public AimMode ProjectileAimMode
	{
		get
		{
			return _projectileAimMode;
		}
	}

	public BasicProjectile ProjectilePrefab
	{
		get
		{
			return _projectilePrefab;
		}
	}

	public bool ProjectileParryable
	{
		get
		{
			return _projectileParryable;
		}
	}

	public float ProjectileSpeed
	{
		get
		{
			return _projectileSpeed;
		}
	}

	public float ArcProjectileMinSpeed
	{
		get
		{
			return _arcProjectileMinSpeed;
		}
	}

	public float ProjectileAngle
	{
		get
		{
			return _projectileAngle;
		}
	}

	public float ArcProjectileMinAngle
	{
		get
		{
			return _arcProjectileMinAngle;
		}
	}

	public float ProjectileGravity
	{
		get
		{
			return _projectileGravity;
		}
	}

	public float ProjectileStoneTime
	{
		get
		{
			return _projectileStoneTime;
		}
	}

	public MinMax ProjectileDelay
	{
		get
		{
			return _projectileDelay;
		}
	}

	public MinMax MushroomPinkNumber
	{
		get
		{
			return _mushroomPinkNumber;
		}
	}

	public float AcornFlySpeed
	{
		get
		{
			return _acornFlySpeed;
		}
	}

	public float AcornDropSpeed
	{
		get
		{
			return _acornDropSpeed;
		}
	}

	public float AcornPropellerSpeed
	{
		get
		{
			return _acornPropellerSpeed;
		}
	}

	public MinMax BlobRunnerMeltDelay
	{
		get
		{
			return _blobRunnerMeltDelay;
		}
	}

	public float BlobRunnerUnmeltLoopTime
	{
		get
		{
			return _blobRunnerUnnmeltLoopTime;
		}
	}

	public EnemyProperties()
	{
		_id = TimeUtils.GetCurrentSecond();
	}
}
