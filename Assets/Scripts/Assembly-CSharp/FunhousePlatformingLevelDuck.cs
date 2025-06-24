using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelDuck : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private bool isBigDuck;

	[SerializeField]
	private bool parryable;

	[SerializeField]
	private CollisionChild child;

	public bool smallFirst;

	public bool smallLast;

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		base.Start();
		if (child != null)
		{
			child.OnAnyCollision += OnCollision;
			child.OnPlayerCollision += OnCollisionPlayer;
		}
		if (parryable)
		{
			_canParry = true;
		}
		if (!isBigDuck)
		{
			StartCoroutine(idle_sound_cr());
		}
		StartCoroutine(move_cr());
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if ((bool)hit.GetComponent<FunhousePlatformingLevelCar>())
		{
			Die();
		}
	}

	private IEnumerator idle_sound_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(5f, 15f));
			AudioManager.Play("funhouse_small_duck_idle_sweet");
			emitAudioFromObject.Add("funhouse_small_duck_idle_sweet");
			yield return null;
		}
	}

	private IEnumerator move_cr()
	{
		if (isBigDuck)
		{
			AudioManager.PlayLoop("funhouse_big_duck_idle");
			emitAudioFromObject.Add("funhouse_big_duck_idle");
		}
		else if (smallFirst)
		{
			AudioManager.PlayLoop("funhouse_small_duck_idle_loop");
			emitAudioFromObject.Add("funhouse_small_duck_idle_loop");
		}
		float size = GetComponent<Collider2D>().bounds.size.x;
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMin - size)
		{
			base.transform.position -= base.transform.right * base.Properties.MoveSpeed * CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
		DoneAnimation();
	}

	protected override void Die()
	{
		StopAllCoroutines();
		if (smallLast)
		{
			AudioManager.Stop("funhouse_small_duck_idle_loop");
		}
		if (isBigDuck)
		{
			AudioManager.Stop("funhouse_big_duck_idle");
			AudioManager.Play("funhouse_big_duck_death");
			emitAudioFromObject.Add("funhouse_big_duck_death");
			base.animator.SetTrigger("OnDeath");
		}
		else
		{
			AudioManager.Play("funhouse_small_duck_death");
			AudioManager.Play("funhouse_small_duck_death");
			base.Die();
		}
	}

	private void DoneAnimation()
	{
		if (isBigDuck)
		{
			AudioManager.Stop("funhouse_big_duck_idle");
		}
		if (smallLast)
		{
			AudioManager.Stop("funhouse_small_duck_idle_loop");
		}
		base.Die();
	}
}
