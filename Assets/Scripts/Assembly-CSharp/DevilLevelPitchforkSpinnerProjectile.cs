using System;
using System.Collections;
using UnityEngine;

public class DevilLevelPitchforkSpinnerProjectile : AbstractProjectile
{
	private const float SIN_HEIGHT = 10f;

	private const float SIN_PERIOD = 1.5f;

	private const float DESTROY_X = 1500f;

	private float waitTime;

	private float homingDuration;

	private float homingMaxSpeed;

	private float homingAcceleration;

	private float startingY;

	private float t;

	private bool waitTimeUp;

	private bool SpinSFXPlaying;

	private DevilLevelSittingDevil parent;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	public DevilLevelPitchforkSpinnerProjectile Create(Vector2 pos, float maxSpeed, float acceleration, float homingDuration, DevilLevelSittingDevil parent, float waitTime)
	{
		DevilLevelPitchforkSpinnerProjectile devilLevelPitchforkSpinnerProjectile = InstantiatePrefab<DevilLevelPitchforkSpinnerProjectile>();
		devilLevelPitchforkSpinnerProjectile.transform.position = pos;
		devilLevelPitchforkSpinnerProjectile.homingDuration = homingDuration;
		devilLevelPitchforkSpinnerProjectile.startingY = pos.y;
		devilLevelPitchforkSpinnerProjectile.parent = parent;
		devilLevelPitchforkSpinnerProjectile.waitTime = waitTime;
		devilLevelPitchforkSpinnerProjectile.homingMaxSpeed = maxSpeed;
		devilLevelPitchforkSpinnerProjectile.homingAcceleration = acceleration;
		devilLevelPitchforkSpinnerProjectile.StartCoroutine(devilLevelPitchforkSpinnerProjectile.main_cr());
		devilLevelPitchforkSpinnerProjectile.SetParryable(true);
		devilLevelPitchforkSpinnerProjectile.animator.SetBool("IsPink", true);
		devilLevelPitchforkSpinnerProjectile.OrbitStartSFX();
		return devilLevelPitchforkSpinnerProjectile;
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

	protected override void FixedUpdate()
	{
		if (waitTimeUp)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetPosition(null, startingY + Mathf.Sin(t * (float)Math.PI * 2f / 1.5f) * 10f);
			if (Mathf.Abs(base.transform.position.x) > 1500f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			base.Update();
		}
	}

	private IEnumerator main_cr()
	{
		float t = 0f;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<GroundHomingMovement>().EnableHoming = false;
		yield return CupheadTime.WaitForSeconds(this, waitTime);
		waitTimeUp = true;
		animator.SetTrigger("Continue");
		animator.SetBool("StartAtHalf", Rand.Bool());
		GroundHomingMovement homingMovement = GetComponent<GroundHomingMovement>();
		homingMovement.maxSpeed = homingMaxSpeed;
		homingMovement.acceleration = homingAcceleration;
		homingMovement.bounceEnabled = false;
		homingMovement.destroyOffScreen = false;
		homingMovement.TrackingPlayer = PlayerManager.GetNext();
		homingMovement.EnableHoming = false;
		GetComponent<Collider2D>().enabled = true;
		GetComponent<GroundHomingMovement>().EnableHoming = true;
		yield return CupheadTime.WaitForSeconds(this, homingDuration);
		GetComponent<GroundHomingMovement>().EnableHoming = false;
	}

	protected override void Die()
	{
		base.Die();
		UnityEngine.Object.Destroy(base.gameObject);
		OrbitStopSFX();
	}

	public override void OnParry(AbstractPlayerController player)
	{
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}

	private void OrbitStartSFX()
	{
		if (!SpinSFXPlaying)
		{
			AudioManager.PlayLoop("devil_orbit_projectile");
			emitAudioFromObject.Add("devil_orbit_projectile");
			SpinSFXPlaying = true;
		}
	}

	private void OrbitStopSFX()
	{
		AudioManager.Stop("devil_orbit_projectile");
		SpinSFXPlaying = false;
	}
}
