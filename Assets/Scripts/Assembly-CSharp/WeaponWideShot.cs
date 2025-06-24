using System.Collections;

public class WeaponWideShot : AbstractLevelWeapon
{
	private float maxAngle;

	private bool isInitialized;

	protected override bool rapidFire
	{
		get
		{
			return true;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.LevelWeaponWideShot.Basic.rapidFireRate;
		}
	}

	protected override void Start()
	{
		base.Start();
		maxAngle = WeaponProperties.LevelWeaponWideShot.Basic.angleRange.max;
		StartCoroutine(angle_cr());
		isInitialized = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (isInitialized)
		{
			StartCoroutine(angle_cr());
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		float damage = WeaponProperties.LevelWeaponWideShot.Basic.damage;
		BasicProjectile basicProjectile = null;
		MinMax minMax = new MinMax(0f, maxAngle);
		for (int i = 0; (float)i < 3f; i++)
		{
			float floatAt = minMax.GetFloatAt((float)i / 2f);
			float num = minMax.max / 2f;
			basicProjectile = base.fireBasic() as BasicProjectile;
			basicProjectile.Speed = WeaponProperties.LevelWeaponWideShot.Basic.speed;
			basicProjectile.DestroyDistance = WeaponProperties.LevelWeaponWideShot.Basic.distance - 20f * (float)(i + 1);
			basicProjectile.Damage = damage;
			basicProjectile.PlayerId = player.id;
			basicProjectile.transform.AddEulerAngles(0f, 0f, floatAt - num);
		}
		return basicProjectile;
	}

	private IEnumerator angle_cr()
	{
		float openTimeMax = WeaponProperties.LevelWeaponWideShot.Basic.openingAngleSpeed;
		float closeTimeMax = WeaponProperties.LevelWeaponWideShot.Basic.closingAngleSpeed;
		float t = 0f;
		float val = 0f;
		while (true)
		{
			if (player.motor.Locked)
			{
				if (val < 1f)
				{
					val = t / openTimeMax;
					t += (float)CupheadTime.Delta;
				}
				else
				{
					val = 1f;
					t = 1f;
				}
			}
			else if (val > 0f)
			{
				val = t / closeTimeMax;
				t -= (float)CupheadTime.Delta;
			}
			else
			{
				val = 0f;
				t = 0f;
			}
			maxAngle = WeaponProperties.LevelWeaponWideShot.Basic.angleRange.GetFloatAt(val);
			yield return null;
		}
	}
}
