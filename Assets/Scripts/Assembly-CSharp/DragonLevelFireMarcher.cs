using System;
using System.Collections;
using UnityEngine;

public class DragonLevelFireMarcher : AbstractCollidableObject
{
	private const float sinOffset = 70f;

	private const float sinPeriod = 450f;

	private const float sinHeight = 5f;

	private const float linearOffset = -23f;

	private const float linearOffsetDistance = 300f;

	private const float minJumpX = 50f;

	private const float maxJumpX = 590f;

	private DamageDealer damageDealer;

	private LevelProperties.Dragon.FireMarchers properties;

	[SerializeField]
	private float squeezeDistance;

	[SerializeField]
	private bool canJump;

	private bool wantsToJump;

	private bool slowing;

	private AbstractPlayerController targetPlayer;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		if (canJump)
		{
			base.animator.Play("Idle", 0, UnityEngine.Random.Range(0f, 1f));
		}
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

	public DragonLevelFireMarcher Create(Transform root, LevelProperties.Dragon.FireMarchers properties)
	{
		DragonLevelFireMarcher dragonLevelFireMarcher = InstantiatePrefab<DragonLevelFireMarcher>();
		dragonLevelFireMarcher.transform.parent = root;
		dragonLevelFireMarcher.transform.ResetLocalPosition();
		dragonLevelFireMarcher.properties = properties;
		dragonLevelFireMarcher.StartCoroutine(dragonLevelFireMarcher.move_cr());
		return dragonLevelFireMarcher;
	}

	private IEnumerator move_cr()
	{
		float initialYOffset = Mathf.Sin(0.9773844f) * 5f;
		float timeSlowed = 0f;
		while (base.transform.position.x < (float)(Level.Current.Right + 100))
		{
			float speed = properties.moveSpeed;
			if (slowing)
			{
				timeSlowed += (float)CupheadTime.Delta;
				if (timeSlowed > 0.25f)
				{
					yield break;
				}
				speed = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, speed, 0f, timeSlowed / 0.25f);
			}
			float x = base.transform.localPosition.x + speed * (float)CupheadTime.Delta;
			float y = Mathf.Sin((70f + base.transform.localPosition.x) * 2f * (float)Math.PI / 450f) * 5f - initialYOffset;
			y += Mathf.Min(1f, x / 300f) * -23f;
			if (x < squeezeDistance)
			{
				base.transform.SetScale(x / squeezeDistance);
			}
			else
			{
				base.transform.SetScale(1f);
			}
			base.transform.SetLocalPosition(x, y);
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public bool CanJump()
	{
		if (!canJump || wantsToJump)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = base.animator.GetCurrentAnimatorStateInfo(0);
		float num = base.transform.localPosition.x + (1f - currentAnimatorStateInfo.normalizedTime % 1f) * currentAnimatorStateInfo.length * properties.moveSpeed;
		return num > properties.jumpX.min && num < properties.jumpX.max;
	}

	public void StartJump(AbstractPlayerController targetPlayer)
	{
		this.targetPlayer = targetPlayer;
		wantsToJump = true;
		StartCoroutine(jump_cr());
	}

	private IEnumerator jump_cr()
	{
		base.animator.SetTrigger("StartJump");
		yield return base.animator.WaitForAnimationToStart(this, "Crouch_Start");
		AudioManager.Play("level_dragon_fire_marcher_b_couch_start");
		emitAudioFromObject.Add("level_dragon_fire_marcher_b_couch_start");
		slowing = true;
		yield return base.animator.WaitForAnimationToStart(this, "Crouch_Loop");
		Vector2 targetPos = targetPlayer.center;
		if (targetPos.x < base.transform.position.x)
		{
			base.transform.SetScale(-1f);
		}
		float bestDistance = float.MaxValue;
		Vector2 bestLaunchVelocity = Vector2.zero;
		Vector2 relativeTargetPos = targetPos - (Vector2)base.transform.position;
		relativeTargetPos.x = Mathf.Abs(relativeTargetPos.x);
		for (float num = 0f; num < 1f; num += 0.01f)
		{
			float floatAt = properties.jumpAngle.GetFloatAt(num);
			float floatAt2 = properties.jumpSpeed.GetFloatAt(num);
			Vector2 vector = MathUtils.AngleToDirection(floatAt) * floatAt2;
			float num2 = relativeTargetPos.x / vector.x;
			float num3 = vector.y * num2 - 0.5f * properties.gravity * num2 * num2;
			float num4 = Mathf.Abs(relativeTargetPos.y - num3);
			if (num4 < bestDistance)
			{
				bestDistance = num4;
				bestLaunchVelocity = vector;
			}
		}
		yield return CupheadTime.WaitForSeconds(this, properties.crouchTime);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Jump_Start");
		AudioManager.Play("level_dragon_fire_marcher_b_jump_start");
		emitAudioFromObject.Add("level_dragon_fire_marcher_b_jump_start");
		Vector2 velocity = bestLaunchVelocity;
		velocity.x *= base.transform.localScale.x;
		float t = 0f;
		Vector2 initialPos = base.transform.localPosition;
		while (base.transform.position.y > -400f)
		{
			t += CupheadTime.FixedDelta;
			base.transform.SetLocalPosition(initialPos.x + t * velocity.x, initialPos.y + t * velocity.y - 0.5f * properties.gravity * t * t);
			yield return new WaitForFixedUpdate();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
