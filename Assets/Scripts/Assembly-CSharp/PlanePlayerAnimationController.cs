using System;
using System.Collections;
using UnityEngine;

public class PlanePlayerAnimationController : AbstractPlanePlayerComponent
{
	public enum ShrinkStates
	{
		Ready = 0,
		Shrunk = 1,
		Cooldown = 2
	}

	public enum Triggers
	{

	}

	public enum Integers
	{
		Y = 0
	}

	private const float ROTATION_MAX = 9f;

	private const float SHUNK_ROTATION_ADD = 5f;

	private const float ROTATION_SPEED = 7f;

	private const float INTRO_X = -150f;

	private const float PUFF_DELAY = 0.17f;

	private const float PUFF_DELAY_MOVING = 0.07f;

	private static readonly Vector2 PUFF_OFFSET = new Vector3(-50f, 0f);

	private const float SHRINK_COOLDOWN = 0.23300001f;

	[SerializeField]
	private Transform cuphead;

	[SerializeField]
	private Transform mugman;

	[Space(10f)]
	[SerializeField]
	private Transform introRoot;

	[Space(10f)]
	[SerializeField]
	private Effect breakoutPrefab;

	[SerializeField]
	private Effect poofPrefab;

	[SerializeField]
	private Effect greenPrefab;

	[SerializeField]
	private PlaneLevelEffect puffPrefab;

	[Space(10f)]
	[SerializeField]
	private Effect hitSparkEffect;

	[SerializeField]
	private Effect hitDustEffect;

	[SerializeField]
	private Effect smokeDashEffect;

	[Space(10f)]
	[SerializeField]
	private Effect shrinkEffect;

	[SerializeField]
	private Effect growEffect;

	[Space(10f)]
	[SerializeField]
	private PlanePlayerDeathPart[] deathPieces;

	[SerializeField]
	private PlaneLevelEffect deathEffect;

	private Transform playerSprite;

	private SpriteRenderer renderer;

	private float rotation;

	private int shakeFrame;

	private float shrinkCooldownTimeLeft;

	private bool isStoned;

	private IEnumerator colorCoroutine;

	private Transform activeTransform
	{
		get
		{
			if (base.player.id == PlayerId.PlayerOne)
			{
				return cuphead;
			}
			return mugman;
		}
	}

	public ShrinkStates ShrinkState { get; set; }

	public bool Shrinking { get; private set; }

	private bool Flashing
	{
		get
		{
			return base.player.damageReceiver.state == PlayerDamageReceiver.State.Invulnerable;
		}
	}

	public event Action OnExFireAnimEvent;

	protected override void Start()
	{
		base.Start();
		base.player.weaponManager.OnExStartEvent += OnExStart;
		base.player.weaponManager.OnSuperStartEvent += OnSuperStart;
		base.player.parryController.OnParryStartEvent += OnParryStart;
		base.player.parryController.OnParrySuccessEvent += OnParrySuccess;
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		base.player.stats.OnPlayerDeathEvent += OnDeath;
		base.player.OnReviveEvent += OnRevive;
		base.player.stats.OnStoneShake += onStoneShake;
		base.player.stats.OnStoned += onStoned;
		if (renderer == null)
		{
			renderer = playerSprite.GetComponent<SpriteRenderer>();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StartCoroutine(flash_cr());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		HandleRotation();
		HandleShrunk();
		SetInteger(Integers.Y, base.player.motor.MoveDirection.y);
	}

	public void LevelInit()
	{
		PlayerId id = base.player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			playerSprite = cuphead;
		}
		else
		{
			playerSprite = mugman;
		}
		cuphead.gameObject.SetActive(false);
		mugman.gameObject.SetActive(false);
		if (Level.Current.Started)
		{
			playerSprite.gameObject.SetActive(true);
		}
	}

