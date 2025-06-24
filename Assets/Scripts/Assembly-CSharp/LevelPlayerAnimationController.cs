using System.Collections;
using UnityEngine;

public class LevelPlayerAnimationController : AbstractLevelPlayerComponent
{
	public enum Booleans
	{
		Dashing = 0,
		Locked = 1,
		Shooting = 2,
		Grounded = 3,
		Turning = 4,
		Intro = 5,
		Dead = 6,
		HasParryCharm = 7,
		HasParryAttack = 8
	}

	public enum Integers
	{
		MoveX = 0,
		MoveY = 1,
		LookX = 2,
		LookY = 3
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

	[SerializeField]
	private Effect smokeDashEffect;

	[SerializeField]
	private Sprite cupheadScaredSprite;

	[SerializeField]
	private Sprite mugmanScaredSprite;

	private SpriteRenderer renderer;

	private bool hitAnimation;

	private bool super;

	private bool shooting;

	private bool fired;

	private bool intropowerupactive;

	private Trilean2 lastTrueLookDir = new Trilean2(1, 0);

	private float timeSinceStoppedShooting = 100f;

	private Material tempMaterial;

	private const float STOP_SHOOTING_DELAY = 0.0833f;

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
		base.player.motor.OnDashStartEvent += OnDashStart;
		base.player.motor.OnDashEndEvent += OnDashEnd;
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		base.player.weaponManager.OnExStart += OnEx;
		base.player.weaponManager.OnSuperStart += OnSuper;
		base.player.weaponManager.OnSuperEnd += OnSuperEnd;
		base.player.weaponManager.OnWeaponFire += OnShotFired;
		LevelPauseGUI.OnPauseEvent += OnGuiPause;
		LevelPauseGUI.OnPauseEvent += OnGuiUnpause;
		lastTrueLookDir = base.player.motor.TrueLookDirection;
		SetBool(Booleans.HasParryCharm, base.player.stats.Loadout.charm == Charm.charm_parry_plus);
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
		if (!hitAnimation && (int)base.player.motor.LookDirection.x != 0 && (int)lastTrueLookDir.x != (int)base.player.motor.TrueLookDirection.x)
		{
			SetBool(Booleans.Turning, true);
		}
		else
		{
			SetBool(Booleans.Turning, false);
		}
		lastTrueLookDir = base.player.motor.TrueLookDirection;
		SetBool(Booleans.Grounded, base.player.motor.Grounded);
		SetBool(Booleans.Locked, base.player.motor.Locked);
		if (base.player.motor.Locked)
		{
			SetInt(Integers.MoveX, 0);
		}
		else
		{
			SetInt(Integers.MoveX, base.player.motor.LookDirection.x);
		}
		if (base.player.motor.Ducking)
		{
			SetInt(Integers.MoveY, 0);
		}
		else
		{
			SetInt(Integers.MoveY, base.player.motor.MoveDirection.y);
		}
		SetInt(Integers.LookX, base.player.motor.LookDirection.x);
		SetInt(Integers.LookY, base.player.motor.LookDirection.y);
		SetBool(Booleans.Shooting, base.player.weaponManager.IsShooting);
		float num = ((!base.player.weaponManager.IsShooting && !(timeSinceStoppedShooting < 0.0833f)) ? 0f : 1f);
		base.animator.SetLayerWeight(1, num);
		base.animator.SetLayerWeight(2, ((int)base.player.motor.LookDirection.y <= 0) ? 0f : num);
		if (shooting)
		{
			timeSinceStoppedShooting = 0f;
		}
		else
		{
			timeSinceStoppedShooting += CupheadTime.Delta;
		}
		bool flag = false;
		if (fired && base.player.motor.Grounded && ((int)base.player.motor.LookDirection.x == 0 || base.player.motor.Locked || (int)base.player.motor.LookDirection.y < 0))
		{
			SetTrigger(Triggers.OnFire);
			flag = true;
		}
		fired = false;
		shooting = base.player.weaponManager.IsShooting;
		if (!shooting && !flag)
		{
			ResetTrigger(Triggers.OnFire);
		}
		if (base.player.motor.Dashing && GetBool(Booleans.Dashing) != base.player.motor.Dashing)
		{
			Play("Dash.Air");
			if (base.player.stats.Loadout.charm != Charm.charm_smoke_dash)
			{
				dashEffect.Create(base.transform.position, base.transform.localScale);
			}
		}
		SetBool(Booleans.Dashing, base.player.motor.Dashing);
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

