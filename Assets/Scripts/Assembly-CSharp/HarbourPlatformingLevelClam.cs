using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelClam : PlatformingLevelShootingEnemy
{
	private const float GRAVITY = -100f;

	[SerializeField]
	private SpriteDeathParts[] deathParts;

	[SerializeField]
	private Transform main;

	[SerializeField]
	private Transform onTrigger;

	[SerializeField]
	private Transform offTrigger;

	[SerializeField]
	private HarbourPlatformingLevelOctopus octopus;

	private bool startClam;

	private bool endClam;

	private bool visible;

	private bool isDead;

	private float dist = -1000f;

	private float offset = 100f;

	private int counter;

	private Vector3 startPos;

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position;
	}

	protected override void StartShoot()
	{
		if (counter < base.Properties.ClamShotCount)
		{
			counter++;
			base.StartShoot();
			AttackSFX();
		}
	}

	protected override void Update()
	{
		if (!startClam && octopus != null && octopus.Started())
		{
			Popup();
			startClam = true;
		}
		if (!(_target != null))
		{
			return;
		}
		if (!startClam && octopus == null)
		{
			dist = _target.transform.position.x - onTrigger.transform.position.x;
			if (dist > 0f && !startClam)
			{
				Popup();
				startClam = true;
			}
		}
		else
		{
			dist = _target.transform.position.x - offTrigger.transform.position.x;
			if (dist > 0f && !endClam)
			{
				startClam = false;
				endClam = true;
			}
		}
	}

	private void Popup()
	{
		base.animator.SetTrigger("OnPopup");
		base.transform.parent = CupheadLevelCamera.Current.transform;
		StartCoroutine(pop_up_cr());
	}

	private IEnumerator pop_up_cr()
	{
		while (true)
		{
			if (!isDead)
			{
				EaseUtils.EaseType ease = EaseUtils.EaseType.easeInOutSine;
				float t = 0f;
				float time = base.Properties.ClamTimeSpeedUp;
				float startY = startPos.y;
				float endY = base.Properties.ClamMaxPointRange.RandomFloat();
				base.transform.SetPosition(CupheadLevelCamera.Current.Bounds.xMin + offset);
				Show();
				while (t < time)
				{
					TransformExtensions.SetPosition(y: EaseUtils.Ease(ease, startY, endY, t / time), transform: base.transform);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				base.animator.SetTrigger("OnSlowdown");
				base.transform.SetPosition(null, endY);
				t = 0f;
				time = base.Properties.ClamTimeSpeedDown;
				yield return null;
				while (t < time)
				{
					TransformExtensions.SetPosition(y: EaseUtils.Ease(ease, endY, startY, t / time), transform: base.transform);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				Hide();
				base.transform.SetPosition(null, startY);
			}
			yield return CupheadTime.WaitForSeconds(this, base.Properties.ProjectileDelay.RandomFloat());
			if (!endClam)
			{
				isDead = false;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 0f, 1f, 1f);
		Gizmos.DrawLine(offTrigger.transform.position, new Vector3(offTrigger.transform.position.x, 5000f, 0f));
		Gizmos.DrawLine(onTrigger.transform.position, new Vector3(onTrigger.transform.position.x, 5000f, 0f));
	}

	protected override void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		GetComponent<DamageReceiver>().enabled = false;
		base.animator.Play("Off");
		DeathParts();
		isDead = true;
		StopAllCoroutines();
		StartCoroutine(fall_cr());
		Explode();
	}

	private IEnumerator fall_cr()
	{
		Vector2 velocity = new Vector2(0f, 200f);
		float accumulatedGravity = 0f;
		float speed = 400f;
		while (base.transform.position.y > CupheadLevelCamera.Current.Bounds.yMin - 100f)
		{
			base.transform.position += (Vector3)(velocity + new Vector2(0f - speed, accumulatedGravity)) * Time.fixedDeltaTime;
			accumulatedGravity += -100f;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, base.Properties.ClamDespawnDelayRange.RandomFloat());
		base.transform.SetPosition(CupheadLevelCamera.Current.Bounds.xMin + offset, startPos.y);
		Hide();
		yield return null;
	}

	private void Hide()
	{
		counter = 0;
		base.animator.Play("Off");
		GetComponent<Collider2D>().enabled = false;
		GetComponent<DamageReceiver>().enabled = false;
		visible = false;
		OnStart();
		if (isDead)
		{
			Popup();
		}
	}

	private void Show()
	{
		base.animator.SetTrigger("OnPopup");
		GetComponent<Collider2D>().enabled = true;
		GetComponent<DamageReceiver>().enabled = true;
		visible = true;
	}

	private void DeathParts()
	{
		SpriteDeathParts[] array = deathParts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}

	private void AttackSFX()
	{
		AudioManager.Play("harbour_clam_attack");
		emitAudioFromObject.Add("harbour_clam_attack");
	}
}
