using System.Collections;
using UnityEngine;

public class FlyingBirdLevelBird : LevelProperties.FlyingBird.Entity
{
	public enum State
	{
		Init = 0,
		Idle = 1,
		Feathers = 2,
		Eggs = 3,
		Dying = 4,
		Dead = 5,
		Whistle = 6,
		Lasers = 7,
		LasersEnding = 8,
		Reviving = 9,
		Revived = 10,
		Garbage = 11,
		Heart = 12,
		HeartTrans = 13
	}

	[SerializeField]
	private GameObject bootPrefab;

	[SerializeField]
	private GameObject bootPinkPrefab;

	[SerializeField]
	private GameObject fishPrefab;

	[SerializeField]
	private GameObject applePrefab;

	[Space(10f)]
	[SerializeField]
	private FlyingBirdLevelSmallBird smallBird;

	[Space(10f)]
	[SerializeField]
	private FlyingBirdLevelBirdFeather featherPrefab;

	[Space(10f)]
	[SerializeField]
	private Transform eggRoot;

	[SerializeField]
	private FlyingBirdLevelBirdEgg eggPrefab;

	[SerializeField]
	private GameObject deathParts;

	[Space(10f)]
	[SerializeField]
	private Transform nurse1Root;

	[SerializeField]
	private Transform nurse2Root;

	[SerializeField]
	private Transform garbageRoot;

	[SerializeField]
	private FlyingBirdLevelHeart heart;

	[SerializeField]
	private GameObject heartSpitFX;

	[SerializeField]
	private FlyingBirdLevelNurses nurses;

	[SerializeField]
	private GameObject head;

	private new Animator animator;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool introEnded;

	private bool feathersFirstTime;

	private Transform houseCollider;

	private Coroutine patternCoroutine;

	private int garbageIndex;

	private int typeIndex;

	private int blinks;

	private int maxBlinks = 6;

	[Space(10f)]
	[SerializeField]
	private Transform[] laserRoots;

	[SerializeField]
	private BasicProjectile laserPrefab;

	[SerializeField]
	private Effect laserEffect;

	[Space(10f)]
	[SerializeField]
	private Transform deathEffectsRoot;

	[SerializeField]
	private Effect deathEffectFront;

	[SerializeField]
	private Effect deathEffectBack;

	public State state { get; private set; }

	public bool floating { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		state = State.Init;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		heart.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		heart.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		houseCollider = new GameObject("House Collider").transform;
		BoxCollider2D boxCollider2D = houseCollider.gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = true;
		boxCollider2D.offset = new Vector2(60f, -50f);
		boxCollider2D.size = new Vector2(400f, 300f);
		CollisionChild collisionChild = houseCollider.gameObject.AddComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
	}

