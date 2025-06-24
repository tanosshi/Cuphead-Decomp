using UnityEngine;

public class FlyingBirdLevelNursePillProjectile : BasicProjectile
{
	public enum PillColor
	{
		Yellow = 0,
		Blue = 1,
		LightPink = 2,
		DarkPink = 3
	}

	[SerializeField]
	private GameObject yellowPill;

	[SerializeField]
	private GameObject bluePill;

	[SerializeField]
	private GameObject lightPinkPill;

	[SerializeField]
	private GameObject darkPinkPill;

	public void SetPillColor(PillColor color)
	{
		switch (color)
		{
		case PillColor.Yellow:
			yellowPill.SetActive(true);
			break;
		case PillColor.Blue:
			bluePill.SetActive(true);
			break;
		case PillColor.LightPink:
			lightPinkPill.SetActive(true);
			break;
		default:
			darkPinkPill.SetActive(true);
			break;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		Object.Destroy(base.gameObject);
	}
}
