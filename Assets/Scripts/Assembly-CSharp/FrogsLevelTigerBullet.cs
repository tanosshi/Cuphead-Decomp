using System.Collections;
using UnityEngine;

public class FrogsLevelTigerBullet : AbstractFrogsLevelSlotBullet
{
	private const float BULLET_TIME = 0.5f;

	private const float BULLET_HEIGHT = 500f;

	public CollisionChild bullet;

	protected override void Start()
	{
		base.Start();
		bullet.OnPlayerCollision += base.DealDamage;
		StartCoroutine(bullet_cr());
	}

	private IEnumerator bullet_cr()
	{
		float t = 0f;
		Transform trans = bullet.transform;
		float start = trans.localPosition.y;
		float end = start + 500f;
		while (true)
		{
			t = 0f;
			AudioManager.Play("level_frogs_ball_platform_ball_launch");
			while (t < 0.5f)
			{
				float val = t / 0.5f;
				float y = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, start, end, val);
				trans.SetLocalPosition(null, y);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
			while (t < 0.5f)
			{
				float val2 = t / 0.5f;
				float y2 = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, end, start, val2);
				trans.SetLocalPosition(null, y2);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
		}
	}
}
