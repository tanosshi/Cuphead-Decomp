using UnityEngine;

public class CircusPlatformingLevelPretzel : AbstractPlatformingLevelEnemy
{
	private const string SaltParameterName = "Salt";

	public bool goingLeft;

	[SerializeField]
	private float jumpMultiplierX;

	[SerializeField]
	private float jumpMultiplierY;

	[SerializeField]
	private float inverseJumpMultiplierY;

	[SerializeField]
	private Transform transformDustA;

	[SerializeField]
	private Transform transformDustB;

	private Transform[] path;

	private bool moving;

	private int nextPoint;

	private Vector3 startPoint;

	protected override void OnStart()
	{
	}

	public void SetPath(Transform[] path)
	{
		this.path = path;
	}

	public void SetStartPosition(int index)
	{
		nextPoint = index;
		base.transform.position = path[nextPoint].position;
	}

	public void Jump()
	{
		moving = true;
		if (goingLeft)
		{
			nextPoint--;
		}
		else
		{
			nextPoint++;
		}
		if (nextPoint < 0 || (path != null && nextPoint >= path.Length))
		{
			Die();
		}
		if (nextPoint < path.Length)
		{
			startPoint = base.transform.position;
		}
	}

	public void Land()
	{
		moving = false;
		base.animator.SetTrigger("Salt");
		if (nextPoint < path.Length)
		{
			base.transform.position = path[nextPoint].position;
		}
	}

	public void JumpSFX()
	{
		AudioManager.Play("circus_pretzel_jump");
		emitAudioFromObject.Add("circus_pretzel_jump");
	}

	protected override void Die()
	{
		AudioManager.Stop("circus_pretzel_jump");
		AudioManager.Play("circus_generic_death_big");
		emitAudioFromObject.Add("circus_generic_death_big");
		base.Die();
		Object.Destroy(base.transform.parent.gameObject);
	}
}
