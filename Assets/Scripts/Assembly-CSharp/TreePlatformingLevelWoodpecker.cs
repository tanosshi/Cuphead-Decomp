using System.Collections;
using UnityEngine;

public class TreePlatformingLevelWoodpecker : PlatformingLevelShootingEnemy
{
	[SerializeField]
	private Transform setEndPos;

	private Vector2 endPos;

	private Vector2 midPos;

	private Vector2 startPos;

	private bool isDown;

	protected override void Start()
	{
		base.Start();
		isDown = false;
		startPos = base.transform.position;
		endPos = setEndPos.transform.position;
		midPos = new Vector3(endPos.x, endPos.y + 200f);
		GetComponent<DamageReceiver>().enabled = false;
	}

	protected override void Shoot()
	{
		if (!isDown)
		{
			StartCoroutine(move_down_cr());
		}
	}

	private IEnumerator move_down_cr()
	{
		isDown = true;
		base.animator.SetBool("movingDown", true);
		float t = 0f;
		Vector2 start = base.transform.position;
		while (t < base.Properties.WoodpeckermoveDownTime)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / base.Properties.WoodpeckermoveDownTime);
			base.transform.position = Vector2.Lerp(start, midPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = midPos;
		start = base.transform.position;
		yield return CupheadTime.WaitForSeconds(this, base.Properties.WoodpeckerWarningDuration);
		base.animator.SetTrigger("Continue");
		t = 0f;
		while (t < 0.2f)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / 0.5f);
			base.transform.position = Vector2.Lerp(start, endPos, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		base.transform.position = endPos;
		start = base.transform.position;
		base.animator.SetBool("isAttacking", true);
		CupheadLevelCamera.Current.Shake(10f, base.Properties.WoodpeckerAttackDuration);
		yield return CupheadTime.WaitForSeconds(this, base.Properties.WoodpeckerAttackDuration);
		base.animator.SetBool("isAttacking", false);
		while (t < base.Properties.WoodpeckermoveUpTime)
		{
			float val3 = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / base.Properties.WoodpeckermoveUpTime);
			base.transform.position = Vector2.Lerp(start, startPos, val3);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetBool("movingDown", false);
		isDown = false;
		yield return null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 1f, 0f, 1f);
		Gizmos.DrawWireSphere(endPos, 100f);
	}

	private void SoundWoodpeckerStart()
	{
		AudioManager.Play("level_platform_woodpecker_attack_start");
		emitAudioFromObject.Add("level_platform_woodpecker_attack_start");
	}
}
