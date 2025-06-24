using System.Collections;
using UnityEngine;

public class DicePalaceFlyingHorseLevelPresent : AbstractProjectile
{
	[SerializeField]
	private BasicProjectile bullet;

	private LevelProperties.DicePalaceFlyingHorse.GiftBombs properties;

	private Vector3 targetPos;

	public void Init(Vector3 startPos, Vector3 targetPos, LevelProperties.DicePalaceFlyingHorse.GiftBombs properties)
	{
		base.transform.position = startPos;
		this.targetPos = targetPos;
		this.properties = properties;
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator move_cr()
	{
		while (base.transform.position != targetPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, properties.initialSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, properties.explosionTime);
		string[] spreadCountPattern = properties.spreadCount.Split(',');
		float angle = 0f;
		int parryIndex = Random.Range(0, spreadCountPattern.Length);
		for (int i = 0; i < spreadCountPattern.Length; i++)
		{
			float.TryParse(spreadCountPattern[i], out angle);
			SpawnBullet(angle, parryIndex == i);
		}
		yield return null;
		Die();
	}

	private void SpawnBullet(float angle, bool parryable)
	{
		AudioManager.Play("projectile_explo");
		emitAudioFromObject.Add("projectile_explo");
		BasicProjectile basicProjectile = bullet.Create(base.transform.position, angle, properties.explosionSpeed);
		basicProjectile.SetParryable(parryable);
	}

	protected override void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		base.Die();
	}
}
