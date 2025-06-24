using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBirdLevelSmallBird : LevelProperties.FlyingBird.Entity
{
	public enum State
	{
		Init = 0,
		Starting = 1,
		Right = 2,
		Left = 3,
		Shooting = 4,
		Dead = 5
	}

	public enum Direction
	{
		Right = -1,
		Left = 1
	}

	[SerializeField]
	private FlyingBirdLevelSmallBirdSprite sprite;

	private CollisionChild collisionChild;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	[Space(10f)]
	[SerializeField]
	private FlyingBirdLevelSmallBirdEgg eggPrefab;

	[Space(10f)]
	[SerializeField]
	private BasicProjectile bulletPrefab;

	[SerializeField]
	private Transform bulletRoot;

	private Transform aim;

	private Transform eggContainer;

	private bool SmallBirdDeadSFX;

	private List<FlyingBirdLevelSmallBirdEgg> eggs;

	public State state { get; private set; }

	public Direction direction { get; private set; }

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = sprite.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		collisionChild = sprite.GetComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnPlayerCollision;
		aim = new GameObject("Aim").transform;
		aim.SetParent(bulletRoot);
		aim.ResetLocalTransforms();
		base.gameObject.SetActive(false);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		PositionEggs();
	}

	public override void LevelInit(LevelProperties.FlyingBird properties)
	{
		base.LevelInit(properties);
		if (Level.Current.mode == Level.Mode.Easy)
		{
			properties.OnBossDeath += OnBossDeath;
		}
	}

	private void OnBossDeath()
	{
		if (Level.Current.mode == Level.Mode.Easy)
		{
			base.properties.OnBossDeath -= OnBossDeath;
		}
		base.properties.OnStateChange -= OnBossDeath;
		StopAllCoroutines();
		sprite.transform.ResetLocalTransforms();
		base.animator.Play("Death");
		AudioManager.Play("level_flyingbird_small_bird_death_cry");
		AudioManager.Stop("level_flyingbird_small_bird_rotating_eggs_loop");
		SmallBirdDeadSFX = true;
		foreach (FlyingBirdLevelSmallBirdEgg egg in eggs)
		{
			egg.Explode();
		}
		if (Level.Current.mode != Level.Mode.Easy)
		{
			sprite.GetComponent<LevelBossDeathExploder>().StartExplosion();
			StartCoroutine(leave_cr());
		}
	}

	private void OnPlayerCollision(GameObject hit, CollisionPhase phase)
	{
		if (state != State.Dead && damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state != State.Dead)
		{
			base.properties.DealDamage(info.damage);
		}
	}

	public void StartPattern(Vector2 pos)
	{
		base.properties.OnStateChange += OnBossDeath;
		if (state == State.Init)
		{
			state = State.Starting;
			base.transform.position = pos;
			base.gameObject.SetActive(true);
			StartCoroutine(float_cr());
			StartCoroutine(start_cr());
		}
	}

	private void TurnComplete()
	{
	}

	private void PositionEggs()
	{
		if (eggs == null || eggs.Count < 1)
		{
			return;
		}
		foreach (FlyingBirdLevelSmallBirdEgg egg in eggs)
		{
			egg.transform.localPosition = Vector3.zero;
		}
	}

	private IEnumerator start_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		StartCoroutine(eggs_cr());
		yield return CupheadTime.WaitForSeconds(this, 1f);
		StartCoroutine(moveX_cr());
		StartCoroutine(moveY_cr());
		StartCoroutine(shooting_cr());
	}

	private IEnumerator float_cr()
	{
		yield return sprite.TweenLocalPositionY(0f, 10f, 1f, EaseUtils.EaseType.easeOutSine);
		while (true)
		{
			yield return sprite.TweenLocalPositionY(10f, -10f, 1f, EaseUtils.EaseType.easeInOutSine);
			yield return sprite.TweenLocalPositionY(-10f, 10f, 1f, EaseUtils.EaseType.easeInOutSine);
		}
	}

	private IEnumerator leave_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
		float end = ((!(base.transform.position.x > 0f)) ? Level.Current.Left : Level.Current.Right);
		StartCoroutine(tweenX_cr(end: end + renderer.bounds.size.x / 2f * Mathf.Sign(base.transform.position.x), start: base.transform.position.x, time: base.properties.CurrentState.smallBird.leaveTime, ease: EaseUtils.EaseType.easeInOutSine));
		sprite.GetComponent<Collider2D>().enabled = false;
		while (state != State.Dead)
		{
			yield return null;
		}
		Object.Destroy(sprite.GetComponent<LevelBossDeathExploder>());
		base.gameObject.SetActive(false);
	}

	private void ShootProjectile()
	{
		aim.LookAt2D(PlayerManager.Current.center);
		bulletPrefab.Create(bulletRoot.position, aim.eulerAngles.z + 180f, 0f - base.properties.CurrentState.smallBird.shotSpeed).SetParryable(true);
	}

	private IEnumerator shooting_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.smallBird.shotDelay);
			AbstractPlayerController target = PlayerManager.GetNext();
			if (direction == Direction.Left)
			{
				if (target.center.x > base.transform.position.x)
				{
					yield return Turn(Direction.Right);
				}
			}
			else if (target.center.x < base.transform.position.x)
			{
				yield return Turn(Direction.Left);
			}
			State lastState = state;
			state = State.Shooting;
			base.animator.SetTrigger("Shoot");
			base.animator.WaitForAnimationToEnd(this, "Shoot");
			state = lastState;
			yield return null;
		}
	}

	private Coroutine Turn(Direction d)
	{
		return StartCoroutine(turn_cr(d));
	}

	private IEnumerator turn_cr(Direction d)
	{
		if (direction != d)
		{
			sprite.transform.SetScale((float)d);
			direction = d;
			base.animator.Play("Turn");
			yield return base.animator.WaitForAnimationToEnd(this, "Turn");
		}
	}

	private IEnumerator eggs_cr()
	{
		int count = base.properties.CurrentState.smallBird.eggCount;
		eggs = new List<FlyingBirdLevelSmallBirdEgg>();
		eggContainer = new GameObject("Eggs").transform;
		eggContainer.SetParent(base.transform);
		eggContainer.ResetLocalTransforms();
		eggContainer.SetLocalPosition(null, -65f);
		for (int i = 0; i < count; i++)
		{
			float value = (float)i / (float)count * 360f;
			FlyingBirdLevelSmallBirdEgg flyingBirdLevelSmallBirdEgg = eggPrefab.InstantiatePrefab<FlyingBirdLevelSmallBirdEgg>();
			flyingBirdLevelSmallBirdEgg.SetParent(eggContainer, base.properties);
			flyingBirdLevelSmallBirdEgg.container.SetEulerAngles(0f, 0f, value);
			eggs.Add(flyingBirdLevelSmallBirdEgg);
		}
		AudioManager.PlayLoop("level_flyingbird_small_bird_rotating_eggs_loop");
		emitAudioFromObject.Add("level_flyingbird_small_bird_rotating_eggs_loop");
		while (true)
		{
			eggContainer.AddLocalEulerAngles(0f, 0f, base.properties.CurrentState.smallBird.eggRotationSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
	}

	private IEnumerator moveX_cr()
	{
		direction = Direction.Left;
		while (true)
		{
			float minX = base.properties.CurrentState.smallBird.minX;
			state = State.Left;
			yield return StartCoroutine(tweenX_cr(base.transform.position.x, minX, base.properties.CurrentState.smallBird.timeX, EaseUtils.EaseType.easeInOutSine));
			yield return Turn(Direction.Right);
			state = State.Right;
			yield return StartCoroutine(tweenX_cr(minX, 520f, base.properties.CurrentState.smallBird.timeX, EaseUtils.EaseType.easeInOutSine));
			yield return Turn(Direction.Left);
		}
	}

	private IEnumerator tweenX_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		base.transform.SetPosition(start);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetPosition(x: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		state = State.Dead;
		base.transform.SetPosition(end);
		yield return null;
	}

	private IEnumerator moveY_cr()
	{
		if (Rand.Bool())
		{
			float currentDist = 260f - base.transform.position.y;
			float normalDist = -490f;
			float time = base.properties.CurrentState.smallBird.timeY - currentDist / normalDist;
			yield return StartCoroutine(tweenY_cr(base.transform.position.y, 260f, base.properties.CurrentState.smallBird.timeY, EaseUtils.EaseType.easeInOutSine));
		}
		else
		{
			float currentDist2 = -230f - base.transform.position.y;
			float normalDist2 = 490f;
			yield return StartCoroutine(tweenY_cr(time: base.properties.CurrentState.smallBird.timeY - currentDist2 / normalDist2, start: base.transform.position.y, end: -230f, ease: EaseUtils.EaseType.easeInOutSine));
			yield return StartCoroutine(tweenY_cr(-230f, 260f, base.properties.CurrentState.smallBird.timeY, EaseUtils.EaseType.easeInOutSine));
		}
		while (true)
		{
			yield return StartCoroutine(tweenY_cr(260f, -230f, base.properties.CurrentState.smallBird.timeY, EaseUtils.EaseType.easeInOutSine));
			yield return StartCoroutine(tweenY_cr(-230f, 260f, base.properties.CurrentState.smallBird.timeY, EaseUtils.EaseType.easeInOutSine));
		}
	}

	private IEnumerator tweenY_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		base.transform.SetPosition(null, start);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetPosition(y: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetPosition(null, end);
		yield return null;
	}

	private void SmallLaserShootSFX()
	{
		AudioManager.Play("level_flyingbird_small_bird_shoot");
		emitAudioFromObject.Add("level_flyingbird_small_bird_shoot");
	}
}
