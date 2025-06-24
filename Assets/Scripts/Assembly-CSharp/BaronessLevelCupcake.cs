using System.Collections;
using UnityEngine;

public class BaronessLevelCupcake : BaronessLevelMiniBossBase
{
	public enum State
	{
		Moving = 0,
		Dying = 1
	}

	private LevelProperties.Baroness.Cupcake properties;

	private DamageDealer damageDealer;

	[SerializeField]
	private Effect splashPrefab;

	[SerializeField]
	private Transform launchOffset;

	[SerializeField]
	private BasicProjectile cupcakeProjectile;

	[SerializeField]
	private Transform collisionChild;

	[SerializeField]
	private Transform deathRoot;

	private float health;

	private float ySpeedUp = 1800f;

	private float ySpeedDown = 2500f;

	private float xSpeed;

	private float changeXSpeed;

	private float offset = 250f;

	private float directionX;

	private bool isGoingDown;

	private bool isGoingRight;

	private int patternIndex;

	private int mainPatternIndex;

	private int numOfBullets = 2;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		isGoingDown = false;
		isGoingRight = false;
		xSpeed = changeXSpeed;
		patternIndex = 0;
		fadeTime = 0.3f;
		damageDealer = DamageDealer.NewEnemy();
		collisionChild.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		collisionChild.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(LevelProperties.Baroness.Cupcake properties, Vector2 pos, float health)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.health = health;
		state = State.Moving;
		StartCoroutine(select_x_speed_cr());
		StartCoroutine(moving_cr());
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		health -= info.damage;
		if (health < 0f && state != State.Dying)
		{
			state = State.Dying;
			StartDeath();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override float hitPauseCoefficient()
	{
		return (!collisionChild.GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private void SetLaunchOffset()
	{
		base.transform.position = launchOffset.transform.position;
	}

	private IEnumerator moving_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		bool curlAni = false;
		bool flatAni = false;
		while (true)
		{
			if (!isGoingDown)
			{
				if (flatAni)
				{
					StartCoroutine(select_x_speed_cr());
					yield return CupheadTime.WaitForSeconds(this, properties.hold);
					base.animator.SetTrigger("Continue");
					yield return base.animator.WaitForAnimationToEnd(this, "Slam_Start");
					flatAni = false;
				}
				GoingUp();
				curlAni = true;
			}
			else
			{
				if (curlAni)
				{
					yield return base.animator.WaitForAnimationToEnd(this, "Slam_Curl");
					curlAni = false;
				}
				GoingDown();
				flatAni = true;
			}
			yield return wait;
		}
	}

	private void GoingUp()
	{
		if (base.transform.position.y < 360f - offset)
		{
			Vector3 position = base.transform.position;
			if (!isGoingRight)
			{
				position.x -= changeXSpeed * CupheadTime.FixedDelta * hitPauseCoefficient();
			}
			else
			{
				position.x += changeXSpeed * CupheadTime.FixedDelta * hitPauseCoefficient();
			}
			position.y += ySpeedUp * CupheadTime.FixedDelta;
			base.transform.position = position;
			BoundaryCheck();
		}
		else
		{
			isGoingDown = true;
		}
	}

	private void GoingDown()
	{
		if (base.transform.position.y > (float)Level.Current.Ground + 120f)
		{
			if (xSpeed == 0f)
			{
				xSpeed = changeXSpeed;
			}
			Vector3 position = base.transform.position;
			position.y -= ySpeedDown * CupheadTime.FixedDelta * hitPauseCoefficient();
			base.transform.position = position;
		}
		else
		{
			Vector3 position2 = base.transform.position;
			position2.y = (float)Level.Current.Ground + 120f;
			base.transform.position = position2;
			isGoingDown = false;
		}
	}

	private void BoundaryCheck()
	{
		if (base.transform.position.x < -540f && !isGoingRight)
		{
			xSpeed = 0f;
			base.transform.SetScale(-1f, 1f, 1f);
			isGoingRight = true;
		}
		else if (base.transform.position.x > 540f && isGoingRight)
		{
			xSpeed = 0f;
			base.transform.SetScale(1f, 1f, 1f);
			isGoingRight = false;
		}
		else
		{
			xSpeed = changeXSpeed;
		}
	}

	private IEnumerator select_x_speed_cr()
	{
		string[] pattern = properties.XSpeedString[0].Split(',');
		float.TryParse(pattern[patternIndex], out changeXSpeed);
		if (patternIndex < pattern.Length - 1)
		{
			patternIndex++;
		}
		else
		{
			patternIndex = 0;
		}
		yield return null;
	}

	private void FireBullets()
	{
		if (properties.projectileOn)
		{
			StartSplashes();
		}
	}

	private void StartSplashes()
	{
		StartCoroutine(splash_cr(true, deathRoot.transform.position.x));
		StartCoroutine(splash_cr(false, deathRoot.transform.position.x));
	}

	private IEnumerator splash_cr(bool onLeft, float posX)
	{
		float originalOffset = ((!onLeft) ? (0f - properties.splashOriginalOffset) : properties.splashOriginalOffset);
		float offset = ((!onLeft) ? (0f - properties.splashOffset) : properties.splashOffset);
		float delay = 0.4f;
		float scale = 1f;
		int value = 0;
		for (int i = 0; i < 3; i++)
		{
			if (onLeft)
			{
				value = i;
			}
			else
			{
				switch (i)
				{
				case 0:
					value = 2;
					break;
				case 1:
					value = 0;
					break;
				default:
					value = 1;
					break;
				}
			}
			Effect splash = splashPrefab.Create(new Vector2(posX + originalOffset + offset * (float)i, Level.Current.Ground));
			splash.animator.SetInteger("SplashType", value);
			Transform obj = splash.transform;
			float value2 = ((!onLeft) ? splash.transform.localScale.x : (0f - splash.transform.localScale.x));
			obj.SetScale(value2);
			yield return CupheadTime.WaitForSeconds(this, delay);
		}
	}

	private void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(death_cr());
	}

	private IEnumerator death_cr()
	{
		StartExplosions();
		collisionChild.GetComponent<Collider2D>().enabled = false;
		base.transform.position = deathRoot.transform.position;
		isDying = true;
		base.animator.SetTrigger("Death");
		yield return base.animator.WaitForAnimationToEnd(this, "Cupcake_Death");
		EndExplosions();
		Die();
	}

	private void SoundCupcakeJump()
	{
		AudioManager.Play("level_baroness_cupcake_jump");
		emitAudioFromObject.Add("level_baroness_cupcake_jump");
	}

	private void SoundCupcakeLand()
	{
		AudioManager.Play("level_baroness_cupcake_land");
		emitAudioFromObject.Add("level_baroness_cupcake_land");
	}

	private void SoundCupcakeSpin()
	{
		AudioManager.Play("level_baroness_cupcake_spin");
		emitAudioFromObject.Add("level_baroness_cupcake_spin");
	}
}