	public void UpdateAnimator()
	{
		Update();
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

	public void OnRevive(Vector3 pos)
	{
		base.animator.Play("Jump");
	}

	public void OnGravityReversed()
	{
		base.transform.SetScale(null, base.player.motor.GravityReversalMultiplier);
	}

	public void OnLevelWin()
	{
		base.player.damageReceiver.OnWin();
		SetTrigger(Triggers.OnWin);
	}

	public void PlayIntro()
	{
		SetBool(Booleans.Intro, true);
		if (SceneLoader.CurrentLevel != Levels.Devil)
		{
			string text = ((base.player.id != PlayerId.PlayerOne) ? "Mugman" : "Cuphead");
			Play("Intro_" + text);
			return;
		}
		if (base.player.id == PlayerId.PlayerOne)
		{
			AudioManager.Play("player_scared_intro");
		}
		Play("Intro_Scared");
	}

	public void ScaredSprite(bool facingLeft)
	{
		if (base.player.id == PlayerId.PlayerOne)
		{
			base.animator.enabled = false;
			cuphead.GetComponent<SpriteRenderer>().sprite = cupheadScaredSprite;
			cuphead.GetComponent<SpriteRenderer>().flipX = facingLeft;
		}
		else
		{
			base.animator.enabled = false;
			mugman.GetComponent<SpriteRenderer>().sprite = mugmanScaredSprite;
			mugman.GetComponent<SpriteRenderer>().flipX = facingLeft;
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
		}
		else
		{
			renderer = mugman.GetComponent<SpriteRenderer>();
		}
		tempMaterial = renderer.material;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!base.player.stats.SuperInvincible)
		{
			CupheadLevelCamera.Current.Shake(20f, 0.6f);
			if (base.player.stats.Health == 4)
			{
				AudioManager.Play("player_damage_crack_level1");
			}
			else if (base.player.stats.Health == 3)
			{
				AudioManager.Play("player_damage_crack_level2");
			}
			else if (base.player.stats.Health == 2)
			{
				AudioManager.Play("player_damage_crack_level3");
			}
			else if (base.player.stats.Health == 1)
			{
				AudioManager.Play("player_damage_crack_level4");
			}
			AudioManager.Play(Sfx.Player_Hit);
			if (base.player.motor.Grounded)
			{
				Play("Hit.Hit_Ground");
			}
			else
			{
				Play("Hit.Hit_Air");
			}
			hitAnimation = true;
			hitEffect.Create(base.player.center, base.transform.localScale);
		}
	}

	private void OnDashStart()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_smoke_dash)
		{
			renderer.enabled = false;
			smokeDashEffect.Create(base.player.center);
		}
	}

	private void OnDashEnd()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_smoke_dash)
		{
			renderer.enabled = true;
			smokeDashEffect.Create(base.player.center);
		}
	}

	private void OnRunDust()
	{
		runEffect.Create(runDustRoot.position);
	}

	private void onHitAnimationComplete()
	{
		hitAnimation = false;
	}

	public void SetSpriteProperties(SpriteLayer layer, int order)
	{
		renderer.sortingLayerName = layer.ToString();
		renderer.sortingOrder = order;
	}

	public void ResetSpriteProperties()
	{
		renderer.sortingLayerName = SpriteLayer.Player.ToString();
		renderer.sortingOrder = ((base.player.id == PlayerId.PlayerOne) ? 1 : (-1));
	}

	private void OnParryStart()
	{
		if (!super)
		{
			if (base.player.stats.Loadout.charm == Charm.charm_parry_plus)
			{
				SetBool(Booleans.HasParryCharm, true);
			}
			if (base.player.stats.Loadout.charm == Charm.charm_parry_attack && !GetComponent<IParryAttack>().AttackParryUsed)
			{
				SetBool(Booleans.HasParryAttack, true);
			}
			SetTrigger(Triggers.OnParry);
		}
	}

	public void OnParrySuccess()
	{
		if (base.player.stats.Loadout.charm == Charm.charm_parry_plus)
		{
			SetBool(Booleans.HasParryCharm, false);
		}
		if (base.player.stats.Loadout.charm == Charm.charm_parry_attack)
		{
			SetBool(Booleans.HasParryAttack, false);
		}
		SetAlpha(1f);
	}

	public void OnParryPause()
	{
		base.animator.enabled = false;
		renderer.GetComponent<LevelPlayerParryAnimator>().StartSet();
	}

	public void OnParryAnimEnd()
	{
		ResumeNormanAnim();
	}

	public void ResumeNormanAnim()
	{
		renderer.GetComponent<LevelPlayerParryAnimator>().StopSet();
		base.animator.enabled = true;
	}

	private void OnGrounded()
	{
		if (Level.Current.Started)
		{
			AudioManager.Play(Sfx.Player_Grounded);
			groundedEffect.Create(base.transform.position, base.transform.localScale);
		}
	}

	private void OnEx()
	{
		string text = "Forward";
		if ((int)base.player.motor.LookDirection.x == 0 && (int)base.player.motor.LookDirection.y > 0)
		{
			text = "Up";
			AudioManager.Play("player_ex_forward_ground");
		}
		else if ((int)base.player.motor.LookDirection.x != 0 && (int)base.player.motor.LookDirection.y > 0)
		{
			text = "Diagonal_Up";
			AudioManager.Play("player_ex_forward_ground");
		}
		else if ((int)base.player.motor.LookDirection.x == 0 && (int)base.player.motor.LookDirection.y < 0)
		{
			text = "Down";
			AudioManager.Play("player_ex_forward_ground");
		}
		else if ((int)base.player.motor.LookDirection.x != 0 && (int)base.player.motor.LookDirection.y < 0)
		{
			text = "Diagonal_Down";
			AudioManager.Play("player_ex_forward_ground");
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

	public void SetOldMaterial()
	{
		renderer.material = tempMaterial;
	}

	public void SetMaterial(Material m)
	{
		tempMaterial = renderer.material;
		renderer.material = m;
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
			if (!Flashing)
			{
				yield return true;
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, 0.417f);
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
			yield return null;
		}
	}

	private void SoundIntroPowerup()
	{
		if (!intropowerupactive)
		{
			AudioManager.Play("player_powerup");
			emitAudioFromObject.Add("player_powerup");
			intropowerupactive = true;
		}
	}

	private void SoundParryAxe()
	{
		AudioManager.Play("player_parry_axe");
		emitAudioFromObject.Add("player_parry_axe");
	}
}
