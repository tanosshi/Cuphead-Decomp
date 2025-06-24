using System;
using UnityEngine;

public abstract class AbstractPlayerSuper : AbstractCollidableObject
{
	[SerializeField]
	[Header("Player Sprites")]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	protected SpriteRenderer renderer;

	protected LevelPlayerController player;

	protected DamageDealer damageDealer;

	protected AnimationHelper animHelper;

	protected override bool allowCollisionPlayer
	{
		get
		{
			return false;
		}
	}

	public event Action OnEndedEvent;

	public event Action OnStartedEvent;

	protected override void Awake()
	{
		base.tag = Tags.PlayerProjectile.ToString();
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		animHelper = GetComponent<AnimationHelper>();
		base.transform.position = player.transform.position;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
		base.OnCollisionEnemy(hit, phase);
	}

	public AbstractPlayerSuper Create(LevelPlayerController player)
	{
		AbstractPlayerSuper abstractPlayerSuper = InstantiatePrefab<AbstractPlayerSuper>();
		abstractPlayerSuper.player = player;
		PlayerId id = player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			abstractPlayerSuper.renderer = cuphead;
			abstractPlayerSuper.mugman.gameObject.SetActive(false);
		}
		else
		{
			abstractPlayerSuper.renderer = mugman;
			abstractPlayerSuper.cuphead.gameObject.SetActive(false);
		}
		abstractPlayerSuper.StartSuper();
		return abstractPlayerSuper;
	}

	protected virtual void StartSuper()
	{
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = true;
		PauseManager.Pause();
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Super.ToString(), 0.1f);
		AudioManager.ChangeBGMPitch(1.3f, 1.5f);
		base.transform.SetScale(player.transform.localScale.x, player.transform.localScale.y, 1f);
		base.transform.position = player.transform.position;
		if (this.OnStartedEvent != null)
		{
			this.OnStartedEvent();
		}
		this.OnStartedEvent = null;
	}

	protected virtual void Fire()
	{
		PauseManager.Unpause();
		player.PauseAll();
		AnimationHelper component = GetComponent<AnimationHelper>();
		component.IgnoreGlobal = false;
	}

	protected virtual void EndSuper(bool changePitch = true)
	{
		AudioManager.SnapshotReset(SceneLoader.SceneName, 2f);
		if (changePitch)
		{
			AudioManager.ChangeBGMPitch(1f, 2f);
		}
		player.UnpauseAll();
		if (this.OnEndedEvent != null)
		{
			this.OnEndedEvent();
		}
		this.OnEndedEvent = null;
	}
}
