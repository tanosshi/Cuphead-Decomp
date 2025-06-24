using System;
using UnityEngine;

public class PlatformingLevelPitMoveTrigger : AbstractPausableComponent
{
	[Serializable]
	public class TriggerProperties
	{
		public Vector2 Position = Vector2.zero;

		public Vector2 Size = Vector2.one * 100f;

		public TriggerProperties(Vector2 position)
		{
			Position = position;
		}
	}

	[SerializeField]
	private float pitOffset;

	[Header("Triggers")]
	public TriggerProperties trigger = new TriggerProperties(new Vector2(-200f, 0f));

	private Rect rect;

	protected override void Start()
	{
		base.Start();
		Vector2 vector = base.transform.position;
		rect = RectUtils.NewFromCenter(trigger.Position.x + vector.x, trigger.Position.y + vector.y, trigger.Size.x, trigger.Size.y);
	}

	protected override void Update()
	{
		base.Update();
		if (rect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerOne).center) || (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && rect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerTwo).center)))
		{
			OnTriggerHit();
		}
	}

	private void OnTriggerHit()
	{
		LevelPit.Instance.ExtraOffset = pitOffset;
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
		Gizmos.DrawWireCube(base.baseTransform.position + (Vector3)trigger.Position, trigger.Size);
	}
}
