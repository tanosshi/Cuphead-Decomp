using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MountainPlatformingLevelMudman : PlatformingLevelGroundMovementEnemy
{
	[SerializeField]
	private PlatformingLevelGenericExplosion splash;

	[SerializeField]
	private Transform[] explodeSpawns;

	[SerializeField]
	private PlatformingLevelGenericExplosion otherExplosion;

	[SerializeField]
	private bool isBig;

	private bool melting = true;

	protected override void Start()
	{
		base.Start();
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(come_up_cr());
		StartCoroutine(check_cr());
	}

	protected override void FixedUpdate()
	{
		if (!melting)
		{
			base.FixedUpdate();
		}
	}

	public void Init(Vector3 pos, Direction direction)
	{
		base.transform.position = pos;
		_direction = direction;
		base.transform.SetScale((direction != Direction.Right) ? 1 : (-1));
	}

	private IEnumerator come_up_cr()
	{
		base.animator.SetTrigger("Intro");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		GetComponent<Collider2D>().enabled = true;
		melting = false;
		yield return null;
	}

	private IEnumerator check_cr()
	{
		while (MountainPlatformingLevelElevatorHandler.elevatorIsMoving)
		{
			yield return null;
		}
		base.animator.SetTrigger("Outro");
		yield return base.animator.WaitForAnimationToStart(this, "Outro");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
	}

	protected override void CalculateDirection()
	{
	}

	protected override Coroutine Turn()
	{
		StopAllCoroutines();
		return StartCoroutine(despawn_cr());
	}

	private IEnumerator despawn_cr()
	{
		melting = true;
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("Outro");
		yield return null;
	}

	private IEnumerator explode_cr()
	{
		for (int i = 0; i < explodeSpawns.Length; i++)
		{
			splash.Create(explodeSpawns[i].position);
		}
		yield return null;
	}

	protected override void Die()
	{
		FrameDelayedCallback(delegate
		{
			otherExplosion.Create(GetComponent<Collider2D>().bounds.center);
			_003CDie_003E__BaseCallProxy0();
		}, 1);
		StartCoroutine(explode_cr());
		if (isBig)
		{
			MudmanBigDeathSFX();
		}
		else
		{
			MudmanSmallDeathSFX();
		}
	}

	private void Delete()
	{
		Object.Destroy(base.gameObject);
	}

	private void MudmanBigSpawnSFX()
	{
		AudioManager.Play("castle_mudman_small_spawn");
		emitAudioFromObject.Add("castle_mudman_small_spawn");
	}

	private void MudmanBigDeathSFX()
	{
		AudioManager.Play("castle_mudman_large_death");
		emitAudioFromObject.Add("castle_mudman_large_death");
	}

	private void MudmanSmallSpawnSFX()
	{
		AudioManager.Play("castle_mudman_large_spawn");
		emitAudioFromObject.Add("castle_mudman_large_spawn");
	}

	private void MudmanSmallDeathSFX()
	{
		AudioManager.Play("castle_mudman_small_death");
		emitAudioFromObject.Add("castle_mudman_small_death");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
