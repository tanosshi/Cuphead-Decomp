using System.Collections;
using UnityEngine;

public class BaronessLevelFollowingProjectile : AbstractProjectile
{
	public LevelProperties.Baroness.BaronessVonBonbon properties;

	private AbstractPlayerController player;

	private Vector3 target;

	private BaronessLevelCastle parent;

	private float velocity;

	private bool timesUp;

	private bool isActive;

	protected override void Awake()
	{
		base.Awake();
		isActive = true;
	}

	public void Init(Vector2 pos, float velocity, Vector3 target, LevelProperties.Baroness.BaronessVonBonbon properties, AbstractPlayerController player, BaronessLevelCastle parent)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.target = target;
		this.velocity = velocity;
		this.player = player;
		this.parent = parent;
		this.parent.OnDeathEvent += KillProjectile;
	}

	private void KillProjectile()
	{
		isActive = false;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
		if (!isActive)
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float count = 0f;
		while (true)
		{
			Vector2 start = base.transform.position;
			target = player.transform.position;
			float followTime = properties.finalProjectileMoveDuration;
			float t = 0f;
			while (t < followTime)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, target, properties.finalProjectileSpeed * CupheadTime.FixedDelta);
				t += CupheadTime.FixedDelta;
				yield return wait;
			}
			player = PlayerManager.GetNext();
			count += 1f;
			if (count > properties.finalProjectileRedirectCount)
			{
				break;
			}
			yield return CupheadTime.WaitForSeconds(this, properties.finalProjectileRedirectDelay);
		}
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		Vector3 dir = player.transform.position - base.transform.position;
		while (true)
		{
			base.transform.position += dir.normalized * properties.finalProjectileSpeed * CupheadTime.FixedDelta;
			yield return wait;
		}
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		base.Die();
	}
}
