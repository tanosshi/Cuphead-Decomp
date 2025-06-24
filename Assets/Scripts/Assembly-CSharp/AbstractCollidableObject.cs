using System.Collections.Generic;
using UnityEngine;

public class AbstractCollidableObject : AbstractPausableComponent
{
	private List<CollisionChild> collisionChildren = new List<CollisionChild>();

	protected virtual bool allowCollisionPlayer
	{
		get
		{
			return true;
		}
	}

	protected virtual bool allowCollisionEnemy
	{
		get
		{
			return true;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnregisterAllCollisionChildren();
	}

	protected override void OnTriggerEnter2D(Collider2D col)
	{
		base.OnTriggerEnter2D(col);
		checkCollision(col, CollisionPhase.Enter);
	}

	protected override void OnCollisionEnter2D(Collision2D col)
	{
		base.OnCollisionEnter2D(col);
		checkCollision(col.collider, CollisionPhase.Enter);
	}

	protected override void OnTriggerStay2D(Collider2D col)
	{
		base.OnTriggerStay2D(col);
		checkCollision(col, CollisionPhase.Stay);
	}

	protected override void OnCollisionStay2D(Collision2D col)
	{
		base.OnCollisionStay2D(col);
		checkCollision(col.collider, CollisionPhase.Stay);
	}

	protected override void OnTriggerExit2D(Collider2D col)
	{
		base.OnTriggerExit2D(col);
		checkCollision(col, CollisionPhase.Exit);
	}

	protected override void OnCollisionExit2D(Collision2D col)
	{
		base.OnCollisionExit2D(col);
		checkCollision(col.collider, CollisionPhase.Exit);
	}

	protected virtual void checkCollision(Collider2D col, CollisionPhase phase)
	{
		GameObject gameObject = col.gameObject;
		OnCollision(gameObject, phase);
		string text = gameObject.tag;
		if (text == Tags.Wall.ToString())
		{
			OnCollisionWalls(gameObject, phase);
		}
		else if (text == Tags.Ceiling.ToString())
		{
			OnCollisionCeiling(gameObject, phase);
		}
		else if (text == Tags.Ground.ToString())
		{
			OnCollisionGround(gameObject, phase);
		}
		else if (text == Tags.Enemy.ToString())
		{
			if (allowCollisionEnemy)
			{
				OnCollisionEnemy(gameObject, phase);
			}
		}
		else if (text == Tags.EnemyProjectile.ToString())
		{
			OnCollisionEnemyProjectile(gameObject, phase);
		}
		else if (text == Tags.Player.ToString())
		{
			if (allowCollisionPlayer)
			{
				OnCollisionPlayer(gameObject, phase);
			}
		}
		else if (text == Tags.PlayerProjectile.ToString())
		{
			OnCollisionPlayerProjectile(gameObject, phase);
		}
		else
		{
			OnCollisionOther(gameObject, phase);
		}
	}

	protected virtual void OnCollision(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
	}

	protected virtual void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
	}

	protected void RegisterCollisionChild(GameObject go)
	{
		CollisionChild component = go.GetComponent<CollisionChild>();
		if (component == null)
		{
			Debug.LogWarning("GameObject " + go.name + " does not contain a CollisionSwitch component");
		}
		else
		{
			RegisterCollisionChild(component);
		}
	}

	public void RegisterCollisionChild(CollisionChild s)
	{
		collisionChildren.Add(s);
		s.OnAnyCollision += OnCollision;
		s.OnWallCollision += OnCollisionWalls;
		s.OnGroundCollision += OnCollisionGround;
		s.OnCeilingCollision += OnCollisionCeiling;
		s.OnPlayerCollision += OnCollisionPlayer;
		s.OnPlayerProjectileCollision += OnCollisionPlayerProjectile;
		s.OnEnemyCollision += OnCollisionEnemy;
		s.OnEnemyProjectileCollision += OnCollisionEnemyProjectile;
		s.OnOtherCollision += OnCollisionOther;
	}

	protected void UnregisterCollisionChild(CollisionChild s)
	{
		if (collisionChildren.Contains(s))
		{
			s.OnAnyCollision -= OnCollision;
			s.OnWallCollision -= OnCollisionWalls;
			s.OnGroundCollision -= OnCollisionGround;
			s.OnCeilingCollision -= OnCollisionCeiling;
			s.OnPlayerCollision -= OnCollisionPlayer;
			s.OnPlayerProjectileCollision -= OnCollisionPlayerProjectile;
			s.OnEnemyCollision -= OnCollisionEnemy;
			s.OnEnemyProjectileCollision -= OnCollisionEnemyProjectile;
			s.OnOtherCollision -= OnCollisionOther;
			collisionChildren.Remove(s);
		}
	}

	protected void UnregisterAllCollisionChildren()
	{
		for (int num = collisionChildren.Count - 1; num >= 0; num--)
		{
			UnregisterCollisionChild(collisionChildren[num]);
		}
	}
}
