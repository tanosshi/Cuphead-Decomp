using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelWall : PlatformingLevelBigEnemy
{
	[SerializeField]
	private Effect hornEffect;

	[SerializeField]
	private Effect honkEffect;

	[SerializeField]
	private bool isTongue;

	[SerializeField]
	private FunhousePlatformingLevelCar carPrefab;

	[SerializeField]
	private BasicProjectile shootProjectile;

	[SerializeField]
	private GameObject mouthBlockageTop;

	[SerializeField]
	private GameObject mouthBlockageBottom;

	[SerializeField]
	private GameObject middleBlockage;

	[SerializeField]
	private GameObject deadBlockage;

	[SerializeField]
	private Transform tongue;

	[SerializeField]
	private Transform topTransform;

	[SerializeField]
	private Transform bottomTransform;

	[SerializeField]
	private Transform topProjectileRoot;

	[SerializeField]
	private Transform bottomProjectileRoot;

	[SerializeField]
	private LevelBossDeathExploder explosion;

	private float carDelay = 0.7f;

	private Color openColor = new Color(0f, 0f, 1f, 1f);

	private Color currentColor;

	public bool IsDead
	{
		get
		{
			return isDead;
		}
	}

	protected override void OnLock()
	{
		base.OnLock();
		StartCoroutine(slide_camera_cr());
	}

	private IEnumerator slide_camera_cr()
	{
		CupheadLevelCamera.Current.SetAutoScroll(true);
		CupheadLevelCamera.Current.LockCamera(false);
		float dist = CupheadLevelCamera.Current.transform.position.x - base.transform.position.x;
		while (dist < -500f)
		{
			dist = CupheadLevelCamera.Current.transform.position.x - base.transform.position.x;
			yield return null;
		}
		CupheadLevelCamera.Current.SetAutoScroll(false);
		CupheadLevelCamera.Current.LockCamera(true);
		yield return null;
	}

	protected override void Start()
	{
		base.Start();
		LockDistance = 800f;
		currentColor = GetComponent<SpriteRenderer>().color;
		StartCoroutine(shoot_projectiles_cr(base.Properties.funWallTopDelayRange, true));
		StartCoroutine(shoot_projectiles_cr(base.Properties.funWallBottomDelayRange, false));
		if (isTongue)
		{
			StartCoroutine(spawn_tongue_cr());
		}
		else
		{
			StartCoroutine(spawn_cars_cr());
		}
	}

	protected override void Shoot()
	{
		if (!isDead)
		{
		}
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!isDead)
		{
			base.OnDamageTaken(info);
			base.animator.SetTrigger("eyeHit");
			if (!AudioManager.CheckIfPlaying("funhouse_wall1_eye_hit"))
			{
				AudioManager.Play("funhouse_wall1_eye_hit");
				emitAudioFromObject.Add("funhouse_wall1_eye_hit");
			}
		}
	}

	private IEnumerator shoot_projectiles_cr(MinMax delay, bool isTop)
	{
		while (!bigEnemyCameraLock)
		{
			yield return null;
		}
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay.RandomFloat());
			string name = ((!isTop) ? "Bottom" : "Top");
			base.animator.SetTrigger("horn" + name);
			AudioManager.Play("funhouse_wall1_horn_attack");
			emitAudioFromObject.Add("funhouse_wall1_horn_attack");
			yield return null;
		}
	}

	private void ShootProjectileTop()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = next.transform.position - topProjectileRoot.transform.position;
		hornEffect.Create(topProjectileRoot.transform.position);
		shootProjectile.Create(topProjectileRoot.transform.position, MathUtils.DirectionToAngle(vector), base.Properties.funWallProjectileSpeed);
	}

	private void ShootProjectileBottom()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = next.transform.position - bottomProjectileRoot.transform.position;
		hornEffect.Create(bottomProjectileRoot.transform.position);
		shootProjectile.Create(bottomProjectileRoot.transform.position, MathUtils.DirectionToAngle(vector), base.Properties.funWallProjectileSpeed);
	}

	private IEnumerator spawn_cars_cr()
	{
		while (!bigEnemyCameraLock)
		{
			yield return null;
		}
		int typeIndex = 0;
		bool isTop = Rand.Bool();
		Vector3 pos = ((!isTop) ? bottomTransform.position : topTransform.position);
		while (true)
		{
			GameObject blockage = ((!isTop) ? mouthBlockageBottom : mouthBlockageTop);
			base.animator.SetBool("isTop", isTop);
			yield return CupheadTime.WaitForSeconds(this, base.Properties.funWallCarDelayRange.RandomFloat());
			base.animator.SetBool("isOpen", true);
			string name = ((!isTop) ? "Bottom" : "Top");
			yield return base.animator.WaitForAnimationToStart(this, name + "_Open_Start");
			AudioManager.Play("funhouse_wall1_wall_open_start");
			emitAudioFromObject.Add("funhouse_wall1_wall_open_start");
			AudioManager.Play("funhouse_car_honk_sweet");
			SpawnHonk((!isTop) ? bottomTransform.position.y : topTransform.position.y);
			yield return base.animator.WaitForAnimationToEnd(this, name + "_Open_Start");
			blockage.SetActive(false);
			for (int i = 0; i < 2; i++)
			{
				FunhousePlatformingLevelCar car = Object.Instantiate(carPrefab);
				car.Init(pos, 180f, base.Properties.funWallCarSpeed, typeIndex, true, true);
				car.transform.SetScale(null, isTop ? car.transform.localScale.y : (0f - car.transform.localScale.y));
				typeIndex = ((typeIndex < 3) ? (typeIndex + 1) : 0);
				yield return CupheadTime.WaitForSeconds(this, carDelay);
			}
			yield return CupheadTime.WaitForSeconds(this, base.Properties.funWallMouthOpenTime);
			base.animator.SetBool("isOpen", false);
			AudioManager.Play("funhouse_wall1_wall_close");
			emitAudioFromObject.Add("funhouse_wall1_wall_close");
			blockage.SetActive(true);
			isTop = !isTop;
			pos = ((!isTop) ? bottomTransform.position : topTransform.position);
			yield return null;
		}
	}

	private void SpawnHonk(float rootY)
	{
		Vector2 vector = new Vector2(CupheadLevelCamera.Current.Bounds.xMax, rootY);
		honkEffect.Create(vector).transform.parent = CupheadLevelCamera.Current.transform;
	}

	private IEnumerator spawn_tongue_cr()
	{
		while (!bigEnemyCameraLock)
		{
			yield return null;
		}
		bool isTop = Rand.Bool();
		Vector3 pos = ((!isTop) ? bottomTransform.position : topTransform.position);
		while (true)
		{
			GameObject blockage = ((!(pos == bottomTransform.position)) ? mouthBlockageTop : mouthBlockageBottom);
			base.animator.SetBool("isTop", isTop);
			yield return CupheadTime.WaitForSeconds(this, base.Properties.funWallTongueDelayRange.RandomFloat());
			base.animator.SetBool("isOpen", true);
			string name = ((!isTop) ? "Bottom" : "Top");
			yield return CupheadTime.WaitForSeconds(this, 0.8f);
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToEnd(this, name + "_Open_Start");
			blockage.SetActive(false);
			tongue.transform.SetScale(null, (!isTop) ? 1 : (-1));
			tongue.transform.position = pos;
			tongue.GetComponent<Animator>().SetBool("IsTongue", true);
			yield return CupheadTime.WaitForSeconds(this, base.Properties.funWallTongueLoopTime);
			tongue.GetComponent<Animator>().SetBool("IsTongue", false);
			yield return tongue.GetComponent<Animator>().WaitForAnimationToEnd(this, "Outro");
			yield return CupheadTime.WaitForSeconds(this, base.Properties.funWallMouthOpenTime);
			base.animator.SetBool("isOpen", false);
			blockage.SetActive(true);
			isTop = !isTop;
			pos = ((!isTop) ? bottomTransform.position : topTransform.position);
			yield return null;
		}
	}

	protected override void OnPass()
	{
		base.OnPass();
		StopAllCoroutines();
		Die();
	}

	protected override void Die()
	{
		isDead = true;
		GetComponent<Collider2D>().enabled = false;
		deadBlockage.SetActive(true);
		if (CupheadLevelCamera.Current.autoScrolling)
		{
			CupheadLevelCamera.Current.SetAutoScroll(false);
		}
		CupheadLevelCamera.Current.LockCamera(false);
		mouthBlockageBottom.SetActive(false);
		mouthBlockageTop.SetActive(false);
		middleBlockage.SetActive(false);
		if (tongue != null)
		{
			tongue.gameObject.SetActive(false);
		}
		StopAllCoroutines();
		StartCoroutine(Explode());
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("Dead");
	}

	private new IEnumerator Explode()
	{
		explosion.StartExplosion();
		yield return CupheadTime.WaitForSeconds(this, 3f);
		explosion.StopExplosions();
	}
}
