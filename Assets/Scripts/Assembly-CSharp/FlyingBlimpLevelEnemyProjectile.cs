using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelEnemyProjectile : BasicProjectile
{
	[SerializeField]
	private Effect FX;

	[SerializeField]
	private Transform root;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_fx_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		animator.SetTrigger("dead");
		base.OnCollisionPlayer(hit, phase);
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private IEnumerator spawn_fx_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.17f);
		while (true)
		{
			FX.Create(root.transform.position).transform.SetEulerAngles(null, null, base.transform.eulerAngles.z);
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
		}
	}
}
