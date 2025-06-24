using UnityEngine;

public class CollisionChild : AbstractCollidableObject
{
	public delegate void OnCollisionHandler(GameObject hit, CollisionPhase phase);

	[SerializeField]
	[Tooltip("OPTIONAL: Drag collision parent to this slot to register all collision events to this child. If null, no collisions are registered.")]
	private AbstractCollidableObject collisionParent;

	public event OnCollisionHandler OnAnyCollision;

	public event OnCollisionHandler OnWallCollision;

	public event OnCollisionHandler OnGroundCollision;

	public event OnCollisionHandler OnCeilingCollision;

	public event OnCollisionHandler OnPlayerCollision;

	public event OnCollisionHandler OnPlayerProjectileCollision;

	public event OnCollisionHandler OnEnemyCollision;

	public event OnCollisionHandler OnEnemyProjectileCollision;

	public event OnCollisionHandler OnOtherCollision;

	protected override void Start()
	{
		base.Start();
		if (collisionParent != null)
		{
			collisionParent.RegisterCollisionChild(this);
		}
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		if (this.OnAnyCollision != null)
		{
			this.OnAnyCollision(hit, phase);
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		if (this.OnWallCollision != null)
		{
			this.OnWallCollision(hit, phase);
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (this.OnGroundCollision != null)
		{
			this.OnGroundCollision(hit, phase);
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		if (this.OnCeilingCollision != null)
		{
			this.OnCeilingCollision(hit, phase);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (this.OnPlayerCollision != null)
		{
			this.OnPlayerCollision(hit, phase);
		}
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		if (this.OnPlayerProjectileCollision != null)
		{
			this.OnPlayerProjectileCollision(hit, phase);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (this.OnEnemyCollision != null)
		{
			this.OnEnemyCollision(hit, phase);
		}
	}

	protected override void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
	{
		if (this.OnEnemyProjectileCollision != null)
		{
			this.OnEnemyProjectileCollision(hit, phase);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (this.OnOtherCollision != null)
		{
			this.OnOtherCollision(hit, phase);
		}
	}
}
