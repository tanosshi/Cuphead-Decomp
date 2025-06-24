using UnityEngine;

public class PlaneWeaponPeashotExProjectile : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Frozen = 1
	}

	public float MaxSpeed;

	public float Acceleration;

	public float FreezeTime;

	[SerializeField]
	private Effect chompFxPrefab;

	[SerializeField]
	private Transform chompFxRoot;

	[SerializeField]
	private SpriteRenderer Cuphead;

	[SerializeField]
	private SpriteRenderer Mugman;

	private State state;

	private float timeSinceFrozen;

	public float speed;

	public void Init()
	{
		Cuphead.enabled = PlayerId == PlayerId.PlayerOne;
		Mugman.enabled = PlayerId != PlayerId.PlayerOne;
	}

	protected override void OnDealDamage(float damage, DamageReceiver receiver, DamageDealer damageDealer)
	{
		base.OnDealDamage(damage, receiver, damageDealer);
		chompFxPrefab.Create(chompFxRoot.position);
		state = State.Frozen;
		speed = 0f;
		timeSinceFrozen = 0f;
		AudioManager.Play("player_plane_weapon_ex_chomp");
		emitAudioFromObject.Add("player_plane_weapon_ex_chomp");
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.dead)
		{
			return;
		}
		switch (state)
		{
		case State.Idle:
			speed = Mathf.Min(MaxSpeed, speed + Acceleration * CupheadTime.FixedDelta);
			break;
		case State.Frozen:
			timeSinceFrozen += CupheadTime.FixedDelta;
			if (timeSinceFrozen > FreezeTime)
			{
				state = State.Idle;
				speed = MaxSpeed;
			}
			break;
		}
		base.transform.AddPosition(speed * CupheadTime.FixedDelta);
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		DealDamage(hit);
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (!(hit.tag == "Parry"))
		{
			base.OnCollisionOther(hit, phase);
		}
	}

	private void DealDamage(GameObject hit)
	{
		base.damageDealer.DealDamage(hit);
	}
}
