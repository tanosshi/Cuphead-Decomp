using UnityEngine;

public class PlaneWeaponBombProjectile : AbstractProjectile
{
	[SerializeField]
	private PlaneWeaponBombExplosion explosion;

	public bool shootsUp;

	public float explosionSize;

	public float bulletSize;

	public float gravity;

	public Vector2 velocity;

	protected override void Start()
	{
		base.Start();
		base.transform.SetScale(bulletSize, bulletSize);
		AudioManager.Play("plane_shmup_bomb_fire");
		emitAudioFromObject.Add("plane_shmup_bomb_fire");
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead)
		{
			if (shootsUp)
			{
				velocity.y += gravity * CupheadTime.FixedDelta;
				base.transform.position += (Vector3)(velocity * CupheadTime.FixedDelta);
			}
			else
			{
				velocity.y -= gravity * CupheadTime.FixedDelta;
				base.transform.position += (Vector3)(velocity * CupheadTime.FixedDelta);
			}
		}
	}

	private void DealDamage(GameObject hit)
	{
		base.damageDealer.DealDamage(hit);
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		DealDamage(hit);
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void Die()
	{
		base.Die();
		GetComponent<SpriteRenderer>().enabled = false;
		AudioManager.Play("plane_shmup_bomb_explosion");
		emitAudioFromObject.Add("plane_shmup_bomb_explosion");
		explosion.Create(base.transform.position, Damage, base.DamageMultiplier, explosionSize);
	}

	public void SetAnimation(PlayerId player)
	{
		animator.Play((player != PlayerId.PlayerOne) ? "Bomb_MM" : "Bomb_CH");
	}
}
