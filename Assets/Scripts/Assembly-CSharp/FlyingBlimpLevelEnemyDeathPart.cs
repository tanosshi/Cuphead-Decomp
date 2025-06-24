using UnityEngine;

public class FlyingBlimpLevelEnemyDeathPart : AbstractProjectile
{
	[SerializeField]
	private bool gear;

	private LevelProperties.FlyingBlimp.Gear properties;

	private const float VELOCITY_X_MIN = -500f;

	private const float VELOCITY_X_MAX = 500f;

	private const float VELOCITY_Y_MIN = 500f;

	private const float VELOCITY_Y_MAX = 1200f;

	private const float GRAVITY = -100f;

	private Vector2 velocity;

	private float accumulatedGravity;

	private float parryCounter;

	private bool getNewWeapon;

	private bool parriedIt;

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0.25f;
		}
	}

	public FlyingBlimpLevelEnemyDeathPart CreatePart(Vector3 position, LevelProperties.FlyingBlimp.Gear properties)
	{
		FlyingBlimpLevelEnemyDeathPart flyingBlimpLevelEnemyDeathPart = InstantiatePrefab<FlyingBlimpLevelEnemyDeathPart>();
		flyingBlimpLevelEnemyDeathPart.transform.position = position;
		flyingBlimpLevelEnemyDeathPart.properties = properties;
		return flyingBlimpLevelEnemyDeathPart;
	}

	protected override void Start()
	{
		base.Start();
		if (!gear)
		{
			velocity = new Vector2(Random.Range(-500f, 500f), Random.Range(500f, 1200f));
		}
		else
		{
			velocity = new Vector2(-500f, properties.bounceHeight);
		}
	}

	protected override void FixedUpdate()
	{
		Update();
		base.transform.position += (Vector3)(velocity + new Vector2(0f - properties.bounceSpeed, accumulatedGravity)) * Time.fixedDeltaTime;
		accumulatedGravity += -100f;
		if (base.transform.position.y < -360f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
		if (!getNewWeapon)
		{
			if (parryCounter < (float)properties.parryCount)
			{
				parryCounter += 1f;
				accumulatedGravity = 0f;
			}
			else
			{
				GetComponent<SpriteRenderer>().color = ColorUtils.HexToColor("FF00EDFF");
				FrameDelayedCallback(SetWeapon, 5);
				accumulatedGravity = 0f;
			}
		}
		else
		{
			parriedIt = true;
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (getNewWeapon && !parriedIt)
		{
			AbstractPlayerController next = PlayerManager.GetNext();
			PlanePlayerController planePlayerController = next as PlanePlayerController;
			planePlayerController.weaponManager.SwitchToWeapon(Weapon.plane_weapon_laser);
			Die();
		}
	}

	private void SetWeapon()
	{
		getNewWeapon = true;
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
