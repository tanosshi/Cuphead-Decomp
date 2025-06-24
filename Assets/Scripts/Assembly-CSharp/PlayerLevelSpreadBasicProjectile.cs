using UnityEngine;

public class PlayerLevelSpreadBasicProjectile : BasicProjectile
{
	protected override void OnDieDistance()
	{
		if (!base.dead)
		{
			Die();
			animator.SetTrigger("OnDistanceDie");
		}
	}

	protected override void Die()
	{
		base.Die();
		base.transform.SetEulerAngles(0f, 0f, Random.Range(0, 360));
		base.transform.SetScale(MathUtils.PlusOrMinus(), MathUtils.PlusOrMinus(), 1f);
	}

	private void _OnDieAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
