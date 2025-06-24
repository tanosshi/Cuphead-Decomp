using UnityEngine;

public class LevelPit : AbstractCollidableObject
{
	private const float PLATFORMING_LEVEL_CAMERA_OFFSET_Y = -500f;

	[SerializeField]
	private float extraOffset;

	public static LevelPit Instance { get; private set; }

	public float ExtraOffset
	{
		get
		{
			return extraOffset;
		}
		set
		{
			extraOffset = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (Level.Current.LevelType == Level.Type.Platforming)
		{
			Instance = this;
			base.transform.SetParent(CupheadLevelCamera.Current.transform);
			base.transform.ResetLocalTransforms();
			base.transform.SetLocalPosition(0f, -500f, 0f);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		CheckPlayer(PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController);
		CheckPlayer(PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController);
	}

	private void CheckPlayer(LevelPlayerController player)
	{
		if (player == null || player.IsDead)
		{
			return;
		}
		float num = 1f;
		if (Level.Current.LevelType == Level.Type.Platforming)
		{
			num *= 1.3f;
		}
		if (player.motor.GravityReversed && Level.Current.LevelType == Level.Type.Platforming)
		{
			float num2 = base.transform.parent.position.y - base.transform.localPosition.y;
			if (player.transform.position.y >= num2 - extraOffset)
			{
				player.OnPitKnockUp(num2 - extraOffset, num);
			}
		}
		else if (player.transform.position.y <= base.transform.position.y + extraOffset)
		{
			player.OnPitKnockUp(base.transform.position.y + extraOffset, num);
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.3f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Rect rect = new Rect(base.baseTransform.position.x + -1000f, base.baseTransform.position.y, 2000f, 0f);
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Gizmos.DrawLine(new Vector2(rect.xMin, rect.y), new Vector2(rect.xMax, rect.y));
		for (int i = 0; i < 20; i++)
		{
			float num = 100f;
			Rect rect2 = new Rect(rect.xMin + num * (float)i, rect.y, num, -20f);
			Gizmos.DrawLine(new Vector2(rect2.xMin, rect2.y), new Vector2(rect2.center.x, rect2.yMax));
			Gizmos.DrawLine(new Vector2(rect2.xMax, rect2.y), new Vector2(rect2.center.x, rect2.yMax));
		}
	}
}
