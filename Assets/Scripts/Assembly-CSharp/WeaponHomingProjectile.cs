using System;
using UnityEngine;

public class WeaponHomingProjectile : AbstractProjectile
{
	public enum State
	{
		Homing = 0,
		Swirling = 1
	}

	[Space(10f)]
	public Sfx SfxOnDeath;

	[SerializeField]
	private float spriteRotation;

	[SerializeField]
	private float trailSpriteRotation;

	[SerializeField]
	private Transform trail;

	[SerializeField]
	private float destroyPadding;

	public float speed;

	public MinMax rotationSpeed;

	public float timeBeforeEaseRotationSpeed;

	public float rotationSpeedEaseTime;

	public float rotation;

	public float swirlDistance;

	public float swirlEaseTime;

	public int trailFollowFrames;

	private State state;

	private Vector2 velocity;

	private float t;

	private bool move = true;

	private Collider2D target;

	private float swirlLaunchRotation;

	private float swirlRotation;

	private AbstractPlayerController player;

	private Vector2 swirlLaunchPos;

	private float timeSinceUpdateRotation = 1f / 24f;

	private float trailRotation;

	public bool isEx;

	private Vector2[] trailPositions = new Vector2[10];

	private float[] trailRotations = new float[10];

	private int trailFollowIndex;

	protected override float DestroyLifetime
	{
		get
		{
			return 100f;
		}
	}

	protected override void OnCollisionDie(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionDie(hit, phase);
		if (base.tag == Tags.PlayerProjectile.ToString() && phase == CollisionPhase.Enter)
		{
			if ((bool)hit.GetComponent<DamageReceiver>() && hit.GetComponent<DamageReceiver>().enabled)
			{
				AudioManager.Play("player_shoot_hit_cuphead");
			}
			else
			{
				AudioManager.Play("player_weapon_homing_impact");
			}
		}
	}

	public override AbstractProjectile Create(Vector2 position, float rotation, Vector2 scale)
	{
		WeaponHomingProjectile weaponHomingProjectile = base.Create(position, rotation, scale) as WeaponHomingProjectile;
		for (int i = 0; i < trailPositions.Length; i++)
		{
			weaponHomingProjectile.trailPositions[i] = base.transform.position;
			weaponHomingProjectile.trailRotations[i] = base.transform.eulerAngles.z;
		}
		if (MathUtils.RandomBool())
		{
			trail.SetScale(-1f);
		}
		if (MathUtils.RandomBool())
		{
			trail.SetScale(null, -1f);
		}
		return weaponHomingProjectile;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
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
		if (isEx)
		{
			AudioManager.Play("player_ex_impact_hit");
			emitAudioFromObject.Add("player_ex_impact_hit");
		}
		base.OnCollisionEnemy(hit, phase);
	}

	private void DealDamage(GameObject hit)
	{
		base.damageDealer.DealDamage(hit);
	}

	protected override void Die()
	{
		move = false;
		AudioManager.Play(SfxOnDeath);
		EffectSpawner component = GetComponent<EffectSpawner>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		trail.gameObject.SetActive(false);
		base.Die();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		switch (state)
		{
		case State.Homing:
			UpdateHoming();
			break;
		case State.Swirling:
			UpdateSwirling();
			break;
		}
		trailFollowIndex = (trailFollowIndex + 1) % trailFollowFrames;
		trailRotation = trailRotations[trailFollowIndex];
		trail.transform.position = trailPositions[trailFollowIndex];
		trailRotations[trailFollowIndex] = rotation;
		trailPositions[trailFollowIndex] = base.transform.position;
	}

	private void UpdateHoming()
	{
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
		if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(destroyPadding, destroyPadding)))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateSwirling()
	{
		if (!move)
		{
			return;
		}
		if (player.IsDead)
		{
			StopSwirling();
			return;
		}
		t += CupheadTime.FixedDelta;
		Vector2 a = swirlLaunchPos + MathUtils.AngleToDirection(swirlLaunchRotation) * t * speed;
		float num = 360f * speed / (swirlDistance * 2f * (float)Math.PI);
		swirlRotation += num * CupheadTime.FixedDelta;
		Vector2 vector = (Vector2)player.center + MathUtils.AngleToDirection(swirlRotation) * swirlDistance;
		if (t < swirlEaseTime)
		{
			Vector2 vector2 = base.transform.position;
			base.transform.position = Vector2.Lerp(a, vector, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / swirlEaseTime));
			rotation = MathUtils.DirectionToAngle((Vector2)base.transform.position - vector2);
		}
		else
		{
			base.transform.position = vector;
			rotation = swirlRotation + 90f;
		}
	}

	protected override void Update()
	{
		base.Update();
		timeSinceUpdateRotation += CupheadTime.Delta;
		if (timeSinceUpdateRotation > 1f / 24f)
		{
			timeSinceUpdateRotation -= 1f / 24f;
			Vector2 vector = trail.transform.position;
			base.transform.SetEulerAngles(0f, 0f, rotation + spriteRotation);
			trail.SetEulerAngles(0f, 0f, trailRotation + trailSpriteRotation);
			trail.position = vector;
		}
	}

	public void FindTarget()
	{
		float num = float.MaxValue;
		Collider2D collider2D = null;
		Vector2 vector = (Vector2)base.transform.position + speed * (timeBeforeEaseRotationSpeed + rotationSpeedEaseTime * 0.75f) * MathUtils.AngleToDirection(rotation);
		DamageReceiver[] array = UnityEngine.Object.FindObjectsOfType<DamageReceiver>();
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

	public void StartSwirling(int index, int bulletCount, float spread, AbstractPlayerController player)
	{
		state = State.Swirling;
		swirlLaunchRotation = base.transform.eulerAngles.z + ((float)index / (float)(bulletCount - 1) - 0.5f) * spread;
		swirlRotation = base.transform.eulerAngles.z + ((float)index / (float)(bulletCount - 1) - 0.5f) * (((float)bulletCount - 1f) / (float)bulletCount) * 360f;
		swirlLaunchPos = base.transform.position;
		rotation = swirlLaunchRotation;
		animator.Play("A", 0, (float)index / (float)bulletCount);
		animator.Play("Idle", 1, (float)index / (float)bulletCount);
		this.player = player;
	}

	public void StopSwirling()
	{
		state = State.Homing;
		FindTarget();
		t = 0f;
	}
}
