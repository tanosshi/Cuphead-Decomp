using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelMiner : PlatformingLevelGroundMovementEnemy
{
	[SerializeField]
	private MountainPlatformingLevelPickaxeProjectile pickaxe;

	[SerializeField]
	private SpriteRenderer straight;

	[SerializeField]
	private SpriteRenderer up;

	[SerializeField]
	private SpriteRenderer down;

	[SerializeField]
	private Transform lookAt;

	[SerializeField]
	private MountainPlatformingLevelMinerRope rope;

	[SerializeField]
	private Transform root;

	[SerializeField]
	private Transform catchRoot;

	private Vector3 startPos;

	private AbstractPlayerController player;

	private MountainPlatformingLevelPickaxeProjectile currentPickaxe;

	private bool isShooting;

	private bool inAttack;

	private bool leftRope;

	private float offset = 50f;

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position;
		StartCoroutine(descend_cr());
	}

	private IEnumerator descend_cr()
	{
		floating = false;
		landing = true;
		float t = 0f;
		float time = base.Properties.minerDescendTime;
		Vector3 endPos = new Vector3(base.transform.position.x, base.transform.position.y - 400f);
		Vector3 startPos = base.transform.position;
		AudioManager.Play("castle_miner_spawn");
		emitAudioFromObject.Add("castle_miner_spawn");
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(startPos, endPos, val);
			yield return null;
		}
		rope.transform.parent = null;
		yield return CupheadTime.WaitForSeconds(this, 0.4f);
		base.animator.SetTrigger("Continue");
		rope.animator.SetTrigger("Jump");
		floating = true;
		landing = false;
		yield return base.animator.WaitForAnimationToEnd(this, "Jump_Start");
		rope.animator.SetTrigger("Pull");
		while (!base.Grounded)
		{
			yield return null;
		}
		base.animator.SetTrigger("Land");
		StartCoroutine(rope.pull_up_rope_cr(base.Properties.minerRopeAscendTime, startPos));
		leftRope = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Jump_End");
		StartCoroutine(shoot_cr());
		StartCoroutine(look_direction_cr());
		StartCoroutine(face_direction_cr());
		yield return null;
	}

	private IEnumerator look_direction_cr()
	{
		float maxDist = 30f;
		while (player == null)
		{
			yield return null;
		}
		while (true)
		{
			float dist = player.transform.position.y - lookAt.transform.position.y;
			if (dist < maxDist && dist > 0f - maxDist)
			{
				straight.enabled = true;
				down.enabled = false;
				up.enabled = false;
			}
			else if (dist > maxDist)
			{
				straight.enabled = false;
				down.enabled = false;
				up.enabled = true;
			}
			else
			{
				straight.enabled = false;
				down.enabled = true;
				up.enabled = false;
			}
			yield return null;
		}
	}

	private IEnumerator face_direction_cr()
	{
		while (player == null)
		{
			yield return null;
		}
		while (true)
		{
			if (!inAttack && ((player.transform.position.x > base.transform.position.x && base.direction == Direction.Left) || (player.transform.position.x < base.transform.position.x && base.direction == Direction.Right)) && !base.animator.GetCurrentAnimatorStateInfo(0).IsName("Turn"))
			{
				Turn();
			}
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			player = PlayerManager.GetNext();
			yield return CupheadTime.WaitForSeconds(this, base.Properties.minerShotDelay.RandomFloat());
			inAttack = true;
			base.animator.SetTrigger("Shoot");
			while (currentPickaxe != null || !isShooting)
			{
				yield return null;
			}
			base.animator.Play("Catch");
			isShooting = false;
			inAttack = false;
			yield return null;
		}
	}

	private void ShootPickaxe()
	{
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		Vector3 vector = player.transform.position - root.transform.position;
		Vector3 targetPos = root.transform.position + vector.normalized * base.Properties.minerDistance;
		currentPickaxe = pickaxe.Create(root.transform.position, MathUtils.DirectionToAngle(vector), base.Properties.minerShootSpeed, this, targetPos, catchRoot.transform.position);
		isShooting = true;
	}

	private void ShootSFX()
	{
		AudioManager.Play("castle_miner_throw");
		emitAudioFromObject.Add("castle_miner_throw");
	}

	private void CatchSFX()
	{
		AudioManager.Play("castle_miner_catch_pick");
		emitAudioFromObject.Add("castle_miner_catch_pick");
	}

	private void Offset()
	{
		if (base.direction == Direction.Left)
		{
			base.transform.AddPosition(47f);
		}
		else
		{
			base.transform.AddPosition(-47f);
		}
	}

	protected override void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		if (!leftRope)
		{
			rope.animator.SetTrigger("Jump");
			rope.animator.SetTrigger("Pull");
			rope.transform.parent = null;
			rope.PullRope(base.Properties.minerRopeAscendTime, startPos);
		}
		AudioManager.Play("castle_generic_death");
		emitAudioFromObject.Add("castle_generic_death");
		base.Die();
	}
}
