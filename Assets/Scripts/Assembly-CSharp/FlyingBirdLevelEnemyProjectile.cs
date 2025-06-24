using System.Collections;
using UnityEngine;

public class FlyingBirdLevelEnemyProjectile : AbstractProjectile
{
	private float time;

	private float height;

	public virtual AbstractProjectile Create(float time, float height, Vector2 pos)
	{
		FlyingBirdLevelEnemyProjectile flyingBirdLevelEnemyProjectile = Create(pos, 0f) as FlyingBirdLevelEnemyProjectile;
		flyingBirdLevelEnemyProjectile.time = time;
		flyingBirdLevelEnemyProjectile.height = height;
		flyingBirdLevelEnemyProjectile.DamagesType.OnlyPlayer();
		flyingBirdLevelEnemyProjectile.CollisionDeath.OnlyPlayer();
		flyingBirdLevelEnemyProjectile.Init();
		return flyingBirdLevelEnemyProjectile;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private void Init()
	{
		StartCoroutine(go_cr());
	}

	private void Check()
	{
		if (base.transform.position.y < -460f)
		{
			StopAllCoroutines();
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator go_cr()
	{
		float start = base.transform.position.y;
		float end = start + height;
		float t = 0f;
		float speed = 0f;
		t = 0f;
		while (t < time)
		{
			TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, start, end, t / time), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		while (t < time)
		{
			float val = t / time;
			float last = base.transform.position.y;
			base.transform.SetPosition(null, EaseUtils.Ease(EaseUtils.EaseType.easeInSine, end, start, val));
			speed = base.transform.position.y - last;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		while (true)
		{
			Check();
			base.transform.AddPosition(0f, speed * CupheadTime.GlobalSpeed);
			yield return null;
		}
	}
}
