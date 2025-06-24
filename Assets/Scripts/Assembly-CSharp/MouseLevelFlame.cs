using UnityEngine;

public class MouseLevelFlame : AbstractCollidableObject
{
	[SerializeField]
	private Transform root;

	[SerializeField]
	private Transform flippedRoot;

	private DamageDealer damageDealer;

	private Vector3 localPosition;

	private Vector3 localScale;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		localPosition = base.transform.localPosition;
		localScale = base.transform.localScale;
		base.transform.parent = null;
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void SetColliderEnabled(bool enabled)
	{
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = enabled;
		}
	}

	public void UpdateParentTransform(Transform mouseTransform)
	{
		base.transform.position = ((mouseTransform.localScale.x != 1f) ? flippedRoot.position : root.position);
	}
}
