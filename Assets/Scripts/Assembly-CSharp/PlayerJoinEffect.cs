using UnityEngine;

public class PlayerJoinEffect : AbstractMonoBehaviour
{
	public const string NAME = "Player_Join";

	[SerializeField]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	private PlayerId playerId;

	private SpriteRenderer spriteRenderer;

	private bool exiting;

	private PlayerMode playerMode;

	public event AbstractPlayerController.OnReviveHandler OnPreReviveEvent;

	public event AbstractPlayerController.OnReviveHandler OnReviveEvent;

	public static PlayerJoinEffect Create(PlayerId playerId, Vector2 pos, PlayerMode mode)
	{
		PlayerJoinEffect playerJoinEffect = Object.Instantiate(Level.Current.LevelResources.joinEffect);
		playerJoinEffect.name = playerJoinEffect.name.Replace("(Clone)", string.Empty);
		playerJoinEffect.Init(playerId, pos, mode);
		return playerJoinEffect;
	}

	protected override void Awake()
	{
		base.Awake();
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
	}

	protected override void Start()
	{
		base.Start();
	}

	private void Init(PlayerId playerId, Vector2 pos, PlayerMode mode)
	{
		this.playerId = playerId;
		playerMode = mode;
		base.animator.SetInteger("Mode", (int)playerMode);
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
		base.transform.position = pos;
	}

	public void GameOverUnpause()
	{
		base.animator.enabled = true;
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		ignoreGlobalTime = true;
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
}
