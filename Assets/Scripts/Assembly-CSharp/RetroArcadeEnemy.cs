using System.Collections;
using UnityEngine;

public abstract class RetroArcadeEnemy : AbstractCollidableObject
{
	public enum Type
	{
		A = 0,
		B = 1,
		C = 2,
		None = 3,
		IsBoss = 4
	}

	[SerializeField]
	public Type type;

	private static int COUNTER;

	private static Type LAST_TYPE;

	private Coroutine moveCoroutine;

	private float pointsBonusAccuracy;

	private bool inComboChain;

	protected float hp;

	protected bool movingY;

	private DamageDealer damageDealer;

	public bool IsDead { get; protected set; }

	public float PointsWorth { get; protected set; }

	public float PointsBonus { get; protected set; }

	protected override void Awake()
	{
		base.Awake();
		if (GetComponent<DamageReceiver>() != null)
		{
			GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		}
		damageDealer = DamageDealer.NewEnemy();
		IsDead = false;
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
		damageDealer.DealDamage(hit);
	}

	protected virtual void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		RetroArcadeLevel.TOTAL_POINTS += PointsWorth;
		hp -= info.damage;
		if (hp < 0f && !IsDead)
		{
			Die();
		}
	}

	public void MoveY(float moveAmount, float moveSpeed)
	{
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
		}
		movingY = true;
		moveCoroutine = StartCoroutine(moveY_cr(moveAmount, Mathf.Abs(moveAmount) / moveSpeed));
	}

	private IEnumerator moveY_cr(float moveAmount, float time)
	{
		float t = 0f;
		float startY = base.transform.position.y;
		float endY = startY + moveAmount;
		while (t < time)
		{
			base.transform.SetPosition(null, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, startY, endY, t / time));
			t += CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
		base.transform.SetPosition(null, endY);
		movingY = false;
	}

	public virtual void Die()
	{
		if (type != Type.IsBoss)
		{
			CheckForColorBonus();
		}
		Collider2D component = GetComponent<Collider2D>();
		if (component != null)
		{
			component.enabled = false;
		}
		IsDead = true;
		GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.25f);
	}

	private void CheckForColorBonus()
	{
		if (type == LAST_TYPE)
		{
			COUNTER++;
			if (COUNTER >= 3)
			{
				GiveBonus();
				COUNTER = 0;
				LAST_TYPE = Type.None;
			}
			else
			{
				LAST_TYPE = type;
			}
		}
		else
		{
			COUNTER = 1;
			LAST_TYPE = type;
		}
	}

	protected virtual void GiveBonus()
	{
		RetroArcadeLevel.TOTAL_POINTS += PointsBonus;
	}
}
