using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelJackinBox : PlatformingLevelShootingEnemy
{
	[SerializeField]
	private FunhousePlatformingLevelJackinBoxProjectile projectile;

	[SerializeField]
	private GameObject jack;

	[SerializeField]
	private Transform jackRoot;

	[SerializeField]
	private Transform topRoot;

	[SerializeField]
	private Transform bottomRoot;

	[SerializeField]
	private Transform rightRoot;

	[SerializeField]
	private Transform leftRoot;

	[SerializeField]
	private Transform top;

	[SerializeField]
	private Transform bottom;

	[SerializeField]
	private Transform right;

	[SerializeField]
	private Transform left;

	private int directionIndex;

	private bool justDied;

	private bool shootTime;

	private float offset = 50f;

	private AbstractPlayerController player;

	protected override void Start()
	{
		base.Start();
		directionIndex = Random.Range(0, base.Properties.jackinDirectionString.Split(',').Length);
		jack.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		AudioManager.PlayLoop("funhouse_jackbox_eye_spin_loop");
		emitAudioFromObject.Add("funhouse_jackbox_eye_spin_loop");
		StartCoroutine(check_to_start_cr());
	}

	protected override void OnStart()
	{
		base.OnStart();
		StartCoroutine(pop_up_cr());
	}

	private IEnumerator check_to_start_cr()
	{
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset)
		{
			yield return null;
		}
		OnStart();
		yield return null;
	}

	protected override void Shoot()
	{
		if (shootTime)
		{
			base.Shoot();
		}
	}

	private IEnumerator pop_up_cr()
	{
		string dir = string.Empty;
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			justDied = false;
			if (base.Properties.jackinDirectionString.Split(',')[directionIndex][0] == 'U')
			{
				base.animator.SetInteger("Direction", 1);
				jack.transform.position = topRoot.transform.position;
				jack.transform.SetEulerAngles(null, null, 270f);
				jack.GetComponent<SpriteRenderer>().sortingOrder = -5;
				dir = "Up";
			}
			else if (base.Properties.jackinDirectionString.Split(',')[directionIndex][0] == 'L')
			{
				base.animator.SetInteger("Direction", 2);
				jack.transform.position = leftRoot.transform.position;
				jack.transform.SetEulerAngles(null, null, 0f);
				jack.GetComponent<SpriteRenderer>().sortingOrder = 5;
				dir = "Left";
			}
			else if (base.Properties.jackinDirectionString.Split(',')[directionIndex][0] == 'D')
			{
				base.animator.SetInteger("Direction", 3);
				jack.transform.position = bottomRoot.transform.position;
				jack.transform.SetEulerAngles(null, null, 90f);
				jack.GetComponent<SpriteRenderer>().sortingOrder = -5;
				dir = "Down";
			}
			else if (base.Properties.jackinDirectionString.Split(',')[directionIndex][0] == 'R')
			{
				base.animator.SetInteger("Direction", 4);
				jack.transform.position = rightRoot.transform.position;
				jack.transform.SetEulerAngles(null, null, 180f);
				jack.GetComponent<SpriteRenderer>().sortingOrder = -5;
				dir = "Right";
			}
			base.animator.SetTrigger("OnDirection");
			yield return base.animator.WaitForAnimationToStart(this, "Eye_" + dir, 1);
			yield return CupheadTime.WaitForSeconds(this, 0.3f);
			base.animator.SetTrigger("OnHead");
			AudioManager.Stop("funhouse_jackbox_eye_spin_loop");
			yield return base.animator.WaitForAnimationToEnd(this, "Jack_Head", 3);
			if (!justDied)
			{
				shootTime = true;
				shootTime = false;
			}
			yield return CupheadTime.WaitForSeconds(this, DieTime());
			directionIndex = (directionIndex + 1) % base.Properties.jackinDirectionString.Split(',').Length;
			yield return null;
		}
	}

	private void HideSprite()
	{
		switch (base.animator.GetInteger("Direction"))
		{
		case 1:
			top.GetComponent<SpriteRenderer>().enabled = false;
			break;
		case 2:
			left.GetComponent<SpriteRenderer>().enabled = false;
			break;
		case 3:
			bottom.GetComponent<SpriteRenderer>().enabled = false;
			break;
		case 4:
			right.GetComponent<SpriteRenderer>().enabled = false;
			break;
		}
	}

	private void SlideSprite()
	{
		switch (base.animator.GetInteger("Direction"))
		{
		case 1:
			StartCoroutine(slide_in(top, Vector3.up));
			break;
		case 2:
			StartCoroutine(slide_in(left, Vector3.left));
			break;
		case 3:
			StartCoroutine(slide_in(bottom, Vector3.down));
			break;
		case 4:
			StartCoroutine(slide_in(right, Vector3.right));
			break;
		}
	}

	private void ShootProjectile()
	{
		if (!justDied)
		{
			player = PlayerManager.GetNext();
			projectile.Create(jackRoot.transform.position, base.Properties.ProjectileSpeed, base.Properties.jackinShootDelay, player, base.animator.GetInteger("Direction"));
			AudioManager.Play("funhouse_jackbox_shoot");
			emitAudioFromObject.Add("funhouse_jackbox_shoot");
		}
	}

	private IEnumerator slide_in(Transform sprite, Vector3 direction)
	{
		Vector3 startPos = sprite.transform.position + -direction * 100f;
		Vector3 endPos = sprite.transform.position;
		sprite.transform.position = startPos;
		float t = 0f;
		float time = 1f;
		sprite.GetComponent<SpriteRenderer>().enabled = true;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutBounce, 0f, 1f, t / time);
			sprite.transform.position = Vector3.Lerp(startPos, endPos, val);
			yield return null;
		}
		AudioManager.Play("funhouse_jackbox_shoot_launch");
		emitAudioFromObject.Add("funhouse_jackbox_shoot_launch");
		yield return null;
	}

	private float DieTime()
	{
		return (!justDied) ? base.Properties.jackinAppearDelay : base.Properties.jackinDeathAppearDelay;
	}

	protected override void Die()
	{
		justDied = true;
	}

	private void SoundJackInBoxHeadPop()
	{
		AudioManager.Play("funhouse_jackbox_jack_head");
		emitAudioFromObject.Add("funhouse_jackbox_jack_head");
	}
}
