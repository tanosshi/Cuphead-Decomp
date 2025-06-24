using System.Collections;
using UnityEngine;

public class PlaneWeaponBombExProjectile : AbstractProjectile
{
	[SerializeField]
	private float spriteRotation;

	[SerializeField]
	private Effect trailFxPrefab;

	[SerializeField]
	private Transform trailFxRoot;

	[SerializeField]
	private float trailFxMaxOffset;

	[SerializeField]
	private float trailDelay;

	[SerializeField]
	private float destroyPadding;

	[SerializeField]
	private SpriteRenderer Cuphead;

	[SerializeField]
	private SpriteRenderer Mugman;

	public float speed;

	public MinMax rotationSpeed;

	public float timeBeforeEaseRotationSpeed;

	public float rotationSpeedEaseTime;

	public float rotation;

	private Vector2 velocity;

	private float t;

	private bool move = true;

	private Collider2D target;

	private AbstractPlayerController player;

	private float timeSinceUpdateRotation = 1f / 24f;

	protected override float DestroyLifetime
	{
		get
		{
			return 100f;
		}
	}

	public void Init()
	{
		Cuphead.enabled = PlayerId == PlayerId.PlayerOne;
		Mugman.enabled = PlayerId != PlayerId.PlayerOne;
		StartCoroutine(trail_cr());
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (!(hit.tag == "Parry"))
		{
			base.OnCollisionOther(hit, phase);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		DealDamage(hit);
		base.OnCollisionEnemy(hit, phase);
	}

	private void DealDamage(GameObject hit)
	{
		base.damageDealer.DealDamage(hit);
	}

	protected override void Die()
	{
		move = false;
		base.transform.rotation.SetEulerAngles(0f, 0f, Random.Range(0f, 360f));
		base.Die();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!move)
		{
			return;
		}
		t += CupheadTime.FixedDelta;
		if (target != null && target.gameObject.activeInHierarchy && target.isActiveAndEnabled && t < WeaponProperties.LevelWeaponHoming.Basic.maxHomingTime)
		{
			float num;
			for (num = MathUtils.DirectionToAngle(target.bounds.center - base.transform.position); num > rotation + 180f; num -= 360f)
			{
			}
			for (; num < rotation - 180f; num += 360f)
			{
			}
			float num2 = rotationSpeed.min;
			if (t > timeBeforeEaseRotationSpeed + rotationSpeedEaseTime)
			{
				num2 = rotationSpeed.max;
			}
			else if (t > timeBeforeEaseRotationSpeed)
			{
				num2 = rotationSpeed.GetFloatAt((t - timeBeforeEaseRotationSpeed) / rotationSpeedEaseTime);
			}
			if (Mathf.Abs(num - rotation) < num2 * CupheadTime.FixedDelta)
			{
				rotation = num;
			}
			else if (num > rotation)
			{
				rotation += num2 * CupheadTime.FixedDelta;
			}
			else
			{
				rotation -= num2 * CupheadTime.FixedDelta;
			}
		}
		Vector3 vector = MathUtils.AngleToDirection(rotation);
		base.transform.position += vector * speed * CupheadTime.FixedDelta;
		base.transform.SetEulerAngles(0f, 0f, rotation + spriteRotation);
		if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(destroyPadding, destroyPadding)))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void FindTarget()
	{
		float num = float.MaxValue;
		Collider2D collider2D = null;
		Vector2 vector = (Vector2)base.transform.position + speed * (timeBeforeEaseRotationSpeed + rotationSpeedEaseTime * 0.75f) * MathUtils.AngleToDirection(rotation);
		DamageReceiver[] array = Object.FindObjectsOfType<DamageReceiver>();
		foreach (DamageReceiver damageReceiver in array)
		{
			if (!damageReceiver.gameObject.activeInHierarchy || damageReceiver.type != DamageReceiver.Type.Enemy)
			{
				continue;
			}
			Collider2D[] components = damageReceiver.GetComponents<Collider2D>();
			foreach (Collider2D collider2D2 in components)
			{
				if (collider2D2.isActiveAndEnabled && CupheadLevelCamera.Current.ContainsPoint(collider2D2.bounds.center, collider2D2.bounds.size / 2f))
				{
					float sqrMagnitude = (vector - (Vector2)collider2D2.bounds.center).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						collider2D = collider2D2;
					}
				}
			}
			DamageReceiverChild[] componentsInChildren = damageReceiver.GetComponentsInChildren<DamageReceiverChild>();
			foreach (DamageReceiverChild damageReceiverChild in componentsInChildren)
			{
				Collider2D[] components2 = damageReceiverChild.GetComponents<Collider2D>();
				foreach (Collider2D collider2D3 in components2)
				{
					if (collider2D3.isActiveAndEnabled && CupheadLevelCamera.Current.ContainsPoint(collider2D3.bounds.center, collider2D3.bounds.size / 2f))
					{
						float sqrMagnitude2 = (vector - (Vector2)collider2D3.bounds.center).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							collider2D = collider2D3;
						}
					}
				}
			}
		}
		target = collider2D;
	}

	private IEnumerator trail_cr()
	{
		while (!base.dead)
		{
			yield return CupheadTime.WaitForSeconds(this, trailDelay);
			if (base.dead)
			{
				break;
			}
			trailFxPrefab.Create((Vector2)trailFxRoot.position + MathUtils.RandomPointInUnitCircle() * trailFxMaxOffset);
		}
	}
}
