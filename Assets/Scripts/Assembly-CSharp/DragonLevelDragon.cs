using System.Collections;
using UnityEngine;

public class DragonLevelDragon : LevelProperties.Dragon.Entity
{
	public enum State
	{
		Init = 0,
		Idle = 1,
		Meteor = 2,
		Peashot = 3
	}

	[Space(10f)]
	[SerializeField]
	private Transform mouthRoot;

	[SerializeField]
	private DragonLevelMeteor meteorPrefab;

	[SerializeField]
	private Effect smokePrefab;

	[SerializeField]
	private Transform smokeRoot;

	[Space(10f)]
	[SerializeField]
	private DragonLevelPeashot peashotPrefab;

	[SerializeField]
	private Transform peashotRoot;

	[Space(10f)]
	[SerializeField]
	private Transform chargeRoot;

	[SerializeField]
	private Transform dash;

	[SerializeField]
	private DragonLevelLeftSideDragon leftSideDragon;

	private LevelProperties.Dragon.Meteor currentMeteorProperties;

	private new Animator animator;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool dead;

	private DragonLevelMeteor.State meteorState;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			base.properties.DealDamage(info.damage);
		}
	}

	protected override void Start()
	{
		base.Start();
		Level.Current.OnIntroEvent += OnIntro;
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
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public override void LevelInit(LevelProperties.Dragon properties)
	{
		base.LevelInit(properties);
	}

	private void OnBossDeath()
	{
		if (!dead)
		{
			dead = true;
			AudioManager.Stop(Sfx.Level_Dragon_Sucking_Air);
			StopAllCoroutines();
			animator.Play("Death");
		}
	}

	private void StartWingSFX()
	{
		AudioManager.PlayLoop("level_dragon_left_dragon_peashot_idle_loop");
		emitAudioFromObject.Add("level_dragon_left_dragon_peashot_idle_loop");
	}

	private void StopWingSFX()
	{
		AudioManager.Stop("level_dragon_left_dragon_peashot_idle_loop");
	}

	private void OnIntro()
	{
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		state = State.Init;
		animator.Play("Intro");
		AudioManager.Play("level_dragon_left_dragon_intro");
		emitAudioFromObject.Add("level_dragon_left_dragon_intro");
		yield return animator.WaitForAnimationToEnd(this, "Intro");
		yield return CupheadTime.WaitForSeconds(this, 0.6f);
		state = State.Idle;
	}

	public void StartPeashot()
	{
		state = State.Peashot;
		StartCoroutine(peashot_cr());
	}

	private void PeashotInSFX()
	{
		AudioManager.Play("level_dragon_left_dragon_peashot_in");
		emitAudioFromObject.Add("level_dragon_left_dragon_peashot_in");
	}

	private IEnumerator peashot_cr()
	{
		LevelProperties.Dragon.Peashot p = base.properties.CurrentState.peashot;
		string[] pattern = p.patternString.GetRandom().Split(',');
		animator.SetBool("Peashot", true);
		yield return animator.WaitForAnimationToEnd(this, "Peashot_In");
		animator.Play("Peashot_Zinger");
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i].ToLower() == "p")
			{
				peashotRoot.LookAt2D(PlayerManager.GetNext().center);
				for (int c = 0; c < p.colorString.Length; c++)
				{
					int color = 0;
					switch (p.colorString[c])
					{
					case 'O':
						color = 0;
						break;
					case 'B':
						color = 1;
						break;
					case 'P':
						color = 2;
						break;
					}
					AudioManager.Play("level_dragon_left_dragon_peashot_fire");
					emitAudioFromObject.Add("level_dragon_left_dragon_peashot_fire");
					(peashotPrefab.Create(peashotRoot.position, peashotRoot.eulerAngles.z, p.speed) as DragonLevelPeashot).color = color;
					yield return CupheadTime.WaitForSeconds(this, p.shotDelay);
				}
			}
			else
			{
				float delay = 0f;
				float.TryParse(pattern[i], out delay);
				yield return CupheadTime.WaitForSeconds(this, delay);
			}
		}
		animator.SetBool("Peashot", false);
		yield return animator.WaitForAnimationToStart(this, "Peashot_Out");
		AudioManager.Play("level_dragon_left_dragon_peashot_out");
		emitAudioFromObject.Add("level_dragon_left_dragon_peashot_out");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private void SpawnSmokeFX()
	{
		Effect effect = Object.Instantiate(smokePrefab);
		effect.transform.position = smokeRoot.transform.position;
		effect.GetComponent<Animator>().Play((!Rand.Bool()) ? "Smoke_FX_B" : "Smoke_FX_A");
	}

	public void StartMeteor()
	{
		state = State.Meteor;
		StartCoroutine(meteor_cr());
	}

	private void FireMeteor()
	{
		AudioManager.Play("level_dragon_left_dragon_meteor_spit");
		emitAudioFromObject.Add("level_dragon_left_dragon_meteor_spit");
		if (meteorState == DragonLevelMeteor.State.Both)
		{
			meteorPrefab.Create(mouthRoot.position, new DragonLevelMeteor.Properties(currentMeteorProperties.timeY, currentMeteorProperties.speedX, DragonLevelMeteor.State.Up));
			meteorPrefab.Create(mouthRoot.position, new DragonLevelMeteor.Properties(currentMeteorProperties.timeY, currentMeteorProperties.speedX, DragonLevelMeteor.State.Down));
		}
		else
		{
			meteorPrefab.Create(mouthRoot.position, new DragonLevelMeteor.Properties(currentMeteorProperties.timeY, currentMeteorProperties.speedX, meteorState));
		}
	}

	private IEnumerator meteor_cr()
	{
		currentMeteorProperties = base.properties.CurrentState.meteor;
		char[] meteorPattern = currentMeteorProperties.pattern.GetRandom().ToCharArray();
		animator.SetTrigger("OnMeteor");
		animator.SetBool("Repeat", true);
		yield return animator.WaitForAnimationToStart(this, "MeteorStart");
		AudioManager.Play("level_dragon_left_dragon_meteor_start");
		emitAudioFromObject.Add("level_dragon_left_dragon_meteor_start");
		for (int i = 0; i < meteorPattern.Length; i++)
		{
			switch (meteorPattern[i])
			{
			default:
				meteorState = DragonLevelMeteor.State.Up;
				break;
			case 'D':
				meteorState = DragonLevelMeteor.State.Down;
				break;
			case 'B':
				meteorState = DragonLevelMeteor.State.Both;
				break;
			case 'F':
				meteorState = DragonLevelMeteor.State.Forward;
				break;
			}
			if (i >= meteorPattern.Length - 1)
			{
				animator.SetBool("Repeat", false);
			}
			yield return animator.WaitForAnimationToStart(this, "Meteor_Anticipation_Loop");
			AudioManager.Play("level_dragon_left_dragon_meteor_anticipation_loop");
			emitAudioFromObject.Add("level_dragon_left_dragon_meteor_anticipation_loop");
			yield return CupheadTime.WaitForSeconds(this, currentMeteorProperties.shotDelay);
			animator.SetTrigger("OnMeteor");
			AudioManager.Stop("level_dragon_left_dragon_meteor_anticipation_loop");
			yield return animator.WaitForAnimationToStart(this, "Meteor_Attack");
			AudioManager.Play("level_dragon_left_dragon_meteor_attack");
			yield return animator.WaitForAnimationToEnd(this, "Meteor_Attack");
		}
		yield return animator.WaitForAnimationToEnd(this, "Meteor_Attack_End");
		yield return CupheadTime.WaitForSeconds(this, currentMeteorProperties.hesitate);
		state = State.Idle;
	}

	public void Leave()
	{
		StartCoroutine(leave_cr());
	}

	private IEnumerator leave_cr()
	{
		while (state != State.Idle)
		{
			yield return null;
		}
		Vector2 endPos = base.transform.position;
		endPos.x += 500f;
		yield return StartCoroutine(tween_cr(base.transform, base.transform.position, endPos, EaseUtils.EaseType.easeInSine, 1.5f));
		yield return CupheadTime.WaitForSeconds(this, 1f);
		Vector2 dashEndPos = dash.position;
		dashEndPos.x = -1300f;
		StopWingSFX();
		AudioManager.Play("level_dragon_dash");
		yield return StartCoroutine(tween_cr(dash, dash.position, dashEndPos, EaseUtils.EaseType.linear, 1.3f));
		yield return CupheadTime.WaitForSeconds(this, 0.75f);
		leftSideDragon.StartIntro();
		Object.Destroy(dash.gameObject);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator tween_cr(Transform trans, Vector2 start, Vector2 end, EaseUtils.EaseType ease, float time)
	{
		float t = 0f;
		trans.position = start;
		while (t < time)
		{
			float val = EaseUtils.Ease(ease, 0f, 1f, t / time);
			trans.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		trans.position = end;
		yield return null;
	}
}
