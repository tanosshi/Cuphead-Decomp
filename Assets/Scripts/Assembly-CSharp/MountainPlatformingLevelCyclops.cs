using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelCyclops : PlatformingLevelAutoscrollObject
{
	[SerializeField]
	private float autoScrollMultiplier;

	private const float MAX_AUTOSCROLL_DIST = 1300f;

	private const float MAX_CYCLOPS_DIST = 1600f;

	private bool autoscrollMoving;

	private bool cyclopsMoving;

	private DamageDealer damageDealer;

	protected override void Start()
	{
		damageDealer = DamageDealer.NewEnemy();
		checkToLock = true;
		lockDistance = -1300f;
		IsEnabled(false);
		StartCoroutine(start_scrolling_cr());
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void IsEnabled(bool isenabled)
	{
		GetComponent<Collider2D>().enabled = isenabled;
		GetComponent<SpriteRenderer>().enabled = isenabled;
	}

	protected override void StartAutoscroll()
	{
		base.StartAutoscroll();
		CupheadLevelCamera.Current.OffsetCamera(true, false);
	}

	private IEnumerator start_scrolling_cr()
	{
		while (!isLocked)
		{
			yield return null;
		}
		cyclopsMoving = true;
		StartCoroutine(start_moving_cr());
		IsEnabled(true);
		PlatformingLevel level = (PlatformingLevel)Level.Current;
		level.useAltQuote = true;
		while (base.transform.position.x < CupheadLevelCamera.Current.transform.position.x - 650f)
		{
			yield return null;
		}
		autoscrollMoving = true;
		StartCoroutine(check_to_move_forward_cr());
		StartAutoscroll();
	}

	private IEnumerator start_moving_cr()
	{
		base.animator.Play("Run");
		while (cyclopsMoving)
		{
			base.transform.position += Vector3.right * (200f * autoScrollMultiplier) * CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator check_to_move_forward_cr()
	{
		while (autoscrollMoving)
		{
			float dist = PlayerManager.Center.x - base.transform.position.x;
			if (base.transform.position.x < CupheadLevelCamera.Current.transform.position.x - 1600f)
			{
				base.transform.position = new Vector3(CupheadLevelCamera.Current.transform.position.x - 1600f, base.transform.position.y);
			}
			if (dist > 1300f)
			{
				if (CupheadLevelCamera.Current.autoScrolling)
				{
					CupheadLevelCamera.Current.SetAutoScroll(false);
				}
			}
			else if (!CupheadLevelCamera.Current.autoScrolling)
			{
				CupheadLevelCamera.Current.SetAutoScroll(true);
				CupheadLevelCamera.Current.SetAutoscrollSpeedMultiplier(autoScrollMultiplier);
			}
			yield return null;
		}
	}

	protected override void StartEndingAutoscroll()
	{
		base.StartEndingAutoscroll();
		autoscrollMoving = false;
	}

	protected override void EndAutoscroll()
	{
		base.EndAutoscroll();
		StartCoroutine(fall_cr());
	}

	private IEnumerator fall_cr()
	{
		while (base.transform.position.x < CupheadLevelCamera.Current.transform.position.x - 1300f)
		{
			yield return null;
		}
		CupheadLevelCamera.Current.LockCamera(true);
		cyclopsMoving = false;
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Map.ToString();
		base.animator.SetTrigger("OnFall");
		yield return base.animator.WaitForAnimationToEnd(this, "Fall");
		CupheadLevelCamera.Current.LockCamera(false);
		CupheadLevelCamera.Current.OffsetCamera(false, false);
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		Object.Destroy(base.gameObject);
		yield return null;
	}

	public void CameraShake()
	{
		CupheadLevelCamera.Current.Shake(10f, 0.5f);
	}

	private void SoundCyclopsFall()
	{
		AudioManager.Play("castle_giant_rock_chase_death");
	}

	private void SoundCyclopsFootstep()
	{
		AudioManager.Play("castle_giant_rock_chase_footstep");
		emitAudioFromObject.Add("castle_giant_rock_chase_footstep");
	}
}
