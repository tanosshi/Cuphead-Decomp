using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelCannonProjectile : BasicProjectile
{
	[SerializeField]
	private Effect deathFx;

	public Vector3 direction;

	public EnemyProperties Properties { get; set; }

	protected override void Start()
	{
		base.Start();
		animator.Play("anim_level_starcannon_bullet", -1, Random.value);
	}

	public void Init()
	{
		StartCoroutine(delayedDeath_cr());
	}

	private IEnumerator delayedDeath_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, Properties.bulletDeathTime);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		Effect effect = deathFx.Create(base.transform.position, new Vector3(1.25f, 1.25f, 1f));
		effect.animator.SetInteger("PickAni", Random.Range(0, 3));
		Object.Destroy(base.gameObject);
	}

	protected override void Move()
	{
		if (Speed == 0f)
		{
			Debug.Log("[Basic Projectile] Speed is 0", base.gameObject);
		}
		base.transform.position += direction * Speed * CupheadTime.FixedDelta - new Vector3(0f, _accumulativeGravity * CupheadTime.FixedDelta, 0f);
		_accumulativeGravity += Gravity * CupheadTime.FixedDelta;
	}
}
