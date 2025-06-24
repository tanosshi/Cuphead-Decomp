using System.Collections;
using UnityEngine;

public class ArcadePlayerAnimationController : AbstractArcadePlayerComponent
{
	public enum Booleans
	{
		Dashing = 0,
		Shooting = 1,
		Grounded = 2,
		Turning = 3,
		Intro = 4,
		Dead = 5,
		NearLanding = 6,
		NearDashEnd = 7
	}

	public enum Integers
	{
		MoveX = 0,
		MoveY = 1,
		LookX = 2,
		LookY = 3,
		ArmVariant = 4
	}

	public enum Triggers
	{
		OnJump = 0,
		OnGround = 1,
		OnParry = 2,
		OnWin = 3,
		OnTurn = 4,
		OnFire = 5
	}

	[SerializeField]
	private GameObject cuphead;

	[SerializeField]
	private GameObject mugman;

	[SerializeField]
	private GameObject cupheadArm;

	[SerializeField]
	private GameObject mugmanArm;

	[Space(10f)]
	[SerializeField]
	private Transform runDustRoot;

	[Space(10f)]
	[SerializeField]
	private Effect dashEffect;

	[SerializeField]
	private Effect groundedEffect;

	[SerializeField]
	private Effect hitEffect;

	[SerializeField]
	private Effect runEffect;

	private SpriteRenderer renderer;

	private SpriteRenderer armRenderer;

	private bool hitAnimation;

	private bool super;

	private bool shooting;

	private bool fired;

	private float timeSinceStoppedShooting = 100f;

	private const float STOP_SHOOTING_DELAY = 0.0833f;

	private const float JUMP_END_ANIMATION_TIME = 0.15f;

	private const float DASH_END_ANIMATION_TIME = 0.15f;

	private const float DASH_END_AIR_ANIMATION_TIME = 13f / 120f;

	private IEnumerator colorCoroutine;

