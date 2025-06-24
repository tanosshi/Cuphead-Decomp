using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelCrab : PlatformingLevelGroundMovementEnemy
{
	private const float ON_SCREEN_SOUND_PADDING = 1000f;

	private string target;

	private bool walkingBack;

	protected override void Awake()
	{
		base.Awake();
		base.animator.SetBool("goingLeft", (base.direction != Direction.Right) ? true : false);
		GetComponent<DamageReceiver>().enabled = false;
		walkingBack = false;
		SetTurnTarget(target);
		StartCoroutine(play_loop_SFX());
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (phase == CollisionPhase.Enter && (bool)hit.GetComponent<HarbourPlatformingLevelCrab>())
		{
			StartCoroutine(prepare_turn_cr(hit));
		}
	}

	protected override void CalculateDirection()
	{
	}

	private IEnumerator prepare_turn_cr(GameObject hit)
	{
		float dist = Vector3.Distance(hit.transform.position, base.transform.position);
		while (dist > 670f)
		{
			dist = Vector3.Distance(hit.transform.position, base.transform.position);
			yield return null;
		}
		Turn();
		yield return null;
	}

	protected override Coroutine Turn()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(1000f, 1000f)))
		{
			AudioManager.Play("harbour_crab_turn");
			emitAudioFromObject.Add("harbour_crab_turn");
		}
		walkingBack = !walkingBack;
		base.animator.SetBool("walkingBack", walkingBack);
		base.animator.SetBool("goingLeft", (base.direction != Direction.Right) ? true : false);
		target = ((base.direction != Direction.Right) ? "Turn_Right" : "Turn_Left");
		SetTurnTarget(target);
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, AbstractPlatformingLevelEnemy.CAMERA_DEATH_PADDING))
		{
			CupheadLevelCamera.Current.Shake(10f, 0.4f);
		}
		return base.Turn();
	}

	private IEnumerator play_loop_SFX()
	{
		bool playerLeft = false;
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(1000f, 1000f)))
			{
				playerLeft = false;
				if (!AudioManager.CheckIfPlaying("harbour_crab_walk") && !AudioManager.CheckIfPlaying("harbour_crab_turn"))
				{
					AudioManager.PlayLoop("harbour_crab_walk");
					emitAudioFromObject.Add("harbour_crab_walk");
				}
			}
			else if (!playerLeft)
			{
				AudioManager.Stop("harbour_crab_walk");
				playerLeft = true;
			}
			yield return null;
		}
	}
}
