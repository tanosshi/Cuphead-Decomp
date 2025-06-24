using System;
using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelFlamer : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private float loopSize;

	[SerializeField]
	private MinMax startDelayRange;

	[SerializeField]
	private MinMax respawnRange;

	private GameObject pivotPoint;

	private float angle;

	private float speed;

	private float moveSpeed;

	private AbstractPlayerController player;

	private Vector3 startPos;

	private bool isDead;

	protected override void OnStart()
	{
		isDead = false;
		angle = (float)Math.PI;
		base.transform.position = startPos;
		StartCoroutine(check_dist_cr());
	}

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position;
		pivotPoint = new GameObject("pivotPoint");
		pivotPoint.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 200f);
		angle = (float)Math.PI;
		StartCoroutine(check_dist_cr());
	}

	private void PathMovement()
	{
		angle += speed * CupheadTime.FixedDelta;
		Vector3 vector = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
		base.transform.position = pivotPoint.transform.position;
		base.transform.position += vector + vector2;
	}

	private void MovePivot()
	{
		pivotPoint.transform.AddPosition(moveSpeed * CupheadTime.FixedDelta);
	}

	private IEnumerator check_dist_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		player = PlayerManager.GetNext();
		bool movingLeft = ((base.transform.position.x > player.transform.position.x) ? true : false);
		moveSpeed = ((!movingLeft) ? base.Properties.flamerXSpeed.RandomFloat() : (0f - base.Properties.flamerXSpeed.RandomFloat()));
		AudioManager.PlayLoop("castle_flamer_loop");
		emitAudioFromObject.Add("castle_flamer_loop");
		base.animator.SetTrigger("OnFlame");
		yield return base.animator.WaitForAnimationToEnd(this, "Flame_Appear");
		GetComponent<Collider2D>().enabled = true;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, startDelayRange.RandomFloat());
		YieldInstruction wait = new WaitForFixedUpdate();
		StartCoroutine(accelerate_speed_cr());
		while (!isDead)
		{
			PathMovement();
			MovePivot();
			yield return wait;
		}
	}

	private IEnumerator accelerate_speed_cr()
	{
		float incrementBy = 1f;
		while (speed < base.Properties.flamerCirSpeed && !isDead)
		{
			speed += incrementBy * (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private void PlayFace()
	{
		base.animator.Play("Flame_Face", 1);
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawWireSphere(base.transform.position, 100f);
	}

	protected override void Die()
	{
		Deactivate();
	}

	private void Deactivate()
	{
		isDead = true;
		AudioManager.Stop("castle_flamer_loop");
		speed = 0f;
		GetComponent<Collider2D>().enabled = false;
		base.animator.Play("Flame_Appear_Loop", 0);
		base.animator.Play("Off", 1);
		StartCoroutine(activate_cr());
	}

	private IEnumerator activate_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, respawnRange.RandomFloat());
		OnStart();
	}
}
