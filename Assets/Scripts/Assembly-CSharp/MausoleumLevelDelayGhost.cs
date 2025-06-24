using System.Collections;
using UnityEngine;

public class MausoleumLevelDelayGhost : MausoleumLevelGhostBase
{
	private float rotation;

	private LevelProperties.Mausoleum.DelayGhost properties;

	public MausoleumLevelDelayGhost Create(Vector2 position, float rotation, float speed, LevelProperties.Mausoleum.DelayGhost properties)
	{
		MausoleumLevelDelayGhost mausoleumLevelDelayGhost = base.Create(position, rotation, speed) as MausoleumLevelDelayGhost;
		mausoleumLevelDelayGhost.rotation = rotation;
		mausoleumLevelDelayGhost.properties = properties;
		return mausoleumLevelDelayGhost;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(wait_cr());
	}

	private IEnumerator wait_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.dashDelay);
		Speed = properties.speed;
		yield return null;
	}
}