	private bool Flashing
	{
		get
		{
			return base.player.damageReceiver.state == PlayerDamageReceiver.State.Invulnerable;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		SetSprites(base.player.id == PlayerId.PlayerOne);
	}

	protected override void Start()
	{
		base.Start();
		base.basePlayer.OnPlayIntroEvent += PlayIntro;
		base.player.motor.OnParryEvent += OnParryStart;
		base.player.motor.OnGroundedEvent += OnGrounded;
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		base.player.weaponManager.OnExStart += OnEx;
		base.player.weaponManager.OnSuperStart += OnSuper;
		base.player.weaponManager.OnSuperEnd += OnSuperEnd;
		base.player.weaponManager.OnWeaponFire += OnShotFired;
		LevelPauseGUI.OnPauseEvent += OnGuiPause;
		LevelPauseGUI.OnPauseEvent += OnGuiUnpause;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StartCoroutine(flash_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.player.IsDead || !base.player.levelStarted)
		{
			return;
		}
		if (!hitAnimation && (int)base.player.motor.LookDirection.x != 0 && (int)base.player.motor.LookDirection.x != GetInt(Integers.LookX))
		{
			SetBool(Booleans.Turning, true);
		}
		else
		{
			SetBool(Booleans.Turning, false);
		}
		SetBool(Booleans.Grounded, base.player.motor.Grounded);
		SetBool(Booleans.NearLanding, base.player.motor.GetTimeUntilLand() <= 0.15f && !base.player.motor.Parrying);
		SetInt(Integers.MoveX, base.player.motor.LookDirection.x);
		SetInt(Integers.MoveY, base.player.motor.MoveDirection.y);
		SetInt(Integers.LookX, base.player.motor.TrueLookDirection.x);
		SetInt(Integers.LookY, base.player.motor.TrueLookDirection.y);
		SetBool(Booleans.Shooting, base.player.weaponManager.IsShooting);
		AnimatorStateInfo currentAnimatorStateInfo = base.animator.GetCurrentAnimatorStateInfo(0);
		bool flag = currentAnimatorStateInfo.IsName("Idle") || currentAnimatorStateInfo.IsName("Run");
		if (shooting)
		{
			timeSinceStoppedShooting = 0f;
		}
		else
		{
			timeSinceStoppedShooting += CupheadTime.Delta;
		}
		bool flag2 = false;
		if (fired && flag)
		{
			SetTrigger(Triggers.OnFire);
			SetInt(Integers.ArmVariant, (!Rand.Bool()) ? 1 : 0);
			flag2 = true;
		}
		fired = false;
		shooting = base.player.weaponManager.IsShooting;
		if (!shooting && !flag2)
		{
			ResetTrigger(Triggers.OnFire);
		}
		SetBool(Booleans.Dashing, base.player.motor.Dashing);
		SetBool(Booleans.NearDashEnd, base.player.motor.GetTimeUntilDashEnd() < ((!base.player.motor.Grounded) ? (13f / 120f) : 0.15f));
		if (!base.player.motor.Dashing)
		{
			if ((int)base.player.motor.LookDirection.x != 0)
			{
				base.transform.SetScale(base.player.motor.LookDirection.x);
			}
		}
		else
		{
			base.transform.SetScale(base.player.motor.DashDirection);
		}
		base.animator.Update(Time.deltaTime);
		for (int i = 0; i < 3; i++)
		{
			base.animator.Update(0f);
		}
	}

	public override void OnPause()
	{
		base.OnPause();
		SetAlpha(1f);
	}

	private void OnGuiPause()
	{
	}

	private void OnGuiUnpause()
	{
	}

	public void OnShotFired()
	{
		fired = true;
	}

	public void OnLevelWin()
	{
		base.player.damageReceiver.OnWin();
		SetTrigger(Triggers.OnWin);
	}

	public void PlayIntro()
	{
		SetBool(Booleans.Intro, true);
		string text = ((base.player.id != PlayerId.PlayerOne) ? "Mugman" : "Cuphead");
		Play("Intro_" + text);
		if (text == "Cuphead")
		{
			AudioManager.Play("player_intro_cuphead");
		}
		else
		{
			AudioManager.Play("player_intro_mugman");
		}
	}

	public void LevelInit()
	{
		SetSprites(base.player.id == PlayerId.PlayerOne);
	}

	public void SetSprites(bool isCuphead)
	{
		cuphead.SetActive(isCuphead);
		mugman.SetActive(!isCuphead);
		if (isCuphead)
		{
			renderer = cuphead.GetComponent<SpriteRenderer>();
			armRenderer = cupheadArm.GetComponent<SpriteRenderer>();
		}
		else
		{
			renderer = mugman.GetComponent<SpriteRenderer>();
			armRenderer = mugmanArm.GetComponent<SpriteRenderer>();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		CupheadLevelCamera.Current.Shake(20f, 0.6f);
		AudioManager.Play(Sfx.Player_Hit);
		Play("Hit");
		hitAnimation = true;
	}

	private void OnRunDust()
	{
		runEffect.Create(runDustRoot.position);
	}

	private void onHitAnimationComplete()
	{
		hitAnimation = false;
	}

	private void SetSpriteProperties(SpriteLayer layer, int order)
	{
		renderer.sortingLayerName = layer.ToString();
		renderer.sortingOrder = order;
	}

	private void ResetSpriteProperties()
	{
		renderer.sortingLayerName = SpriteLayer.Player.ToString();
		renderer.sortingOrder = ((base.player.id == PlayerId.PlayerOne) ? 1 : (-1));
	}

	private void OnParryStart()
	{
		if (!super)
		{
			SetTrigger(Triggers.OnParry);
		}
	}

	public void OnParrySuccess()
	{
		SetAlpha(1f);
	}

	public void OnParryPause()
	{
	}

	public void OnParryAnimEnd()
	{
	}

	public void ResumeNormanAnim()
	{
	}

	private void OnGrounded()
	{
		if (Level.Current.Started)
		{
			AudioManager.Play(Sfx.Player_Grounded);
		}
	}

	private void OnEx()
	{
		string text = "Forward";
		if ((int)base.player.motor.LookDirection.x == 0 && (int)base.player.motor.LookDirection.y > 0)
		{
			text = "Up";
		}
		else if ((int)base.player.motor.LookDirection.x != 0 && (int)base.player.motor.LookDirection.y > 0)
		{
			text = "Diagonal_Up";
		}
		else if ((int)base.player.motor.LookDirection.x == 0 && (int)base.player.motor.LookDirection.y < 0)
		{
			text = "Down";
		}
		else if ((int)base.player.motor.LookDirection.x != 0 && (int)base.player.motor.LookDirection.y < 0)
		{
			text = "Diagonal_Down";
		}
		if (text == "Forward")
		{
			AudioManager.Play("player_ex_forward_ground");
		}
		string text2 = "Ex." + text + "_";
		text2 = ((!base.player.motor.Grounded) ? (text2 + "Air") : (text2 + "Ground"));
		Play(text2);
	}

	private void OnSuper()
	{
		Super super = PlayerData.Data.Loadouts.GetPlayerLoadout(base.player.id).super;
		Debug.Log("Super: " + super);
		this.super = true;
		renderer.enabled = false;
	}

	private void OnSuperEnd()
	{
		super = false;
		renderer.enabled = true;
		ResetSpriteProperties();
	}

	private void _OnSuperAnimEnd()
	{
		base.player.UnpauseAll();
		base.player.motor.OnSuperEnd();
	}

	protected void Play(string animation)
	{
		base.animator.Play(animation, 0, 0f);
	}

	protected bool GetBool(Booleans b)
	{
		return base.animator.GetBool(b.ToString());
	}

	protected void SetBool(Booleans b, bool value)
	{
		base.animator.SetBool(b.ToString(), value);
	}

	protected int GetInt(Integers i)
	{
		return base.animator.GetInteger(i.ToString());
	}

	protected void SetInt(Integers i, int value)
	{
		base.animator.SetInteger(i.ToString(), value);
	}

	protected void SetTrigger(Triggers t)
	{
		base.animator.SetTrigger(t.ToString());
	}

	protected void ResetTrigger(Triggers t)
	{
		base.animator.ResetTrigger(t.ToString());
	}

	private void SetAlpha(float a)
	{
		Color color = renderer.color;
		color.a = a;
		renderer.color = color;
		armRenderer.color = color;
	}

	public void SetColor(Color color)
	{
		float a = renderer.color.a;
		color.a = a;
		renderer.color = color;
	}

	public void ResetColor()
	{
		float a = renderer.color.a;
		renderer.color = new Color(1f, 1f, 1f, a);
	}

	public void SetColorOverTime(Color color, float time)
	{
		StopColorCoroutine();
		colorCoroutine = setColor_cr(color, time);
		StartCoroutine(colorCoroutine);
	}

	public void StopColorCoroutine()
	{
		if (colorCoroutine != null)
		{
			StopCoroutine(colorCoroutine);
		}
		colorCoroutine = null;
	}

	private IEnumerator setColor_cr(Color color, float time)
	{
		float t = 0f;
		Color startColor = renderer.color;
		while (t < time)
		{
			float val = t / time;
			SetColor(Color.Lerp(startColor, color, val));
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		SetColor(color);
		yield return null;
	}

	private IEnumerator flash_cr()
	{
		float t = 0f;
		while (true)
		{
			if (Flashing)
			{
				yield return CupheadTime.WaitForSeconds(this, 0.5f);
				while (Flashing)
				{
					SetAlpha(0.3f);
					t = 0f;
					while (t < 0.05f)
					{
						if (!Flashing)
						{
							SetAlpha(1f);
							break;
						}
						t += base.LocalDeltaTime;
						yield return null;
					}
					if (!Flashing)
					{
						SetAlpha(1f);
						break;
					}
					SetAlpha(1f);
					t = 0f;
					while (t < 0.2f)
					{
						if (!Flashing)
						{
							SetAlpha(1f);
							break;
						}
						t += base.LocalDeltaTime;
						yield return null;
					}
					if (!Flashing)
					{
						SetAlpha(1f);
						break;
					}
				}
			}
			else
			{
				yield return true;
			}
		}
	}
}
