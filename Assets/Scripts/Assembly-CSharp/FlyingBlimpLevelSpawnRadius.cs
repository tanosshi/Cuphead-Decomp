using UnityEngine;

public class FlyingBlimpLevelSpawnRadius : AbstractMonoBehaviour
{
	[SerializeField]
	private float _radius = 100f;

	public float radius
	{
		get
		{
			return _radius;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
		Gizmos.DrawSphere(base.baseTransform.position, radius);
		Gizmos.color = new Color(1f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(base.baseTransform.position, radius);
	}
}
