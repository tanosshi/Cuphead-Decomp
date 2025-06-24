using System.Collections;
using UnityEngine;

public class PlayerDeathEffect : AbstractMonoBehaviour
{
	public const string NAME = "Player_Death";

	public const string EFFECT_NAME = "Player_Death_Explosion";

	private const string PATH = "Player/Player_Death";

	private const float TIME_TO_SPEED = 1f;

	private static readonly float[] FLOAT_SPEEDS = new float[3] { 125f, 200f, 275f };

	private const int REVIVE_Y = 10;

	public const int DEATH_COUNT_MAX = 10;

	[SerializeField]
	protected SpriteRenderer cuphead;

	[SerializeField]
	protected SpriteRenderer mugman;

	[SerializeField]
	protected PlayerDeathParrySwitch parrySwitch;

	[SerializeField]
	private LevelPlayerDeathEffect explosionPrefab;

	protected PlayerId playerId;

	protected SpriteRenderer spriteRenderer;

	protected bool exiting;

	private PlayerInput playerInput;

	private int deathCount;

	private PlayerMode playerMode;

	public event AbstractPlayerController.OnReviveHandler OnPreReviveEvent;

	public event AbstractPlayerController.OnReviveHandler OnReviveEvent;

	public PlayerDeathEffect Create(PlayerId playerId, PlayerInput input, Vector2 pos, int deathCount, PlayerMode mode, bool canParry)
	{
		PlayerDeathEffect playerDeathEffect = Object.Instantiate(this);
		playerDeathEffect.name = playerDeathEffect.name.Replace("(Clone)", string.Empty);
		playerDeathEffect.Init(playerId, input, pos, deathCount, mode, canParry);
		return playerDeathEffect;
	}

	public void CreateExplosionOnly(PlayerId playerId, Vector2 pos, PlayerMode mode)
	{
		switch (mode)
		{
		case PlayerMode.Level:
		{
			PlayerDeathEffect playerDeathEffect2 = Object.Instantiate(this);
			LevelPlayerDeathEffect levelPlayerDeathEffect2 = Object.Instantiate(playerDeathEffect2.explosionPrefab);
			LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(playerId) as LevelPlayerController;
			levelPlayerDeathEffect2.Init(pos, playerId, levelPlayerController.motor.Grounded);
			Object.Destroy(playerDeathEffect2.gameObject);
			break;
		}
		case PlayerMode.Plane:
		{
			PlayerDeathEffect playerDeathEffect = Object.Instantiate(this);
			LevelPlayerDeathEffect levelPlayerDeathEffect = Object.Instantiate(playerDeathEffect.explosionPrefab);
			levelPlayerDeathEffect.Init(pos);
			Object.Destroy(playerDeathEffect.gameObject);
			break;
		}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		parrySwitch.OnActivate += OnParrySwitch;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(checkOutOfFrame_cr());
	}

	private void Init(PlayerId playerId, PlayerInput input, Vector2 pos, int deathCount, PlayerMode mode, bool canParry)
	{
		playerInput = input;
		this.playerId = playerId;
		this.deathCount = deathCount;
		if (deathCount >= 10)
		{
			parrySwitch.gameObject.SetActive(false);
		}
		playerMode = mode;
		base.animator.SetInteger("Mode", (int)playerMode);
		base.animator.SetBool("CanParry", canParry);
		if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
		{
			spriteRenderer = cuphead;
		}
		else
		{
			spriteRenderer = mugman;
		}
		cuphead.gameObject.SetActive(false);
		mugman.gameObject.SetActive(false);
		spriteRenderer.gameObject.SetActive(true);
		parrySwitch.gameObject.SetActive(canParry);
		base.transform.position = pos;
	}

	private void OnAnimationComplete()
	{
		AbstractPlayerController player = PlayerManager.GetPlayer(playerId);
		LevelPlayerController levelPlayerController = (LevelPlayerController)player;
		LevelPlayerDeathEffect levelPlayerDeathEffect = Object.Instantiate(explosionPrefab);
		levelPlayerDeathEffect.Init(base.transform.position, playerId, levelPlayerController.motor.Grounded);
		StartCoroutine(float_cr());
	}

	private void OnAnimationCompletePlane()
	{
		LevelPlayerDeathEffect levelPlayerDeathEffect = Object.Instantiate(explosionPrefab);
		levelPlayerDeathEffect.Init(base.transform.position);
		StartCoroutine(float_cr());
	}

	public void GameOverUnpause()
	{
		base.animator.enabled = true;
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		ignoreGlobalTime = true;
	}

	protected virtual void OnParrySwitch()
	{
		if (!exiting)
		{
			exiting = true;
			StopAllCoroutines();
			parrySwitch.gameObject.SetActive(false);
			if (this.OnPreReviveEvent != null)
			{
				this.OnPreReviveEvent(base.transform.position);
			}
			AudioManager.Play(Sfx.Player_Revive);
			AudioManager.Play("player_revive_thank_you");
			base.animator.SetTrigger("OnParry");
		}
	}

	protected virtual void OnReviveParryAnimComplete()
	{
		if (this.OnReviveEvent != null)
		{
			this.OnReviveEvent(base.transform.position);
		}
		this.OnReviveEvent = null;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private void ReviveOutOfFrame()
	{
		if (exiting || !PlayerManager.Multiplayer)
		{
			return;
		}
		exiting = true;
		PlayerId id = ((playerId == PlayerId.PlayerOne) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
		AbstractPlayerController player = PlayerManager.GetPlayer(id);
		if (!(player == null) && !player.IsDead && player.stats.PartnerCanSteal)
		{
			player.stats.OnPartnerStealHealth();
			StopAllCoroutines();
			if (this.OnPreReviveEvent != null)
			{
				this.OnPreReviveEvent(player.center);
			}
			AudioManager.Play(Sfx.Player_Revive);
			AudioManager.Play("player_revive_thank_you");
			base.animator.SetTrigger("OnSteal");
			base.transform.position = player.center;
		}
	}

	private void OnReviveStealAnimComplete()
	{
		if (this.OnReviveEvent != null)
		{
			this.OnReviveEvent(base.transform.position);
		}
		this.OnReviveEvent = null;
		Object.Destroy(base.gameObject);
	}

	private void OnPlayerLeave(PlayerId id)
	{
		if (playerId == id)
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
	}

	protected IEnumerator float_cr()
	{
		base.animator.SetTrigger("OnIdle");
		float floatSpeed = FLOAT_SPEEDS[Mathf.Clamp(deathCount, 0, FLOAT_SPEEDS.Length - 1)];
		while (true && !exiting)
		{
			base.transform.AddPosition(0f, floatSpeed * base.LocalDeltaTime);
			yield return null;
		}
	}

	protected virtual IEnumerator checkOutOfFrame_cr()
	{
		while (true)
		{
			if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(1000f, 10f)) && playerInput.actions.GetButtonDown(8) && !exiting)
			{
				yield return new WaitForSeconds(0.1f);
				ReviveOutOfFrame();
			}
			yield return null;
		}
	}
}