	public void PlayIntro()
	{
		PlayerId id = base.player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			base.animator.Play("Intro");
		}
		else
		{
			base.animator.Play("Intro_Alt");
		}
		renderer = playerSprite.GetComponent<SpriteRenderer>();
		playerSprite.gameObject.SetActive(true);
		if (base.gameObject.activeSelf)
		{
			if (!Level.Current.Started && base.player.id == PlayerId.PlayerTwo)
			{
				playerSprite.SetLocalPosition(introRoot.transform.localPosition.x, introRoot.transform.localPosition.y, 0f);
			}
			StartCoroutine(done_intro_cr());
		}
	}

	private IEnumerator done_intro_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, (base.player.id != PlayerId.PlayerOne) ? "Intro_Alt" : "Intro");
		StartCoroutine(puff_cr());
		yield return null;
	}

	private void ResetPosition()
	{
		playerSprite.SetLocalPosition(0f, 0f);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (base.player.stats.Health > 0 && !(info.damage <= 0f))
		{
			hitSparkEffect.Create(base.player.center);
			hitDustEffect.Create(base.player.center);
			CupheadLevelCamera.Current.Shake(20f, 0.6f);
		}
	}

	private void onStoned()
	{
		ShrinkState = ShrinkStates.Ready;
		base.animator.SetLayerWeight(1, 0f);
		StopCoroutine(ex_cr());
		greenPrefab.Create(base.player.center);
		base.animator.Play("Stone_Idle");
		base.animator.ResetTrigger("Breakout");
		StartCoroutine(stone_animation_cr());
		StartCoroutine(create_poofs_cr());
		isStoned = true;
	}

	private IEnumerator stone_animation_cr()
	{
		while (base.player.stats.StoneTime > 0f)
		{
			yield return null;
		}
		base.animator.SetTrigger("Breakout");
		AnimatorStateInfo animState = base.animator.GetCurrentAnimatorStateInfo(0);
		while (animState.IsName("Stone_Idle") || animState.IsName("Stone_Shake_A") || animState.IsName("Stone_Shake_B") || animState.IsName("Stone_Shake_C") || animState.IsName("Stone_Shake_C") || animState.IsName("Stone_Shake_D") || animState.IsName("Stone_Shake_E") || animState.IsName("Breakout"))
		{
			yield return null;
		}
		yield return null;
	}

	private void Breakout()
	{
		isStoned = false;
		breakoutPrefab.Create(base.player.center).transform.parent = base.transform;
		StopCoroutine(create_poofs_cr());
	}

	private void onStoneShake()
	{
		base.animator.SetTrigger("Shake");
	}

	private IEnumerator create_poofs_cr()
	{
		float t = 0f;
		float time = 0.1f;
		while (base.player.stats.StoneTime > 0f)
		{
			if (!base.animator.GetCurrentAnimatorStateInfo(0).IsName("Stone_Idle") && !base.animator.GetCurrentAnimatorStateInfo(0).IsName("Breakout"))
			{
				string layerName = ((!Rand.Bool()) ? SpriteLayer.Effects.ToString() : SpriteLayer.Enemies.ToString());
				Effect poof = UnityEngine.Object.Instantiate(poofPrefab);
				poof.transform.position = base.player.center;
				poof.animator.SetInteger("Poof", UnityEngine.Random.Range(0, 3));
				poof.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
				while (t < time)
				{
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
			}
			yield return null;
		}
		yield return null;
	}

	private void HandleRotation()
	{
		float num = 0f;
		if ((int)base.player.motor.MoveDirection.x < 0)
		{
			num = 9f;
		}
		else if ((int)base.player.motor.MoveDirection.x > 0)
		{
			num = -9f;
		}
		if (base.player.Shrunk)
		{
			num += 5f * (float)(-(int)base.player.motor.MoveDirection.x);
		}
		rotation = Mathf.Lerp(rotation, num, 7f * CupheadTime.FixedDelta);
		activeTransform.SetEulerAngles(0f, 0f, rotation);
	}

	private void HandleShrunk()
	{
		if (ShrinkState == ShrinkStates.Cooldown)
		{
			if (shrinkCooldownTimeLeft <= 0f)
			{
				ShrinkState = ShrinkStates.Ready;
			}
			shrinkCooldownTimeLeft -= CupheadTime.FixedDelta;
		}
		if (base.player.Parrying || base.player.WeaponBusy || base.player.stats.StoneTime > 0f || ShrinkState == ShrinkStates.Cooldown)
		{
			return;
		}
		if (ShrinkState == ShrinkStates.Ready && (base.player.input.actions.GetButtonDown(7) || base.player.input.actions.GetButtonDown(6)))
		{
			base.animator.SetLayerWeight(1, 1f);
			base.animator.Play("Shrink_In", 0);
			Shrinking = true;
			ShrinkState = ShrinkStates.Shrunk;
			if (base.player.stats.Loadout.charm == Charm.charm_smoke_dash)
			{
				smokeDashEffect.Create(base.player.center);
			}
			AudioManager.Play("Player_Plane_Shrink");
		}
		if (ShrinkState == ShrinkStates.Shrunk && !base.player.input.actions.GetButton(7) && !base.player.input.actions.GetButton(6))
		{
			Shrinking = false;
			base.animator.SetLayerWeight(1, 0f);
			base.animator.Play("Shrink_Out", 0);
			ShrinkState = ShrinkStates.Cooldown;
			shrinkCooldownTimeLeft = 0.23300001f;
			AudioManager.Play("Player_Plane_Expand");
		}
	}

	private IEnumerator bomb_cr()
	{
		yield return null;
		base.animator.SetLayerWeight(2, 1f);
		float t = 0f;
		float[] slowShakeScales = new float[3] { 1f, 1.184f, 1.09f };
		float[] fastShakeScales = new float[6] { 1f, 1.184f, 1.09f, 1.34f, 1.09f, 1.184f };
		while (base.player.weaponManager.states.super == PlanePlayerWeaponManager.States.Super.Intro)
		{
			yield return null;
		}
		while (base.player.weaponManager.states.super == PlanePlayerWeaponManager.States.Super.Countdown)
		{
			if (t < 0.4f * WeaponProperties.PlaneSuperBomb.countdownTime)
			{
				yield return null;
				t += (float)CupheadTime.Delta;
			}
			else if (t < 0.7f * WeaponProperties.PlaneSuperBomb.countdownTime)
			{
				float[] array = slowShakeScales;
				foreach (float scale in array)
				{
					base.transform.SetScale(scale, scale);
					yield return CupheadTime.WaitForSeconds(this, 1f / 12f);
					t += 1f / 12f;
				}
			}
			else
			{
				float[] array2 = fastShakeScales;
				foreach (float scale2 in array2)
				{
					base.transform.SetScale(scale2, scale2);
					yield return CupheadTime.WaitForSeconds(this, 1f / 24f);
					t += 1f / 24f;
				}
			}
		}
		base.animator.SetLayerWeight(2, 0f);
		base.transform.SetScale(1f, 1f);
	}

	public void SetSpriteVisible(bool visible)
	{
		playerSprite.gameObject.SetActive(visible);
	}

	private void SetAlpha(float a)
	{
		Color color = renderer.color;
		color.a = a;
		renderer.color = color;
	}

	private void OnShrinkInComplete()
	{
		shrinkEffect.Create(base.player.center);
		Shrinking = false;
	}

	private void OnShrinkOutComplete()
	{
		growEffect.Create(base.player.center);
	}

	private void CreatePuff()
	{
		if (!(playerSprite == null))
		{
			PlaneLevelEffect planeLevelEffect = puffPrefab.Create(playerSprite.position + (Vector3)PUFF_OFFSET) as PlaneLevelEffect;
			if ((int)base.player.motor.MoveDirection.x < 0)
			{
				planeLevelEffect.speed = 2f;
			}
		}
	}

	private IEnumerator puff_cr()
	{
		float delay = 0.17f;
		while (true)
		{
			delay = 0.17f;
			if (((int)base.player.motor.MoveDirection.x != 0 || (int)base.player.motor.MoveDirection.y != 0) && (int)base.player.motor.MoveDirection.x >= 0)
			{
				delay = 0.07f;
			}
			if ((int)base.player.motor.MoveDirection.x >= 0)
			{
				CreatePuff();
			}
			yield return CupheadTime.WaitForSeconds(this, delay);
		}
	}

	private IEnumerator flash_cr()
	{
		float t = 0f;
		while (true)
		{
			if (Flashing)
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
				}
				else
				{
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
					if (Flashing)
					{
						continue;
					}
					SetAlpha(1f);
				}
			}
			yield return null;
		}
	}

	private void OnExStart()
	{
		StartCoroutine(ex_cr());
	}

	private IEnumerator ex_cr()
	{
		Debug.Log("ANIMATE_EX");
		string dir = (((int)base.player.motor.MoveDirection.y > 0) ? "Up" : "Down");
		base.animator.Play("Ex_" + dir);
		if (dir == "Up")
		{
			AudioManager.Play("player_plane_up_ex");
		}
		yield return base.animator.WaitForAnimationToEnd(this, "Ex_" + dir);
		if (this.OnExFireAnimEvent != null)
		{
			this.OnExFireAnimEvent();
		}
	}

	private void OnSuperStart()
	{
		StartCoroutine(bomb_cr());
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

	private void OnParryStart()
	{
		if (isStoned)
		{
			Breakout();
		}
		base.animator.SetBool("ParrySuccess", false);
		base.animator.SetBool("ParryPlusCharm", base.player.stats.Loadout.charm == Charm.charm_parry_plus);
		if (base.player.stats.Loadout.charm == Charm.charm_parry_attack)
		{
			base.animator.Play("ParryAttack");
		}
		else
		{
			base.animator.Play("Parry");
		}
	}

	private void OnParrySuccess()
	{
		base.animator.SetBool("ParrySuccess", true);
	}

	private void OnDeath(PlayerId playerId)
	{
		PlanePlayerDeathPart[] array = deathPieces;
		foreach (PlanePlayerDeathPart planePlayerDeathPart in array)
		{
			planePlayerDeathPart.CreatePart(base.player.id, base.transform.position);
		}
		deathEffect.Create(base.transform.position);
	}

	private void OnRevive(Vector3 pos)
	{
		SetAlpha(1f);
	}

	private void SetInteger(Integers integer, int value)
	{
		base.animator.SetInteger(integer.ToString(), value);
	}

	private void SetTrigger(Triggers trigger)
	{
		base.animator.SetTrigger(trigger.ToString());
	}
}
