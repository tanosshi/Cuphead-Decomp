using System;
using UnityEngine;

public class ForestPlatformingLevelChomperSpawner : AbstractPausableComponent
{
	[Serializable]
	public class TriggerProperties
	{
		public Vector2 Position = Vector2.zero;

		public Vector2 Size = Vector2.one * 100f;

		public float xVariation;

		public TriggerProperties(Vector2 position)
		{
			Position = position;
		}
	}

	[Header("Triggers")]
	public TriggerProperties startTrigger = new TriggerProperties(new Vector2(-200f, 0f));

	[Header("Chompers")]
	public ForestPlatformingLevelChomper[] chompers;

	private bool started;

	private Rect startRect;

	protected override void Start()
	{
		base.Start();
		started = false;
		Vector2 vector = base.transform.position;
		startRect = RectUtils.NewFromCenter(startTrigger.Position.x + vector.x, startTrigger.Position.y + vector.y, startTrigger.Size.x + UnityEngine.Random.Range(0f - startTrigger.xVariation, startTrigger.xVariation), startTrigger.Size.y);
	}

	protected override void Update()
	{
		base.Update();
		if (startRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerOne).center) || (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && startRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerTwo).center)))
		{
			OnStartTriggerHit();
		}
	}

	private void OnStartTriggerHit()
	{
		if (started)
		{
			return;
		}
		started = true;
		ForestPlatformingLevelChomper[] array = chompers;
		foreach (ForestPlatformingLevelChomper forestPlatformingLevelChomper in array)
		{
			if (forestPlatformingLevelChomper != null)
			{
				forestPlatformingLevelChomper.StartAttacking();
			}
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Gizmos.color = new Color(0f, 1f, 0f, a);
		Gizmos.DrawWireCube(base.baseTransform.position + (Vector3)startTrigger.Position, startTrigger.Size);
	}
}
