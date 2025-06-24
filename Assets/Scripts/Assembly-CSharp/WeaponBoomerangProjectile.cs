using System;
using System.Collections;
using UnityEngine;

public class WeaponBoomerangProjectile : AbstractProjectile
{
	private const float TurnTimeRatio = 0.4f;

	public float Speed;

	public float forwardDistance;

	public float lateralDistance;

	public float maxDamage;

	public float hitFreezeTime;

	public LevelPlayerController player;

	[SerializeField]
	private bool isEx;

	[SerializeField]
	private Transform trail1;

	[SerializeField]
	private Transform trail2;

	[SerializeField]
	private Effect hitFXPrefab;

	private Vector2[] trailPositions;

	private int currentPositionIndex;

	private const int trailFrameDelay = 3;

	private Vector2 forwardDir;

	private Vector2 lateralDir;

	private bool hasTurned;

	private bool wasCaught;

	private bool headedOffscreen;

	private float totalDamage;

	private int variant;

	private float timeUntilUnfreeze;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	protected override void Start()
	{
		base.Start();
		forwardDir = MathUtils.AngleToDirection(base.transform.rotation.eulerAngles.z);
		lateralDir = new Vector2(0f - forwardDir.y, forwardDir.x);
		lateralDir *= (float)player.motor.TrueLookDirection.x;
		DestroyDistance = 0f;
		if (isEx)
		{
			trailPositions = new Vector2[6];
			for (int i = 0; i < trailPositions.Length; i++)
			{
				trailPositions[i] = base.transform.position;
			}
			StartCoroutine(ex_cr());
		}
		else
		{
			StartCoroutine(basic_cr());
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead)
		{
			if (wasCaught)
			{
				Die();
			}
			if (isEx && hasTurned && ((Vector2)(base.transform.position - player.center)).magnitude < player.colliderManager.Width / 2f + GetComponent<CircleCollider2D>().radius)
			{
				wasCaught = true;
			}
			bool flag = CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 150f));
			GetComponent<Collider2D>().enabled = flag;
			if (!flag && headedOffscreen)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (isEx)
			{
				updateTrails();
			}
		}
	}

	private void updateTrails()
	{
		int num = currentPositionIndex - 2;
		if (num < 0)
		{
			num += trailPositions.Length;
		}
		int num2 = currentPositionIndex - 5;
		if (num2 < 0)
		{
			num2 += trailPositions.Length;
		}
		trail1.position = trailPositions[num];
		trail2.position = trailPositions[num2];
		currentPositionIndex = (currentPositionIndex + 1) % trailPositions.Length;
		trailPositions[currentPositionIndex] = base.transform.position;
	}

	protected override void Die()
	{
		base.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0, 360));
		base.Die();
		StopAllCoroutines();
		SetInt(Integers.Variant, variant);
	}

	private IEnumerator basic_cr()
	{
		Vector2 startPos = base.transform.position;
		Vector2 turnPos = startPos + forwardDir * forwardDistance + lateralDir * lateralDistance * 0.5f;
		Vector2 returnPos = startPos + lateralDir * lateralDistance;
		float moveTime = forwardDistance / Speed * ((float)Math.PI / 2f);
		yield return StartCoroutine(move_cr(turnPos, EaseUtils.EaseType.easeOutSine, EaseUtils.EaseType.easeInSine, moveTime));
		hasTurned = true;
		yield return StartCoroutine(move_cr(returnPos, EaseUtils.EaseType.easeInSine, EaseUtils.EaseType.easeOutSine, moveTime));
		Vector2 velocity = Speed * -forwardDir;
		headedOffscreen = true;
		while (true)
		{
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator ex_cr()
	{
		Vector2 startPos = base.transform.position;
		Vector2 turnPos = startPos + forwardDir * forwardDistance + lateralDir * lateralDistance * 0.5f;
		bool behindPlayer = false;
		while (true)
		{
			EaseUtils.EaseType ease = ((!hasTurned) ? EaseUtils.EaseType.easeOutSine : EaseUtils.EaseType.easeInOutSine);
			float moveTime = ((!hasTurned) ? forwardDistance : (forwardDistance * 2f)) / Speed;
			yield return StartCoroutine(move_cr(turnPos, ease, ease, moveTime));
			hasTurned = true;
			startPos = base.transform.position;
			Vector2 playerPos = player.transform.position;
			turnPos = playerPos + (playerPos - startPos).normalized * forwardDistance;
		}
	}

	private IEnumerator move_cr(Vector2 endPos, EaseUtils.EaseType forwardEaseType, EaseUtils.EaseType lateralEaseType, float time)
	{
		float t = 0f;
		Vector2 startPos = base.transform.localPosition;
		Vector2 relativeEndPos = endPos - startPos;
		float forwardMovement = Vector2.Dot(forwardDir, relativeEndPos);
		float lateralMovement = Vector2.Dot(lateralDir, relativeEndPos);
		while (t < time)
		{
			while (timeUntilUnfreeze > 0f)
			{
				timeUntilUnfreeze -= CupheadTime.FixedDelta;
				yield return new WaitForFixedUpdate();
			}
			base.transform.position = startPos + forwardDir * EaseUtils.Ease(forwardEaseType, 0f, forwardMovement, t / time) + lateralDir * EaseUtils.Ease(lateralEaseType, 0f, lateralMovement, t / time);
			t += CupheadTime.FixedDelta;
			if (t / time > 0.4f)
			{
				animator.SetTrigger("Trans");
			}
			yield return new WaitForFixedUpdate();
		}
		base.transform.position = endPos;
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		float num = base.damageDealer.DealDamage(hit);
		if (isEx)
		{
			totalDamage += num;
			if (totalDamage > maxDamage)
			{
				Die();
			}
			if (num > 0f)
			{
				hitFXPrefab.Create(base.transform.position);
				timeUntilUnfreeze = hitFreezeTime;
				AudioManager.Play("player_ex_impact_hit");
				emitAudioFromObject.Add("player_ex_impact_hit");
			}
		}
	}

	public void SetPink(bool pink)
	{
		if (pink)
		{
			SetParryable(true);
			variant = 2;
		}
		else
		{
			SetParryable(false);
			variant = UnityEngine.Random.Range(0, 2);
		}
		SetInt(Integers.Variant, variant);
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
				AudioManager.Play("player_weapon_peashot_miss");
			}
		}
	}
}
