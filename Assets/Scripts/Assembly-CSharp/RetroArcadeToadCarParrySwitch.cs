using UnityEngine;

public class RetroArcadeToadCarParrySwitch : ParrySwitch
{
	[SerializeField]
	private RetroArcadeToadCar parent;

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		parent.ShootMissile();
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.25f);
	}
}
