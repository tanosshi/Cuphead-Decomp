using System.Collections;
using UnityEngine;

public class BeeLevelGruntBriefcase : AbstractProjectile
{
	private const float Y_SPEED = 1500f;

	private const float X_SPEED = 200f;

	private const float TIME = 0.5f;

	public BeeLevelGruntBriefcase Create(int xScale, Vector2 pos)
	{
		BeeLevelGruntBriefcase beeLevelGruntBriefcase = Object.Instantiate(this);
		beeLevelGruntBriefcase.transform.position = pos;
		beeLevelGruntBriefcase.transform.SetScale(xScale, 1f, 1f);
		beeLevelGruntBriefcase.CollisionDeath.OnlyPlayer();
		beeLevelGruntBriefcase.DamagesType.OnlyPlayer();
		return beeLevelGruntBriefcase;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(x_cr());
		StartCoroutine(y_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator x_cr()
	{
		while (true)
		{
			if (base.transform.position.x < -1280f || base.transform.position.x > 1280f)
			{
				Object.Destroy(base.gameObject);
			}
			base.transform.AddPosition(200f * (float)CupheadTime.Delta * (0f - base.transform.localScale.x));
			yield return null;
		}
	}

	private IEnumerator y_cr()
	{
		float t = 0f;
		float time = 0.5f;
		while (t < time)
		{
			float val = t / time;
			TransformExtensions.AddPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 1500f, 0f, val) * (float)CupheadTime.Delta, transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		while (t < time)
		{
			float val2 = t / time;
			TransformExtensions.AddPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, -1500f, val2) * (float)CupheadTime.Delta, transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		while (true)
		{
			if (base.transform.position.y < -720f)
			{
				Object.Destroy(base.gameObject);
			}
			base.transform.AddPosition(0f, -1500f * (float)CupheadTime.Delta);
			yield return null;
		}
	}
}
