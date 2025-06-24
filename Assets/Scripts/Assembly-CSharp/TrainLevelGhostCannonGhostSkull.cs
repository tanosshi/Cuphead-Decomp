using System.Collections;
using UnityEngine;

public class TrainLevelGhostCannonGhostSkull : AbstractProjectile
{
	private const float DEATH_Y = -325f;

	private float maxSpeed;

	private float speed;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	public TrainLevelGhostCannonGhostSkull Create(Vector3 pos, float speed)
	{
		TrainLevelGhostCannonGhostSkull trainLevelGhostCannonGhostSkull = Object.Instantiate(this);
		trainLevelGhostCannonGhostSkull.transform.position = pos;
		trainLevelGhostCannonGhostSkull.maxSpeed = speed;
		return trainLevelGhostCannonGhostSkull;
	}

	protected override void Start()
	{
		base.Start();
		SetParryable(true);
		StartCoroutine(speed_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (!base.dead && base.transform.position.y < -325f)
		{
			Die();
		}
		base.transform.AddPosition(0f, (0f - speed) * (float)CupheadTime.Delta);
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (phase != CollisionPhase.Enter)
		{
			return;
		}
		if (hit.tag == Tags.ParrySwitch.ToString())
		{
			ParrySwitch component = hit.GetComponent<ParrySwitch>();
			if (!(component.name != "Right") || !(component.name != "Left"))
			{
				component.ActivateFromOtherSource();
				Die();
			}
		}
		else if (hit.name == "HandCar")
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private IEnumerator speed_cr()
	{
		yield return TweenPositionY(base.transform.position.y, base.transform.position.y + 100f, 0.4f, EaseUtils.EaseType.easeOutCubic);
		yield return StartCoroutine(tweenSpeed_cr(0f, maxSpeed, 0.4f, EaseUtils.EaseType.linear));
	}

	private IEnumerator tweenSpeed_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			speed = EaseUtils.Ease(ease, start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		speed = maxSpeed;
	}
}
