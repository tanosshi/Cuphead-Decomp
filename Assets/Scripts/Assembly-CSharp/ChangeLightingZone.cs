using UnityEngine;

public class ChangeLightingZone : AbstractCollidableObject
{
	[SerializeField]
	private Color _minTint;

	[SerializeField]
	private Color _maxTint;

	[SerializeField]
	private BoxCollider2D _collider;

	[SerializeField]
	private float _maxDistance;

	private ContactFilter2D _filter;

	private Collider2D[] buffer = new Collider2D[10];

	protected override void Start()
	{
		base.Start();
		_filter = default(ContactFilter2D).NoFilter();
	}

	protected override void Update()
	{
		base.Update();
		int num = _collider.OverlapCollider(_filter, buffer);
		for (int i = 0; i < num; i++)
		{
			MapPlayerAnimationController component = buffer[i].GetComponent<MapPlayerAnimationController>();
			if (!(component == null))
			{
				float magnitude = ((Vector2)component.transform.position - (Vector2)base.transform.position).magnitude;
				float t = Mathf.Clamp(magnitude / _maxDistance, 0f, 1f);
				component.renderer.color = Color.Lerp(_minTint, _maxTint, t);
			}
		}
	}
}
