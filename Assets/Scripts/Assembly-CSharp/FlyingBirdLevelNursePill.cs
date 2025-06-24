using System.Collections;
using UnityEngine;

public class FlyingBirdLevelNursePill : AbstractProjectile
{
	[SerializeField]
	private FlyingBirdLevelNursePillProjectile topHalf;

	[SerializeField]
	private FlyingBirdLevelNursePillProjectile bottomHalf;

	[SerializeField]
	private GameObject normalPill;

	[SerializeField]
	private GameObject parryPill;

	private bool gravity;

	private Vector3 velocity;

	private PlayerId target;

	private LevelProperties.FlyingBird.Nurses properties;

	protected override void FixedUpdate()
	{
		if (gravity)
		{
			if (velocity.magnitude < properties.pillSpeed)
			{
			}
			velocity.y -= 10f;
		}
		base.FixedUpdate();
	}

	public void InitPill(LevelProperties.FlyingBird.Nurses properties, PlayerId target, bool parryable)
	{
		SetParryable(parryable);
		this.target = target;
		this.properties = properties;
		velocity = base.transform.up.normalized * properties.pillSpeed;
		StartCoroutine(move_cr());
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		if (parryable)
		{
			parryPill.SetActive(true);
		}
		else
		{
			normalPill.SetActive(true);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			base.transform.position += velocity * CupheadTime.Delta;
			if (base.transform.position.y >= properties.pillMaxHeight)
			{
				gravity = true;
				StartCoroutine(detonate_cr());
			}
			yield return null;
		}
	}

	private IEnumerator detonate_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.pillExplodeDelay);
		AbstractPlayerController player = PlayerManager.GetPlayer(target);
		if (player == null || player.IsDead)
		{
			player = PlayerManager.GetNext();
		}
		base.transform.right = (player.center - base.transform.position).normalized;
		FlyingBirdLevelNursePillProjectile top = topHalf.Create(base.transform.position, base.transform.eulerAngles.z, properties.bulletSpeed) as FlyingBirdLevelNursePillProjectile;
		FlyingBirdLevelNursePillProjectile bottom = bottomHalf.Create(base.transform.position, base.transform.eulerAngles.z + 180f, properties.bulletSpeed) as FlyingBirdLevelNursePillProjectile;
		if (base.CanParry)
		{
			top.SetPillColor(FlyingBirdLevelNursePillProjectile.PillColor.LightPink);
			top.SetParryable(true);
			bottom.SetPillColor(FlyingBirdLevelNursePillProjectile.PillColor.DarkPink);
			bottom.SetParryable(true);
		}
		else
		{
			top.SetPillColor(FlyingBirdLevelNursePillProjectile.PillColor.Yellow);
			bottom.SetPillColor(FlyingBirdLevelNursePillProjectile.PillColor.Blue);
		}
		Object.Destroy(base.gameObject);
	}
}
