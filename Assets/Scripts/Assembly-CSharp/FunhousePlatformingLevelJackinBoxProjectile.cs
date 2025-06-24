using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelJackinBoxProjectile : BasicProjectile
{
	private AbstractPlayerController player;

	private float delay;

	protected override void Start()
	{
		base.Start();
		move = false;
		StartCoroutine(animation_cr());
	}

	public FunhousePlatformingLevelJackinBoxProjectile Create(Vector3 pos, float speed, float delay, AbstractPlayerController player, int direction)
	{
		FunhousePlatformingLevelJackinBoxProjectile funhousePlatformingLevelJackinBoxProjectile = base.Create(pos, 0f, speed) as FunhousePlatformingLevelJackinBoxProjectile;
		funhousePlatformingLevelJackinBoxProjectile.delay = delay;
		funhousePlatformingLevelJackinBoxProjectile.player = player;
		funhousePlatformingLevelJackinBoxProjectile.StartAnimation(direction);
		return funhousePlatformingLevelJackinBoxProjectile;
	}

	private void StartAnimation(int direction)
	{
		switch (direction)
		{
		case 1:
			animator.Play("Top_Start");
			break;
		case 2:
			animator.Play("Left_Start");
			break;
		case 3:
			animator.Play("Bottom_Start");
			break;
		case 4:
			animator.Play("Right_Start");
			break;
		}
	}

	private IEnumerator animation_cr()
	{
		yield return animator.WaitForAnimationToStart(this, "Projectile");
		yield return CupheadTime.WaitForSeconds(this, delay);
		animator.SetTrigger("Move");
		yield return animator.WaitForAnimationToEnd(this, "Projectile_Move_Start");
		Vector3 dir = player.transform.position - base.transform.position;
		float start = base.transform.rotation.z;
		float end = MathUtils.DirectionToAngle(dir);
		float t = 0f;
		float time = 0.1f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetEulerAngles(null, null, Mathf.Lerp(start, end, t / time));
			yield return null;
		}
		move = true;
		yield return null;
	}
}
