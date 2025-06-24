using System.Collections;
using UnityEngine;

public class PlayerLevelSpreadExChild : BasicProjectile
{
	private const float TRAIL_TIME = 0.15f;

	[SerializeField]
	private Effect trailEffectPrefab;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(trail_cr());
	}

	private IEnumerator trail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.15f);
			Transform t = trailEffectPrefab.Create(base.transform.position).transform;
			t.SetParent(base.transform);
			t.ResetLocalTransforms();
			t.AddPositionForward2D(100f);
			t.SetParent(null);
		}
	}

	private void _OnDieAnimComplete()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
