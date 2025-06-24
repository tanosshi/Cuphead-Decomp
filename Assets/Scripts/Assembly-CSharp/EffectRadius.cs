using UnityEngine;

public class EffectRadius : AbstractPausableComponent
{
	[SerializeField]
	private Effect effect;

	[SerializeField]
	private float _radius = 100f;

	[SerializeField]
	private Vector2 _offset = Vector2.zero;

	private Vector2 target;

	public float radius
	{
		get
		{
			return _radius;
		}
	}

	public Vector2 offset
	{
		get
		{
			return _offset;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(1f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + offset, radius);
	}

	public void CreateInRadius()
	{
		Vector2 vector = (Vector2)base.baseTransform.position + offset;
		Vector2 vector2 = new Vector2(Random.value * (float)(Rand.Bool() ? 1 : (-1)), Random.value * (float)(Rand.Bool() ? 1 : (-1)));
		target = vector + vector2.normalized * radius * Random.value;
		effect.Create(target);
	}
}
