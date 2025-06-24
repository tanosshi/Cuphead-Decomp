using UnityEngine;

public class PlayerCameraController : AbstractPlayerComponent
{
	public const float WIDTH = 200f;

	public const float HEIGHT = 300f;

	private Rect rect = default(Rect);

	public Vector2 center
	{
		get
		{
			return rect.center;
		}
	}

	public void LevelInit()
	{
		rect = default(Rect);
		rect.x = base.transform.position.x - 100f;
		rect.width = 200f;
		rect.y = base.transform.position.y - 150f;
		rect.height = 300f;
	}

	protected override void Update()
	{
		base.Update();
		if (base.basePlayer.right > rect.x + rect.width)
		{
			rect.x = base.basePlayer.right - 200f;
		}
		if (base.basePlayer.left < rect.xMin)
		{
			rect.x = base.basePlayer.left;
		}
		if (base.basePlayer.top > rect.y + 300f)
		{
			rect.y = base.basePlayer.top - 300f;
		}
		if (base.basePlayer.bottom < rect.y)
		{
			rect.y = base.basePlayer.bottom;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (PlayerDebug.Enabled)
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			Gizmos.DrawWireCube(rect.center, new Vector3(rect.width, rect.height, 1f));
			Gizmos.color = Color.white;
		}
	}
}
