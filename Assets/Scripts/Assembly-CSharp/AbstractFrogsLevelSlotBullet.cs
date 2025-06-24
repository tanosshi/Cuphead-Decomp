using System.Collections;
using UnityEngine;

public abstract class AbstractFrogsLevelSlotBullet : AbstractPausableComponent
{
	protected DamageDealer damageDealer;

	protected float speed;

	protected virtual float Y
	{
		get
		{
			return Level.Current.Ground + 50;
		}
	}

	protected virtual float Y_Time
	{
		get
		{
			return 0.45f;
		}
	}

	protected virtual EaseUtils.EaseType Y_Ease
	{
		get
		{
			return EaseUtils.EaseType.easeOutBounce;
		}
	}

	public AbstractFrogsLevelSlotBullet Create(Vector2 pos, float speed)
	{
		AbstractFrogsLevelSlotBullet abstractFrogsLevelSlotBullet = InstantiatePrefab<AbstractFrogsLevelSlotBullet>();
		abstractFrogsLevelSlotBullet.transform.SetPosition(pos.x, pos.y);
		abstractFrogsLevelSlotBullet.speed = speed;
		return abstractFrogsLevelSlotBullet;
	}

	protected override void Start()
	{
		base.Start();
		damageDealer = new DamageDealer(1f, 0.3f, DamageDealer.DamageSource.Enemy, true, false, false);
		damageDealer.SetDirection(DamageDealer.Direction.Neutral, base.transform);
		GameObject gameObject = new GameObject("Damage Shit!");
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.ResetLocalTransforms();
		BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.size = new Vector2(240f, 40f);
		boxCollider2D.isTrigger = true;
		CollisionChild collisionChild = gameObject.AddComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += DealDamage;
		StartCoroutine(x_cr());
		StartCoroutine(y_cr());
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
	}

	protected void DealDamage(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
	}

	protected virtual void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator x_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			if (base.transform.position.x < -1280f)
			{
				End();
			}
			base.transform.AddPosition((0f - speed) * CupheadTime.FixedDelta);
			yield return wait;
		}
	}

	private IEnumerator y_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float start = base.transform.position.y;
		float t = 0f;
		while (t < Y_Time)
		{
			TransformExtensions.SetPosition(y: EaseUtils.Ease(value: t / Y_Time, ease: Y_Ease, start: start, end: Y), transform: base.transform);
			t += CupheadTime.FixedDelta;
			yield return wait;
		}
	}
}
