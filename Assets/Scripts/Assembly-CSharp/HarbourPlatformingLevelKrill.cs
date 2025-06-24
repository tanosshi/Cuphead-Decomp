using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelKrill : AbstractPlatformingLevelEnemy
{
	private Vector2 velocity;

	private float gravity;

	public bool isParryable;

	protected override void Start()
	{
		base.Start();
		gravity = base.Properties.krillGravity;
		velocity.x = Random.Range(0f - base.Properties.krillVelocityX.min, 0f - base.Properties.krillVelocityX.max);
		velocity.y = Random.Range(base.Properties.krillVelocityY.min, base.Properties.krillVelocityY.max);
		_canParry = isParryable;
		StartCoroutine(move_cr());
	}

	protected override void OnStart()
	{
	}

	public void SetType(string type)
	{
		GetComponent<PlatformingLevelEnemyAnimationHandler>().SelectAnimation(type);
	}

	protected override void Update()
	{
		base.Update();
	}

	private IEnumerator move_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.Properties.krillLaunchDelay);
		JumpSFX();
		while (true)
		{
			base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
			velocity.y -= gravity * (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private void JumpSFX()
	{
		AudioManager.Play("harbour_shrimp_jump");
		emitAudioFromObject.Add("harbour_shrimp_jump");
	}
}
