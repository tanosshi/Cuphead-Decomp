using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelOctopus : PlatformingLevelAutoscrollObject
{
	[SerializeField]
	private Effect puff;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Transform tentacleFront;

	[SerializeField]
	private Transform tentacleBack;

	[SerializeField]
	private ParrySwitch anchor;

	[SerializeField]
	private HarbourPlatformingLevelOctoProjectile projectile;

	[SerializeField]
	private MinMax scrollMinMax = new MinMax(-300f, 200f);

	[SerializeField]
	private GameObject pinkGem;

	[SerializeField]
	private CollisionChild collisionChild;

	[SerializeField]
	private float accelerationTime = 0.3f;

	[SerializeField]
	private float holdSpeedTime = 2f;

	[SerializeField]
	private float deccelerationTime = 1f;

	[SerializeField]
	private float speedupMultiplier = 1.2f;

	[SerializeField]
	private float tuckDownDelay = 2f;

	[SerializeField]
	private float gemOffTime = 0.7f;

	private bool firstSwitch = true;

	private float timeSinceShot = 1000f;

	private bool tuckedDown;

	private bool moveTentacles;

	private float tentacleOffset = 100f;

	private float yPosStart;

	private const float LOCK_DISTANCE = 600f;

	protected override void Awake()
	{
		base.Awake();
		anchor.OnActivate += Switched;
		yPosStart = base.transform.position.y;
		collisionChild.OnPlayerProjectileCollision += OnCollisionPlayerProjectile;
		collisionChild.OnAnyCollision += OnCollision;
		checkToLock = false;
		pinkGem.SetActive(true);
		StartCoroutine(gem_shine_switch_cr());
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayerProjectile(hit, phase);
		if (!tuckedDown)
		{
			timeSinceShot = 0f;
			base.animator.SetTrigger("PlayerShooting");
		}
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if ((bool)hit.GetComponent<HarbourPlatformingLevelIceberg>())
		{
			if (!tuckedDown)
			{
				StartCoroutine(disable_cr());
			}
			hit.GetComponent<HarbourPlatformingLevelIceberg>().DeathParts();
			Object.Destroy(hit.gameObject);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		CupheadLevelCamera current = CupheadLevelCamera.Current;
		float autoScrollSpeedMultiplier = current.autoScrollSpeedMultiplier;
		timeSinceShot += CupheadTime.Delta;
		if (tuckedDown || timeSinceShot > holdSpeedTime)
		{
			autoScrollSpeedMultiplier -= (float)CupheadTime.Delta * (speedupMultiplier - 1f) / deccelerationTime;
			autoScrollSpeedMultiplier = Mathf.Max(1f, autoScrollSpeedMultiplier);
		}
		else
		{
			autoScrollSpeedMultiplier += (float)CupheadTime.Delta * (speedupMultiplier - 1f) / accelerationTime;
			autoScrollSpeedMultiplier = Mathf.Min(speedupMultiplier, autoScrollSpeedMultiplier);
		}
		current.SetAutoscrollSpeedMultiplier(autoScrollSpeedMultiplier);
		if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			base.animator.speed = autoScrollSpeedMultiplier;
		}
		else
		{
			base.animator.speed = 1f;
		}
	}

	private void Switched()
	{
		if (firstSwitch)
		{
			base.animator.SetTrigger("StartOctopus");
			StartAutoscroll();
			StartCoroutine(start_octopus_cr());
			firstSwitch = false;
		}
		else
		{
			base.animator.SetTrigger("Shoot");
			ShootSFX();
		}
	}

	public bool Started()
	{
		return base.isMoving;
	}

	private IEnumerator start_octopus_cr()
	{
		while (base.transform.position.x > CupheadLevelCamera.Current.transform.position.x + scrollMinMax.min)
		{
			yield return null;
		}
		CupheadLevelCamera.Current.OffsetCamera(true, true);
		StartCoroutine(idle_bounce_cr());
		base.animator.SetTrigger("StartTentacles");
		IdleTentaclesSFX();
		base.transform.parent = CupheadLevelCamera.Current.transform;
		yield return null;
	}

	private IEnumerator disable_cr()
	{
		HeadSquishSFX();
		base.animator.SetBool("IsHit", true);
		tuckedDown = true;
		float endPos = base.transform.position.y - 500f;
		float speed = 300f;
		Vector3 pos = base.transform.position;
		while (base.transform.position.y > endPos)
		{
			base.transform.AddPosition(0f, (0f - speed) * CupheadTime.FixedDelta);
			yield return null;
		}
		pos = base.transform.position;
		yield return CupheadTime.WaitForSeconds(this, tuckDownDelay);
		yield return null;
		while (base.transform.position.y < yPosStart)
		{
			base.transform.AddPosition(0f, speed * CupheadTime.FixedDelta);
			yield return null;
		}
		base.transform.position = new Vector3(base.transform.position.x, yPosStart);
		base.animator.SetBool("IsHit", false);
		HeadSquishSFX();
		tuckedDown = false;
		yield return null;
	}

	private IEnumerator end_octopus_cr()
	{
		MoveLoop();
		base.animator.SetTrigger("EndOctopus");
		base.transform.parent = null;
		float endPos = base.transform.position.y - 1000f;
		float speed = 100f;
		while (base.transform.position.y > endPos)
		{
			base.transform.AddPosition(0f, (0f - speed) * CupheadTime.FixedDelta);
			yield return null;
		}
		CupheadLevelCamera.Current.OffsetCamera(false, true);
		Object.Destroy(base.gameObject);
		yield return null;
	}

	protected override void EndAutoscroll()
	{
		StartCoroutine(end_octopus_cr());
	}

	private void Shoot()
	{
		ShootSFX();
		pinkGem.SetActive(false);
		anchor.enabled = false;
		puff.Create(projectileRoot.transform.position);
		projectile.Create(projectileRoot.transform.position);
		StartCoroutine(gem_timer_cr());
	}

	private IEnumerator gem_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, gemOffTime);
		pinkGem.SetActive(true);
		anchor.enabled = true;
		yield return null;
	}

	private IEnumerator gem_shine_switch_cr()
	{
		string order = "A1,B1,B2,A2,B1,A1,B2,A2";
		int orderIndex = 0;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.42f, 0.67f));
			base.animator.Play("Shine_" + order.Split(',')[orderIndex], 1);
			orderIndex = (orderIndex + 1) % order.Split(',').Length;
			yield return null;
		}
	}

	private void TentacleBackSwitch()
	{
		Vector3 localPosition = tentacleBack.localPosition;
		localPosition.x = ((!moveTentacles) ? (tentacleBack.localPosition.x + tentacleOffset) : (tentacleBack.localPosition.x - tentacleOffset));
		tentacleBack.localPosition = localPosition;
	}

	private void TentacleFrontSwitch()
	{
		moveTentacles = !moveTentacles;
		Vector3 localPosition = tentacleFront.localPosition;
		localPosition.x = ((!moveTentacles) ? (tentacleFront.localPosition.x + tentacleOffset) : (tentacleFront.localPosition.x - tentacleOffset));
		tentacleFront.localPosition = localPosition;
	}

	private IEnumerator idle_bounce_cr()
	{
		float angle = 0f;
		float yVelocity = 7f;
		float sinSize = 2f;
		while (true)
		{
			if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && (float)CupheadTime.Delta != 0f)
			{
				angle += yVelocity * (float)CupheadTime.Delta;
				Vector3 moveY = new Vector3(0f, Mathf.Sin(angle) * sinSize);
				base.transform.localPosition += moveY;
				yield return null;
			}
			yield return null;
		}
	}

	private void HeadSquishSFX()
	{
		AudioManager.Play("harbour_octopus_head_squish");
		emitAudioFromObject.Add("harbour_octopus_head_squish");
	}

	private void ShootSFX()
	{
		AudioManager.Play("harbour_octopus_shoot");
		emitAudioFromObject.Add("harbour_octopus_shoot");
	}

	private void IdleTentaclesSFX()
	{
		AudioManager.Stop("harbour_octopus_move_loop");
		AudioManager.PlayLoop("harbour_octopus_idle_tentacles");
		emitAudioFromObject.Add("harbour_octopus_idle_tentacles");
	}

	private void MoveLoop()
	{
		AudioManager.Stop("harbour_octopus_idle_tentacles");
		AudioManager.PlayLoop("harbour_octopus_move_loop");
		emitAudioFromObject.Add("harbour_octopus_move_loop");
	}

	private void RideStartSFX()
	{
		AudioManager.Stop("harbour_octopus_idle_tentacles");
		AudioManager.Stop("harbour_octopus_move_loop");
		AudioManager.Play("harbour_octopus_ride_start");
		emitAudioFromObject.Add("harbour_octopus_ride_start");
	}
}
