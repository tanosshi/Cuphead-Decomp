using System.Collections;
using UnityEngine;

public class BatLevelLightning : AbstractCollidableObject
{
	[SerializeField]
	private GameObject lightning;

	private CollisionChild collisionChild;

	private LevelProperties.Bat.BatLightning properties;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		collisionChild = lightning.GetComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		lightning.SetActive(false);
	}

	public void Init(LevelProperties.Bat.BatLightning properties, Vector2 startPos)
	{
		this.properties = properties;
		base.transform.position = startPos;
		StartCoroutine(lightning_cr());
	}

	private IEnumerator lightning_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.cloudWarning);
		lightning.SetActive(true);
		yield return CupheadTime.WaitForSeconds(this, properties.lightningOnDuration);
		Die();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
