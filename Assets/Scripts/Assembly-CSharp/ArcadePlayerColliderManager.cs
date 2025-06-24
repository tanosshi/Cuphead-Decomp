using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcadePlayerColliderManager : AbstractArcadePlayerComponent
{
	public enum State
	{
		Default = 0,
		Air = 1,
		Dashing = 2
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
		public ColliderProperties @default = new ColliderProperties(new Vector2(0f, 40f), new Vector2(33f, 70f));

		public ColliderProperties air = new ColliderProperties(new Vector2(0f, 33f), new Vector2(33f, 33f));

		public ColliderProperties dashing = new ColliderProperties(new Vector2(0f, 18f), new Vector2(33f, 23f));
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
			return collider.offset + (Vector2)base.transform.position;
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
		pairs[State.Dashing] = colliders.dashing;
		state = State.Default;
	}

	protected override void Update()
	{
		base.Update();
		UpdateColliders();
	}

	private void UpdateColliders()
	{
		collider.enabled = base.player.CanTakeDamage;
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
		else if (state != State.Default)
		{
			state = State.Default;
		}
	}
}
