using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelUrchin : PlatformingLevelGroundMovementEnemy
{
	public enum Type
	{
		A = 0,
		B = 1
	}

	private const float ON_SCREEN_SOUND_PADDING = 100f;

	public Type type;

	private bool isInSight;

	protected override void Start()
	{
		base.Start();
		GetComponent<PlatformingLevelEnemyAnimationHandler>().SelectAnimation(type.ToString());
		StartCoroutine(play_loop_SFX());
	}

	private IEnumerator play_loop_SFX()
	{
		bool playerLeft = false;
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
			{
				playerLeft = false;
				if (!AudioManager.CheckIfPlaying("harbour_urchin_walk"))
				{
					AudioManager.PlayLoop("harbour_urchin_walk");
					emitAudioFromObject.Add("harbour_urchin_walk");
				}
			}
			else if (!playerLeft)
			{
				AudioManager.Stop("harbour_urchin_walk");
				playerLeft = true;
			}
			yield return null;
		}
	}

	protected override Coroutine Turn()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
		{
			AudioManager.Play("harbour_urchin_turn");
			emitAudioFromObject.Add("harbour_urchin_turn");
		}
		return base.Turn();
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Stop("harbour_urchin_walk");
		AudioManager.Play("harmour_urchin_death");
		emitAudioFromObject.Add("harmour_urchin_death");
	}
}
