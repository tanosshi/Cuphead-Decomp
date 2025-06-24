using System.Collections;
using UnityEngine;

public class ForestPlatformingLevelChomper : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private float speed = 1000f;

	[SerializeField]
	private float gravityUp = 1600f;

	[SerializeField]
	private float gravityDown = 2400f;

	[SerializeField]
	private MinMax initialDelay = new MinMax(0f, 0.5f);

	[SerializeField]
	private MinMax mainDelay = new MinMax(1f, 3f);

	private const float UP_ANIM_LENGTH = 0.333333f;

	private const float BITE_ANIM_TIME_TO_APEX = 0.333333f;

	private const float FREEZE_TIME = 1f / 24f;

	private const float ON_SCREEN_SOUND_PADDING = 100f;

	private float startY;

	private bool moving;

	protected override void OnStart()
	{
		startY = base.transform.position.y;
	}

	public void StartAttacking()
	{
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, initialDelay.RandomFloat());
		float timeToApex = speed / gravityUp;
		float upAnimTime = timeToApex - 0.333333f;
		int numUpAnimLoops = (int)(upAnimTime / 0.333333f);
		float normalizedExtraTime = upAnimTime / 0.333333f % 1f;
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
			{
				AudioManager.Play("level_chomper_up");
				emitAudioFromObject.Add("level_chomper_up");
			}
			base.animator.Play("Up", 0, 1f - normalizedExtraTime);
			StartCoroutine(move_cr());
			yield return CupheadTime.WaitForSeconds(this, upAnimTime - 0.1666665f);
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
			{
				AudioManager.Play("level_chomper_bite");
				emitAudioFromObject.Add("level_chomper_bite");
			}
			base.animator.SetTrigger("Bite");
			while (moving)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, mainDelay.RandomFloat());
		}
	}

	private IEnumerator move_cr()
	{
		float timeToApex = speed / gravityUp;
		float t = 0f;
		moving = true;
		while (t < timeToApex)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetPosition(null, startY + speed * t - 0.5f * gravityUp * t * t);
			yield return new WaitForFixedUpdate();
		}
		yield return CupheadTime.WaitForSeconds(this, 1f / 24f);
		float apexY = base.transform.position.y;
		t = 0f;
		while (t < timeToApex)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetPosition(null, apexY - 0.5f * gravityDown * t * t);
			yield return new WaitForFixedUpdate();
		}
		moving = false;
	}
}
