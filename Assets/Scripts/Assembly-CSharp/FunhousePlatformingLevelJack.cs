using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelJack : AbstractPlatformingLevelEnemy
{
	private const float SCREEN_PADDING = 100f;

	private AbstractPlayerController player;

	private bool _enteredScreen;

	private Vector2 homingDirection = Vector2.down;

	private Vector2 launchVelocity;

	public bool HomingEnabled { get; set; }

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		base.Start();
		bool flag = Rand.Bool();
		base.animator.Play((!flag) ? "Green_Idle_A" : "Pink_Idle_A");
		_canParry = flag;
		HomingEnabled = true;
		player = PlayerManager.GetNext();
		launchVelocity = homingDirection * base.Properties.jackLaunchVelocity;
		StartCoroutine(move_cr());
		StartCoroutine(switch_cr());
	}

	protected override void Update()
	{
		base.Update();
		CalculateRender();
	}

	public void SelectDirection(bool fromBottom)
	{
		homingDirection = ((!fromBottom) ? Vector2.down : Vector2.up);
	}

	private IEnumerator move_cr()
	{
		float t = 0f;
		while (t < base.Properties.jacktimeBeforeDeath + base.Properties.jackEaseTime + base.Properties.jacktimeBeforeHoming)
		{
			while (!HomingEnabled)
			{
				yield return null;
			}
			t += CupheadTime.FixedDelta;
			if (player != null && !player.IsDead)
			{
				Vector3 center = player.center;
				Vector2 direction = (center - base.transform.position).normalized;
				Quaternion b = Quaternion.Euler(0f, 0f, MathUtils.DirectionToAngle(direction));
				Quaternion a = Quaternion.Euler(0f, 0f, MathUtils.DirectionToAngle(homingDirection));
				homingDirection = MathUtils.AngleToDirection(Quaternion.Slerp(a, b, Mathf.Min(1f, CupheadTime.FixedDelta * base.Properties.jackRotationSpeed)).eulerAngles.z);
			}
			Vector2 homingVelocity = homingDirection * base.Properties.jackHomingMoveSpeed;
			Vector2 velocity = homingVelocity;
			if (t < base.Properties.jacktimeBeforeHoming)
			{
				velocity = launchVelocity;
			}
			else if (t < base.Properties.jacktimeBeforeHoming + base.Properties.jackEaseTime)
			{
				float t2 = EaseUtils.EaseOutSine(0f, 1f, (t - base.Properties.jacktimeBeforeHoming) / base.Properties.jackEaseTime);
				velocity = Vector2.Lerp(launchVelocity, homingVelocity, t2);
			}
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
		Die();
	}

	private IEnumerator switch_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(1.5f, 3f));
			base.animator.SetTrigger("OnSwitch");
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		Die();
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (hit.GetComponent<FunhousePlatformingLevelJack>() != null)
		{
			Die();
		}
	}

	private void CalculateRender()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position) && !_enteredScreen)
		{
			_enteredScreen = true;
		}
		if (_enteredScreen && !CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 100f)))
		{
			Object.Destroy(base.gameObject);
		}
		if (PlatformingLevel.Current != null && (base.transform.position.x < (float)PlatformingLevel.Current.Left - 100f || base.transform.position.x > (float)PlatformingLevel.Current.Right + 100f || base.transform.position.y < (float)PlatformingLevel.Current.Ground - 100f))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
