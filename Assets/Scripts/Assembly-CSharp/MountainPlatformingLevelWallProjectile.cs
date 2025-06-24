using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelWallProjectile : AbstractProjectile
{
	[SerializeField]
	private Transform root;

	[SerializeField]
	private Transform root1;

	private Vector2 velocity;

	private Vector2 startVelocity;

	private float gravity;

	private float timeToApex;

	private float yGround;

	private bool onGround;

	public MountainPlatformingLevelWallProjectile Create(Vector2 pos, float rotation, Vector2 velocity, float gravity, float yGround)
	{
		MountainPlatformingLevelWallProjectile mountainPlatformingLevelWallProjectile = base.Create() as MountainPlatformingLevelWallProjectile;
		mountainPlatformingLevelWallProjectile.transform.position = pos;
		mountainPlatformingLevelWallProjectile.velocity = velocity;
		mountainPlatformingLevelWallProjectile.startVelocity = velocity;
		mountainPlatformingLevelWallProjectile.gravity = gravity;
		mountainPlatformingLevelWallProjectile.yGround = yGround;
		mountainPlatformingLevelWallProjectile.transform.SetEulerAngles(null, null, rotation);
		return mountainPlatformingLevelWallProjectile;
	}

	protected override void Start()
	{
		base.Start();
		float num = velocity.y / gravity;
		timeToApex = Mathf.Sqrt(2f * velocity.y / gravity);
		startVelocity.y = timeToApex * gravity;
		StartCoroutine(check_to_kill_cr());
	}

	private IEnumerator handle_hit_ground_cr()
	{
		yield return animator.WaitForAnimationToEnd(this, "Hit");
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		onGround = false;
		yield return null;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		if (!onGround && base.transform.position.y <= yGround)
		{
			onGround = true;
			HandleHitGround();
		}
	}

	private void HandleHitGround()
	{
		velocity.y = startVelocity.y;
		animator.SetTrigger("OnHitGround");
		StartCoroutine(handle_hit_ground_cr());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
		{
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			velocity.y -= gravity * CupheadTime.FixedDelta;
			base.transform.SetEulerAngles(null, null, Mathf.Atan2(0f - velocity.y, 0f - velocity.x) * 57.29578f);
		}
	}

	private void ChangeRootEnd()
	{
		base.transform.position = root.transform.position;
		base.transform.SetEulerAngles(null, null, Mathf.Atan2(0f - velocity.y, 0f - velocity.x) * 57.29578f);
	}

	private void ChangeRootBeginning()
	{
		AudioManager.Play("castle_mountain_wall_oil_bounce");
		emitAudioFromObject.Add("castle_mountain_wall_oil_bounce");
		base.transform.position = root1.transform.position;
		base.transform.SetEulerAngles(null, null, 0f);
	}

	private IEnumerator check_to_kill_cr()
	{
		while (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(0f, 1000f)))
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
