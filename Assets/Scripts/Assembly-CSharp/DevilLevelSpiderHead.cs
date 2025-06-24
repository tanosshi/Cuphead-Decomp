using System.Collections;
using UnityEngine;

public class DevilLevelSpiderHead : AbstractCollidableObject
{
	public enum State
	{
		Idle = 0,
		Attacking = 1
	}

	public State state;

	private DamageDealer damageDealer;

	[SerializeField]
	private float moveDistanceY;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void Attack(float xPos, float downSpeed, float upSpeed)
	{
		base.animator.SetBool("IsFalling", true);
		state = State.Attacking;
		base.transform.SetPosition(xPos);
		StartCoroutine(attack_cr(downSpeed, upSpeed));
	}

	private IEnumerator attack_cr(float downSpeed, float upSpeed)
	{
		float moveTime = Mathf.Abs(moveDistanceY) / downSpeed;
		float startY = base.transform.position.y;
		float t = 0f;
		GetComponent<Collider2D>().enabled = true;
		AudioManager.Play("devil_spider_fall");
		emitAudioFromObject.Add("devil_spider_fall");
		for (; t < moveTime; t += CupheadTime.FixedDelta)
		{
			base.transform.SetPosition(null, EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, startY, startY + moveDistanceY, t / moveTime));
			yield return new WaitForFixedUpdate();
		}
		AudioManager.Play("devil_spider_head_hit_floor");
		emitAudioFromObject.Add("devil_spider_head_hit_floor");
		base.animator.SetBool("IsFalling", false);
		base.transform.SetPosition(null, startY + moveDistanceY);
		t = 0f;
		moveTime = Mathf.Abs(moveDistanceY) / upSpeed;
		yield return base.animator.WaitForAnimationToEnd(this, "Fall_Splat");
		for (; t < moveTime; t += CupheadTime.FixedDelta)
		{
			base.transform.SetPosition(null, EaseUtils.Ease(EaseUtils.EaseType.easeInSine, startY + moveDistanceY, startY, t / moveTime));
			yield return new WaitForFixedUpdate();
		}
		base.transform.SetPosition(null, startY);
		state = State.Idle;
		GetComponent<Collider2D>().enabled = false;
	}
}
