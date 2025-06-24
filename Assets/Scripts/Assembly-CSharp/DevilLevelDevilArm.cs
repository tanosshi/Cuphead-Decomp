using System.Collections;
using UnityEngine;

public class DevilLevelDevilArm : AbstractCollidableObject
{
	public enum State
	{
		Idle = 0,
		Attacking = 1
	}

	public State state;

	private bool RamSlapSFXActive;

	private DamageDealer damageDealer;

	[SerializeField]
	private Transform endPos;

	private float speed;

	private float startX;

	public bool isRight;

	protected override void Awake()
	{
		base.Awake();
		startX = base.transform.position.x;
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

	public void Attack(float speed)
	{
		state = State.Attacking;
		base.animator.SetTrigger("ArmsIn");
		StartCoroutine(attack_cr(speed));
	}

	private IEnumerator attack_cr(float speed)
	{
		this.speed = speed;
		bool isClapping = false;
		float t = 0f;
		GetComponent<Collider2D>().enabled = true;
		while (t < speed)
		{
			base.transform.SetPosition(Mathf.Lerp(startX, endPos.position.x, t / speed));
			yield return new WaitForFixedUpdate();
			t += CupheadTime.FixedDelta;
			if (t / speed > 0.85f && !isClapping)
			{
				base.animator.SetTrigger("OnAttack");
				isClapping = true;
			}
		}
		base.transform.SetPosition(endPos.position.x);
		CupheadLevelCamera.Current.Shake(20f, 0.7f);
		yield return new WaitForFixedUpdate();
	}

	public void MoveAway()
	{
		StartCoroutine(move_away_cr());
	}

	private IEnumerator move_away_cr()
	{
		float currentPos = base.transform.position.x;
		float t = 0f;
		float moveTime = speed;
		for (t = 0f; t < moveTime; t += CupheadTime.FixedDelta)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / moveTime);
			base.transform.SetPosition(Mathf.Lerp(currentPos, startX, val));
			yield return new WaitForFixedUpdate();
		}
		base.transform.SetPosition(startX);
		RamSlapSFXActive = false;
		GetComponent<Collider2D>().enabled = false;
		state = State.Idle;
		yield return null;
	}

	private void HandclapSFX()
	{
		if (isRight)
		{
			AudioManager.Play("devil_hand_clap");
		}
	}

	private void RamSlapSFX()
	{
		if (!RamSlapSFXActive)
		{
			AudioManager.Play("devil_ram_slap");
			RamSlapSFXActive = true;
		}
	}
}
