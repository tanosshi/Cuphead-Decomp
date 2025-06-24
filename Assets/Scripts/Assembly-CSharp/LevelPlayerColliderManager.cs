using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayerColliderManager : AbstractLevelPlayerComponent
{
	public enum State
	{
		Default = 0,
		Air = 1,
		Ducking = 2,
		Dashing = 3
	}

	[Serializable]
	public class ColliderProperties
	{
		public Vector2 center;

		public Vector2 size;

		public ColliderProperties(Vector2 center, Vector2 size)
		{
			this.center = center;
			this.size = size;
		}

		public BoxCollider2D CreateCollider(GameObject gameObject)
		{
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.offset = center;
			boxCollider2D.size = size;
			boxCollider2D.isTrigger = true;
			return boxCollider2D;
		}

		public void SetCollider(BoxCollider2D boxCollider)
		{
			boxCollider.offset = center;
			boxCollider.size = size;
			boxCollider.isTrigger = true;
		}
	}

	[Serializable]
	public class ColliderPropertiesGroup
	{
		public ColliderProperties @default = new ColliderProperties(new Vector2(0f, 62f), new Vector2(50f, 105f));

		public ColliderProperties air = new ColliderProperties(new Vector2(0f, 50f), new Vector2(50f, 50f));

		public ColliderProperties ducking = new ColliderProperties(new Vector2(0f, 27f), new Vector2(50f, 35f));

		public ColliderProperties dashing;
	}

	[SerializeField]
	private ColliderPropertiesGroup colliders;

	private Dictionary<State, ColliderProperties> pairs;

	private BoxCollider2D collider;

	private State _state;

	public ColliderProperties @default
	{
		get
		{
			return colliders.@default;
		}
	}

	public float DefaultWidth
	{
		get
		{
			return colliders.@default.size.x;
		}
	}

	public float DefaultHeight
	{
		get
		{
			return colliders.@default.size.y;
		}
	}

	public float Width
	{
		get
		{
			return pairs[_state].size.x;
		}
	}

	public float Height
	{
		get
		{
			return pairs[_state].size.y;
		}
	}

	public Vector2 Center
	{
		get
		{
			return new Vector2(collider.offset.x, collider.offset.y * base.player.motor.GravityReversalMultiplier) + (Vector2)base.transform.position;
		}
	}

	public Vector2 DefaultCenter
	{
		get
		{
			return new Vector2(colliders.@default.center.x, colliders.@default.center.y * base.player.motor.GravityReversalMultiplier) + (Vector2)base.transform.position;
		}
	}

	public State state
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state != value)
			{
				pairs[value].SetCollider(collider);
			}
			_state = value;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<BoxCollider2D>();
		pairs = new Dictionary<State, ColliderProperties>();
		pairs[State.Default] = colliders.@default;
		pairs[State.Air] = colliders.air;
		pairs[State.Ducking] = colliders.ducking;
		pairs[State.Dashing] = colliders.ducking;
		state = State.Default;
	}

	protected override void FixedUpdate()
	{
		base.Update();
		UpdateColliders();
	}

	private void UpdateColliders()
	{
		base.gameObject.layer = ((!base.player.CanTakeDamage) ? 9 : 8);
		if (base.player.motor.Dashing)
		{
			if (state != State.Dashing)
			{
				state = State.Dashing;
			}
		}
		else if (!base.player.motor.Grounded)
		{
			if (state != State.Air)
			{
				state = State.Air;
			}
		}
		else if (base.player.motor.Ducking)
		{
			if (state != State.Ducking)
			{
				state = State.Ducking;
			}
		}
		else if (state != State.Default)
		{
			state = State.Default;
		}
	}
}
