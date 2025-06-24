using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelRocket : PlatformingLevelGroundMovementEnemy
{
	[SerializeField]
	private Transform sprite;

	[SerializeField]
	private FunhousePlatformingLevelExplosionFX explosion;

	[SerializeField]
	private float distToLaunch;

	[SerializeField]
	private float launchSpeed;

	private bool launched;

	private CollisionChild collisionChild;

	[SerializeField]
	private DamageReceiver collisionDamageReceiver;

	protected override void Update()
	{
		base.Update();
	}

	protected override void Start()
	{
		base.Start();
		collisionChild = GetComponentInChildren<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		collisionDamageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public void Init(Vector2 pos, bool gravityReversed, bool onRight)
	{
		base.transform.position = pos;
		AudioManager.PlayLoop("funhouse_rocket_idle_loop");
		emitAudioFromObject.Add("funhouse_rocket_idle_loop");
		base.gravityReversed = gravityReversed;
		if (gravityReversed)
		{
			base.transform.SetScale(null, -1f);
		}
		_direction = ((!onRight) ? Direction.Right : Direction.Left);
		StartCoroutine(launch_cr());
	}

	private IEnumerator launch_cr()
	{
		AudioManager.Stop("funhouse_rocket_idle_loop");
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AbstractPlayerController player = PlayerManager.GetNext();
		float dist = player.transform.position.x - base.transform.position.x;
		while (Mathf.Abs(dist) > distToLaunch)
		{
			player = PlayerManager.GetNext();
			dist = player.transform.position.x - base.transform.position.x;
			yield return null;
		}
		landing = true;
		launched = true;
		base.animator.SetTrigger("OnShoot");
		while (launched)
		{
			if (gravityReversed)
			{
				base.transform.position += Vector3.down * launchSpeed * CupheadTime.FixedDelta;
			}
			else
			{
				base.transform.position += Vector3.up * launchSpeed * CupheadTime.FixedDelta;
			}
			yield return null;
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		if (launched && phase == CollisionPhase.Enter)
		{
			Die();
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (launched && phase == CollisionPhase.Enter)
		{
			Die();
		}
	}

	protected override void Die()
	{
		AudioManager.Stop("funhouse_rocket_trans_to_spin");
		AudioManager.Play("funhouse_rocket_explode");
		emitAudioFromObject.Add("funhouse_rocket_explode");
		explosion.Create(sprite.transform.position);
		base.Die();
	}

	private void SoundRocketTransToSpin()
	{
		AudioManager.Play("funhouse_rocket_trans_to_spin");
		emitAudioFromObject.Add("funhouse_rocket_trans_to_spin");
		AudioManager.Play("funhouse_rocket_explode");
		emitAudioFromObject.Add("funhouse_rocket_explode");
	}
}
