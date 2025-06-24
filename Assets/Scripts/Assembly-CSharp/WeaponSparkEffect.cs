using UnityEngine;

public class WeaponSparkEffect : Effect
{
	private LevelPlayerController player;

	private float playerXScale;

	public void SetPlayer(LevelPlayerController player)
	{
		if (player.motor.Grounded)
		{
			this.player = player;
			Vector3 localScale = base.transform.localScale;
			base.transform.parent = player.transform;
			base.transform.localScale = localScale;
			playerXScale = player.transform.localScale.x;
		}
	}

	public void BringToFrontOfPlayer()
	{
		GetComponent<SpriteRenderer>().sortingOrder = 100;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (player != null && !player.motor.Grounded)
		{
			player = null;
			base.transform.parent = null;
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (player != null && player.transform.localScale.x != playerXScale)
		{
			base.transform.SetLocalPosition(0f - base.transform.localPosition.x);
			player = null;
			base.transform.parent = null;
		}
	}
}
