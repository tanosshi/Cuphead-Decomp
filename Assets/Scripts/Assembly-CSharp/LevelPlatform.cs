using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelPlatform : AbstractCollidableObject
{
	public bool canFallThrough = true;

	[SerializeField]
	private bool allowShadows = true;

	protected List<Transform> players { get; private set; }

	public bool AllowShadows
	{
		get
		{
			return allowShadows;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		players = new List<Transform>();
		base.gameObject.layer = LayerMask.NameToLayer(Layers.Bounds_Ground.ToString());
	}

	public virtual void AddChild(Transform player)
	{
		if (!players.Contains(player))
		{
			players.Add(player);
		}
		player.parent = base.transform;
	}

	public virtual void OnPlayerExit(Transform player)
	{
		if (players.Contains(player))
		{
			players.Remove(player);
		}
	}

	protected override void OnDestroy()
	{
		foreach (Transform player in players)
		{
			if (!(player == null))
			{
				player.parent = null;
			}
		}
		base.OnDestroy();
	}
}
