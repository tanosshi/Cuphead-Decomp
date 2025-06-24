using System.Collections;
using UnityEngine;

public class MausoleumLevelGhostBase : BasicProjectile
{
	[SerializeField]
	private MinMax idleDelay;

	[SerializeField]
	private string idleSound;

	[SerializeField]
	private bool hasIdleSFX;

	public bool Counts;

	private const float DIST_TO_DIE = 30f;

	protected MausoleumLevel parent;

	public bool isDead { get; private set; }

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public override float ParryMeterMultiplier
	{
		get
		{
			return 0f;
		}
	}

	protected override void Start()
	{
		base.Start();
		isDead = false;
		if (base.transform.position.x > 0f)
		{
			GetComponent<SpriteRenderer>().flipY = true;
		}
		SetParryable(true);
		if (Counts)
		{
			MausoleumLevel.SPAWNCOUNTER++;
		}
		StartCoroutine(check_dist_cr());
		if (!hasIdleSFX)
		{
		}
	}

	public void GetParent(MausoleumLevel parent)
	{
		this.parent = parent;
	}

	public override void OnParry(AbstractPlayerController player)
	{
		Die();
	}

	public void OnBossDeath()
	{
		Die();
	}

	protected override void Die()
	{
		StopAllCoroutines();
		isDead = true;
		base.Die();
		GetComponent<SpriteRenderer>().enabled = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private IEnumerator check_dist_cr()
	{
		while (Vector3.Distance(base.transform.position, MausoleumLevelUrn.URN_POS) > 30f)
		{
			yield return null;
		}
		if (parent.LoseGame != null)
		{
			parent.LoseGame();
		}
		yield return null;
	}

	private IEnumerator idle_sound_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, idleDelay.RandomFloat());
			AudioManager.Play(idleSound);
			emitAudioFromObject.Add(idleSound);
			while (AudioManager.CheckIfPlaying(idleSound))
			{
				yield return null;
			}
			yield return null;
		}
	}
}
