using System;
using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelBlimpLady : LevelProperties.FlyingBlimp.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Dash = 2,
		Tornado = 3,
		Shoot = 4,
		Taurus = 5,
		Sagittarius = 6,
		Gemini = 7,
		Death = 8
	}

	public enum constellationPossibility
	{
		Taurus = 1,
		Sagittarius = 2,
		Gemini = 3
	}

	[Header("Phase Materials")]
	[SerializeField]
	private Material blimpMat;

	[SerializeField]
	private Material taurusMat;

	[SerializeField]
	private Material sagittariusMat;

	[SerializeField]
	private Material geminiMat;

	[Space(10f)]
	[SerializeField]
	private Transform pivotPoint;

	[SerializeField]
	private Transform transformationPoint;

	[SerializeField]
	private Effect dashExplosionEffect;

	[SerializeField]
	private GameObject cloudEffect;

	[SerializeField]
	private GameObject bigCloud;

	[SerializeField]
	private SpriteRenderer constellationHandler;

	[SerializeField]
	private SpriteRenderer blackDim;

	[SerializeField]
	private FlyingBlimpLevelEnemy enemyPrefabA;

	[SerializeField]
	private FlyingBlimpLevelEnemy enemyPrefabB;

	[SerializeField]
	private FlyingBlimpLevelTornado tornadoPrefab;

	private FlyingBlimpLevelTornado tornado;

	[SerializeField]
	private FlyingBlimpLevelShootProjectile shootProjectilePrefab;

	[SerializeField]
	private FlyingBlimpLevelArrowProjectile sagittariusStarPrefab;

	[SerializeField]
	private BasicProjectile sagittariusArrowPrefab;

	[SerializeField]
	private FlyingBlimpLevelGeminiShoot geminiObjectPrefab;

	private FlyingBlimpLevelGeminiShoot geminiObject;

	[SerializeField]
	private SpriteRenderer geminiClone;

	[SerializeField]
	private SpriteRenderer sphere;

	[SerializeField]
	private FlyingBlimpLevelSpawnRadius objectSpawnRoot;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Transform arrowRoot;

	[SerializeField]
	private Transform arrowEffectRoot;

	[SerializeField]
	private FlyingBlimpLevelMoonLady moonLady;

	[SerializeField]
	private Vector2 explosionOffset = Vector2.zero;

	[SerializeField]
	private Effect arrowEffect;

	[SerializeField]
	private float explosionRadius = 100f;

	private float angle;

	private float originalAngle;

	private float originalSpeed;

	private float loopSize = 80f;

	private float movementSpeed;

	private float waitLoopTime;

	private Vector3 startPos;

	private Vector3 startCloudPos;

	private Vector3 pivotOffset;

	private Vector3 getPos;

	private Vector2 geminiTarget;

	private bool invert;

	private bool isLooping;

	private bool smallClouds;

	private bool dashExplosions;

	private bool transitionToSummon = true;

	private int index;

	private int taurusOffset = 210;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Coroutine patternCoroutine;

	private constellationPossibility constellation;

	public State state { get; private set; }

	public bool moving { get; private set; }

	public bool fading { get; private set; }

	public event Action OnDeathEvent;

	public event Action OnStateChanged;

	protected override void Awake()
	{
		base.Awake();
		state = State.Intro;
		pivotOffset = Vector3.up * 2f * loopSize;
		pivotPoint.position = base.transform.position;
		startPos = base.transform.position;
		ResetPivotPos(base.transform.position);
		startCloudPos = bigCloud.transform.position;
		originalAngle = angle;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		constellationHandler.color = new Color(1f, 1f, 1f, 0f);
	}

	public override void LevelInit(LevelProperties.FlyingBlimp properties)
	{
		base.LevelInit(properties);
		originalSpeed = properties.CurrentState.move.pathSpeed;
		moving = true;
		StartCoroutine(intro_cr());
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Death)
		{
			state = State.Death;
			StartDeath();
		}
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

	private void ResetPivotPos(Vector3 newPos)
	{
		pivotPoint.position = newPos;
		Vector3 position = base.transform.position;
		position.y = newPos.y + loopSize;
		base.transform.position = position;
	}

	private IEnumerator intro_cr()
	{
		AudioManager.Play("level_flying_blimp_intro");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_End");
		AudioManager.PlayLoop("level_flying_blimp_pedal_loop");
		StartCoroutine(move_cr());
		while (movementSpeed < originalSpeed)
		{
			movementSpeed += 0.2f;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.move.initalAttackDelayRange.RandomFloat());
		state = State.Idle;
	}

	private IEnumerator move_cr()
	{
		angle = ((!Rand.Bool()) ? 0f : ((float)Math.PI * 2f));
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			if (moving)
			{
				PathMovement();
				yield return wait;
			}
			else
			{
				yield return wait;
			}
		}
	}

	public void PathMovement()
	{
		LevelProperties.FlyingBlimp.Move move = base.properties.CurrentState.move;
		angle += movementSpeed * CupheadTime.FixedDelta;
		if (angle > (float)Math.PI * 2f)
		{
			invert = !invert;
			angle -= (float)Math.PI * 2f;
		}
		if (angle < 0f)
		{
			angle += (float)Math.PI * 2f;
		}
		float num;
		if (invert)
		{
			base.transform.position = pivotPoint.position + pivotOffset;
			num = -1f;
		}
		else
		{
			base.transform.position = pivotPoint.position;
			num = 1f;
		}
		Vector3 vector = new Vector3((0f - Mathf.Sin(angle)) * loopSize, Mathf.Cos(angle) * num * loopSize, 0f);
		base.transform.position += vector;
	}

	private void ChangeMat(Material mat)
	{
		GetComponent<SpriteRenderer>().material = mat;
	}

	public void StartDash()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(dash_cr());
	}

	private IEnumerator dash_cr()
	{
		bool startedClouds = false;
		smallClouds = true;
		transitionToSummon = true;
		Animator constAnimator = constellationHandler.GetComponent<Animator>();
		state = State.Dash;
		LevelProperties.FlyingBlimp.DashSummon p = base.properties.CurrentState.dashSummon;
		string[] pattern = p.patternString.GetRandom().Split(',');
		moving = false;
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i][0] == 'D')
			{
				float waitTime = 0f;
				float.TryParse(pattern[i].Substring(1), out waitTime);
				yield return CupheadTime.WaitForSeconds(this, waitTime);
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			AudioManager.Stop("level_flying_blimp_pedal_loop");
			AudioManager.Play("level_flying_blimp_inhale");
			base.animator.Play("Dash_Start");
			yield return base.animator.WaitForAnimationToEnd(this, "Dash_Start");
			yield return CupheadTime.WaitForSeconds(this, p.hold);
			AudioManager.Play("level_flying_blimp_exhale");
			base.animator.SetBool("Deflate", true);
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
			fading = true;
			StartCoroutine(fade_constellation_cr(true));
			AudioManager.Play("level_flying_blimp_lady_constellation_loop");
			switch (constellation)
			{
			case constellationPossibility.Taurus:
				constAnimator.Play("Taurus");
				break;
			case constellationPossibility.Sagittarius:
				constAnimator.Play("Sagittarius");
				break;
			case constellationPossibility.Gemini:
				constAnimator.Play("Gemini");
				break;
			default:
				Debug.Log("Invalid number");
				break;
			}
			base.animator.SetTrigger("Move");
			while (base.transform.position.x >= -1280f)
			{
				if (state != State.Death)
				{
					base.transform.position += base.transform.right * (0f - p.dashSpeed) * CupheadTime.Delta;
				}
				yield return null;
			}
			dashExplosions = false;
			base.animator.SetTrigger("ToOff");
			yield return CupheadTime.WaitForSeconds(this, p.reeentryDelay);
			base.animator.SetTrigger("StartSummon");
			AudioManager.PlayLoop("level_flying_blimp_pedal_loop");
			AudioManager.Play("level_flying_blimp_lady_constellation_transform");
			Vector3 endPos = startPos;
			if (constellation == constellationPossibility.Gemini)
			{
				endPos.x = startPos.x + 100f;
				ResetPivotPos(endPos);
			}
			Vector3 pos = base.transform.position;
			pos.y = startPos.y;
			base.transform.position = pos;
			while (base.transform.position.x <= endPos.x)
			{
				if (base.transform.position.x >= transformationPoint.position.x && !startedClouds)
				{
					fading = false;
					StartCoroutine(fade_constellation_cr(false));
					StartCoroutine(spawn_clouds_cr());
					startedClouds = true;
				}
				if (state != State.Death)
				{
					base.transform.position += base.transform.right * p.summonSpeed * CupheadTime.FixedDelta;
				}
				yield return new WaitForFixedUpdate();
			}
			if (base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Generic)
			{
				transitionToSummon = false;
			}
			else
			{
				transitionToSummon = true;
			}
			AudioManager.Stop("level_flying_blimp_pedal_loop");
			base.animator.Play("Big_Cloud");
			smallClouds = false;
			base.animator.SetBool("Deflate", false);
		}
	}

	private IEnumerator select_constellation_cr()
	{
		if (transitionToSummon)
		{
			switch (constellation)
			{
			case constellationPossibility.Taurus:
				ToTaurus();
				base.animator.Play("Taurus_Idle");
				ChangeMat(taurusMat);
				break;
			case constellationPossibility.Sagittarius:
				ToSagittarius();
				base.animator.Play("Sag_Cloud");
				base.animator.Play("Sagittarius_Idle");
				ChangeMat(sagittariusMat);
				break;
			case constellationPossibility.Gemini:
				ToGemini();
				base.animator.Play("Gemini");
				base.animator.Play("Sphere_Idle");
				ChangeMat(geminiMat);
				break;
			default:
				Debug.Log("Invalid number");
				break;
			}
			transitionToSummon = false;
		}
		else
		{
			ResetPivotPos(startPos);
			AudioManager.Play("level_flying_blimp_lady_constellation_transform_end");
			base.animator.Play("Appear");
			ChangeMat(blimpMat);
			if (constellation == constellationPossibility.Sagittarius)
			{
				base.animator.Play("Sag_Off");
			}
			yield return base.animator.WaitForAnimationToEnd(this, "Appear");
			moving = true;
			AudioManager.PlayLoop("level_flying_blimp_pedal_loop");
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.dashSummon.summonHesitate);
			state = State.Idle;
		}
		yield return null;
	}

	private IEnumerator check_state_cr(LevelProperties.FlyingBlimp.States currentState)
	{
		isLooping = true;
		while (base.properties.CurrentState.stateName == currentState)
		{
			yield return null;
		}
		isLooping = false;
		waitLoopTime = 0f;
	}

	private void StartSmoke()
	{
		base.animator.Play("Dash_Smoke");
		dashExplosions = true;
		StartCoroutine(spawn_explosions_cr());
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + explosionOffset, explosionRadius);
	}

	private IEnumerator spawn_explosions_cr()
	{
		while (dashExplosions)
		{
			Effect explosion = UnityEngine.Object.Instantiate(dashExplosionEffect);
			explosion.transform.position = base.transform.position;
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
		}
	}

	private IEnumerator spawn_clouds_cr()
	{
		while (smallClouds)
		{
			GameObject cloud = UnityEngine.Object.Instantiate(cloudEffect);
			Vector3 scale = new Vector3(1f, 1f, 1f);
			scale.x = ((!Rand.Bool()) ? (0f - scale.x) : scale.x);
			scale.y = ((!Rand.Bool()) ? (0f - scale.y) : scale.y);
			cloud.transform.SetScale(scale.x, scale.y, 1f);
			cloud.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
			cloud.GetComponent<SpriteRenderer>().sortingOrder = UnityEngine.Random.Range(0, 3);
			cloud.transform.position = GetRandomPoint();
			StartCoroutine(delete_cloud_cr(cloud));
			yield return CupheadTime.WaitForSeconds(this, 0.05f);
		}
	}

	private IEnumerator delete_cloud_cr(GameObject cloud)
	{
		yield return cloud.GetComponent<Animator>().WaitForAnimationToEnd(this, "Cloud");
		UnityEngine.Object.Destroy(cloud);
	}

	private Vector2 GetRandomPoint()
	{
		Vector2 vector = (Vector2)base.transform.position + explosionOffset;
		Vector2 vector2 = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized * (explosionRadius * UnityEngine.Random.value) * 2f;
		return vector + vector2;
	}

	private IEnumerator fade_constellation_cr(bool fadeIn)
	{
		float fadeTime = 0.5f;
		float blackMaxFade = 0.25f;
		float blackMidFade = 0.13f;
		float blackCurrentFade = 0f;
		if (fadeIn)
		{
			float t = 0f;
			while (t < fadeTime)
			{
				constellationHandler.color = new Color(1f, 1f, 1f, t / fadeTime);
				if (blackCurrentFade < blackMaxFade)
				{
					blackDim.color = new Color(0f, 0f, 0f, blackCurrentFade + t / 3f);
				}
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			constellationHandler.color = new Color(1f, 1f, 1f, 1f);
			blackDim.color = new Color(0f, 0f, 0f, blackMaxFade);
			yield break;
		}
		float t2 = 0f;
		blackCurrentFade = blackMaxFade;
		while (t2 < fadeTime)
		{
			constellationHandler.color = new Color(1f, 1f, 1f, 1f - t2 / fadeTime);
			if (blackCurrentFade > blackMidFade)
			{
				blackDim.color = new Color(0f, 0f, 0f, blackCurrentFade - t2 / 3f);
			}
			t2 += (float)CupheadTime.Delta;
			yield return null;
		}
		constellationHandler.color = new Color(1f, 1f, 1f, 0f);
		blackDim.color = new Color(0f, 0f, 0f, blackMidFade);
		yield return null;
	}

	private IEnumerator final_fade_cr()
	{
		float fadeTime = 0.5f;
		float blackMidFade = 0.13f;
		float t = 0f;
		while (t < fadeTime)
		{
			blackDim.color = new Color(0f, 0f, 0f, blackMidFade - t / 3f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		constellationHandler.color = new Color(1f, 1f, 1f, 0f);
		yield return null;
	}

	public void StartTaurus()
	{
		constellation = constellationPossibility.Taurus;
		StartDash();
	}

	private void ToTaurus()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(taurus_cr());
	}

	private IEnumerator taurus_cr()
	{
		LevelProperties.FlyingBlimp.Taurus p = base.properties.CurrentState.taurus;
		StartCoroutine(check_state_cr(base.properties.CurrentState.stateName));
		waitLoopTime = p.attackDelayRange.RandomFloat();
		moving = true;
		if (base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Taurus)
		{
			state = State.Taurus;
			movementSpeed = p.movementSpeed;
			while (isLooping)
			{
				float t = 0f;
				while (t < waitLoopTime)
				{
					t += (float)CupheadTime.Delta;
					if (!isLooping)
					{
						break;
					}
					yield return null;
				}
				t = 0f;
				moving = false;
				base.animator.SetTrigger("TaurusATK");
				yield return base.animator.WaitForAnimationToStart(this, "Taurus_Attack");
				AudioManager.Play("level_flying_blimp_taurus_attack");
				AudioManager.Stop("level_flying_blimp_taurus_idle");
				emitAudioFromObject.Add("level_flying_blimp_taurus_attack");
				yield return base.animator.WaitForAnimationToEnd(this, "Taurus_Attack");
				moving = true;
				yield return null;
			}
		}
		base.animator.Play("Big_Cloud");
		movementSpeed = originalSpeed;
		StartCoroutine(final_fade_cr());
		yield return null;
	}

	public void StartSagittarius()
	{
		constellation = constellationPossibility.Sagittarius;
		StartDash();
	}

	private void ToSagittarius()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(sagittarius_cr());
	}

	private IEnumerator sagittarius_cr()
	{
		LevelProperties.FlyingBlimp.Sagittarius p = base.properties.CurrentState.sagittarius;
		StartCoroutine(check_state_cr(base.properties.CurrentState.stateName));
		waitLoopTime = p.attackDelayRange.RandomFloat();
		moving = true;
		if (base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Sagittarius || base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.SagOrGem)
		{
			state = State.Sagittarius;
			movementSpeed = p.movementSpeed;
			while (isLooping)
			{
				base.animator.SetTrigger("SagittariusATK");
				yield return base.animator.WaitForAnimationToStart(this, "Sagittarius_Attack_Loop");
				AudioManager.Play("level_flying_blimp_sagittarius_anticipation");
				yield return CupheadTime.WaitForSeconds(this, p.arrowWarning);
				base.animator.SetTrigger("Continue");
				AudioManager.Stop("level_flying_blimp_sagittarius_anticipation");
				float t = 0f;
				while (t < waitLoopTime)
				{
					t += (float)CupheadTime.Delta;
					if (!isLooping)
					{
						break;
					}
					yield return null;
				}
				t = 0f;
				yield return null;
			}
		}
		base.animator.Play("Big_Cloud");
		movementSpeed = originalSpeed;
		StartCoroutine(final_fade_cr());
		yield return null;
	}

	private void FireArrowsStars()
	{
		LevelProperties.FlyingBlimp.Sagittarius sagittarius = base.properties.CurrentState.sagittarius;
		int num = 3;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < num; i++)
		{
			AbstractPlayerController next = PlayerManager.GetNext();
			num3 = sagittarius.homingSpreadAngle.GetFloatAt((float)i / ((float)num - 1f));
			float num4 = sagittarius.homingSpreadAngle.max / 2f;
			num3 -= num4;
			num2 = Mathf.Atan2(0f, -360f) * 57.29578f;
			sagittariusStarPrefab.Create(arrowEffectRoot.transform.position, num2 + num3, sagittarius.arrowInitialSpeed, sagittarius.homingSpeed, sagittarius.homingRotation, sagittarius.homingDurationRange.RandomFloat(), sagittarius.homingDelay, next, sagittarius.arrowHP);
		}
		arrowEffect.Create(arrowEffectRoot.transform.position);
		sagittariusArrowPrefab.Create(arrowRoot.position, 180f, sagittarius.arrowInitialSpeed);
	}

	public void StartGemini()
	{
		constellation = constellationPossibility.Gemini;
		StartDash();
	}

	private void ToGemini()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(gemini_cr());
	}

	private IEnumerator gemini_cr()
	{
		LevelProperties.FlyingBlimp.Gemini p = base.properties.CurrentState.gemini;
		waitLoopTime = base.properties.CurrentState.gemini.spawnerDelay.RandomFloat();
		StartCoroutine(check_state_cr(base.properties.CurrentState.stateName));
		pivotPoint.position = base.transform.position;
		moving = true;
		bool repeat = false;
		if (base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Gemini || base.properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.SagOrGem)
		{
			state = State.Gemini;
			while (isLooping)
			{
				if (geminiObject == null)
				{
					if (repeat)
					{
						AudioManager.Play("level_flying_blimp_gemini_sphere_reappear");
						base.animator.Play("Sphere_Reappear");
					}
					float t = 0f;
					while (t < waitLoopTime)
					{
						t += (float)CupheadTime.Delta;
						if (!isLooping)
						{
							break;
						}
						yield return null;
					}
					t = 0f;
					yield return base.animator.WaitForAnimationToEnd(this, "Gemini");
					base.animator.SetTrigger("GeminiATK");
					AudioManager.Play("level_flying_blimp_gemini_attack");
					base.animator.Play("Gemini_Attack");
					yield return CupheadTime.WaitForSeconds(this, p.spawnerSpeed);
					SpawnGemini();
					repeat = true;
					yield return null;
				}
				yield return null;
			}
		}
		base.animator.Play("Big_Cloud");
		movementSpeed = originalSpeed;
		StartCoroutine(final_fade_cr());
		yield return null;
	}

	private void SpawnGemini()
	{
		geminiTarget = objectSpawnRoot.transform.position;
		Vector2 vector = geminiTarget;
		Vector2 vector2 = new Vector2(UnityEngine.Random.value * (float)(Rand.Bool() ? 1 : (-1)), UnityEngine.Random.value * (float)(Rand.Bool() ? 1 : (-1)));
		geminiTarget = vector + vector2.normalized * objectSpawnRoot.radius * UnityEngine.Random.value;
		geminiObject = UnityEngine.Object.Instantiate(geminiObjectPrefab);
		geminiObject.Init(base.properties.CurrentState.gemini, geminiTarget, base.properties.CurrentState.gemini.spawnerSpeed);
	}

	private void SwitchCloneBottomLayer()
	{
		geminiClone.sortingOrder = 1;
		GetComponent<SpriteRenderer>().sortingOrder = 3;
	}

	private void SwitchCloneTopLayer()
	{
		geminiClone.sortingOrder = 3;
		GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	private void SwitchSphereLayer(int layer)
	{
		sphere.sortingOrder = layer;
	}

	public void SummonTornado()
	{
		LevelProperties.FlyingBlimp.Tornado tornado = base.properties.CurrentState.tornado;
		this.tornado = UnityEngine.Object.Instantiate(tornadoPrefab);
		this.tornado.Init(projectileRoot.transform.position, PlayerManager.GetNext(), tornado);
	}

	private void MoveTornado()
	{
		if (tornadoPrefab != null)
		{
			StartCoroutine(tornado.move_cr());
		}
	}

	public void StartTornado()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(tornado_cr());
	}

	public IEnumerator tornado_cr()
	{
		state = State.Tornado;
		LevelProperties.FlyingBlimp.Tornado p = base.properties.CurrentState.tornado;
		moving = false;
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AudioManager.Stop("level_flying_blimp_pedal_loop");
		AudioManager.Play("level_flying_blimp_tornado");
		SummonTornado();
		base.animator.Play("Tornado_Start");
		yield return base.animator.WaitForAnimationToEnd(this, "Tornado_Start");
		yield return CupheadTime.WaitForSeconds(this, p.loopDuration);
		base.animator.SetTrigger("FinishTornado");
		yield return base.animator.WaitForAnimationToEnd(this, "Tornado_Finish");
		AudioManager.PlayLoop("level_flying_blimp_pedal_loop");
		moving = true;
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack);
		state = State.Idle;
	}

	public void StartShoot()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(shoot_cr());
	}

	private IEnumerator shoot_cr()
	{
		state = State.Shoot;
		LevelProperties.FlyingBlimp.Shoot p = base.properties.CurrentState.shoot;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		AudioManager.Play("level_flying_blimp_fire");
		base.animator.Play("Shoot_Start");
		yield return base.animator.WaitForAnimationToEnd(this, "Shoot_Start");
		spawnProjectile();
		yield return CupheadTime.WaitForSeconds(this, 0.7f);
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttackRange.RandomFloat());
		state = State.Idle;
	}

	private void spawnProjectile()
	{
		LevelProperties.FlyingBlimp.Shoot shoot = base.properties.CurrentState.shoot;
		shootProjectilePrefab.Create(projectileRoot.position, 0f, base.properties.CurrentState.shoot);
	}

	private void SummonEnemy(FlyingBlimpLevelEnemy prefab, Vector3 startPoint, float stopPoint, bool type)
	{
		FlyingBlimpLevelEnemy flyingBlimpLevelEnemy = UnityEngine.Object.Instantiate(prefab);
		Vector2 vector = flyingBlimpLevelEnemy.transform.position;
		vector.y = 360f - startPoint.y;
		vector.x = 740f;
		stopPoint = base.properties.CurrentState.enemy.stopDistance.RandomFloat();
		flyingBlimpLevelEnemy.transform.position = vector;
		flyingBlimpLevelEnemy.Init(base.properties, startPoint, stopPoint, type, this);
	}

	public IEnumerator spawnEnemy_cr()
	{
		LevelProperties.FlyingBlimp.Enemy p = base.properties.CurrentState.enemy;
		string[] spawnPattern = p.spawnString.GetRandom().Split(',');
		string[] typePattern = p.typeString.GetRandom().Split(',');
		bool AParryable = false;
		float waitTime = 0f;
		int counter = 0;
		int typeIndex = 0;
		int spawnIndex = UnityEngine.Random.Range(0, spawnPattern.Length);
		Vector3 spawnPos = Vector3.zero;
		while (true)
		{
			int i;
			for (i = spawnIndex; i < spawnPattern.Length; i++)
			{
				if (waitTime > 0f)
				{
					yield return CupheadTime.WaitForSeconds(this, waitTime);
				}
				if (spawnPattern[i][0] == 'D')
				{
					float.TryParse(spawnPattern[i].Substring(1), out waitTime);
				}
				else
				{
					string[] array = spawnPattern[i].Split('-');
					string[] array2 = array;
					foreach (string s in array2)
					{
						float result = 0f;
						float result2 = 0f;
						float.TryParse(s, out result);
						float.TryParse(s, out result2);
						FlyingBlimpLevelEnemy prefab = null;
						if (typePattern[typeIndex][0] == 'A')
						{
							prefab = enemyPrefabA;
							if ((float)counter >= p.APinkOccurance.RandomFloat())
							{
								AParryable = true;
								counter = 0;
							}
							else
							{
								AParryable = false;
								counter++;
							}
						}
						else if (typePattern[typeIndex][0] == 'B')
						{
							prefab = enemyPrefabB;
							AParryable = false;
						}
						spawnPos.y = result;
						if (state != State.Death)
						{
							SummonEnemy(prefab, spawnPos, result2, AParryable);
						}
						typeIndex = (typeIndex + 1) % typePattern.Length;
					}
					waitTime = p.stringDelay;
				}
				i %= spawnPattern.Length;
			}
			spawnIndex = 0;
		}
	}

	public void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private IEnumerator die_cr()
	{
		base.animator.SetTrigger("Death");
		moving = false;
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		GetComponent<Collider2D>().enabled = false;
		yield return null;
	}

	public void SpawnMoonLady()
	{
		StartCoroutine(spawn_moon_lady_cr());
	}

	private IEnumerator spawn_moon_lady_cr()
	{
		while (angle > (float)Math.PI / 12f || angle < -(float)Math.PI / 12f)
		{
			yield return null;
		}
		moving = false;
		moonLady.StartIntro();
		UnityEngine.Object.Destroy(base.gameObject);
		yield return null;
	}

	private void SagAttackSFX()
	{
		AudioManager.Play("level_flying_blimp_sagittarius_attack");
		emitAudioFromObject.Add("level_flying_blimp_sagittarius_attack");
	}

	private void TaurusIdleSFX()
	{
		AudioManager.Play("level_flying_blimp_taurus_idle");
		emitAudioFromObject.Add("level_flying_blimp_taurus_idle");
	}
}
