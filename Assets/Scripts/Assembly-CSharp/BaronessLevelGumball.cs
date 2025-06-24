using System.Collections;
using UnityEngine;

public class BaronessLevelGumball : BaronessLevelMiniBossBase
{
	public enum State
	{
		On = 0,
		Off = 1,
		Dying = 2
	}

	[SerializeField]
	private BaronessLevelGumballProjectile[] projectilePrefabs;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private SpriteRenderer lid;

	[SerializeField]
	private SpriteRenderer legs;

	[SerializeField]
	private CollisionChild headCollider;

	[SerializeField]
	private GameObject headSpark;

	[SerializeField]
	private Transform feetDust;

	private LevelProperties.Baroness.Gumball properties;

	private AbstractPlayerController player;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private DamageReceiver damageReceiverChild;

	private float speed;

	private float health;

	private float offTime;

	private float onTime;

	private float originalSpeed;

	private bool movingLeft;

	private bool slowDown;

	private int layerNum = 3;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		fadeTime = 0.6f;
		isDying = false;
		movingLeft = true;
		RegisterCollisionChild(headCollider);
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiverChild = headCollider.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageReceiverChild.OnDamageTaken += OnDamageTaken;
		headCollider.OnPlayerCollision += OnCollisionPlayer;
		headCollider.OnPlayerProjectileCollision += OnCollisionPlayerProjectile;
	}

	protected override void Start()
	{
		base.Start();
		legs.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		legs.GetComponent<SpriteRenderer>().sortingOrder = 130;
		lid.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		lid.GetComponent<SpriteRenderer>().sortingOrder = 140;
		AudioManager.PlayLoop("level_baroness_gumball_feet_loop");
		emitAudioFromObject.Add("level_baroness_gumball_feet_loop");
	}

	public void Init(LevelProperties.Baroness.Gumball properties, AbstractPlayerController player, Vector2 pos, float aspeed, float health)
	{
		this.properties = properties;
		this.player = player;
		speed = aspeed;
		this.health = health;
		base.transform.position = pos;
		originalSpeed = speed;
		offTime = properties.gumballAttackDurationOffRange;
		StartCoroutine(leaving_castle_cr());
		StartCoroutine(switch_child_cr());
		StartCoroutine(gumball_off_timer_cr());
		StartCoroutine(move_cr());
	}

	protected virtual IEnumerator leaving_castle_cr()
	{
		float t = 0f;
		float offTime = 0.22f;
		while (t < offTime)
		{
			lid.GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<SpriteRenderer>().enabled = false;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		lid.GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		yield return null;
	}

	private IEnumerator switch_child_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		legs.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		lid.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		lid.GetComponent<SpriteRenderer>().sortingOrder = 252;
		legs.GetComponent<SpriteRenderer>().sortingOrder = 251;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		health -= info.damage;
		if (health < 0f && state != State.Dying)
		{
			state = State.Dying;
			StartCoroutine(death_cr());
		}
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		bool endedLoop = false;
		bool movingRight = false;
		float time = properties.gumballMovementSpeed;
		float end = 0f;
		float t = 0f;
		while (true)
		{
			float start = base.transform.position.x;
			end = ((!movingRight) ? (-640f + properties.offsetX.min) : (640f - properties.offsetX.max));
			while (t < time)
			{
				float val = t / time;
				base.transform.SetPosition(EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, val));
				if (val > 0.8f && !endedLoop)
				{
					if (isDying && !movingRight)
					{
						break;
					}
					if (state == State.On)
					{
						headSpark.SetActive(false);
					}
					base.animator.SetBool("Turn", true);
					base.animator.Play("Run_Legs");
					endedLoop = true;
				}
				t += CupheadTime.FixedDelta;
				yield return wait;
			}
			if (isDying)
			{
				break;
			}
			endedLoop = false;
			t = 0f;
			base.transform.SetPosition(end);
			movingRight = !movingRight;
			yield return wait;
		}
		while (base.transform.position.x > -940f)
		{
			base.transform.AddPosition((0f - properties.gumballDeathSpeed) * CupheadTime.FixedDelta);
			yield return wait;
		}
		AudioManager.Stop("level_baroness_gumball_feet_loop");
		Die();
	}

	private void Switch()
	{
		base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
		feetDust.SetScale(0f - feetDust.localScale.x, 1f, 1f);
		base.animator.SetBool("Turn", false);
		if (state == State.On)
		{
			headSpark.SetActive(true);
		}
	}

	private IEnumerator on_cr()
	{
		float rateTime = 0f;
		float attackTime = 0f;
		float attackDuration = properties.gumballAttackDurationOnRange.RandomFloat();
		base.animator.SetBool("Open", true);
		yield return base.animator.WaitForAnimationToStart(this, "Run_Open_Trans");
		AudioManager.PlayLoop("level_baroness_gumball_shoot_loop");
		emitAudioFromObject.Add("level_baroness_gumball_shoot_loop");
		headSpark.SetActive(true);
		state = State.On;
		while (attackTime < attackDuration && !isDying)
		{
			attackTime += (float)CupheadTime.Delta;
			if (rateTime > properties.rateOfFire)
			{
				fireProjectiles();
				rateTime = 0f;
			}
			else
			{
				rateTime += (float)CupheadTime.Delta;
			}
			yield return null;
		}
		base.animator.SetBool("Open", false);
		yield return new WaitForEndOfFrame();
		StartCoroutine(gumball_off_timer_cr());
		yield return null;
	}

	private void fireProjectiles()
	{
		Vector2 zero = Vector2.zero;
		float num = ((!movingLeft) ? 200 : (-200));
		zero.y = properties.velocityY.RandomFloat();
		zero.x = properties.velocityX.RandomFloat() + num;
		projectilePrefabs[Random.Range(0, projectilePrefabs.Length - 1)].Create(projectileRoot.position, zero, properties, properties.gravity);
	}

	private IEnumerator gumball_off_timer_cr()
	{
		headSpark.SetActive(false);
		AudioManager.Stop("level_baroness_gumball_shoot_loop");
		state = State.Off;
		offTime = properties.gumballAttackDurationOffRange.RandomFloat();
		yield return CupheadTime.WaitForSeconds(this, offTime);
		StartCoroutine(on_cr());
		yield return null;
	}

	private IEnumerator death_cr()
	{
		StartExplosions();
		headCollider.GetComponent<Collider2D>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		isDying = true;
		base.animator.Play("Run_Death");
		base.animator.SetTrigger("Death");
		yield return null;
	}

	private void SoundGumballLidOpen()
	{
		AudioManager.Play("level_baroness_gumball_lid_open");
		emitAudioFromObject.Add("level_baroness_gumball_lid_open");
	}

	private void SoundGumballLidClose()
	{
		AudioManager.Play("level_baroness_gumball_lid_close");
		emitAudioFromObject.Add("level_baroness_gumball_lid_close");
	}
}
