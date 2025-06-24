using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronessLevelJawbreaker : BaronessLevelMiniBossBase
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1,
		Explode = 2,
		Ghost = 3
	}

	private const float ROTATE_FRAME_TIME = 1f / 12f;

	[SerializeField]
	private Transform sprite;

	[SerializeField]
	private BaronessLevelJawbreakerMini miniBluePrefab;

	[SerializeField]
	private BaronessLevelJawbreakerMini miniRedPrefab;

	[SerializeField]
	private Transform followPoint;

	[SerializeField]
	private BaronessLevelJawbreakerGhost ghostPrefab;

	private List<BaronessLevelJawbreakerMini> prefabsList;

	private LevelProperties.Baroness.Jawbreaker properties;

	private AbstractPlayerController player;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float health;

	private float speed;

	private float rotationSpeed;

	private bool lookingLeft = true;

	private bool isTurning;

	private Transform aim;

	private Transform targetPos;

	private Vector3 updateJawbreakerPos;

	private Vector3 spawnPos;

	private Vector3 deathPosition;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		isDying = false;
		state = State.Spawned;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		sprite.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		sprite.GetComponent<SpriteRenderer>().sortingOrder = 150;
		StartCoroutine(check_rotation_cr());
		StartCoroutine(switch_cr());
		StartCoroutine(reset_sprite_cr());
	}

	private IEnumerator switch_cr()
	{
		StartCoroutine(fade_color_cr());
		yield return CupheadTime.WaitForSeconds(this, 3f);
		sprite.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		sprite.GetComponent<SpriteRenderer>().sortingOrder = 251;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		if (!(aim == null) && !(player == null) && state != State.Unspawned)
		{
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		updateJawbreakerPos = base.transform.position;
		if (state == State.Spawned)
		{
			base.transform.position -= base.transform.right * properties.jawbreakerHomingSpeed * CupheadTime.FixedDelta * hitPauseCoefficient();
			aim.LookAt2D(2f * base.transform.position - player.center);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, aim.rotation, rotationSpeed * CupheadTime.FixedDelta * hitPauseCoefficient());
		}
	}

	private IEnumerator check_rotation_cr()
	{
		while (true)
		{
			if (((player.transform.position.x < base.transform.position.x && !lookingLeft) || (player.transform.position.x > base.transform.position.x && lookingLeft)) && !isTurning)
			{
				isTurning = true;
				base.animator.SetTrigger("Turn");
				yield return base.animator.WaitForAnimationToEnd(this, "Turn");
				lookingLeft = !lookingLeft;
				isTurning = false;
			}
			yield return null;
		}
	}

	private void Turn()
	{
		sprite.transform.SetScale(0f - sprite.transform.localScale.x, 1f, 1f);
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		health -= info.damage;
		if (health < 0f && state == State.Spawned)
		{
			StartCoroutine(stopminis_cr());
			StartDeath();
		}
	}

	public void Init(LevelProperties.Baroness.Jawbreaker properties, AbstractPlayerController player, Vector2 pos, float speed, float rotationSpeed, float health)
	{
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
		this.properties = properties;
		this.player = player;
		this.speed = speed;
		this.rotationSpeed = rotationSpeed;
		this.health = health;
		base.transform.position = pos;
		spawnPos = base.transform.position;
		StartCoroutine(pickplayer_cr());
		StartCoroutine(minis_cr());
	}

	private IEnumerator pickplayer_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.jawbreakerHomeDuration);
			player = PlayerManager.GetNext();
			yield return null;
		}
	}

	private IEnumerator minis_cr()
	{
		targetPos = followPoint;
		Transform targetPos2 = targetPos;
		prefabsList = new List<BaronessLevelJawbreakerMini>();
		float spawnTime = properties.jawbreakerMiniSpace / properties.jawbreakerHomingSpeed;
		for (int i = 0; i < properties.jawbreakerMinis; i++)
		{
			if (i % 2 == 0)
			{
				yield return CupheadTime.WaitForSeconds(this, spawnTime);
				BaronessLevelJawbreakerMini blueminijawbreakers = Object.Instantiate(miniBluePrefab);
				blueminijawbreakers.Init(properties, spawnPos, targetPos, speed, rotationSpeed);
				targetPos2 = blueminijawbreakers.transform;
				prefabsList.Add(blueminijawbreakers);
			}
			else if (i % 2 == 1)
			{
				yield return CupheadTime.WaitForSeconds(this, spawnTime);
				BaronessLevelJawbreakerMini redminijawbreakers = Object.Instantiate(miniRedPrefab);
				redminijawbreakers.Init(properties, spawnPos, targetPos2, speed, rotationSpeed);
				targetPos = redminijawbreakers.transform;
				prefabsList.Add(redminijawbreakers);
			}
		}
		yield return null;
	}

	private IEnumerator stopminis_cr()
	{
		StopCoroutine(minis_cr());
		for (int i = 0; i < properties.jawbreakerMinis; i++)
		{
			prefabsList[i].Stop();
			yield return null;
		}
		StartCoroutine(killminis_cr());
	}

	private IEnumerator killminis_cr()
	{
		prefabsList.Reverse();
		for (int i = 0; i < properties.jawbreakerMinis; i++)
		{
			prefabsList[i].StartDying();
			yield return CupheadTime.WaitForSeconds(this, 0.8f);
		}
	}

	public void StartDeath()
	{
		state = State.Explode;
		StartCoroutine(dying_cr());
	}

	public IEnumerator dying_cr()
	{
		StartExplosions();
		isDying = true;
		base.transform.rotation = Quaternion.identity;
		base.animator.SetTrigger("Dead");
		GetComponent<Collider2D>().enabled = false;
		yield return base.animator.WaitForAnimationToEnd(this, "Death");
		BaronessLevelJawbreakerGhost ghost = Object.Instantiate(ghostPrefab);
		ghost.transform.position = base.transform.position;
		Die();
	}

	private IEnumerator reset_sprite_cr()
	{
		while (true)
		{
			sprite.transform.SetEulerAngles(0f, 0f, 0f);
			yield return null;
		}
	}

	private void SoundJawbreakerMouth()
	{
		AudioManager.Play("level_baroness_large_jawbreaker_mouth");
		emitAudioFromObject.Add("level_baroness_large_jawbreaker_mouth");
	}

	private void SoundJawbreakerDeath()
	{
		AudioManager.Stop("level_baroness_large_jawbreaker_mouth");
		AudioManager.Play("level_baroness_large_jawbreaker_death");
	}
}
