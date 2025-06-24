using System;
using System.Collections;
using UnityEngine;

public class MouseLevelCanMouse : LevelProperties.Mouse.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Dash = 2,
		CherryBomb = 3,
		Catapult = 4,
		RomanCandle = 5
	}

	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const int MOUSE_LAYER = 0;

	private const int CAN_LAYER = 1;

	private const float MOVE_START_X = 500f;

	private const float DASH_END_X = 450f;

	[Header("Cannon")]
	[SerializeField]
	private Transform cherryBombRoot;

	[SerializeField]
	private MouseLevelCherryBombProjectile cherryBombPrefab;

	[Header("Catapult")]
	[SerializeField]
	private Transform catapult;

	[SerializeField]
	private MouseLevelCanCatapultProjectile catapultProjectilePrefab;

	[SerializeField]
	private Transform catapultRoot;

	[Header("Roman Candle")]
	[SerializeField]
	private Transform romanCandleRoot;

	[SerializeField]
	private MouseLevelRomanCandleProjectile romanCandlePrefab;

	[Header("Wheels")]
	[SerializeField]
	private SpriteRenderer wheelRenderer;

	[SerializeField]
	private Sprite[] wheelSprites;

	[SerializeField]
	private MouseLevelBrokenCanMouse brokenCan;

	[SerializeField]
	private MouseLevelSawBladeManager sawBlades;

	[SerializeField]
	private MouseLevelCatPeeking catPeeking;

	[Header("Springs")]
	[SerializeField]
	private MouseLevelSpring[] springs;

	private Direction direction;

	private new Animator animator;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private bool moving;

	private bool peeking;

	private bool overrideMove;

	private bool exitAfterMoveBack;

	private float overrideMoveX;

	private bool dash;

	private const float FLIP_OFFSET = 40f;

	private Action onStartPlatform;

	private Action onTransitionComplete;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		SetWheels(false);
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (!peeking && base.properties.CurrentHealth < base.properties.TotalHealth * catPeeking.Peek1Threshold)
		{
			catPeeking.StartPeeking();
			peeking = true;
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

	public override void LevelInit(LevelProperties.Mouse properties)
	{
		base.LevelInit(properties);
		Level.Current.OnIntroEvent += OnLevelStart;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	private float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	public void OnLevelStart()
	{
		StartCoroutine(wheels_cr());
		StartCoroutine(levelStart_cr());
	}

	private IEnumerator levelStart_cr()
	{
		animator.Play("Intro", 0);
		yield return animator.WaitForAnimationToEnd(this, "Intro", 0);
		animator.Play("Intro_Down", 1);
		state = State.Idle;
	}

	private IEnumerator moveBack_cr()
	{
		exitAfterMoveBack = true;
		while (exitAfterMoveBack)
		{
			yield return null;
		}
	}

	private IEnumerator moveToX_cr(float x)
	{
		overrideMove = true;
		overrideMoveX = x;
		if (!moving)
		{
			StartCoroutine(move_cr());
		}
		while (overrideMove)
		{
			yield return null;
		}
	}

	private void SetWheels(bool b)
	{
		wheelRenderer.enabled = b;
	}

	private IEnumerator move_cr()
	{
		animator.Play("Move", 1);
		SetWheels(true);
		moving = true;
		bool movingBack = false;
		while (true)
		{
			LevelProperties.Mouse.CanMove p = base.properties.CurrentState.canMove;
			Vector2 end = new Vector2(500f * base.transform.localScale.x, base.transform.position.y);
			bool overriden = false;
			if (overrideMove)
			{
				end.x = overrideMoveX;
				overriden = true;
			}
			else if (!movingBack)
			{
				end.x -= p.maxXPositionRange.RandomFloat() * base.transform.localScale.x;
			}
			yield return StartCoroutine(tween_cr(time: Mathf.Abs(base.transform.position.x - end.x) / p.speed, trans: base.transform, start: base.transform.position, end: end, ease: EaseUtils.EaseType.easeInOutSine));
			yield return CupheadTime.WaitForSeconds(this, p.stopTime);
			if (overrideMove && overriden)
			{
				overrideMove = false;
				break;
			}
			if (movingBack && exitAfterMoveBack)
			{
				exitAfterMoveBack = false;
				break;
			}
			movingBack = !movingBack;
		}
		SetWheels(false);
		animator.Play("IdleDown", 1);
		moving = false;
	}

	private IEnumerator wheels_cr()
	{
		int currentFrame = 0;
		Vector2 lastPos = base.transform.position;
		int direction = 1;
		while (true)
		{
			float distance = 0f;
			while (distance < 6f)
			{
				float speed = lastPos.x - base.transform.position.x;
				distance += Mathf.Abs(speed);
				direction = ((base.transform.localScale.x > 0f) ? ((!(speed < 0f)) ? 1 : (-1)) : ((speed < 0f) ? 1 : (-1)));
				lastPos = base.transform.position;
				yield return null;
			}
			currentFrame += direction;
			currentFrame = (int)Mathf.Repeat(currentFrame, wheelSprites.Length);
			wheelRenderer.sprite = wheelSprites[currentFrame];
			yield return null;
		}
	}

	public void StartDash()
	{
		state = State.Dash;
		StartCoroutine(dash_cr());
	}

	private void StartDashMove()
	{
		dash = true;
		animator.SetTrigger("CanContinue");
		AudioManager.Play("level_mouse_can_dash_start");
		emitAudioFromObject.Add("level_mouse_can_dash_start");
	}

	private void DashFlipX()
	{
		base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
		if (base.transform.localScale.x < 0f)
		{
			direction = Direction.Right;
			base.transform.AddPosition(40f);
		}
		else
		{
			direction = Direction.Left;
			base.transform.AddPosition(-40f);
		}
		animator.SetTrigger("CanContinue");
	}

	private IEnumerator dash_cr()
	{
		LevelProperties.Mouse.CanDash dashProperties = base.properties.CurrentState.canDash;
		for (int i = 0; i < springs.Length; i++)
		{
			Vector2 velocity = new Vector2(dashProperties.springVelocityX[i].RandomFloat(), dashProperties.springVelocityY[i].RandomFloat());
			if (direction == Direction.Right)
			{
				velocity.x *= -1f;
			}
			springs[i].LaunchSpring(new Vector2(base.transform.position.x, base.transform.position.y + 200f), velocity, dashProperties.springGravity);
			AudioManager.Play("level_mouse_can_springboard_shoot");
			emitAudioFromObject.Add("level_mouse_can_springboard_shoot");
			StartCoroutine(timedAudioMouseSnarky_cr());
		}
		if (moving)
		{
			yield return StartCoroutine(moveBack_cr());
		}
		Vector2 start = base.transform.position;
		Vector2 end = new Vector2(-450f * base.transform.localScale.x, base.transform.position.y);
		animator.Play("Dash", 1);
		AudioManager.PlayLoop("level_mouse_can_dash_loop");
		dash = false;
		while (!dash)
		{
			yield return null;
		}
		yield return StartCoroutine(tween_cr(base.transform, start, end, EaseUtils.EaseType.easeInSine, dashProperties.time));
		animator.SetTrigger("CanContinue");
		AudioManager.Stop("level_mouse_can_dash_loop");
		AudioManager.Play("level_mouse_can_dash_stop");
		emitAudioFromObject.Add("level_mouse_can_dash_stop");
		yield return animator.WaitForAnimationToEnd(this, "Dash_End", 1);
		yield return CupheadTime.WaitForSeconds(this, dashProperties.hesitate);
		state = State.Idle;
	}

	public void StartCherryBomb()
	{
		state = State.CherryBomb;
		StartCoroutine(cherryBomb_cr());
		StartCoroutine(timedAudioMouseSnarky_cr());
		if (!moving)
		{
			StartCoroutine(move_cr());
		}
	}

	private void FireCherryBomb()
	{
		animator.SetTrigger("Shoot");
		Vector2 velocity = new Vector2((float)base.properties.CurrentState.canCherryBomb.xVelocity * base.transform.localScale.x, base.properties.CurrentState.canCherryBomb.yVelocity);
		cherryBombPrefab.Create(cherryBombRoot.position, velocity, base.properties.CurrentState.canCherryBomb.gravity, base.properties.CurrentState.canCherryBomb.childSpeed);
	}

	private IEnumerator cherryBomb_cr()
	{
		animator.ResetTrigger("Continue");
		animator.ResetTrigger("Shoot");
		LevelProperties.Mouse.CanCherryBomb properties = base.properties.CurrentState.canCherryBomb;
		KeyValue[] pattern = KeyValue.ListFromString(properties.patterns[UnityEngine.Random.Range(0, properties.patterns.Length)], new char[2] { 'P', 'D' });
		animator.Play("Cannon_Start", 0);
		yield return animator.WaitForAnimationToEnd(this, "Cannon_Start", 0);
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i].key == "P")
			{
				for (int p = 0; (float)p < pattern[i].value; p++)
				{
					yield return CupheadTime.WaitForSeconds(this, properties.delay);
					FireCherryBomb();
				}
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, pattern[i].value);
			}
			yield return null;
		}
		animator.SetTrigger("Continue");
		yield return animator.WaitForAnimationToStart(this, "Idle_Down", 0);
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	public void StartCatapult()
	{
		state = State.Catapult;
		StartCoroutine(catapult_cr());
		StartCoroutine(timedAudioMouseSnarky_cr());
		if (!moving)
		{
			StartCoroutine(move_cr());
		}
	}

	private void FireCatapult()
	{
		LevelProperties.Mouse.CanCatapult canCatapult = base.properties.CurrentState.canCatapult;
		char[] array = canCatapult.patterns.GetRandom().ToLower().ToCharArray();
		float num = ((direction != Direction.Right) ? 165 : (-45));
		if (array.Length <= 1)
		{
			catapultProjectilePrefab.CreateFromPrefab(catapultRoot.position, num + canCatapult.angleOffset, canCatapult.projectileSpeed, array[0]);
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			float rotation = num + canCatapult.spreadAngle / (float)(array.Length - 1) * (float)i;
			catapultProjectilePrefab.CreateFromPrefab(catapultRoot.position, rotation, canCatapult.projectileSpeed, array[i]);
		}
	}

	private IEnumerator catapult_cr()
	{
		LevelProperties.Mouse.CanCatapult properties = base.properties.CurrentState.canCatapult;
		animator.ResetTrigger("Continue");
		animator.ResetTrigger("Shoot");
		animator.Play("Catapult_Idle", 0);
		yield return StartCoroutine(tweenCatapultY_cr(-280f, 0f, properties.timeIn, EaseUtils.EaseType.easeOutSine));
		yield return CupheadTime.WaitForSeconds(this, properties.pumpDelay);
		for (int i = 0; i < properties.count; i++)
		{
			animator.SetTrigger("Continue");
			SoundMouseCatapultGlug();
			yield return animator.WaitForAnimationToEnd(this, "Catapult_Pump", 0);
			yield return CupheadTime.WaitForSeconds(this, properties.pumpDelay);
			animator.SetTrigger("Shoot");
			yield return animator.WaitForAnimationToEnd(this, "Catapult_Shoot", 0);
			yield return CupheadTime.WaitForSeconds(this, properties.repeatDelay);
		}
		yield return StartCoroutine(tweenCatapultY_cr(0f, -280f, properties.timeOut, EaseUtils.EaseType.easeOutSine));
		animator.Play("Idle_Down", 0);
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	private IEnumerator tweenCatapultY_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		catapult.transform.SetLocalPosition(null, start);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(ease, start, end, t / time), transform: catapult.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		catapult.transform.SetLocalPosition(null, end);
		yield return null;
	}

	public void StartRomanCandle()
	{
		state = State.RomanCandle;
		StartCoroutine(romanCandle_cr());
		StartCoroutine(timedAudioMouseSnarky_cr());
		if (!moving)
		{
			StartCoroutine(move_cr());
		}
	}

	private void FireRomanCandle()
	{
		romanCandlePrefab.Create(romanCandleRoot.position, (base.transform.localScale.x > 0f) ? 180 : 0, base.properties.CurrentState.canRomanCandle.speed, base.properties.CurrentState.canRomanCandle.speed, base.properties.CurrentState.canRomanCandle.rotationSpeed, 100f, base.properties.CurrentState.canRomanCandle.timeBeforeHoming, PlayerManager.GetNext());
	}

	private IEnumerator romanCandle_cr()
	{
		LevelProperties.Mouse.CanRomanCandle properties = base.properties.CurrentState.canRomanCandle;
		animator.ResetTrigger("Continue");
		animator.ResetTrigger("Shoot");
		for (int i = 0; i < properties.count.RandomInt(); i++)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.repeatDelay);
			animator.Play("Roman_Candle", 0);
			yield return animator.WaitForAnimationToEnd(this, "Roman_Candle", 0);
		}
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	public void Explode(Action onStartPlatform, Action onTransitionComplete)
	{
		this.onStartPlatform = onStartPlatform;
		this.onTransitionComplete = onTransitionComplete;
		StartCoroutine(explode_cr());
	}

	private IEnumerator explode_cr()
	{
		while (state != State.Idle)
		{
			yield return null;
		}
		yield return StartCoroutine(moveToX_cr(0f));
		animator.Play("Explode", 1);
	}

	private void OnExplodedAnim()
	{
		onStartPlatform();
		SetWheels(false);
		if (brokenCan.state != MouseLevelBrokenCanMouse.State.Dying)
		{
			sawBlades.Begin(base.properties);
			catPeeking.IsPhase2 = true;
		}
	}

	private void SpawnBrokenCan()
	{
		brokenCan.StartPattern(base.transform);
		onTransitionComplete();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator tween_cr(Transform trans, Vector2 start, Vector2 end, EaseUtils.EaseType ease, float time)
	{
		float t = 0f;
		trans.position = start;
		while (t < time)
		{
			float val = EaseUtils.Ease(ease, 0f, 1f, t / time);
			trans.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta * hitPauseCoefficient();
			yield return null;
		}
		trans.position = end;
		yield return null;
	}

	private void SoundMouseCanIntro()
	{
		AudioManager.Play("level_mouse_can_intro");
		emitAudioFromObject.Add("level_mouse_can_intro");
	}

	private void SoundMouseCannonShoot()
	{
		AudioManager.Play("level_mouse_can_cannon_shoot");
		emitAudioFromObject.Add("level_mouse_can_cannon_shoot");
	}

	private void SoundMouseCannonEnd()
	{
		AudioManager.Play("level_mouse_can_cannon_end");
		emitAudioFromObject.Add("level_mouse_can_cannon_end");
	}

	private void SoundMouseCatapultShoot()
	{
		AudioManager.Play("level_mouse_can_catapult_shoot");
		emitAudioFromObject.Add("level_mouse_can_catapult_shoot");
	}

	private void SoundMouseCatapultGlug()
	{
		AudioManager.Play("level_mouse_can_catapult_glug");
		emitAudioFromObject.Add("level_mouse_can_catapult_glug");
	}

	private void SoundMouseCanDashStart()
	{
		AudioManager.Play("level_mouse_can_dash_start");
		emitAudioFromObject.Add("level_mouse_can_dash_start");
	}

	private void SoundMouseCanDashLoop()
	{
		AudioManager.PlayLoop("level_mouse_can_dash_loop");
		emitAudioFromObject.Add("level_mouse_can_dash_loop");
	}

	private void SoundMouseCanDashStop()
	{
		AudioManager.PlayLoop("level_mouse_can_dash_stop");
		emitAudioFromObject.Add("level_mouse_can_dash_stop");
	}

	private void SoundMouseCanDashEndAnim()
	{
		AudioManager.Play("level_mouse_can_dash_end");
		emitAudioFromObject.Add("level_mouse_can_dash_end");
	}

	private void SoundMouseCanExplode()
	{
		AudioManager.Play("level_mouse_can_explode");
	}

	private void SoundMouseCanExplodePre()
	{
		AudioManager.Play("level_mouse_can_explode_pre");
	}

	private void SoundMouseCanRomanCandle()
	{
		AudioManager.Play("level_mouse_can_roman_candle");
		emitAudioFromObject.Add("level_mouse_can_roman_candle");
	}

	private void SoundMouseChargeVoice()
	{
		AudioManager.Play("level_mouse_charge_voice");
		emitAudioFromObject.Add("level_mouse_charge_voice");
	}

	private IEnumerator timedAudioMouseSnarky_cr()
	{
		yield return new WaitForSeconds(1f);
		AudioManager.Play("level_mouse_snarky_voice");
	}
}
