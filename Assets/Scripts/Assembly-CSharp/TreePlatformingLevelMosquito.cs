using System.Collections;
using UnityEngine;

public class TreePlatformingLevelMosquito : AbstractCollidableObject
{
	public enum Type
	{
		AA = 0,
		AB = 1,
		AC = 2,
		BA = 3,
		BB = 4,
		BC = 5,
		CA = 6,
		CB = 7,
		CC = 8
	}

	public enum State
	{
		Up = 0,
		Down = 1,
		PlayerOn = 2
	}

	[Header("Projectile Variables")]
	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private float projectileSpeed;

	[SerializeField]
	private bool projectileShootsUP;

	[SerializeField]
	private float projectileShootUpTime;

	[Space(10f)]
	[SerializeField]
	private LevelPlatform platform;

	[SerializeField]
	private float reappearDelay = 1f;

	[SerializeField]
	private PlatformingLevelGenericExplosion explosion;

	public float returnTime = 1.5f;

	private bool projectileShooting;

	public Type type;

	public float YPositionUp;

	public const float TIME = 1.5f;

	public const float FALL_TIME = 0.13f;

	public const float FALL_BOUNCE_TIME = 0.12f;

	public const float DELAY = 0f;

	public const EaseUtils.EaseType FLOAT_EASE = EaseUtils.EaseType.easeInOutSine;

	public const EaseUtils.EaseType FALL_EASE = EaseUtils.EaseType.easeOutSine;

	public const EaseUtils.EaseType FALL_BOUNCE_EASE = EaseUtils.EaseType.easeInOutSine;

	[SerializeField]
	private State state;

	private Vector3 startPos;

	private Vector3 endPos;

	private float YPositionDown;

	private float YFall;

	private Coroutine upCoroutine;

	private Coroutine downCoroutine;

	private Coroutine fallCoroutine;

	private Coroutine gotoCoroutine;

