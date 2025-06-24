using UnityEngine;

public class ClownLevelCoasterKnob : ParrySwitch
{
	[SerializeField]
	private SpriteRenderer sprite;

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		player.stats.ParryOneQuarter();
		sprite.GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
	}
}
