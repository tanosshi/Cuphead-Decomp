using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CircusPlatformingLevelPoleBot : AbstractPlatformingLevelEnemy
{
	private const string FallingParameterName = "Falling";

	private const string DeadParameterName = "Dead";

	private Vector2 velocity;

	private Vector2 startVelocity;

	private float gravity;

	public bool isDying;

	public bool isSliding;

	[SerializeField]
	private float fallDelay;

	[SerializeField]
	private float deadSpin;

	[SerializeField]
	private MinMax minVelocity;

	[SerializeField]
	private MinMax maxVelocity;

	private float start;

	protected override void Start()
	{
		base.Start();
		start = base.transform.position.y;
		velocity = new Vector2(Random.Range(minVelocity.min, minVelocity.max), Random.Range(maxVelocity.min, maxVelocity.max));
		startVelocity = velocity;
		gravity = 1000f;
	}

	public void SlideDown()
	{
		StartCoroutine(slide_cr());
	}

	private IEnumerator slide_cr()
	{
		isSliding = true;
		base.animator.SetBool("Falling", true);
		YieldInstruction wait = new WaitForFixedUpdate();
		yield return CupheadTime.WaitForSeconds(this, fallDelay);
		while (base.transform.position.y > start - GetComponent<BoxCollider2D>().size.y * 1.38f)
		{
			base.transform.AddPosition(0f, (0f - base.Properties.poleSpeedMovement) * CupheadTime.FixedDelta);
			yield return wait;
		}
		start = base.transform.position.y;
		isSliding = false;
		base.animator.SetBool("Falling", false);
		yield return null;
	}

	protected override void OnStart()
	{
	}

	protected override void Die()
	{
		PoleBotDeathSFX();
		isDying = true;
		base.animator.SetTrigger("Dead");
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(fly_cr());
	}

	private IEnumerator fly_cr()
	{
		base._damageReceiver.enabled = false;
		YieldInstruction wait = new WaitForFixedUpdate();
		float timeToNormalApex = velocity.y / gravity;
		float timeToApex = Mathf.Sqrt(2f * base.transform.position.y / gravity);
		startVelocity.y = timeToApex * gravity;
		while (base.transform.position.y > CupheadLevelCamera.Current.Bounds.yMin)
		{
			base.transform.AddPosition((0f - velocity.x) * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			velocity.y -= gravity * CupheadTime.FixedDelta;
			base.transform.Rotate(Vector3.forward, deadSpin * (float)CupheadTime.Delta);
			yield return wait;
		}
		_003CDie_003E__BaseCallProxy0();
		yield return null;
	}

	private void PoleBotIdleSFX()
	{
		if (!AudioManager.CheckIfPlaying("circus_pole_guy_idle"))
		{
			AudioManager.Play("circus_pole_guy_idle");
			emitAudioFromObject.Add("circus_pole_guy_idle");
		}
	}

	private void PoleBotFallSFX()
	{
		AudioManager.Play("circus_pole_guy_falling");
		emitAudioFromObject.Add("circus_pole_guy_falling");
	}

	private void PoleBotDeathSFX()
	{
		AudioManager.Play("circus_pole_guy_death");
		emitAudioFromObject.Add("circus_pole_guy_death");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
