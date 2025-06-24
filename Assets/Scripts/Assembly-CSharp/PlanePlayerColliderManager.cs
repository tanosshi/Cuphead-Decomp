using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanePlayerColliderManager : AbstractPlanePlayerComponent
{
	public enum State
	{
		Default = 0,
		Shrunk = 1
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
		public ColliderProperties @default = new ColliderProperties(new Vector2(-10f, 20f), new Vector2(75f, 75f));

		public ColliderProperties shrunk = new ColliderProperties(new Vector2(-10f, 20f), new Vector2(45f, 45f));
	}

	[SerializeField]
	private ColliderPropertiesGroup colliders;

	private Dictionary<State, ColliderProperties> pairs;

	private BoxCollider2D collider;

	private State _state;

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

	public ColliderProperties @default
	{
		get
		{
			return colliders.@default;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<BoxCollider2D>();
		pairs = new Dictionary<State, ColliderProperties>();
		pairs[State.Default] = colliders.@default;
		pairs[State.Shrunk] = colliders.shrunk;
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
		if (base.player.Shrunk)
		{
			if (state != State.Shrunk)
			{
				state = State.Shrunk;
			}
		}
		else if (state != State.Default)
		{
			state = State.Default;
		}
	}
}
