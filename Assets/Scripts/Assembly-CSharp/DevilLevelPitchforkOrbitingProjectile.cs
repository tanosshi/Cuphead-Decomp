using System.Collections;
using UnityEngine;

public class DevilLevelPitchforkOrbitingProjectile : AbstractProjectile
{
	private AbstractProjectile target;

	private float rotationSpeed;

	private float radius;

	private float angle;

	private float waitTime;

	private DevilLevelSittingDevil parent;

	private bool waitTimeUp;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	public DevilLevelPitchforkOrbitingProjectile Create(AbstractProjectile target, float angle, float rotationSpeed, float radius, DevilLevelSittingDevil parent, float waitTime)
	{
		DevilLevelPitchforkOrbitingProjectile devilLevelPitchforkOrbitingProjectile = InstantiatePrefab<DevilLevelPitchforkOrbitingProjectile>();
		devilLevelPitchforkOrbitingProjectile.target = target;
		devilLevelPitchforkOrbitingProjectile.angle = angle;
		devilLevelPitchforkOrbitingProjectile.rotationSpeed = rotationSpeed;
		devilLevelPitchforkOrbitingProjectile.radius = radius;
		devilLevelPitchforkOrbitingProjectile.parent = parent;
		devilLevelPitchforkOrbitingProjectile.waitTime = waitTime;
		devilLevelPitchforkOrbitingProjectile.waitTimeUp = false;
		return devilLevelPitchforkOrbitingProjectile;
	}

	public DevilLevelPitchforkOrbitingProjectile Create(AbstractProjectile target, float angle, float rotationSpeed, float radius, DevilLevelSittingDevil parent)
	{
		DevilLevelPitchforkOrbitingProjectile devilLevelPitchforkOrbitingProjectile = InstantiatePrefab<DevilLevelPitchforkOrbitingProjectile>();
		devilLevelPitchforkOrbitingProjectile.target = target;
		devilLevelPitchforkOrbitingProjectile.angle = angle;
		devilLevelPitchforkOrbitingProjectile.rotationSpeed = rotationSpeed;
		devilLevelPitchforkOrbitingProjectile.radius = radius;
		devilLevelPitchforkOrbitingProjectile.parent = parent;
		devilLevelPitchforkOrbitingProjectile.waitTimeUp = true;
		return devilLevelPitchforkOrbitingProjectile;
	}

	protected override void Update()
	{
		base.Update();
		if (parent == null)
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator wait_time_cr()
	{
		yield return new WaitForSeconds(waitTime);
		waitTimeUp = true;
		GetComponent<Collider2D>().enabled = true;
		animator.SetTrigger("Continue");
		animator.SetBool("StartAtHalf", Rand.Bool());
	}

	protected override void Start()
	{
		base.Start();
		Vector2 vector = (Vector2)target.transform.position + MathUtils.AngleToDirection(angle) * radius;
		base.transform.SetPosition(vector.x, vector.y);
		if (!waitTimeUp)
		{
			GetComponent<Collider2D>().enabled = false;
			StartCoroutine(wait_time_cr());
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (waitTimeUp && !base.dead)
		{
			if (target == null || target.dead)
			{
				Die();
				return;
			}
			angle += rotationSpeed * CupheadTime.FixedDelta;
			Vector2 vector = (Vector2)target.transform.position + MathUtils.AngleToDirection(angle) * radius;
			base.transform.SetPosition(vector.x, vector.y);
		}
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
		OrbitStopSFX();
	}

	private void OrbitStartSFX()
	{
		AudioManager.PlayLoop("devil_orbit_projectile");
		emitAudioFromObject.Add("devil_orbit_projectile");
	}

	private void OrbitStopSFX()
	{
		AudioManager.Stop("devil_orbit_projectile");
	}
}
