using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ForestPlatformingLevelAcornMaker : PlatformingLevelShootingEnemy
{
	private const float ON_SCREEN_SOUND_PADDING = 100f;

	[SerializeField]
	private Effect explosion;

	[SerializeField]
	private Transform gruntRoot;

	[SerializeField]
	private SpriteRenderer gruntSprite;

	[SerializeField]
	private ForestPlatformingLevelAcorn acornPrefab;

	[SerializeField]
	private Transform spawnRoot;

	private bool isDying;

	public Action killAcorns;

	protected override void Shoot()
	{
		ForestPlatformingLevelAcorn.Direction direction = ForestPlatformingLevelAcorn.Direction.Left;
		direction = ((!(_target.transform.position.x < base.transform.position.x)) ? ForestPlatformingLevelAcorn.Direction.Right : ForestPlatformingLevelAcorn.Direction.Left);
		acornPrefab.Spawn(this, spawnRoot.transform.position, direction, true);
	}

	protected override void Die()
	{
		if (!isDying)
		{
			StartCoroutine(dying_cr());
			isDying = true;
			explosion.Create(gruntRoot.transform.position);
			gruntSprite.enabled = false;
		}
	}

	private IEnumerator dying_cr()
	{
		if (killAcorns != null)
		{
			killAcorns();
		}
		base.animator.SetTrigger("Death");
		GetComponent<Collider2D>().enabled = false;
		yield return base.animator.WaitForAnimationToEnd(this, "Death");
		_003CDie_003E__BaseCallProxy0();
		yield return null;
	}

	private void PlayGruntSFX()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
		{
			AudioManager.Play("level_acorn_maker_grunt");
			emitAudioFromObject.Add("level_acorn_maker_grunt");
		}
	}

	private void PlayIdleSFX()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
		{
			AudioManager.Play("level_acorn_maker_idle");
			emitAudioFromObject.Add("level_acorn_maker_idle");
		}
	}

	private void PlayDeathSFX()
	{
		AudioManager.Play("level_acorn_maker_death");
		emitAudioFromObject.Add("level_acorn_maker_death");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
