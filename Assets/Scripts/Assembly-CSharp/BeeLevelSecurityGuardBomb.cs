using System.Collections;
using UnityEngine;

public class BeeLevelSecurityGuardBomb : AbstractProjectile
{
	[SerializeField]
	private BasicProjectile childPrefab;

	private int direction;

	private float idleTime;

	private float warningTime;

	private float childSpeed;

	private int childCount;

	public BeeLevelSecurityGuardBomb Create(Vector2 pos, int direction, float idleTime, float warningTime, float childSpeed, int childCount)
	{
		BeeLevelSecurityGuardBomb beeLevelSecurityGuardBomb = base.Create() as BeeLevelSecurityGuardBomb;
		beeLevelSecurityGuardBomb.direction = direction;
		beeLevelSecurityGuardBomb.idleTime = idleTime;
		beeLevelSecurityGuardBomb.warningTime = warningTime;
		beeLevelSecurityGuardBomb.childSpeed = childSpeed;
		beeLevelSecurityGuardBomb.childCount = childCount;
		beeLevelSecurityGuardBomb.transform.position = pos;
		return beeLevelSecurityGuardBomb;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(go_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
		float num = 360 / childCount;
		for (int i = 0; i < childCount; i++)
		{
			BasicProjectile basicProjectile = childPrefab.Create(base.transform.position, num * (float)i, Vector2.one, childSpeed);
			basicProjectile.SetParryable(i % 2 != 0);
		}
	}

	private IEnumerator go_cr()
	{
		yield return TweenPosition(time: 0.3f, end: (Vector2)base.transform.position + new Vector2(50 * direction, 100f), start: base.transform.position, ease: EaseUtils.EaseType.easeOutSine);
		yield return CupheadTime.WaitForSeconds(this, idleTime);
		AudioManager.PlayLoop("bee_guard_bomb_warning");
		emitAudioFromObject.Add("bee_guard_bomb_warning");
		animator.Play("Warning");
		yield return CupheadTime.WaitForSeconds(this, warningTime);
		AudioManager.Stop("bee_guard_bomb_warning");
		AudioManager.Play("bee_guard_bomb_explode");
		emitAudioFromObject.Add("bee_guard_bomb_explode");
		Die();
	}
}
