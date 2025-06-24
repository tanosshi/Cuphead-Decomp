using System;
using System.Collections;
using UnityEngine;

public abstract class PlatformingLevelEnemySpawner : AbstractPausableComponent
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

	public bool destroyEnemyAfterLeavingScreen = true;

	[Header("Spawning Properties")]
	public MinMax spawnDelay = new MinMax(2f, 2f);

	public MinMax initalSpawnDelay = new MinMax(0f, 0f);

	[Header("Triggers")]
	public TriggerProperties startTrigger = new TriggerProperties(new Vector2(-200f, 0f));

	public TriggerProperties stopTrigger = new TriggerProperties(new Vector2(200f, 0f));

	private bool started;

	private bool ended;

	private Rect startRect;

	private Rect stopRect;

	protected override void Start()
	{
		base.Start();
		started = false;
		ended = false;
		Vector2 vector = base.transform.position;
		startRect = RectUtils.NewFromCenter(startTrigger.Position.x + vector.x, startTrigger.Position.y + vector.y, startTrigger.Size.x, startTrigger.Size.y);
		stopRect = RectUtils.NewFromCenter(stopTrigger.Position.x + vector.x, stopTrigger.Position.y + vector.y, stopTrigger.Size.x, stopTrigger.Size.y);
	}

	protected override void Update()
	{
		base.Update();
		if (startRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerOne).center) || (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && startRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerTwo).center)))
		{
			OnStartTriggerHit();
		}
		if (stopRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerOne).center) || (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && stopRect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerTwo).center)))
		{
			OnStopTriggerHit();
		}
	}

	private void OnStartTriggerHit()
	{
		if (!started)
		{
			started = true;
			StartSpawning();
			StartCoroutine(loop_cr());
		}
	}

	private void OnStopTriggerHit()
	{
		if (started && !ended)
		{
			ended = true;
			EndSpawning();
			StopAllCoroutines();
		}
	}

	protected virtual void EndSpawning()
	{
	}

	protected virtual void StartSpawning()
	{
	}

	protected virtual void Spawn()
	{
	}

	private IEnumerator loop_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, initalSpawnDelay.RandomFloat());
		while (true)
		{
			Spawn();
			yield return CupheadTime.WaitForSeconds(this, spawnDelay.RandomFloat());
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
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Gizmos.DrawWireCube(base.baseTransform.position + (Vector3)stopTrigger.Position, stopTrigger.Size);
	}
}
