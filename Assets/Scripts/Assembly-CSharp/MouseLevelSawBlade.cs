using System.Collections;
using UnityEngine;

public class MouseLevelSawBlade : AbstractCollidableObject
{
	public enum State
	{
		Init = 0,
		Idle = 1,
		Warning = 2,
		Attack = 3
	}

	private const string SawParameterName = "SawID";

	private const string StickParameterName = "StickID";

	private const string AttackParameterName = "Attacking";

	private const int SawIdMax = 6;

	private const int StickIdMax = 8;

	[SerializeField]
	private float initX;

	[SerializeField]
	private float idleX;

	[SerializeField]
	private float attackMinX;

	[SerializeField]
	private float attackMaxX;

	[SerializeField]
	private Transform blade;

	[SerializeField]
	private float rotateSpeed;

	[Range(0f, 5f)]
	[SerializeField]
	private int sawId;

	[Range(0f, 7f)]
	[SerializeField]
	private int stickId;

	private LevelProperties.Mouse properties;

	private bool fullAttacking;

	private bool goBackwards;

	private float attackX;

	private float fullAttackX;

	private float progress;

	private DamageDealer damageDealer;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		GetComponent<Collider2D>().enabled = false;
	}

	protected override void Start()
	{
		base.Start();
		base.animator.SetFloat("SawID", (float)sawId / 5f);
		base.animator.SetFloat("StickID", (float)stickId / 7f);
	}

	protected override void Update()
	{
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

	public void Begin(LevelProperties.Mouse properties)
	{
		this.properties = properties;
		StartCoroutine(intro_cr());
		attackX = attackMinX;
		fullAttackX = attackMinX;
	}

	public void Attack()
	{
		AudioManager.Play("level_mouse_buzzsaw_small");
		if (state == State.Idle)
		{
			state = State.Warning;
			StartCoroutine(attack_cr(false));
		}
	}

	public void FullAttack()
	{
		if (state == State.Warning)
		{
			StopAllCoroutines();
		}
		else if (state == State.Idle)
		{
			state = State.Warning;
		}
		fullAttacking = true;
		StartCoroutine(attack_cr(true));
	}

	private IEnumerator intro_cr()
	{
		float t = 0f;
		float introTime = Mathf.Abs(idleX - initX) / properties.CurrentState.brokenCanSawBlades.entrySpeed;
		while (t < introTime)
		{
			if (t > introTime * 0.75f)
			{
				GetComponent<Collider2D>().enabled = true;
			}
			base.transform.SetLocalPosition(Mathf.Lerp(initX, idleX, t / introTime));
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(idleX);
		state = State.Idle;
	}

	private IEnumerator attack_cr(bool fullAttack)
	{
		LevelProperties.Mouse.BrokenCanSawBlades p = properties.CurrentState.brokenCanSawBlades;
		float t = 0f;
		while (t < p.delayBeforeAttack)
		{
			progress = t / p.delayBeforeAttack;
			float x = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, idleX, attackMinX, progress);
			setX(x, fullAttack);
			t += (float)CupheadTime.Delta;
			blade.transform.Rotate(Vector3.forward, rotateSpeed / 2f * (float)CupheadTime.Delta);
			yield return null;
		}
		base.animator.SetBool("Attacking", true);
		state = State.Attack;
		float attackTime = 2f * Mathf.Abs(attackMaxX - attackMinX) / p.speed;
		t = 0f;
		while (t < attackTime)
		{
			float start = attackMinX;
			progress = t / attackTime * 2f;
			if (progress > 1f)
			{
				start = idleX;
				progress = 2f - progress;
				blade.transform.Rotate(Vector3.forward, EaseUtils.EaseInOutSine(0f, rotateSpeed, progress) * (float)CupheadTime.Delta);
			}
			else
			{
				blade.transform.Rotate(Vector3.forward, rotateSpeed * (float)CupheadTime.Delta);
			}
			float x2 = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, start, attackMaxX, progress);
			setX(x2, fullAttack);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		setX(idleX, fullAttack);
		if (fullAttack)
		{
			fullAttacking = false;
		}
		if (!fullAttacking)
		{
			base.animator.SetBool("Attacking", false);
			attackX = attackMinX;
			fullAttackX = attackMinX;
			state = State.Idle;
		}
	}

	public void Leave()
	{
		StopAllCoroutines();
		StartCoroutine(leave_cr());
	}

	private IEnumerator leave_cr()
	{
		float leaveTime = 0f;
		if (state == State.Warning)
		{
			state = State.Idle;
			base.animator.SetBool("Attacking", false);
		}
		leaveTime = ((state != State.Attack) ? 2f : (Mathf.Abs(base.transform.localPosition.x - initX) / properties.CurrentState.brokenCanSawBlades.speed));
		float t = 0f;
		float startingX = base.transform.localPosition.x;
		while (t < leaveTime)
		{
			if (t > leaveTime * 0.25f)
			{
				GetComponent<Collider2D>().enabled = false;
			}
			base.transform.SetLocalPosition(Mathf.Lerp(startingX, initX, t / leaveTime));
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(initX);
	}

	private void setX(float x, bool fullAttack)
	{
		if (fullAttack)
		{
			fullAttackX = x;
		}
		else
		{
			attackX = x;
		}
		if (idleX > 0f)
		{
			base.transform.SetLocalPosition(Mathf.Min(attackX, fullAttackX));
		}
		else
		{
			base.transform.SetLocalPosition(Mathf.Max(attackX, fullAttackX));
		}
	}
}
