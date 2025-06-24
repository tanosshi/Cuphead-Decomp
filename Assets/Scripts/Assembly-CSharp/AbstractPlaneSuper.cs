using UnityEngine;

public abstract class AbstractPlaneSuper : AbstractCollidableObject
{
	private PlanePlayerWeaponManager.States.Super state = PlanePlayerWeaponManager.States.Super.Intro;

	[SerializeField]
	[Header("Player Sprites")]
	private SpriteRenderer cuphead;

	[SerializeField]
	private SpriteRenderer mugman;

	protected SpriteRenderer renderer;

	protected PlanePlayerController player;

	protected DamageDealer damageDealer;

	protected AnimationHelper animHelper;

	public PlanePlayerWeaponManager.States.Super State
	{
		get
		{
			return state;
		}
	}

	protected override bool allowCollisionPlayer
	{
		get
		{
			return false;
		}
	}

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

	public AbstractPlaneSuper Create(PlanePlayerController player)
	{
		AbstractPlaneSuper abstractPlaneSuper = InstantiatePrefab<AbstractPlaneSuper>();
		abstractPlaneSuper.player = player;
		PlayerId id = player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			abstractPlaneSuper.renderer = cuphead;
			abstractPlaneSuper.mugman.gameObject.SetActive(false);
		}
		else
		{
			abstractPlaneSuper.renderer = mugman;
			abstractPlaneSuper.cuphead.gameObject.SetActive(false);
		}
		abstractPlaneSuper.StartSuper();
		return abstractPlaneSuper;
	}

	protected virtual void StartSuper()
	{
		animHelper = GetComponent<AnimationHelper>();
		animHelper.IgnoreGlobal = true;
		PauseManager.Pause();
		player.PauseAll();
		AudioManager.SnapshotTransition(new string[3] { "Super", "Unpaused", "Unpaused_1920s" }, new float[3] { 1f, 0f, 0f }, 0.1f);
		AudioManager.ChangeBGMPitch(1.3f, 1.5f);
		AudioManager.Play("player_super_beam_start");
		base.transform.SetScale(player.transform.localScale.x, player.transform.localScale.y, 1f);
		base.transform.position = player.transform.position;
	}

	protected virtual void Fire()
	{
		state = PlanePlayerWeaponManager.States.Super.Ending;
	}

	protected virtual void StartCountdown()
	{
		string[] array = new string[2] { "Super", null };
		if (SettingsData.Data.vintageAudioEnabled)
		{
			array[1] = "Unpaused_1920s";
		}
		else
		{
			array[1] = "Unpaused";
		}
		AudioManager.SnapshotTransition(array, new float[2] { 0f, 1f }, 4f);
		AudioManager.ChangeBGMPitch(1f, 4f);
		PauseManager.Unpause();
		player.UnpauseAll();
		animHelper.IgnoreGlobal = false;
		state = PlanePlayerWeaponManager.States.Super.Countdown;
	}
}