	public bool isActive { get; private set; }

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position;
		AudioManager.PlayLoop("level_platform_mosquito_loop");
		emitAudioFromObject.Add("level_platform_mosquito_loop");
		YPositionDown = YPositionUp - 30f;
		YFall = YPositionUp - 35f;
		endPos = base.transform.position;
		endPos.y = YPositionDown;
		StartCoroutine(delay_start_cr(Random.Range(0f, 3f)));
	}

	private IEnumerator delay_start_cr(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(activate_cr());
	}

	private void SetLetters(int one, int two)
	{
		base.animator.SetInteger("FirstLetter", one);
		base.animator.SetInteger("SecondLetter", two);
	}

	private IEnumerator check_platform_cr()
	{
		while (true)
		{
			if (platform.transform.childCount <= 0)
			{
				yield return null;
				continue;
			}
			StopMoveCoroutines();
			StartCoroutine(fall_cr());
			base.animator.SetBool("Struggling", true);
			AudioManager.Play("level_platform_mosquito_step_on");
			emitAudioFromObject.Add("level_platform_mosquito_step_on");
			AudioManager.Stop("level_platform_mosquito_loop");
			AudioManager.PlayLoop("level_platform_mosquito_struggle_loop");
			emitAudioFromObject.Add("level_platform_mosquito_struggle_loop");
			if (!projectileShooting)
			{
				StartCoroutine(shoot_up_cr());
			}
			while (platform.transform.childCount > 0)
			{
				yield return null;
			}
			StopMoveCoroutines();
			StartUp();
			base.animator.SetBool("Struggling", false);
			AudioManager.Stop("level_platform_mosquito_struggle_loop");
			AudioManager.PlayLoop("level_platform_mosquito_loop");
			emitAudioFromObject.Add("level_platform_mosquito_loop");
			yield return null;
		}
	}

	protected override void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemyProjectile(hit, phase);
		if ((bool)hit.GetComponent<TreePlatformingLevelDragonflyProjectile>())
		{
			KillPlatform();
		}
	}

	private IEnumerator activate_cr()
	{
		base.animator.Play("Pick_Type");
		base.transform.position = new Vector3(startPos.x, 1200f);
		float t = 0f;
		GetComponent<Collider2D>().enabled = true;
		platform.gameObject.SetActive(true);
		while (t < returnTime)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / returnTime);
			base.transform.position = Vector2.Lerp(base.transform.position, startPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		isActive = true;
		base.transform.position = startPos;
		StartDown();
		StartCoroutine(check_platform_cr());
		switch (type)
		{
		case Type.AA:
			SetLetters(1, 1);
			break;
		case Type.AB:
			SetLetters(1, 2);
			break;
		case Type.AC:
			SetLetters(1, 3);
			break;
		case Type.BA:
			SetLetters(2, 1);
			break;
		case Type.BB:
			SetLetters(2, 2);
			break;
		case Type.BC:
			SetLetters(2, 3);
			break;
		case Type.CA:
			SetLetters(3, 1);
			break;
		case Type.CB:
			SetLetters(3, 2);
			break;
		case Type.CC:
			SetLetters(3, 3);
			break;
		}
		yield return null;
	}

	public void KillPlatform()
	{
		if (explosion != null)
		{
			explosion.Create(base.transform.position, new Vector3(0.85f, 0.85f, 0.85f));
		}
		platform.transform.DetachChildren();
		platform.gameObject.SetActive(false);
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetBool("Struggling", false);
		AudioManager.Stop("level_platform_mosquito_loop");
		AudioManager.Stop("level_platform_mosquito_struggle_loop");
		AudioManager.Play("level_platform_mosquito_death");
		emitAudioFromObject.Add("level_platform_mosquito_death");
		isActive = false;
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private IEnumerator die_cr()
	{
		base.animator.SetTrigger("Death");
		float velocity = 0f;
		float gravity = 2250f;
		while (base.transform.position.y > 0f - CupheadLevelCamera.Current.Height - 200f)
		{
			base.transform.AddPosition(0f, velocity * (float)CupheadTime.Delta);
			velocity -= gravity * (float)CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, reappearDelay);
		projectileShooting = false;
		StartCoroutine(activate_cr());
		yield return null;
	}

	public IEnumerator sine_cr()
	{
		float time = Random.Range(1f, 1.5f);
		float t = Random.Range(0f, 0.5f);
		float val = 0.5f;
		while (true)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				t += (float)CupheadTime.Delta;
				float num = Mathf.Sin(t / time);
				base.transform.AddPosition(0f, num * val);
			}
			yield return null;
		}
	}

	private IEnumerator shoot_up_cr()
	{
		projectileShooting = true;
		yield return CupheadTime.WaitForSeconds(this, projectileShootUpTime);
		if (projectile != null)
		{
			projectile.Create(new Vector2(base.transform.position.x, base.transform.position.y - 500f), 90f, projectileSpeed);
		}
		projectileShooting = false;
		yield return null;
	}

	private void StopMoveCoroutines()
	{
		if (upCoroutine != null)
		{
			StopCoroutine(upCoroutine);
			upCoroutine = null;
		}
		if (downCoroutine != null)
		{
			StopCoroutine(downCoroutine);
			downCoroutine = null;
		}
		if (fallCoroutine != null)
		{
			StopCoroutine(fallCoroutine);
			fallCoroutine = null;
		}
		if (gotoCoroutine != null)
		{
			StopCoroutine(gotoCoroutine);
			gotoCoroutine = null;
		}
	}

	public void StartDown()
	{
		StopMoveCoroutines();
		downCoroutine = StartCoroutine(down_cr());
	}

	public void StartUp()
	{
		StopMoveCoroutines();
		upCoroutine = StartCoroutine(up_cr());
	}

	private IEnumerator down_cr()
	{
		yield return new WaitForSeconds(0f);
		gotoCoroutine = StartCoroutine(goTo_cr(YPositionUp, YPositionDown, 1.5f, EaseUtils.EaseType.easeInOutSine));
		yield return gotoCoroutine;
		StartUp();
	}

	private IEnumerator up_cr()
	{
		yield return new WaitForSeconds(0f);
		gotoCoroutine = StartCoroutine(goTo_cr(YPositionDown, YPositionUp, 1.5f, EaseUtils.EaseType.easeInOutSine));
		yield return gotoCoroutine;
		StartDown();
	}

	private IEnumerator fall_cr()
	{
		float a = base.transform.position.y;
		float b = startPos.y;
		float c = YPositionDown;
		float d = YFall;
		gotoCoroutine = StartCoroutine(goTo_cr(time: (1f - (base.transform.position.y - startPos.y) / YFall) * 0.13f, start: base.transform.position.y - startPos.y, end: YFall, ease: EaseUtils.EaseType.easeOutSine));
		yield return gotoCoroutine;
		gotoCoroutine = StartCoroutine(goTo_cr(YFall, YPositionDown, 0.12f, EaseUtils.EaseType.easeInOutSine));
		yield return gotoCoroutine;
	}

	private IEnumerator goTo_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		float a = base.transform.position.y;
		float b = startPos.y;
		float c = YPositionDown;
		float d = YFall;
		float t = 0f;
		base.transform.SetPosition(null, startPos.y + start);
		while (t < time)
		{
			float val = t / time;
			base.transform.SetPosition(null, startPos.y + EaseUtils.Ease(ease, start, end, val));
			t += Time.deltaTime;
			yield return StartCoroutine(WaitForPause_CR());
		}
		base.transform.SetPosition(null, startPos.y + end);
	}
}
