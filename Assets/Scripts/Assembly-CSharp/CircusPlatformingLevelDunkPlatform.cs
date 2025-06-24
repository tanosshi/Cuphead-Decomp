using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelDunkPlatform : AbstractCollidableObject
{
	private const string HitParameterName = "Hit";

	private const string DropParameterName = "Drop";

	private const string RaiseParameterName = "Raise";

	private const string StopSpinParameterName = "SpinStop";

	[SerializeField]
	private Collider2D platform;

	[SerializeField]
	private float platformDown;

	[SerializeField]
	private float targetSpin;

	private Collider2D collider2d;

	protected override void Start()
	{
		base.Start();
		collider2d = GetComponent<Collider2D>();
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if ((bool)hit.GetComponent<CircusPlatformingLevelCannonProjectile>())
		{
			collider2d.enabled = false;
			base.animator.SetTrigger("Hit");
			StartCoroutine(waitSpin_cr());
		}
	}

	public void Drop()
	{
		StartCoroutine(deactivate_cr());
	}

	private IEnumerator deactivate_cr()
	{
		base.animator.SetTrigger("Drop");
		platform.enabled = false;
		yield return CupheadTime.WaitForSeconds(this, platformDown);
		base.animator.SetTrigger("Raise");
	}

	public void ActivatePlatform()
	{
		collider2d.enabled = true;
		platform.enabled = true;
	}

	private IEnumerator waitSpin_cr()
	{
		AudioManager.Play("circus_platform_plank_target");
		emitAudioFromObject.Add("circus_platform_plank_target");
		yield return CupheadTime.WaitForSeconds(this, targetSpin);
		base.animator.SetTrigger("SpinStop");
	}

	private void DropSFX()
	{
		AudioManager.Play("circus_platform_plank_drop");
		emitAudioFromObject.Add("circus_platform_plank_drop");
	}

	private void RaiseSFX()
	{
		AudioManager.Play("circus_platform_plank_raise");
		emitAudioFromObject.Add("circus_platform_plank_raise");
	}

	private void PlankSFX()
	{
		AudioManager.Play("circus_platform_plank_target");
		emitAudioFromObject.Add("circus_platform_plank_target");
	}
}
