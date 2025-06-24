using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractPlayerController : AbstractPausableComponent
{
	public delegate void OnReviveHandler(Vector3 pos);

	private PlayerInput _input;

	private PlayerStatsManager _stats;

	private PlayerDamageReceiver _damageReceiver;

	private PlayerCameraController _cameraController;

	private bool _isReviving;

	private BoxCollider2D _collider;

	private GameObjectHelper reviveHelper;

	public PlayerInput input
	{
		get
		{
			if (_input == null)
			{
				_input = GetComponent<PlayerInput>();
			}
			return _input;
		}
	}

	public PlayerStatsManager stats
	{
		get
		{
			if (_stats == null)
			{
				_stats = GetComponent<PlayerStatsManager>();
			}
			return _stats;
		}
	}

	public PlayerDamageReceiver damageReceiver
	{
		get
		{
			if (_damageReceiver == null)
			{
				_damageReceiver = GetComponent<PlayerDamageReceiver>();
			}
			return _damageReceiver;
		}
	}

	public PlayerCameraController cameraController
	{
		get
		{
			if (_cameraController == null)
			{
				_cameraController = GetComponent<PlayerCameraController>();
			}
			return _cameraController;
		}
	}

	public PlayerId id { get; private set; }

	public PlayerMode mode { get; private set; }

	public bool IsDead
	{
		get
		{
			if (_isReviving)
			{
				return false;
			}
			return stats.Health <= 0;
		}
	}

	public bool levelStarted { get; protected set; }

	public bool levelEnded { get; protected set; }

	public BoxCollider2D collider
	{
		get
		{
			if (_collider == null)
			{
				_collider = GetComponent<BoxCollider2D>();
			}
			return _collider;
		}
	}

	public BoxCollider2D collider2D
	{
		get
		{
			return collider;
		}
	}

	public virtual Vector3 center
	{
		get
		{
			if (base.transform == null)
			{
				return Vector3.zero;
			}
			return base.transform.position + (Vector3)collider.offset;
		}
	}

	public virtual Vector3 CameraCenter
	{
		get
		{
			return cameraController.center;
		}
	}

	public float left
	{
		get
		{
			return center.x - collider.size.x * 0.5f;
		}
	}

	public float right
	{
		get
		{
			return center.x + collider.size.x * 0.5f;
		}
	}

	public float top
	{
		get
		{
			return center.y + collider.size.y * 0.5f;
		}
	}

	public float bottom
	{
		get
		{
			return center.y - collider.size.y * 0.5f;
		}
	}

	public float width
	{
		get
		{
			return right - left;
		}
	}

	public float height
	{
		get
		{
			return top - bottom;
		}
	}

	public abstract bool CanTakeDamage { get; }

	public event Action OnPlayIntroEvent;

	public event OnReviveHandler OnReviveEvent;

	public static AbstractPlayerController Create(PlayerId id, Vector2 pos, PlayerMode mode)
	{
		AbstractPlayerController abstractPlayerController;
		switch (mode)
		{
		default:
			abstractPlayerController = UnityEngine.Object.Instantiate(Level.Current.LevelResources.levelPlayer);
			break;
		case PlayerMode.Plane:
			abstractPlayerController = UnityEngine.Object.Instantiate(Level.Current.LevelResources.planePlayer);
			break;
		case PlayerMode.Arcade:
			abstractPlayerController = UnityEngine.Object.Instantiate(((RetroArcadeLevel)Level.Current).playerPrefab);
			break;
		}
		abstractPlayerController.transform.position = pos;
		abstractPlayerController.mode = mode;
		abstractPlayerController.LevelInit(id);
		return abstractPlayerController;
	}

	public virtual void PlayIntro()
	{
		if (this.OnPlayIntroEvent != null)
		{
			this.OnPlayIntroEvent();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if ((Level.Current == null || !Level.Current.PlayersCreated) && base.gameObject != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected virtual void LevelInit(PlayerId id)
	{
		this.id = id;
		base.name = id.ToString();
		PlayerManager.SetPlayer(id, this);
		input.Init(this.id);
		cameraController.LevelInit();
		stats.LevelInit();
		stats.OnPlayerDeathEvent += OnDeath;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public virtual void LevelStart()
	{
		AbstractPlayerComponent[] componentsInChildren = GetComponentsInChildren<AbstractPlayerComponent>();
		foreach (AbstractPlayerComponent abstractPlayerComponent in componentsInChildren)
		{
			abstractPlayerComponent.OnLevelStart();
		}
		levelStarted = true;
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		levelEnded = true;
	}

	protected virtual void OnDeath(PlayerId playerId)
	{
		_isReviving = false;
		base.gameObject.SetActive(false);
	}

	public virtual void OnLeave(PlayerId playerId)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected virtual void OnPreRevive(Vector3 pos)
	{
		Debug.Log("OnPreRevive");
		stats.OnPreRevive();
		_isReviving = true;
		base.transform.position = pos;
	}

	public virtual void LevelJoin(Vector3 pos)
	{
		LevelStart();
		base.gameObject.SetActive(false);
		Vector3 position = base.transform.position;
		PlayerJoinEffect playerJoinEffect = PlayerJoinEffect.Create(id, base.transform.position, mode);
		playerJoinEffect.OnPreReviveEvent += OnPreRevive;
		playerJoinEffect.OnReviveEvent += OnRevive;
		OnPreRevive(pos);
	}

	public virtual void BufferInputs()
	{
	}

	protected virtual void OnRevive(Vector3 pos)
	{
		reviveHelper = new GameObjectHelper("Revive Helper");
		reviveHelper.events.StartCoroutine(reviveDelay_cr(1, pos));
		stats.OnRevive();
		base.transform.position = pos;
	}

	private IEnumerator reviveDelay_cr(int frameDelay, Vector3 pos)
	{
		for (int i = 0; i < frameDelay; i++)
		{
			yield return null;
		}
		_isReviving = false;
		base.gameObject.SetActive(true);
		if (this.OnReviveEvent != null)
		{
			this.OnReviveEvent(pos);
		}
	}
}
