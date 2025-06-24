using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogsLevelTall : LevelProperties.Frogs.Entity
{
	public enum State
	{
		Idle = 0,
		Fan = 1,
		Fireflies = 2,
		Morphing = 3,
		Complete = 1000000,
		Morphed = 1000001
	}

	public static FrogsLevelTall Current;

	[SerializeField]
	private FrogsLevelTallFirefly fireflyPrefab;

	[SerializeField]
	private FrogsLevelTallFireflyRoot[] fireflyRoots;

	[SerializeField]
	private Transform spitRoot;

	[Space(10f)]
	public Transform shortMorphRoot;

	private new Animator animator;

	private LevelPlayerMotor.VelocityManager.Force fanForce;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private int layer;

	private const float FAN_START_TIME = 2f;

	private const float FAN_END_TIME = 0.5f;

	private const float FIRST_FAN_MOVE_OFFSET = 60f;

	private const float FAN_DECELERATE_TIME = 0.75f;

	private bool firstFan = true;

	private int fireflyCount;

	private List<FrogsLevelTallFireflyRoot> tempRoots;

	private LevelProperties.Frogs.TallFireflies fireflyProperties;

	private const float MORPH_MOVE_TIME = 1f;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		animator = GetComponent<Animator>();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 0.3f, DamageDealer.DamageSource.Enemy, true, false, false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Start()
	{
		base.Start();
		Level.Current.OnIntroEvent += OnLevelIntro;
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInit(LevelProperties.Frogs properties)
	{
		base.LevelInit(properties);
		properties.OnBossDeath += OnBossDeath;
	}

	public void AddFanForce(AbstractPlayerController player)
	{
		if (fanForce == null)
		{
			fanForce = new LevelPlayerMotor.VelocityManager.Force(LevelPlayerMotor.VelocityManager.Force.Type.All, 0f);
			fanForce.enabled = false;
		}
		player.GetComponent<LevelPlayerMotor>().AddForce(fanForce);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!FrogsLevel.FINAL_FORM && base.properties.CurrentState.stateName != LevelProperties.Frogs.States.Main)
		{
			base.properties.DealDamage(info.damage);
		}
	}

	private void OnBossDeath()
	{
		StopAllCoroutines();
		fanForce.enabled = false;
		animator.SetTrigger("OnDeath");
		AudioManager.Play("level_frogs_tall_death");
	}

	private void OnLevelIntro()
	{
		AudioManager.Play("level_frogs_tall_intro_full");
		animator.Play("Intro");
	}

	public void StartFan()
	{
		if (state == State.Idle || state == State.Complete)
		{
			state = State.Fan;
			StartCoroutine(fan_cr());
		}
	}

	private IEnumerator fan_cr()
	{
		LevelProperties.Frogs.TallFan p = base.properties.CurrentState.tallFan;
		float time = p.duration;
		animator.Play("Fan");
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		AudioManager.Play("level_frogs_tall_fan_start");
		emitAudioFromObject.Add("level_frogs_tall_fan_start");
		StartCoroutine(fanAccelerate_cr(p));
		yield return CupheadTime.WaitForSeconds(this, 2f);
		AudioManager.PlayLoop("level_frogs_tall_fan_attack_loop");
		emitAudioFromObject.Add("level_frogs_tall_fan_attack_loop");
		if (firstFan)
		{
			firstFan = false;
			float startX = base.transform.position.x;
			yield return CupheadTime.WaitForSeconds(this, 0.25f);
			float t = 0f;
			while (t < 0.5f)
			{
				float val = t / 0.5f;
				TransformExtensions.SetPosition(x: EaseUtils.Ease(EaseUtils.EaseType.easeInSine, startX, startX + 60f, val), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			base.transform.SetPosition(startX + 60f);
			yield return CupheadTime.WaitForSeconds(this, p.duration.RandomFloat() - 0.75f);
		}
		else
		{
			yield return CupheadTime.WaitForSeconds(this, p.duration.RandomFloat());
		}
		yield return CupheadTime.WaitForSeconds(this, time);
		AudioManager.Play("level_frogs_tall_fan_end");
		emitAudioFromObject.Add("level_frogs_tall_fan_end");
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AudioManager.Stop("level_frogs_tall_fan_attack_loop");
		animator.SetTrigger("OnFanEnd");
		StartCoroutine(fanDecelerate_cr(p));
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		state = State.Complete;
	}

	private IEnumerator fanAccelerate_cr(LevelProperties.Frogs.TallFan p)
	{
		fanForce.enabled = true;
		yield return StartCoroutine(fanPowerTween_cr(0f, p.power, p.accelerationTime));
	}

	private IEnumerator fanDecelerate_cr(LevelProperties.Frogs.TallFan p)
	{
		yield return StartCoroutine(fanPowerTween_cr(p.power, 0f, 0.75f));
		fanForce.enabled = false;
	}

	private IEnumerator fanPowerTween_cr(float start, float end, float time)
	{
		fanForce.value = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			fanForce.value = Mathf.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		fanForce.value = end;
	}

	public void StartFireflies()
	{
		layer = 0;
		if (state == State.Idle || state == State.Complete)
		{
			state = State.Fireflies;
			fireflyCount = 0;
			animator.SetBool("EndFirefly", false);
			StartCoroutine(fireflies_cr());
		}
	}

	private void ResetFireflyRoots()
	{
		tempRoots = new List<FrogsLevelTallFireflyRoot>(fireflyRoots);
	}

	private void ShootFirefly()
	{
		AudioManager.Play("level_frogs_tall_spit_shoot");
		emitAudioFromObject.Add("level_frogs_tall_spit_shoot");
		FrogsLevelTallFireflyRoot frogsLevelTallFireflyRoot = tempRoots[Random.Range(0, tempRoots.Count)];
		tempRoots.Remove(frogsLevelTallFireflyRoot);
		Vector2 vector = frogsLevelTallFireflyRoot.transform.position;
		Vector2 vector2 = vector;
		Vector2 vector3 = new Vector2(Random.value * (float)(Rand.Bool() ? 1 : (-1)), Random.value * (float)(Rand.Bool() ? 1 : (-1)));
		vector = vector2 + vector3.normalized * frogsLevelTallFireflyRoot.radius * Random.value;
		fireflyPrefab.Create(spitRoot.position, vector, fireflyProperties.speed, fireflyProperties.hp, fireflyProperties.followDelay, fireflyProperties.followTime, fireflyProperties.followDistance, fireflyProperties.invincibleDuration, PlayerManager.GetNext(), layer++);
		fireflyCount--;
	}

	private IEnumerator fireflies_cr()
	{
		fireflyProperties = base.properties.CurrentState.tallFireflies;
		string patternString = fireflyProperties.patterns[Random.Range(0, fireflyProperties.patterns.Length)];
		KeyValue[] pattern = KeyValue.ListFromString(patternString, new char[2] { 'S', 'D' });
		animator.SetTrigger("OnFirefly");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i].key == "S")
			{
				ResetFireflyRoots();
				fireflyCount = (int)pattern[i].value;
				animator.SetInteger("FireflyCount", fireflyCount);
				animator.SetTrigger("OnFireflyStart");
				animator.SetBool("EndFirefly", i >= pattern.Length - 1);
				while (fireflyCount > 0)
				{
					animator.SetInteger("FireflyCount", fireflyCount);
					yield return null;
				}
				animator.SetInteger("FireflyCount", fireflyCount);
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, pattern[i].value);
			}
		}
		yield return CupheadTime.WaitForSeconds(this, fireflyProperties.hesitate);
		state = State.Complete;
	}

	private void MorphSFX()
	{
		AudioManager.Play("level_frogs_tall_morph_end");
		emitAudioFromObject.Add("level_frogs_tall_morph_end");
	}

	public void StartMorph()
	{
		StopAllCoroutines();
		fanForce.value = 0f;
		fanForce.enabled = false;
		state = State.Morphing;
		animator.Play("Morph");
	}

	public void ContinueMorph()
	{
		StartCoroutine(morph_cr());
	}

	private IEnumerator morph_cr()
	{
		animator.SetTrigger("OnMorphContinue");
		Vector2 start = base.transform.position;
		Vector2 end = new Vector2(631f, -314f);
		float t = 0f;
		while (t < 1f)
		{
			float val = t / 1f;
			float x = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start.x, end.x, val);
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start.y, end.y, val), transform: base.transform, x: x);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = end;
		state = State.Morphed;
	}

	private void OnMorphAnimationComplete()
	{
		FrogsLevelMorphed.Current.Enable(FrogsLevel.DEMON_TRIGGERED);
		CupheadLevelCamera.Current.Shake(20f, 0.6f);
		base.properties.OnBossDeath -= OnBossDeath;
		base.gameObject.SetActive(false);
	}
}
