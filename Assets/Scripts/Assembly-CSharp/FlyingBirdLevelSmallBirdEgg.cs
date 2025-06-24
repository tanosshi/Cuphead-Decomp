using System.Collections;
using UnityEngine;

public class FlyingBirdLevelSmallBirdEgg : AbstractCollidableObject
{
	private DamageDealer damageDealer;

	private LevelProperties.FlyingBird properties;

	public Transform container { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		StartCoroutine(animSpeed_cr());
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		UpdateRotation();
	}

	protected override void LateUpdate()
	{
		UpdateRotation();
	}

	private void UpdateRotation()
	{
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void SetParent(Transform parent, LevelProperties.FlyingBird properties)
	{
		this.properties = properties;
		container = new GameObject("Egg Container").transform;
		container.SetParent(parent);
		container.ResetLocalPosition();
		base.transform.SetParent(container);
		base.transform.ResetLocalTransforms();
		StartCoroutine(move_cr());
	}

	public void Explode()
	{
		GetComponent<CircleCollider2D>().enabled = false;
		StartCoroutine(explode_cr());
	}

	public void OnDeathAnimComplete()
	{
		base.gameObject.SetActive(false);
	}

	private IEnumerator move_cr()
	{
		yield return TweenLocalPositionX(0f, properties.CurrentState.smallBird.eggRange.max, properties.CurrentState.smallBird.eggMoveTime, EaseUtils.EaseType.easeOutCubic);
		while (true)
		{
			yield return TweenLocalPositionX(properties.CurrentState.smallBird.eggRange.max, properties.CurrentState.smallBird.eggRange.min, properties.CurrentState.smallBird.eggMoveTime, EaseUtils.EaseType.easeInOutSine);
			yield return TweenLocalPositionX(properties.CurrentState.smallBird.eggRange.min, properties.CurrentState.smallBird.eggRange.max, properties.CurrentState.smallBird.eggMoveTime, EaseUtils.EaseType.easeInOutSine);
		}
	}

	private IEnumerator explode_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0, 1));
		base.transform.SetLocalEulerAngles(0f, 0f, Random.Range(0, 360));
		base.animator.Play("Explode");
	}

	private IEnumerator animSpeed_cr()
	{
		yield return null;
	}
}
