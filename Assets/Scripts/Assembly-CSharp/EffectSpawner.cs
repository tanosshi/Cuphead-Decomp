using System.Collections;
using UnityEngine;

public class EffectSpawner : AbstractMonoBehaviour
{
	[SerializeField]
	private Effect effectPrefab;

	public Vector2 offset;

	public float delay = 1f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(loop_cr());
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(base.baseTransform.position, 5f);
		Gizmos.color = Color.red;
		Vector3 vector = base.baseTransform.position + (Vector3)offset;
		Gizmos.DrawLine(base.transform.position, vector);
		Gizmos.DrawWireSphere(vector, 5f);
	}

	private IEnumerator loop_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
			Transform t = effectPrefab.Create(base.transform.position).transform;
			t.SetParent(base.transform);
			t.ResetLocalTransforms();
			t.localPosition = offset;
			t.SetParent(null);
		}
	}
}
