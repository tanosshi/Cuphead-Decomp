using System.Collections;
using UnityEngine;

public class FlyingGenieLevelMiniCat : HomingProjectile
{
	private const string PinkParameterName = "Pink";

	private LevelProperties.FlyingGenie.Sphinx properties;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	public FlyingGenieLevelMiniCat Create(Vector3 pos, float rotation, AbstractPlayerController player, LevelProperties.FlyingGenie.Sphinx properties)
	{
		FlyingGenieLevelMiniCat flyingGenieLevelMiniCat = Create(pos, rotation, properties.homingSpeed, properties.homingSpeed, properties.homingRotation, 20f, 0f, player) as FlyingGenieLevelMiniCat;
		flyingGenieLevelMiniCat.properties = properties;
		flyingGenieLevelMiniCat.transform.position = pos;
		return flyingGenieLevelMiniCat;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(timer_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
		if (properties.dieOnCollisionPlayer)
		{
			Die();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.miniHomingDurationRange.RandomFloat());
		base.HomingEnabled = false;
		while (true)
		{
			base.transform.position += base.transform.right * properties.homingSpeed * CupheadTime.Delta;
			yield return null;
		}
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		animator.SetFloat("Pink", parryable ? 1 : 0);
	}
}
