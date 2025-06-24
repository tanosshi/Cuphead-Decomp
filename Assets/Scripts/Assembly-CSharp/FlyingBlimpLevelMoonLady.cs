using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelMoonLady : LevelProperties.FlyingBlimp.Entity
{
	public enum State
	{
		Unspawned = 0,
		Morph = 1,
		Idle = 2,
		Attack = 3,
		Death = 4
	}

	public bool changeStarted;

	[SerializeField]
	private GameObject smoke;

	[SerializeField]
	private Transform smokeFlippedPos;

	[SerializeField]
	private AudioSource pedal;

	[SerializeField]
	private AudioSource gears;

	[SerializeField]
	private FlyingBlimpLevelUFO ufoPrefabA;

	[SerializeField]
	private FlyingBlimpLevelUFO ufoPrefabB;

	[SerializeField]
	private Transform ufoStartPoint;

	[SerializeField]
	private Transform ufoMidPoint;

	[SerializeField]
	private Transform ufoStopPoint;

	[SerializeField]
	private Transform dimColor;

	[SerializeField]
	private Transform transformSpawnPoint;

	[SerializeField]
	private Transform transformMorphEndPoint;

	[SerializeField]
	private FlyingBlimpLevelStars starPrefabA;

	[SerializeField]
	private FlyingBlimpLevelStars starPrefabB;

	[SerializeField]
	private FlyingBlimpLevelStars starPrefabC;

	[SerializeField]
	private FlyingBlimpLevelStars starPrefabPink;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private CollisionChild[] childColliders;

	private float time;

	private bool startTimer;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		childColliders = base.gameObject.GetComponentsInChildren<CollisionChild>();
		changeStarted = false;
		Vector3 vector = new Vector3(1f, 1f, 1f);
		if (Rand.Bool())
		{
			vector.y = 0f - vector.y;
			smoke.transform.position = smokeFlippedPos.transform.position;
		}
		smoke.transform.SetScale(1f, vector.y, 1f);
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		CollisionChild[] array = childColliders;
		foreach (CollisionChild collisionChild in array)
		{
			collisionChild.OnPlayerCollision += OnCollisionPlayer;
			collisionChild.GetComponent<Collider2D>().enabled = false;
		}
	}

	public void StartIntro()
	{
		base.transform.position = transformSpawnPoint.position;
		base.animator.SetTrigger("To A");
		StartCoroutine(intro_cr());
	}

	public override void LevelInit(LevelProperties.FlyingBlimp properties)
	{
		base.LevelInit(properties);
		state = State.Unspawned;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state != State.Morph)
		{
			if (base.properties.CurrentState.uFO.invincibility)
			{
				if (info.damage < base.properties.CurrentHealth)
				{
					base.properties.DealDamage(info.damage);
				}
				else if (state == State.Idle)
				{
					base.properties.DealDamage(info.damage);
				}
			}
			else
			{
				base.properties.DealDamage(info.damage);
			}
		}
		if (base.properties.CurrentHealth <= 0f && state != State.Death)
		{
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
		if (PauseManager.state == PauseManager.State.Paused)
		{
			pedal.Pause();
			gears.Pause();
		}
		else
		{
			pedal.UnPause();
			gears.UnPause();
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		pedal.Stop();
		gears.Stop();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator intro_cr()
	{
		AudioManager.Play("level_flying_blimp_transform_moon");
		state = State.Morph;
		LevelProperties.FlyingBlimp.Morph p = base.properties.CurrentState.morph;
		PlanePlayerController playerOne = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
		PlanePlayerController playerTwo = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
		if (playerOne != null && playerOne.isActiveAndEnabled)
		{
			playerOne.animationController.SetColorOverTime(dimColor.GetComponent<SpriteRenderer>().color, 15f);
		}
		if (playerTwo != null && playerTwo.isActiveAndEnabled)
		{
			playerTwo.animationController.SetColorOverTime(dimColor.GetComponent<SpriteRenderer>().color, 15f);
		}
		yield return null;
		while (base.transform.position != transformMorphEndPoint.position)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, transformMorphEndPoint.position, 300f * (float)CupheadTime.Delta);
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
		}
		yield return CupheadTime.WaitForSeconds(this, p.crazyAHold);
		pedal.Stop();
		base.animator.SetTrigger("To B");
		yield return CupheadTime.WaitForSeconds(this, p.crazyBHold);
		base.animator.SetTrigger("End");
		StartCoroutine(stars_cr());
		yield return base.animator.WaitForAnimationToEnd(this, "Morph_End");
		state = State.Idle;
		CollisionChild[] array = childColliders;
		foreach (CollisionChild collisionChild in array)
		{
			collisionChild.GetComponent<Collider2D>().enabled = true;
		}
		Level.Current.SetBounds(null, Level.Current.Right - 250, null, null);
		StartCoroutine(ufo_attack_handler_cr());
		yield return null;
	}

	private void SpawnStar(FlyingBlimpLevelStars prefab, Vector2 startPoint)
	{
		if (prefab != null)
		{
			Vector2 pos = prefab.transform.position;
			pos.y = 360f - startPoint.y;
			pos.x = 640f;
			prefab.Create(pos, base.properties.CurrentState.stars);
		}
	}

	private IEnumerator stars_cr()
	{
		LevelProperties.FlyingBlimp.Stars p = base.properties.CurrentState.stars;
		string[] positionPattern = p.positionString.GetRandom().Split(',');
		string[] typePattern = p.typeString.GetRandom().Split(',');
		int n = Random.Range(0, typePattern.Length);
		float t = 0f;
		int place = Random.Range(0, positionPattern.Length);
		float waitTime = 0f;
		Vector2 spawnPos = Vector2.zero;
		while (true)
		{
			int i;
			for (i = place; i < positionPattern.Length; i++)
			{
				if (waitTime > 0f)
				{
					yield return CupheadTime.WaitForSeconds(this, waitTime);
				}
				if (positionPattern[i][0] == 'D')
				{
					float.TryParse(positionPattern[i].Substring(1), out waitTime);
				}
				else
				{
					string[] array = positionPattern[i].Split('-');
					string[] array2 = array;
					foreach (string s in array2)
					{
						float result = 0f;
						float.TryParse(s, out result);
						FlyingBlimpLevelStars prefab = null;
						if (typePattern[n][0] == 'A')
						{
							prefab = starPrefabA;
						}
						else if (typePattern[n][0] == 'B')
						{
							prefab = starPrefabB;
						}
						else if (typePattern[n][0] == 'C')
						{
							prefab = starPrefabC;
						}
						else if (typePattern[n][0] == 'P')
						{
							prefab = starPrefabPink;
						}
						float.TryParse(positionPattern[i].Substring(1), out waitTime);
						spawnPos.y = result;
						if (state != State.Death)
						{
							SpawnStar(prefab, spawnPos);
						}
						n = (n + 1) % typePattern.Length;
					}
					waitTime = p.delay;
				}
				t += waitTime;
				i %= positionPattern.Length;
				place = 0;
			}
		}
	}

	private IEnumerator ufo_attack_handler_cr()
	{
		state = State.Idle;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.uFO.moonWaitForNextATK);
		StartCoroutine(ufo_cr());
		yield return null;
	}

	private void SmokeEffect()
	{
		base.animator.Play("Moon_Smoke");
	}

	private void SpawnUFO(FlyingBlimpLevelUFO prefab)
	{
		LevelProperties.FlyingBlimp.UFO uFO = base.properties.CurrentState.uFO;
		FlyingBlimpLevelUFO flyingBlimpLevelUFO = Object.Instantiate(prefab);
		flyingBlimpLevelUFO.Init(ufoStartPoint.position, ufoMidPoint.position, ufoStopPoint.position, uFO.UFOSpeed, uFO.UFOHP, uFO);
	}

	private IEnumerator ufo_cr()
	{
		state = State.Attack;
		float volume = 0.1f;
		LevelProperties.FlyingBlimp.UFO p = base.properties.CurrentState.uFO;
		string[] typePattern = p.UFOString.GetRandom().Split(',');
		int index = Random.Range(0, typePattern.Length);
		AudioManager.Play("level_flying_blimp_moon_anticipation");
		base.animator.SetTrigger("To ATK");
		yield return CupheadTime.WaitForSeconds(this, p.moonATKAnticipation);
		gears.Play();
		gears.volume = volume;
		AudioManager.Play("level_flying_blimp_moon_face_extend");
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Moon_Attack");
		time = 0f;
		startTimer = true;
		StartCoroutine(timer_cr());
		while (time < p.moonATKDuration)
		{
			pedal.volume = volume;
			if (volume < 1f)
			{
				volume += 0.1f;
			}
			if (state != State.Death)
			{
				if (typePattern[index][0] == 'A')
				{
					SpawnUFO(ufoPrefabA);
				}
				else if (typePattern[index][0] == 'B')
				{
					SpawnUFO(ufoPrefabB);
				}
			}
			yield return CupheadTime.WaitForSeconds(this, p.UFODelay);
			index = ((index < typePattern.Length - 1) ? (index + 1) : 0);
		}
		pedal.volume = 1f;
		base.animator.SetTrigger("End");
		startTimer = false;
		gears.Stop();
		AudioManager.Play("level_flying_blimp_moon_gears_idle");
		yield return base.animator.WaitForAnimationToEnd(this, "Moon_Attack_To_Idle");
		StartCoroutine(ufo_attack_handler_cr());
	}

	private IEnumerator timer_cr()
	{
		while (startTimer)
		{
			time += CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	public void StartDeath()
	{
		state = State.Death;
		StartCoroutine(die_cr());
	}

	private IEnumerator die_cr()
	{
		base.animator.SetTrigger("Death");
		GetComponent<Collider2D>().enabled = false;
		yield return null;
	}
}
