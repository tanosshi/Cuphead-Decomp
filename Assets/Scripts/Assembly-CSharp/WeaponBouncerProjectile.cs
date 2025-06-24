using System.Collections;
using UnityEngine;

public class WeaponBouncerProjectile : AbstractProjectile
{
	[SerializeField]
	private bool isEx;

	[SerializeField]
	private WeaponArcProjectileExplosion exExplosion;

	[SerializeField]
	private Effect trailFxPrefab;

	[SerializeField]
	private float trailFxMaxOffset;

	[SerializeField]
	private float trailDelay;

	public float gravity;

	public Vector2 velocity;

	public WeaponBouncer weapon;

	public float bounceRatio;

	public float bounceSpeedDampening;

	private float timeUntilUnfreeze;

	private const float bounceFreezeTime = 1f / 24f;

	private int numBounces;

	private bool firstUpdate = true;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (isEx)
		{
			StartCoroutine(trail_cr());
			return;
		}
		switch (Random.Range(0, 4))
		{
		case 0:
			animator.Play("A", 0, Random.Range(0f, 1f));
			break;
		case 1:
			animator.Play("B", 0, Random.Range(0f, 1f));
			break;
		case 2:
			animator.Play("C", 0, Random.Range(0f, 1f));
			break;
		case 3:
			animator.Play("D", 0, Random.Range(0f, 1f));
			break;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (firstUpdate)
		{
			firstUpdate = false;
			if (velocity.y < 0f)
			{
				return;
			}
		}
		if (!base.dead)
		{
			UpdateInAir();
			if (!isEx && !CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 100f)) && !CupheadLevelCamera.Current.ContainsPoint(new Vector3(base.transform.position.x, base.transform.position.y - 300f, 0f), new Vector2(150f, 100f)))
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void UpdateInAir()
	{
		if (timeUntilUnfreeze > 0f)
		{
			timeUntilUnfreeze -= CupheadTime.FixedDelta;
			base.transform.position += new Vector3(velocity.x * CupheadTime.FixedDelta, 0f, 0f);
		}
		else
		{
			velocity.y -= gravity * CupheadTime.FixedDelta;
			base.transform.position += (Vector3)(velocity * CupheadTime.FixedDelta);
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if ((component == null || !component.canFallThrough) && velocity.y < 0f)
		{
			HitGround(hit);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if (component != null && !component.canFallThrough && velocity.y < 0f)
		{
			HitGround(hit);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (!isEx)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionEnemy(hit, phase);
	}

	private void HitGround(GameObject hit)
	{
		float num = velocity.magnitude * WeaponProperties.LevelWeaponBouncer.Basic.bounceRatio - WeaponProperties.LevelWeaponBouncer.Basic.bounceSpeedDampening;
		if (num <= 0f || numBounces >= WeaponProperties.LevelWeaponBouncer.Basic.numBounces || isEx)
		{
			Die();
			return;
		}
		velocity = velocity.normalized * num;
		velocity.y *= -1f;
		numBounces++;
		timeUntilUnfreeze = 1f / 24f;
		animator.SetTrigger((!Rand.Bool()) ? "Bounce_B" : "Bounce_A");
	}

	private IEnumerator trail_cr()
	{
		while (!base.dead)
		{
			yield return CupheadTime.WaitForSeconds(this, trailDelay);
			if (base.dead)
			{
				break;
			}
			trailFxPrefab.Create((Vector2)base.transform.position + MathUtils.RandomPointInUnitCircle() * trailFxMaxOffset);
		}
	}

	protected override void Die()
	{
		base.Die();
		if (isEx)
		{
			WeaponArcProjectileExplosion weaponArcProjectileExplosion = exExplosion.Create(base.transform.position, Damage, base.DamageMultiplier);
			MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
			meterScoreTracker.Add(weaponArcProjectileExplosion.DamageDealer);
			AudioManager.Play("player_weapon_bouncer_ex_explosion");
			emitAudioFromObject.Add("player_weapon_bouncer_ex_explosion");
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.SetEulerAngles(null, null, Random.Range(0, 360));
		}
	}
}