	protected override void Start()
	{
		base.Start();
		featherPrefab.CreatePool(150);
		heart.gameObject.SetActive(false);
		feathersFirstTime = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (damageReceiver != null)
		{
			damageReceiver.OnDamageTaken -= OnDamageTaken;
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state != State.Dead && state != State.Dying)
		{
			base.properties.DealDamage(info.damage);
			if (base.properties.CurrentHealth <= 0f)
			{
				BirdDie();
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (houseCollider != null)
		{
			houseCollider.position = base.transform.position;
		}
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public override void LevelInit(LevelProperties.FlyingBird properties)
	{
		base.LevelInit(properties);
		properties.OnStateChange += OnStateChange;
		floating = false;
		StartCoroutine(float_cr());
		garbageIndex = Random.Range(0, properties.CurrentState.garbage.garbageTypeString.Length);
	}

	private void OnStateChange()
	{
		if (base.properties.CurrentState.stateName == LevelProperties.FlyingBird.States.Whistle)
		{
			StartCoroutine(whistle_cr());
			if (patternCoroutine != null)
			{
				StopCoroutine(patternCoroutine);
			}
			patternCoroutine = StartCoroutine(whistle_cr());
		}
	}

	public void IntroContinue()
	{
		Animator component = GetComponent<Animator>();
		component.SetTrigger("Continue");
		StartCoroutine(intro_cr());
	}

	private void SfxIntroA()
	{
		AudioManager.Play("level_flying_bird_intro_a");
	}

	private void SfxIntroB()
	{
		AudioManager.Play("level_flying_bird_intro_b");
	}

	private void OnIntroAnimComplete()
	{
		introEnded = true;
	}

	private IEnumerator intro_cr()
	{
		while (!introEnded)
		{
			yield return null;
		}
		floating = true;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.floating.attackInitialDelayRange.RandomFloat());
		state = State.Idle;
	}

	private void IdleNoBlink()
	{
		blinks++;
		if (blinks >= maxBlinks)
		{
			animator.SetBool("Blink", true);
		}
	}

	private void Blink()
	{
		blinks = 0;
		maxBlinks = Random.Range(2, 5);
		animator.SetBool("Blink", false);
	}

	private IEnumerator whistle_cr()
	{
		state = State.Whistle;
		floating = false;
		Animator animator = GetComponent<Animator>();
		animator.Play("Whistle");
		AudioManager.Play("level_flying_bird_whistle");
		yield return animator.WaitForAnimationToEnd(this, "Whistle");
		state = State.Idle;
		floating = true;
	}

	private IEnumerator float_cr()
	{
		bool goUp = Rand.Bool();
		while (true)
		{
			if (goUp)
			{
				yield return StartCoroutine(floatTo_cr(base.properties.CurrentState.floating.top, base.properties.CurrentState.floating.time));
			}
			else
			{
				yield return StartCoroutine(floatTo_cr(base.properties.CurrentState.floating.bottom, base.properties.CurrentState.floating.time));
			}
			goUp = !goUp;
		}
	}

	private IEnumerator floatTo_cr(float end, float time)
	{
		float t = 0f;
		float start = base.transform.position.y;
		while (t < time)
		{
			if (!floating)
			{
				while (!floating)
				{
					yield return null;
				}
			}
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(null, end);
	}

	public void StartFeathers()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(feathers_cr());
	}

	private void FireFeathers(int count, float offset, bool parryable)
	{
		parryable = false;
		for (int i = 0; i < count; i++)
		{
			float num = 360f * ((float)i / (float)count);
			featherPrefab.Spawn(base.transform.position, Quaternion.Euler(new Vector3(0f, 0f, offset + num - 180f))).Init(base.properties.CurrentState.feathers.speed).SetParryable(parryable);
		}
	}

	private IEnumerator feathers_cr()
	{
		state = State.Feathers;
		floating = false;
		Animator animator = GetComponent<Animator>();
		animator.Play("Feathers_Start");
		AudioManager.Play("level_flyingbird_feathers_start");
		emitAudioFromObject.Add("level_flyingbird_feathers_start");
		yield return animator.WaitForAnimationToEnd(this, "Feathers_Start");
		LevelProperties.FlyingBird.Feathers featherProperties = base.properties.CurrentState.feathers;
		KeyValue[] pattern = KeyValue.ListFromString(featherProperties.pattern[Random.Range(0, featherProperties.pattern.Length)], new char[2] { 'P', 'D' });
		AudioManager.PlayLoop("level_flyingbird_feathers_loop");
		emitAudioFromObject.Add("level_flyingbird_feathers_loop");
		for (int i = 0; i < pattern.Length; i++)
		{
			float offset = 0f;
			bool parryable = false;
			if (pattern[i].key == "P")
			{
				for (int p = 0; (float)p < pattern[i].value; p++)
				{
					FireFeathers(featherProperties.count, offset, parryable);
					parryable = !parryable;
					offset += featherProperties.offset;
					yield return CupheadTime.WaitForSeconds(this, feathersFirstTime ? featherProperties.initalShotDelay : featherProperties.shotDelay);
					feathersFirstTime = false;
				}
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, pattern[i].value);
			}
			yield return null;
		}
		AudioManager.Stop("level_flyingbird_feathers_loop");
		floating = true;
		animator.Play("Feathers_End");
		AudioManager.Play("level_flyingbird_feathers_hesitate");
		emitAudioFromObject.Add("level_flyingbird_feathers_hesitate");
		yield return CupheadTime.WaitForSeconds(this, featherProperties.hesitate);
		animator.Play("Feathers_Hesitate_End");
		AudioManager.Stop("level_flyingbird_feathers_hesitate");
		yield return animator.WaitForAnimationToEnd(this, "Feathers_Hesitate_End");
		state = State.Idle;
	}

	public void StartEggs()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(eggs_cr());
	}

