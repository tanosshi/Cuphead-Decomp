using System.Collections;
using UnityEngine;

public class PirateLevelBoatProjectile : AbstractProjectile
{
	[SerializeField]
	private Transform child;

	public PirateLevelBoatProjectile Create(Vector2 pos, float speed, float rotationSpeed)
	{
		PirateLevelBoatProjectile pirateLevelBoatProjectile = Create() as PirateLevelBoatProjectile;
		pirateLevelBoatProjectile.CollisionDeath.OnlyPlayer();
		pirateLevelBoatProjectile.DamagesType.OnlyPlayer();
		pirateLevelBoatProjectile.Init(pos, speed, rotationSpeed);
		return pirateLevelBoatProjectile;
	}

	private void Init(Vector2 pos, float speed, float rotationSpeed)
	{
		StartCoroutine(bullet_cr(pos, speed, rotationSpeed));
	}

	protected override void Update()
	{
		base.Update();
		child.SetEulerAngles(0f, 0f, 0f);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		Die();
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	protected override void Die()
	{
		child.SetLocalEulerAngles(0f, 0f, Random.Range(0, 360));
		base.Die();
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator bullet_cr(Vector2 pos, float speed, float rotationSpeed)
	{
		base.transform.position = pos - (Vector2)child.localPosition;
		(GetComponent<Collider2D>() as CircleCollider2D).offset = child.localPosition;
		while (true)
		{
			if (base.transform.position.x < -1280f)
			{
				End();
			}
			base.transform.AddPosition((0f - speed) * (float)CupheadTime.Delta);
			base.transform.AddEulerAngles(0f, 0f, (0f - rotationSpeed) * (float)CupheadTime.Delta);
			yield return null;
		}
	}

	private IEnumerator die_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		Object.Destroy(base.gameObject);
	}
}