	private void FireEgg()
	{
		eggPrefab.Create(base.properties.CurrentState.feathers.speed, eggRoot.position);
	}

	private void SoundFireEggThroaty()
	{
		AudioManager.Play("level_flying_bird_spit_throaty");
		emitAudioFromObject.Add("level_flying_bird_spit_throaty");
	}

	private void SoundFireEggProjectile()
	{
		AudioManager.Play("level_flying_bird_spit");
		emitAudioFromObject.Add("level_flying_bird_spit");
	}

	private IEnumerator eggs_cr()
	{
		floating = true;
		state = State.Eggs;
		Animator animator = GetComponent<Animator>();
		LevelProperties.FlyingBird.Eggs eggProperties = base.properties.CurrentState.eggs;
		KeyValue[] pattern = KeyValue.ListFromString(eggProperties.pattern[Random.Range(0, eggProperties.pattern.Length)], new char[2] { 'P', 'D' });
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i].key == "P")
			{
				for (int p = 0; (float)p < pattern[i].value; p++)
				{
					yield return CupheadTime.WaitForSeconds(this, eggProperties.shotDelay);
					animator.Play("Spit");
				}
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, pattern[i].value);
			}
			yield return null;
		}
		yield return animator.WaitForAnimationToEnd(this, "Spit");
		yield return CupheadTime.WaitForSeconds(this, eggProperties.hesitate);
		state = State.Idle;
	}

	public void StartLasers()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(lasers_cr());
	}

	private void FireLasers()
	{
		AudioManager.Play("level_flyingbird_bird_laser_fire");
		emitAudioFromObject.Add("level_flyingbird_bird_laser_fire");
		laserEffect.Create(laserRoots[0].position);
		Transform[] array = laserRoots;
		foreach (Transform transform in array)
		{
			laserPrefab.Create(transform.position, 0f - transform.eulerAngles.z, 0f - base.properties.CurrentState.lasers.speed);
		}
	}

	private void LasersAnimEnded()
	{
		state = State.LasersEnding;
	}

	private IEnumerator lasers_cr()
	{
		Animator animator = GetComponent<Animator>();
		state = State.Lasers;
		floating = false;
		LevelProperties.FlyingBird.Lasers properties = base.properties.CurrentState.lasers;
		animator.SetTrigger("StartLasers");
		while (state == State.Lasers)
		{
			yield return null;
		}
		floating = true;
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	private void LasersSFX()
	{
		AudioManager.Play("level_flyingbird_bird_lasers");
		emitAudioFromObject.Add("level_flyingbird_bird_lasers");
	}

	public void BirdFall()
	{
		state = State.Dying;
		AudioManager.Stop("level_flyingbird_feathers_loop");
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		StopAllCoroutines();
		animator.Play("Death");
		StartCoroutine(die_cr());
		nurses.Die();
	}

	public void BirdDie()
	{
		nurses.Die();
		GetComponent<Collider2D>().enabled = false;
		StopCoroutine(checkHeart_cr());
		StopCoroutine(garbage_cr());
		nurses.animator.SetTrigger("Die");
		animator.Play("Stretcher_Death");
		AudioManager.PlayLoop("level_flyingbird_stretcher_death");
		emitAudioFromObject.Add("level_flyingbird_stretcher_death");
	}

	private void OnDeathComplete()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(false);
	}

	private void DeathSfx()
	{
	}

	private void OnDeathExploded()
	{
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		houseCollider.gameObject.SetActive(false);
		BoxCollider2D[] componentsInChildren = GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D boxCollider2D in componentsInChildren)
		{
			boxCollider2D.enabled = false;
		}
		CircleCollider2D[] componentsInChildren2 = GetComponentsInChildren<CircleCollider2D>();
		foreach (CircleCollider2D circleCollider2D in componentsInChildren2)
		{
			circleCollider2D.enabled = false;
		}
		smallBird.StartPattern(base.transform.position);
	}

	private IEnumerator die_cr()
	{
		Animator animator = GetComponent<Animator>();
		while (base.transform.position.y > 100f || base.transform.position.y < 0f)
		{
			yield return null;
		}
		floating = false;
		animator.Play("Death");
		deathEffectFront.Create(deathEffectsRoot.position);
		deathEffectBack.Create(deathEffectsRoot.position);
	}

	public void OnBossRevival()
	{
		state = State.Reviving;
		Object.Destroy(deathParts);
		base.gameObject.SetActive(true);
		animator.Play("Revived");
		nurses.animator.SetTrigger("StartNurses");
		GetComponent<HitFlash>().StopAllCoroutines();
		GetComponent<HitFlash>().SetColor(0f);
		base.transform.SetPosition((float)Level.Current.Right + 250f, Level.Current.Ground - 150);
		StartCoroutine(revival_cr());
		heart.InitHeart(base.properties);
	}

	private IEnumerator revival_cr()
	{
		yield return StartCoroutine(move_to_position_cr(end: (float)Level.Current.Ground + 250f, start: base.transform.position.y, time: base.properties.CurrentState.floating.time, ease: EaseUtils.EaseType.easeInOutSine));
		state = State.Revived;
		nurses.InitNurse(base.properties.CurrentState.nurses, this);
	}

	private IEnumerator move_to_position_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		base.transform.SetPosition(null, start);
		float startX = base.transform.position.x;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			base.transform.SetPosition(EaseUtils.Ease(ease, startX, 0f, val), EaseUtils.Ease(ease, start, end, val));
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(0f, end);
		yield return null;
		StartCoroutine(stretcherMove_cr());
	}

	private IEnumerator stretcherMove_cr()
	{
		bool movingRight = Rand.Bool();
		float time = base.properties.CurrentState.bigBird.speedXTime;
		float end = 0f;
		float pivot = base.transform.position.x;
		do
		{
			if (state != State.Heart)
			{
				float t = 0f;
				float start = base.transform.position.x;
				end = ((!movingRight) ? (-240f) : 290f);
				while (t < time)
				{
					if (state != State.Heart)
					{
						float value = t / time;
						base.transform.SetPosition(EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, value));
						t += (float)CupheadTime.Delta;
					}
					yield return null;
				}
				base.transform.SetPosition(end);
				movingRight = !movingRight;
			}
			yield return null;
		}
		while (!(base.properties.CurrentHealth <= 0f));
	}

	public void StartGarbageOne()
	{
		StartCoroutine(garbage_cr());
	}

	private IEnumerator garbage_cr()
	{
		state = State.Garbage;
		animator.SetBool("OnGarbage", true);
		yield return animator.WaitForAnimationToStart(this, "Garbage_Start");
		AudioManager.Play("level_flyingbird_stretcher_garbage_start");
		emitAudioFromObject.Add("level_flyingbird_stretcher_garbage_start");
		float garbageSpeed = base.properties.CurrentState.garbage.speedX;
		float garbageCounter = 0f;
		GameObject chosenPrefab = null;
		yield return animator.WaitForAnimationToEnd(this, "Garbage_Start");
		int maxShotIndex = Random.Range(0, base.properties.CurrentState.garbage.shotCount.Split(',').Length);
		int maxShot = int.Parse(base.properties.CurrentState.garbage.shotCount.Split(',')[maxShotIndex]);
		while (garbageCounter < (float)maxShot)
		{
			string[] garbageTypes = base.properties.CurrentState.garbage.garbageTypeString[garbageIndex].Split(',');
			if (garbageTypes[typeIndex][0] == 'P')
			{
				chosenPrefab = bootPinkPrefab;
			}
			else if (garbageTypes[typeIndex][0] == 'B')
			{
				chosenPrefab = bootPrefab;
			}
			else if (garbageTypes[typeIndex][0] == 'F')
			{
				chosenPrefab = fishPrefab;
			}
			else if (garbageTypes[typeIndex][0] == 'A')
			{
				chosenPrefab = applePrefab;
			}
			else
			{
				Debug.LogError("Invalid garbage type string.");
			}
			yield return animator.WaitForAnimationToEnd(this, "Garbage");
			AudioManager.Play("level_flyingbird_stretcher_garbage");
			emitAudioFromObject.Add("level_flyingbird_stretcher_garbage");
			GameObject garbage = Object.Instantiate(chosenPrefab, garbageRoot.transform.position, Quaternion.identity);
			garbage.GetComponent<BasicProjectile>().Speed = 1f;
			garbage.transform.localScale = Vector3.one * base.properties.CurrentState.garbage.shotSize;
			StartCoroutine(multiShotGarbage_cr(garbageSpeed, garbage));
			garbageSpeed += base.properties.CurrentState.garbage.speedXIncreaser;
			garbageCounter += 1f;
			if (typeIndex < garbageTypes.Length - 1)
			{
				typeIndex++;
			}
			else
			{
				garbageIndex = (garbageIndex + 1) % base.properties.CurrentState.garbage.garbageTypeString.Length;
				typeIndex = 0;
			}
			if (garbageCounter < (float)maxShot)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.garbage.shotDelay);
			}
		}
		garbageCounter = 0f;
		animator.SetTrigger("Continue");
		animator.SetBool("OnGarbage", false);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.garbage.hesitate.RandomFloat());
		state = State.Revived;
	}

	private IEnumerator multiShotGarbage_cr(float speedX, GameObject proj)
	{
		bool isFalling = false;
		float pct = 1f;
		Vector3 velocity = new Vector3(0f - speedX, base.properties.CurrentState.garbage.speedY);
		while (proj != null)
		{
			if (proj.transform.position.y > (float)Level.Current.Ground - 200f)
			{
				if (isFalling)
				{
					pct -= (float)CupheadTime.Delta * 4f;
					if (pct < -1f)
					{
						pct = -1f;
					}
				}
				velocity.y = base.properties.CurrentState.garbage.speedY * pct;
				proj.transform.position += velocity * CupheadTime.FixedDelta;
				if (proj.transform.position.y >= base.properties.CurrentState.garbage.maxHeight)
				{
					isFalling = true;
				}
				yield return null;
			}
			yield return null;
		}
		Object.Destroy(proj);
	}

	public void StartHeartAttack()
	{
		state = State.HeartTrans;
		animator.SetBool("OnRegurgitate", true);
		AudioManager.Play("level_flyingbird_stretcher_regurgitate_start");
		emitAudioFromObject.Add("level_flyingbird_stretcher_regurgitate_start");
	}

	private void OpenHeart()
	{
		state = State.Heart;
		heart.StartHeartAttack();
		GetComponent<DamageReceiver>().enabled = false;
		heartSpitFX.SetActive(true);
		StartCoroutine(checkHeart_cr());
	}

	private IEnumerator checkHeart_cr()
	{
		while (heart.gameObject.activeSelf)
		{
			yield return null;
		}
		animator.SetBool("OnRegurgitate", false);
		AudioManager.Play("level_flyingbird_stretcher_regurgitate_end");
		emitAudioFromObject.Add("level_flyingbird_stretcher_regurgitate_end");
		GetComponent<DamageReceiver>().enabled = true;
		yield return animator.WaitForAnimationToEnd(this, "Regurgitate_End");
		state = State.HeartTrans;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.heart.hesitate.RandomFloat());
		heartSpitFX.SetActive(false);
		state = State.Revived;
	}

	private void NursesHeartHeight()
	{
		Transform[] array = nurses.nurses;
		foreach (Transform transform in array)
		{
			transform.transform.localPosition = new Vector3(transform.transform.localPosition.x, transform.transform.localPosition.y + 8f);
		}
	}

	private void NursesGarbageHeight()
	{
		Transform[] array = nurses.nurses;
		foreach (Transform transform in array)
		{
			transform.transform.localPosition = new Vector3(transform.transform.localPosition.x, transform.transform.localPosition.y + 6f);
		}
	}

	private void NursesReset()
	{
		Transform[] array = nurses.nurses;
		foreach (Transform transform in array)
		{
			transform.transform.localPosition = Vector3.zero;
		}
	}
}
